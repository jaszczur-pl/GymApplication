namespace GymApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class customeridchange : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Reservations", "CustomerID", "dbo.Customers");
            DropIndex("dbo.Reservations", new[] { "CustomerID" });
            DropPrimaryKey("dbo.Customers");

            DropColumn("dbo.Customers", "ID");
            DropColumn("dbo.Reservations", "CustomerID");

            AddColumn("dbo.Reservations", "CustomerID", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Customers", "ID", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.Customers", "ID");
            CreateIndex("dbo.Reservations", "CustomerID");
            AddForeignKey("dbo.Reservations", "CustomerID", "dbo.Customers", "ID", cascadeDelete: true);

            DropColumn("dbo.Customers", "Password");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Customers", "Password", c => c.String(nullable: false));
            DropForeignKey("dbo.Reservations", "CustomerID", "dbo.Customers");
            DropIndex("dbo.Reservations", new[] { "CustomerID" });
            DropPrimaryKey("dbo.Customers");
            AlterColumn("dbo.Customers", "ID", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Reservations", "CustomerID", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Customers", "ID");
            CreateIndex("dbo.Reservations", "CustomerID");
            AddForeignKey("dbo.Reservations", "CustomerID", "dbo.Customers", "ID", cascadeDelete: true);
        }
    }
}
