namespace GoAgile.DataContexts.AgileDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AllowNullInDatesRetrospectives : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Retrospectives", "DateStared", c => c.DateTime());
            AlterColumn("dbo.Retrospectives", "DateFinished", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Retrospectives", "DateFinished", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Retrospectives", "DateStared", c => c.DateTime(nullable: false));
        }
    }
}
