namespace GoAgile.DataContexts.AgileDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RetrospectiveRepairsNulls : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Retrospectives", "DatePlanned", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Retrospectives", "DatePlanned", c => c.DateTime(nullable: false));
        }
    }
}
