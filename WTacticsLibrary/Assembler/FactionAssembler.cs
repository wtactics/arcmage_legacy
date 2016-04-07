using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WTacticsDAL;
using WTacticsLibrary.Model;

namespace WTacticsLibrary.Assembler
{
    public static class FactionAssembler
    {
        public static Faction FromDal(this FactionModel factionModel)
        {
            if (factionModel == null) return null;
            var result = new Faction
            {
                Id = factionModel.FactionId,
                Name = factionModel.Name
            };
            return result.SyncBase(factionModel);
        }

        public static void Patch(this FactionModel factionModel, Faction faction, UserModel user)
        {
            if (factionModel == null) return;
            factionModel.Name = faction.Name;
            factionModel.Patch(user);
        }
    }
}
