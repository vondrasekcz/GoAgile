namespace GoAgile.DataContexts.AgileDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEnableVoting : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Retrospectives", "EnableVoting", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Retrospectives", "EnableVoting");
        }
    }
}
