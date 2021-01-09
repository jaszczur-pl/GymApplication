namespace GymApplication.EntityMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_SecondFactorVerified : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "SecondFactorVerified", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "SecondFactorVerified");
        }
    }
}
