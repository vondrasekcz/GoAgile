namespace GoAgile.DataContexts.AgileDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RetrospectiveItems",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Retrospective = c.String(nullable: false, maxLength: 255),
                        UserName = c.String(nullable: false, maxLength: 255),
                        Section = c.String(nullable: false, maxLength: 255),
                        Text = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Retrospectives",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Owner = c.String(nullable: false, maxLength: 255),
                        RetrospectiveName = c.String(nullable: false, maxLength: 255),
                        Project = c.String(nullable: false, maxLength: 255),
                        StartDate = c.DateTime(nullable: false),
                        Comment = c.String(maxLength: 255),
                        State = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Retrospectives");
            DropTable("dbo.RetrospectiveItems");
        }
    }
}
