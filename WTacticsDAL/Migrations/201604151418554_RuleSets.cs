namespace WTacticsDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RuleSets : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RuleSetModels",
                c => new
                    {
                        RuleSetId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Guid = c.Guid(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                        LastModifiedTime = c.DateTime(nullable: false),
                        Creator_UserId = c.Int(),
                        LastModifiedBy_UserId = c.Int(),
                        Status_StatusId = c.Int(),
                    })
                .PrimaryKey(t => t.RuleSetId)
                .ForeignKey("dbo.UserModels", t => t.Creator_UserId)
                .ForeignKey("dbo.UserModels", t => t.LastModifiedBy_UserId)
                .ForeignKey("dbo.StatusModels", t => t.Status_StatusId)
                .Index(t => t.Creator_UserId)
                .Index(t => t.LastModifiedBy_UserId)
                .Index(t => t.Status_StatusId);
            
            AddColumn("dbo.CardModels", "RuleSet_RuleSetId", c => c.Int());
            CreateIndex("dbo.CardModels", "RuleSet_RuleSetId");
            AddForeignKey("dbo.CardModels", "RuleSet_RuleSetId", "dbo.RuleSetModels", "RuleSetId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CardModels", "RuleSet_RuleSetId", "dbo.RuleSetModels");
            DropForeignKey("dbo.RuleSetModels", "Status_StatusId", "dbo.StatusModels");
            DropForeignKey("dbo.RuleSetModels", "LastModifiedBy_UserId", "dbo.UserModels");
            DropForeignKey("dbo.RuleSetModels", "Creator_UserId", "dbo.UserModels");
            DropIndex("dbo.RuleSetModels", new[] { "Status_StatusId" });
            DropIndex("dbo.RuleSetModels", new[] { "LastModifiedBy_UserId" });
            DropIndex("dbo.RuleSetModels", new[] { "Creator_UserId" });
            DropIndex("dbo.CardModels", new[] { "RuleSet_RuleSetId" });
            DropColumn("dbo.CardModels", "RuleSet_RuleSetId");
            DropTable("dbo.RuleSetModels");
        }
    }
}
