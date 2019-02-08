namespace ValidareDate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        cod = c.String(nullable: false, maxLength: 128),
                        denumire = c.String(),
                        cod_fiscal = c.String(),
                        analitic = c.String(),
                        tara = c.String(),
                        judet = c.String(),
                        adresa = c.String(),
                        reg_com = c.String(),
                        discount = c.Single(nullable: false),
                        is_tva = c.Boolean(nullable: false),
                        isNew = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.cod);
            
            CreateTable(
                "dbo.Facturas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        nr_fact = c.String(),
                        nr_iesire = c.String(),
                        denumire = c.String(),
                        tvai = c.Single(nullable: false),
                        data = c.String(),
                        scadent = c.String(),
                        nr_bonuri = c.Single(nullable: false),
                        baza_tva = c.Single(nullable: false),
                        tva = c.Single(nullable: false),
                        total = c.Single(nullable: false),
                        cnp = c.String(),
                        produs = c.String(),
                        cantitate = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Facturas");
            DropTable("dbo.Clients");
        }
    }
}
