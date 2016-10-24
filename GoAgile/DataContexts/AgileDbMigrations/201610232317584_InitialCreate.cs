namespace GoAgile.DataContexts.AgileDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Retrospectives",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"),
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
        }
    }
}
