using DotNetDBF;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using ValidareDate.Data;
using ValidareDate.Models;

namespace ValidareDate.Helpers
{
    public class IesiriHelper
    {
        public static List<Iesire> verificaFacturi()
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
                var dbClient = clienti.FirstOrDefault(c => factura.cnp == c.cod_fiscal);
                if (dbClient == null)
                {
                    var lastCod = (Convert.ToInt64(clienti.Last().cod) + 1).ToString();

                    if (Regex.IsMatch(factura.cnp, "^[0-9]{13}$") &&
                        CNPHelper.CheckCnp(factura.cnp))
                    {
                        dbClient = new Client
                        {
                            cod = lastCod,
                            denumire = factura.denumire,
                            cod_fiscal = factura.cnp,
                            analitic = $"4111.{lastCod}",
                            tara = "RO",
                            isNew = true, 
                            adresa = "",
                            reg_com = ""
                        };
                        var judet = judete.FirstOrDefault(j => j.ID == factura.cnp.Substring(7, 2));

                        if(judet == null)
                        {
                            judet = new Judet { Cod = "CJ" };
                        }

                        dbClient.judet = judet.Cod;

                        clienti.Add(dbClient);
                    }
                    else
                    {
                        continue;
                        //todo verificare cnp/cui plus adaugarea in db cu cod nou
                    }
                }

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
                dbContext.Clienti.AddOrUpdate(newClients.ToArray());
                dbContext.SaveChanges();
            }

            return iesiri;
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