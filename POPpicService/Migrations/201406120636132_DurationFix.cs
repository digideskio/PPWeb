namespace POPpicService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DurationFix : DbMigration
    {
        public override void Up()
        {
            AlterColumn("POPpic.POPpicGameResults", "WinnerDuration", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("POPpic.POPpicGameResults", "LoserDuration", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            AlterColumn("POPpic.POPpicGameResults", "LoserDuration", c => c.String());
            AlterColumn("POPpic.POPpicGameResults", "WinnerDuration", c => c.String());
        }
    }
}
