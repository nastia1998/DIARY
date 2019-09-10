namespace DIARY_V4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Contact2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contacts", "Photo", c => c.String());
            DropColumn("dbo.Contacts", "Content");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Contacts", "Content", c => c.Binary());
            DropColumn("dbo.Contacts", "Photo");
        }
    }
}
