namespace GoAgile.DataContexts.AgileDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDatesToRetrospectives : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Retrospectives", "DatePlanned", c => c.DateTime(nullable: false));
            AddColumn("dbo.Retrospectives", "DateStared", c => c.DateTime(nullable: false));
            AddColumn("dbo.Retrospectives", "DateFinished", c => c.DateTime(nullable: false));
            DropColumn("dbo.Retrospectives", "StartDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Retrospectives", "StartDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Retrospectives", "DateFinished");
            DropColumn("dbo.Retrospectives", "DateStared");
            DropColumn("dbo.Retrospectives", "DatePlanned");
        }
    }
}
