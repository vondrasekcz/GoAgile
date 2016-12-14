namespace GoAgile.DataContexts.AgileDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColorRetrospectiveItems : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RetrospectiveItems", "Color", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RetrospectiveItems", "Color");
        }
    }
}
