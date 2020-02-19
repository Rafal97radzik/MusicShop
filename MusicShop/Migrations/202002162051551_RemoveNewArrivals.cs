namespace MusicShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveNewArrivals : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Albums", "IsNewArrival");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Albums", "IsNewArrival", c => c.Boolean(nullable: false));
        }
    }
}
