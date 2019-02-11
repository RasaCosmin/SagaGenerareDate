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
            var judete = new List<Judet>();

            using (var dbContext = new AppDbContext())
            {
                facturi = dbContext.Facturi.ToList();
            }

            using (var dbContext = new AppDbContext())
            {
                clienti = dbContext.Clienti.OrderBy(c => c.cod).ToList();
            }

            using (var dbContext = new AppDbContext())
            {
                judete = dbContext.Judete.ToList();
            }

            foreach (var factura in facturi)
            {

                var dbClient = checkFactura(factura, clienti, judete);

                //var lastCod = (Convert.ToInt64(clienti.Last().cod) + 1).ToString();

                //var dbClient = new Client
                //{
                //    cod = lastCod,
                //    denumire = factura.denumire,
                //    cod_fiscal = factura.cnp,
                //    analitic = $"4111.{lastCod}",
                //    tara = "RO",
                //    isNew = true,
                //    adresa = "",
                //    reg_com = ""
                //};

                //var dbClient = clienti.FirstOrDefault(c => factura.cnp == c.cod_fiscal);
                //if (dbClient == null)
                //{


                //    if (Regex.IsMatch(factura.cnp, "^[0-9]{12,14}$"))
                //    {
                //        if (CNPHelper.CheckCnp(factura.cnp))
                //        {
                //            var judet = judete.FirstOrDefault(j => j.ID == factura.cnp.Substring(7, 2));

                //            if (judet == null)
                //            {
                //                judet = new Judet { Cod = "CJ" };
                //            }

                //            dbClient.judet = judet.Cod;
                //        }
                //        else
                //        {
                //            dbClient.cod_fiscal = "";
                //            dbClient.judet = "CJ";
                //        }
                //    }
                //    else
                //    {
                //        dbClient.shouldCheckAnaf = true;
                //        //continue;
                //        //todo verificare cui plus adaugarea in db cu cod nou
                //    }

                //    clienti.Add(dbClient);
                //}

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

                var resp = await chackAnafClients(anafClients);
                foreach (var item in resp.found)
                {
                    var client = newClients.FirstOrDefault(c => c.cod_fiscal.Contains(item.cui));
                    if (client != null)
                    {
                        client.is_tva = item.scpTVA;
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
                dbClient = clienti.FirstOrDefault(c => c.cod_fiscal == factura.cnp);
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
                        if(localDbClient != null)
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
                        localCnp = localCnp.Substring(2);
                    }

                    if (Int32.TryParse(localCnp, out int cui))
                    {
                        if (cui >= Int32.MinValue && cui <= Int32.MaxValue)
                        {
                            dbClient.shouldCheckAnaf = true;
                        }
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
    }
}