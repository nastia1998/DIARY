namespace DIARY_V4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AttPhoto1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AttPhotoes", "Path", c => c.String());
            DropColumn("dbo.AttPhotoes", "Text");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AttPhotoes", "Text", c => c.String());
            DropColumn("dbo.AttPhotoes", "Path");
        }
    }
}
