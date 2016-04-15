using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WTacticsDAL;
using WTacticsLibrary.Model;

namespace WTacticsLibrary.Assembler
{
    public static class RuleSetAssembler
    {
        public static RuleSet FromDal(this RuleSetModel ruleSetModel)
        {
            if (ruleSetModel == null) return null;
            var result = new RuleSet
            {
                Id = ruleSetModel.RuleSetId,
                Name = ruleSetModel.Name,
                Status = ruleSetModel.Status.FromDal()
            };
            return result.SyncBase(ruleSetModel);
        }

        public static void Patch(this RuleSetModel ruleSetModel, RuleSet ruleSet, UserModel user)
        {
            if (ruleSetModel == null) return;
            ruleSetModel.Name = ruleSet.Name;
            ruleSetModel.Patch(user);
        }

    }
  
}
