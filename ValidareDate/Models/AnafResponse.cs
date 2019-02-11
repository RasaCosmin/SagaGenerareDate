using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidareDate.Models
{
    public class CuiRequest
    {
        public int cui { get; set; }
        public string data { get; set; }
    }

    public class AnafResponse
    {
        public int cod { get; set; }
        public string message { get; set; }
        public List<CuiFound> found { get; set; }
    }

    public class CuiFound
    {
        public string cui { get; set; }
        public string data { get; set; }
        public string denumire { get; set; }
        public string adresa { get; set; }
        public bool scpTVA { get; set; }
        public string data_sfarsit_ScpTVA { get; set; }
        public string data_anul_imp_ScpTVA { get; set; }
        public string mesaj_ScpTVA { get; set; }
        public string dataInceputTvaInc { get; set; }
        public string dataSfarsitTvaInc { get; set; }
        public string dataActualizareTvaInc { get; set; }
        public string dataPublicareTvaInc { get; set; }
        public string tipActTvaInc { get; set; }
        public bool statusTvaIncasare { get; set; }
        public string dataInactivare { get; set; }
        public string dataReactivare { get; set; }
        public string dataPublicare { get; set; }
        public string dataRadiere { get; set; }
        public bool statusInactivi { get; set; }
        public string dataInceputSplitTVA { get; set; }
        public string dataAnulareSplitTVA { get; set; }
        public bool statusSplitTVA { get; set; }
    }
}