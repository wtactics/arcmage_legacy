using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTacticsDAL
{
    public class DataBaseContext : DbContext
    {

        public DataBaseContext():base("WTacticsDAL.DataBaseContext")
        {
            
        }

        public DbSet<UserModel> Users { get; set; }

        public DbSet<RoleModel> Roles { get; set; }

        public DbSet<RightModel> Rights { get; set; }

        public DbSet<CardModel> Cards { get; set; }

        public DbSet<CardTypeModel> CardTypes { get; set; }

        public DbSet<TemplateInfoModel> TemplateInfoModels { get; set; }

        public DbSet<FactionModel> Factions { get; set; }
        
        public DbSet<SerieModel> Series { get; set; }

        public DbSet<RuleSetModel> RuleSets { get; set; }

        public DbSet<StatusModel> Statuses { get; set; }

        public DbSet<DeckModel> Decks { get; set; }

        public DbSet<DeckCardModel> DeckCards { get; set; }

    }
}
