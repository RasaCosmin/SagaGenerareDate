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
    public class DBFHelper
    {
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
                var data = DateTime.Parse(iesire.Data,
                                   System.Globalization.CultureInfo.InvariantCulture);

                if (data < minDate)
                {
                    minDate = data;
                }

                var scadent = DateTime.Parse(iesire.Data,
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
                writer.Close();
            }

            return filePath;
        }

        public static string genereazaFisierClienti(HttpServerUtilityBase server)
        {
            var filePath = "";

            string path = server.MapPath("~/Download/Clienti/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var writer = new DBFWriter();

            var codField = new DBFField("COD", NativeDbType.Char, 8);
            var denumireField = new DBFField("DENUMIRE", NativeDbType.Char, 64);
            var codFiscalField = new DBFField("COD_FISCAL", NativeDbType.Char, 20);
            var regComField = new DBFField("REG_COM", NativeDbType.Char, 16);
            var analiticIField = new DBFField("ANALITIC", NativeDbType.Char, 20);
            var adresaField = new DBFField("ADRESA", NativeDbType.Char, 100);
            var judetField = new DBFField("JUDET", NativeDbType.Char, 36);
            var taraField = new DBFField("TARA", NativeDbType.Char, 2);
            var isTvaField = new DBFField("IS_TVA", NativeDbType.Numeric, 1, 0);
            var tipTertField = new DBFField("TIP_TERT", NativeDbType.Char, 1);
            var nullFlagField = new DBFField("_NullFlags", NativeDbType.Numeric, 1, 0);

            writer.Fields = new[] { codField, denumireField, codFiscalField, regComField, analiticIField, adresaField, judetField, taraField, isTvaField, tipTertField, nullFlagField };


            using (var dbContext = new AppDbContext())
            {
                var clienti = dbContext.Clienti.Where(c => c.isNew).ToList();

                foreach (var client in clienti)
                {
                    var record = new object[] { client.cod, client.denumire, client.cod_fiscal, client.reg_com, client.analitic, client.adresa, client.judet, client.tara, client.is_tva ? 1 : 0, "", 0 };
                    writer.AddRecord(record);
                }

                var date = DateTime.Now.ToString("dd-MM-yyyy");

                filePath = $"{path}Clienti_{date}_{date}.dbf";

                using (Stream fos = System.IO.File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {

                    writer.Write(fos);
                    writer.Close();
                }

            }

            return filePath;
        }
    }
}