using DotNetDBF;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using ValidareDate.Data;
using ValidareDate.Models;
using ValidareDate.Services;

namespace ValidareDate.Helpers
{
    public class IesiriHelper
    {
        public static async Task<List<Iesire>> verificaFacturi()
        {
            var iesiri = new List<Iesire>();

            var facturi = new List<Factura>();
            var clienti = new List<Client>();

            using (var dbContext = new AppDbContext())
            {
                facturi = dbContext.Facturi.ToList();
            }

            using (var dbContext = new AppDbContext())
            {
                clienti = dbContext.Clienti.OrderBy(c => c.cod).ToList();
            }

            foreach (var factura in facturi)
            {

                var dbClient = checkFactura(factura, clienti, judete);

                var iesire = new Iesire
                {
                    NrIesire = factura.nr_fact,
                    Cod = dbClient.cod,
                    Data = factura.data,
                    Scadent = factura.scadent,
                    Valoare = factura.baza_tva,
                    TvaI = factura.tvai,
                    Tva = factura.tva
                };

                completeazaValoriIesire(iesire, factura);

                iesiri.Add(iesire);
            }

            using (var dbContext = new AppDbContext())
            {
                var newClients = clienti.Where(c => c.isNew);

                var anafClients = newClients.Where(c => c.shouldCheckAnaf).ToList();
                if (anafClients.Count > 0)
                {
                    var resp = await chackAnafClients(anafClients);
                    foreach (var item in resp.found)
                    {
                        var client = newClients.FirstOrDefault(c => c.cod_fiscal.Contains(item.cui));
                        if (client != null)
                        {
                            client.is_tva = item.scpTVA;
                        }
                    }
                }

                dbContext.Clienti.AddOrUpdate(newClients.ToArray());
                dbContext.SaveChanges();
            }

            return iesiri;
        }

        private static Client checkFactura(Factura factura, List<Client> clienti, List<Judet> judete)
        {
            Client dbClient = null;

            var localCnp = factura.cnp.Replace(" ", "");

            if (localCnp == "" || localCnp == "1" || localCnp == "2")
            {
                dbClient = clienti.FirstOrDefault(c => c.denumire == factura.denumire);
            }
            else
            {
                dbClient = clienti.FirstOrDefault(c => c.cod_fiscal == factura.cnp || c.cod_fiscal == localCnp);
            }

            if (dbClient == null)
            {
                var lastCod = (Convert.ToInt64(clienti.Last().cod) + 1).ToString();

                dbClient = new Client
                {
                    cod = lastCod,
                    denumire = factura.denumire,
                    cod_fiscal = factura.cnp,
                    analitic = $"4111.{lastCod}",
                    tara = "RO",
                    isNew = true,
                    adresa = "",
                    reg_com = "",
                    shouldCheckAnaf = false
                };

                if (Regex.IsMatch(localCnp, "^[0-9]{12,14}$"))
                {
                    if (CNPHelper.CheckCnp(localCnp))
                    {
                        var judet = judete.FirstOrDefault(j => j.ID == factura.cnp.Substring(7, 2));

                        if (judet == null)
                        {
                            judet = new Judet { Cod = "CJ" };
                        }

                        dbClient.judet = judet.Cod;
                    }
                    else
                    {
                        var localDbClient = clienti.FirstOrDefault(c => c.invalidCnp == factura.cnp);
                        if (localDbClient != null)
                        {
                            return localDbClient;
                        }

                        dbClient.invalidCnp = dbClient.cod_fiscal;
                        dbClient.cod_fiscal = "";
                        dbClient.judet = "CJ";
                    }
                }
                else
                {
                    if (Regex.IsMatch(localCnp, "^[A-Z]{2}[0-9]{1,9}$"))
                    {
                        dbClient.cod_fiscal = localCnp;
                        localCnp = localCnp.Substring(2);
                    }

                    if (localCnp != "" && localCnp != "1" && localCnp != "2")
                    {
                        var localDbClient = clienti.FirstOrDefault(c => c.cod_fiscal == localCnp);
                        if (localDbClient != null)
                        {
                            return localDbClient;
                        }

                        if (Int32.TryParse(localCnp, out int cui))
                        {
                            if (cui >= Int32.MinValue && cui <= Int32.MaxValue)
                            {
                                dbClient.shouldCheckAnaf = true;
                            }
                        }
                    }
                    else
                    {
                        dbClient.cod_fiscal = "";
                    }
                }

                clienti.Add(dbClient);
            }

            return dbClient;
        }

        private async static Task<AnafResponse> chackAnafClients(List<Client> anafClients)
        {
            var checkClients = new List<CuiRequest>();

            foreach (var c in anafClients)
            {

                var codFiscal = c.cod_fiscal.Replace(" ", "");
                if (Regex.IsMatch(codFiscal, "^[A-Z]{2}[0-9]{1,9}$"))
                {
                    codFiscal = codFiscal.Substring(2);
                }

                int cui;
                if (Int32.TryParse(codFiscal, out cui))
                {
                    if (cui >= Int32.MinValue && cui <= Int32.MaxValue)
                    {
                        checkClients.Add(new CuiRequest { cui = cui, data = DateTime.Now.ToString("yyyy-MM-dd") });
                    }
                }
            }

            var response = new AnafResponse();
            response.found = new List<CuiFound>();

            if (checkClients.Count > 500)
            {
                int batches = checkClients.Count / 500;
                int lastBatch = checkClients.Count % 500;

                for (int i = 0; i < batches; i++)
                {
                    var subList = checkClients.GetRange(i, i + 500);
                    var subR = await HttpService.PostCui(subList);
                    response.found.AddRange(subR.found);
                    System.Threading.Thread.Sleep(1000);
                }

                var lastCall = checkClients.GetRange(batches * 500, lastBatch);
                var subRLast = await HttpService.PostCui(lastCall);
                response.found.AddRange(subRLast.found);
            }
            else
            {
                var subR = await HttpService.PostCui(checkClients);
                response.found.AddRange(subR.found);
            }

            return response;
        }

        private static void completeazaValoriIesire(Iesire iesire, Factura factura)
        {
            string denTip, cont;
            double cantitate = factura.cantitate;
            if (factura.produs.Contains("Rabat"))
            {
                denTip = "DISCOUNT";
                cantitate *= -1;
                cont = "667";
            }
            else
            {
                denTip = "EDUCATIONALE P.";
                cont = "7015";
            }

            iesire.DenTip = denTip;
            iesire.Cont = cont;
            iesire.Cantitate = cantitate;

            int tvaIndex = factura.produs.IndexOf("TVA ") + 4;
            int procIndex = factura.produs.LastIndexOf('%');

            var tva = factura.produs.Substring(tvaIndex, procIndex - tvaIndex);

            iesire.TvaArt = Convert.ToInt32(tva);
        }


        private static readonly List<Judet> judete = new List<Judet>(){
                 new Judet{ID="01", Name="Alba", Cod="AB"},
                 new Judet{ID="02", Name="Arad", Cod="AR"},
                 new Judet{ID="03", Name="Argeș", Cod="AG" },
                 new Judet{ID="04", Name="Bacău", Cod="BC"},
                 new Judet{ID="05", Name="Bihor", Cod="BH"},
                 new Judet{ID="06", Name="Bistrița-Năsăud", Cod="BN"},
                 new Judet{ID="07", Name="Botoșani", Cod="BT"},
                 new Judet{ID="08", Name="Brașov", Cod="BV"},
                 new Judet{ID="09", Name="Brăila", Cod="BR"},
                 new Judet{ID="10", Name="Buzău", Cod="BZ"},
                 new Judet{ID="11", Name="Caraș-Severin", Cod="CS"},
                 new Judet{ID="12", Name="Cluj", Cod="CJ"},
                 new Judet{ID="13", Name="Constanța", Cod="CT"},
                 new Judet{ID="14", Name="Covasna", Cod="CV"},
                 new Judet{ID="15", Name="Dâmbovița", Cod="DB"},
                 new Judet{ID="16", Name="Dolj", Cod="BJ"},
                 new Judet{ID="17", Name="Galați", Cod="GL"},
                 new Judet{ID="18", Name="Gorj", Cod="GJ"},
                 new Judet{ID="19", Name="Harghita", Cod="HR"},
                 new Judet{ID="20", Name="Hunedoara", Cod="HD"},
                 new Judet{ID="21", Name="Ialomița", Cod="IL"},
                 new Judet{ID="22", Name="Iași", Cod="IS"},
                 new Judet{ID="23", Name="Ilfov", Cod="IF"},
                 new Judet{ID="24", Name="Maramureș", Cod="MM"},
                 new Judet{ID="25", Name="Mehedinți", Cod="MH"},
                 new Judet{ID="26", Name="Mureș", Cod="MS"},
                 new Judet{ID="27", Name="Neamț", Cod="NT"},
                 new Judet{ID="28", Name="Olt", Cod="OT"},
                 new Judet{ID="29", Name="Prahova", Cod="PH"},
                 new Judet{ID="30", Name="Satu Mare", Cod="SM"},
                 new Judet{ID="31", Name="Sălaj", Cod="SJ"},
                 new Judet{ID="32", Name="Sibiu", Cod="SB"},
                 new Judet{ID="33", Name="Suceava", Cod="SV"},
                 new Judet{ID="34", Name="Teleorman", Cod="TR"},
                 new Judet{ID="35", Name="Timiș", Cod="TM"},
                 new Judet{ID="36", Name="Tulcea", Cod="TL"},
                 new Judet{ID="37", Name="Vaslui", Cod="VS"},
                 new Judet{ID="38", Name="Vâlcea", Cod="VL"},
                 new Judet{ID="39", Name="Vrancea", Cod="VN"},
                 new Judet{ID="40", Name="București", Cod="B"},
                 new Judet{ID="41", Name="București - Sector 1", Cod="B"},
                 new Judet{ID="42", Name="București - Sector 2", Cod="B"},
                 new Judet{ID="43", Name="București - Sector 3", Cod="B"},
                 new Judet{ID="44", Name="București - Sector 4", Cod="B"},
                 new Judet{ID="45", Name="București - Sector 5", Cod="B"},
                 new Judet{ID="46", Name="București - Sector 6", Cod="B"},
                 new Judet{ID="47", Name="București - Sector 7", Cod="B"},
                 new Judet{ID="48", Name="București - Sector 8", Cod="B"},
                 new Judet{ID="51", Name="Călărași", Cod="CL"},
                 new Judet{ID="52", Name="Giurgiu", Cod="GR"}
            };
    }
}