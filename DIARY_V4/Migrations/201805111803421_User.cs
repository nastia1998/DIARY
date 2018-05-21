namespace DIARY_V4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class User : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        Id_Contact = c.Int(nullable: false, identity: true),
                        Id_User = c.Int(nullable: false),
                        Photo = c.String(),
                        Name = c.String(),
                        Surname = c.String(),
                        DateOfBirth = c.DateTime(nullable: false),
                        Country = c.String(),
                        City = c.String(),
                        Email = c.String(),
                        Telephone = c.String(),
                    })
                .PrimaryKey(t => t.Id_Contact)
                .ForeignKey("dbo.Users", t => t.Id_User, cascadeDelete: true)
                .Index(t => t.Id_User);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 25),
                        Login = c.String(nullable: false, maxLength: 25),
                        Password = c.String(nullable: false, maxLength: 25),
                        SecretWord = c.String(nullable: false, maxLength: 25),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Notes",
                c => new
                    {
                        Id_Note = c.Int(nullable: false, identity: true),
                        Id_User = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.Id_Note)
                .ForeignKey("dbo.Users", t => t.Id_User, cascadeDelete: true)
                .Index(t => t.Id_User);
            
            CreateTable(
                "dbo.Reminders",
                c => new
                    {
                        Id_Rem = c.Int(nullable: false, identity: true),
                        Id_User = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Time = c.String(),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.Id_Rem)
                .ForeignKey("dbo.Users", t => t.Id_User, cascadeDelete: true)
                .Index(t => t.Id_User);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reminders", "Id_User", "dbo.Users");
            DropForeignKey("dbo.Notes", "Id_User", "dbo.Users");
            DropForeignKey("dbo.Contacts", "Id_User", "dbo.Users");
            DropIndex("dbo.Reminders", new[] { "Id_User" });
            DropIndex("dbo.Notes", new[] { "Id_User" });
            DropIndex("dbo.Contacts", new[] { "Id_User" });
            DropTable("dbo.Reminders");
            DropTable("dbo.Notes");
            DropTable("dbo.Users");
            DropTable("dbo.Contacts");
        }
    }
}
