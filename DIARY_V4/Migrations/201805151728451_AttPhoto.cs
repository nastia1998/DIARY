namespace DIARY_V4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AttPhoto : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AttPhotoes",
                c => new
                    {
                        Id_Photo = c.Int(nullable: false, identity: true),
                        Id_Note = c.Int(nullable: false),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.Id_Photo)
                .ForeignKey("dbo.Notes", t => t.Id_Note, cascadeDelete: true)
                .Index(t => t.Id_Note);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AttPhotoes", "Id_Note", "dbo.Notes");
            DropIndex("dbo.AttPhotoes", new[] { "Id_Note" });
            DropTable("dbo.AttPhotoes");
        }
    }
}
