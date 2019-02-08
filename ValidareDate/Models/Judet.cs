using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ValidareDate.Models
{
    public class Judet
    {
        [Key]
        public string ID { get; set; }
        public string Name { get; set; }
        public string Cod { get; set; }
    }
}