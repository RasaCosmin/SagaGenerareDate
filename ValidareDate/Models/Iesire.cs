using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidareDate.Models
{
    public class Iesire
    {
        public string NrIesire { get; set; }
        public string Cod { get; set; }
        public string Data { get; set; }
        public string Scadent { get; set; }
        public double TvaI { get; set; }
        public string DenTip { get; set; }
        public string Gestiune = "0001";
        public string DenGest = "SEDIU";
        public int TvaArt { get; set; }
        public double Cantitate { get; set; }
        public double Valoare { get; set; }
        public double Tva { get; set; }
        public string Cont { get; set; }
    }
}