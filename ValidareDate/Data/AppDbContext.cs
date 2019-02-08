using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using ValidareDate.Models;

namespace ValidareDate.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext() : base("AppDbContext")
        {
        }

        public DbSet<Client> Clienti { get; set; }
        public DbSet<Factura> Facturi { get; set; }
        public DbSet<Judet> Judete { get; set; }
    }
}