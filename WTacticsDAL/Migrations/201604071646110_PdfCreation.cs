namespace WTacticsDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PdfCreation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DeckCardModels", "PdfCreationJobId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DeckCardModels", "PdfCreationJobId");
        }
    }
}
