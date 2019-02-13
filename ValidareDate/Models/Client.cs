using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Web;

namespace ValidareDate.Models
{
    public class Client
    {
        [Key]
        public string cod { get; set; }
        public string denumire { get; set; }
        public string cod_fiscal { get; set; }
        public string reg_com { get; set; }
        public string analitic { get; set; }
        public string tara { get; set; }
        public string judet { get; set; }
        public string adresa { get; set; }
        //public string cont_banca { get; set; }
        //public string banca { get; set; }
        //public string tel { get; set; }
        //public string email { get; set; }
        //public string grupa { get; set; }
        //public string delegat { get; set; }
        //public string bi_serie { get; set; }
        //public string bi_numar { get; set; }
        //public string bi_pol { get; set; }
        //public string masina { get; set; }
        //public string den_agent { get; set; }
        public double discount { get; set; }
        //public double zs { get; set; }
        //public string filiala { get; set; }
        //public string inf_supl { get; set; }
        //public string agent { get; set; }
        //public string tip_tert { get; set; }
        public bool is_tva { get; set; }
        //public double blocat { get; set; }
        //public string data_v_tva { get; set; }
        //public double cb_card { get; set; }
        //public string data_s_tva { get; set; }
        //public double c_limit { get; set; }

        public bool isNew { get; set; }

        [NotMapped]
        public bool shouldCheckAnaf { get; set; }

        public string invalidCnp { get; set; }

        internal void ConvertFromDataRow(DataRow row)
        {
            cod = row["cod"].ToString();
            denumire = row["denumire"].ToString();
            cod_fiscal = row["cod_fiscal"].ToString();
            analitic = row["analitic"].ToString();
            tara = row["tara"].ToString();
            judet = row["judet"].ToString();
            adresa = row["adresa"].ToString();
            //cont_banca = row["cont_banca"].ToString();
            //banca = row["banca"].ToString();
            //tel = row["tel"].ToString();
            //email = row["email"].ToString();
            //grupa = row["grupa"].ToString();
            reg_com = row["reg_com"].ToString();
            //delegat = row["delegat"].ToString();
            //bi_serie = row["bi_serie"].ToString();
            //bi_numar = row["bi_numar"].ToString();
            //bi_pol = row["bi_pol"].ToString();
            //masina = row["masina"].ToString();
            //den_agent = row["den_agent"].ToString();
            discount = Convert.ToDouble(row["discount"]);
            //zs = Convert.ToDouble(row["zs"]);
            //filiala = row["filiala"].ToString();
            //inf_supl = row["inf_supl"].ToString();
            //agent = row["agent"].ToString();
            //tip_tert = row["tip_tert"].ToString();
            is_tva = Convert.ToInt32(row["is_tva"]) == 1;
            //blocat = Convert.ToDouble(row["blocat"]);
            //data_v_tva = row["data_v_tva"].ToString();
            //cb_card = Convert.ToDouble(row["cb_card"]);
            //data_s_tva = row["data_s_tva"].ToString();
            //c_limit = Convert.ToDouble(row["c_limit"]);
            isNew = false;
        }
    }
}