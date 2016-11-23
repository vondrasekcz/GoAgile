namespace GoAgile.DataContexts.AgileDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedVotes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RetrospectiveItems", "Votes", c => c.Int(nullable: false));
            AddColumn("dbo.Retrospectives", "Votes", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Retrospectives", "Votes");
            DropColumn("dbo.RetrospectiveItems", "Votes");
        }
    }
}
