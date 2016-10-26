namespace GoAgile.DataContexts.AgileDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate1 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Retrospectives");
            AlterColumn("dbo.Retrospectives", "Id", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.Retrospectives", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Retrospectives");
            AlterColumn("dbo.Retrospectives", "Id", c => c.Guid(nullable: false, identity: true));
            AddPrimaryKey("dbo.Retrospectives", "Id");
        }
    }
}
