namespace ValidareDate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Judets",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        Cod = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            AlterColumn("dbo.Clients", "discount", c => c.Double(nullable: false));
            AlterColumn("dbo.Facturas", "tvai", c => c.Double(nullable: false));
            AlterColumn("dbo.Facturas", "nr_bonuri", c => c.Double(nullable: false));
            AlterColumn("dbo.Facturas", "baza_tva", c => c.Double(nullable: false));
            AlterColumn("dbo.Facturas", "tva", c => c.Double(nullable: false));
            AlterColumn("dbo.Facturas", "total", c => c.Double(nullable: false));
            AlterColumn("dbo.Facturas", "cantitate", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Facturas", "cantitate", c => c.Single(nullable: false));
            AlterColumn("dbo.Facturas", "total", c => c.Single(nullable: false));
            AlterColumn("dbo.Facturas", "tva", c => c.Single(nullable: false));
            AlterColumn("dbo.Facturas", "baza_tva", c => c.Single(nullable: false));
            AlterColumn("dbo.Facturas", "nr_bonuri", c => c.Single(nullable: false));
            AlterColumn("dbo.Facturas", "tvai", c => c.Single(nullable: false));
            AlterColumn("dbo.Clients", "discount", c => c.Single(nullable: false));
            DropTable("dbo.Judets");
        }
    }
}
