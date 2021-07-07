namespace BuildingManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class chk : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Registrations", "IsAdmin", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Registrations", "IsAdmin", c => c.Byte(nullable: false));
        }
    }
}
