namespace ZergTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProjectIdError : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "SelectedRole", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "SelectedRole");
        }
    }
}
