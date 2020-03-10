namespace MusicShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CorrectionNames : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "Address", c => c.String(nullable: false, maxLength: 150));
            AddColumn("dbo.Orders", "Comment", c => c.String());
            AlterColumn("dbo.Orders", "FirstName", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Orders", "LastName", c => c.String(nullable: false, maxLength: 150));
            AlterColumn("dbo.Orders", "PhoneNumber", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.Orders", "Email", c => c.String(nullable: false));
            DropColumn("dbo.Orders", "Adress");
            DropColumn("dbo.Orders", "Coment");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "Coment", c => c.String());
            AddColumn("dbo.Orders", "Adress", c => c.String());
            AlterColumn("dbo.Orders", "Email", c => c.String());
            AlterColumn("dbo.Orders", "PhoneNumber", c => c.String());
            AlterColumn("dbo.Orders", "LastName", c => c.String(maxLength: 150));
            AlterColumn("dbo.Orders", "FirstName", c => c.String(maxLength: 150));
            DropColumn("dbo.Orders", "Comment");
            DropColumn("dbo.Orders", "Address");
        }
    }
}
