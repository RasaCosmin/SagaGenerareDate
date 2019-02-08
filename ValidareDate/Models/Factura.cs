using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Web;

namespace ValidareDate.Models
{
    public class Factura
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string nr_fact { get; set; }
        public string nr_iesire { get; set; }
        public string denumire { get; set; }
        public double tvai { get; set; }
        public string data { get; set; }
        public string scadent { get; set; }
        public double nr_bonuri { get; set; }
        public double baza_tva { get; set; }
        public double tva { get; set; }
        public double total { get; set; }
        public string cnp { get; set; }
        public string produs { get; set; }
        public double cantitate { get; set; }

        internal void ConvertFromDataRow(DataRow row)
        {
            nr_fact = row["nr fact"].ToString();
            nr_iesire = row["nr_iesire"].ToString();
            denumire = row["denumire"].ToString();
            tvai = Convert.ToDouble(row["tvai"]);
            data = row["data"].ToString();
            scadent = row["scadent"].ToString();
            nr_bonuri = Convert.ToDouble(row["nr_bonuri"]);
            baza_tva = Convert.ToDouble(row["baza_tva"]);
            tva = Convert.ToDouble(row["tva"]);
            total = Convert.ToDouble(row["total"]);
            cnp = row["cnp"].ToString();
            produs = row["produs"].ToString();
            cantitate = Convert.ToDouble(row["cantitate"]);
        }
    }
}