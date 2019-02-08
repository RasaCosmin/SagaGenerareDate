using DotNetDBF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            using (var dbContext = new AppDbContext())
            {
                var facturi = dbContext.Facturi.Take(1000);
                var clienti = dbContext.Clienti;
                foreach (var factura in facturi)
                {
                    var dbClient = clienti.FirstOrDefault(c => factura.cnp == c.cod_fiscal);
                    if (dbClient == null)
                    {
                        continue;
                        //todo verificare cnp/cui plus adaugarea in db cu cod nou
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

        public static string genereazaFisierIesiri(List<Iesire> iesiri, HttpServerUtilityBase server)
        {
            string path = server.MapPath("~/Download/Iesiri/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var writer = new DBFWriter();

            var nrField = new DBFField("NR_IESIRE", NativeDbType.Char, 16);
            var codField = new DBFField("COD", NativeDbType.Char, 8);
            var dataField = new DBFField("DATA", NativeDbType.Date);
            var scadentField = new DBFField("SCADENT", NativeDbType.Date);
            var tipField = new DBFField("TIP", NativeDbType.Char, 1);
            var tvaIField = new DBFField("TVAI", NativeDbType.Numeric, 1, 0);
            var denTipField = new DBFField("DEN_TIP", NativeDbType.Char, 36);
            var gestiuneField = new DBFField("GESTIUNE", NativeDbType.Char, 4);
            var dengestField = new DBFField("DEN_GEST", NativeDbType.Char, 24);
            var codArtField = new DBFField("COD_ART", NativeDbType.Char, 16);
            var denArtField = new DBFField("DEN_ART", NativeDbType.Char, 60);
            var tvaArtField = new DBFField("TVA_ART", NativeDbType.Numeric, 2, 0);
            var umField = new DBFField("UM", NativeDbType.Char, 5);
            var cantitateField = new DBFField("CANTITATE", NativeDbType.Numeric, 14, 3);
            var valoareField = new DBFField("VALOARE", NativeDbType.Numeric, 15, 2);
            var tvaField = new DBFField("TVA", NativeDbType.Numeric, 15, 2);
            var contField = new DBFField("CONT", NativeDbType.Char, 20);
            var grupaField = new DBFField("GRUPA", NativeDbType.Char, 16);
            var nullFlagField = new DBFField("N_NULLFLAG", NativeDbType.Numeric, 1, 0);

            writer.Fields = new[] { nrField, codField, dataField, scadentField, tipField, tvaIField, denTipField,
                                        gestiuneField, dengestField, codArtField, denArtField, tvaArtField, umField,
                                        cantitateField, valoareField, tvaField, contField, grupaField, nullFlagField};

            DateTime minDate = DateTime.Now;
            DateTime maxDate = new DateTime(); 

            foreach (var iesire in iesiri)
            {
                var data = DateTime.ParseExact(iesire.Data, "dd-MMM-yy HH:mm:ss",
                                   System.Globalization.CultureInfo.InvariantCulture);

                if(data < minDate)
                {
                    minDate = data;
                }

                var scadent = DateTime.ParseExact(iesire.Data, "dd-MMM-yy HH:mm:ss",
                                  System.Globalization.CultureInfo.InvariantCulture);

                if (scadent > maxDate)
                {
                    maxDate = scadent;
                }

                var record = new object[] { iesire.NrIesire, iesire.Cod, data, scadent, "", iesire.TvaI, iesire.DenTip,
                                        iesire.Gestiune, iesire.DenGest, "", "", iesire.TvaArt, "",
                                        iesire.Cantitate, iesire.Valoare, iesire.Tva, iesire.Cont, "", 0};
                writer.AddRecord(record);
            }

            var startDate = minDate.ToString("dd-MM-yyyy");
            var endDate = maxDate.ToString("dd-MM-yyyy");

            var filePath = $"{path}IE_{startDate}_{endDate}.dbf";

            using (Stream fos = System.IO.File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
               
                writer.Write(fos);
            }

            return filePath;
        }
    }
}