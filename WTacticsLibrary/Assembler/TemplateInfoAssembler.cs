using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WTacticsDAL;
using WTacticsLibrary.Model;

namespace WTacticsLibrary.Assembler
{
    public static class TemplateInfoAssembler
    {
        public static TemplateInfo FromDal(this TemplateInfoModel templateInfoModel)
        {
            if (templateInfoModel == null) return null;
            var result = new TemplateInfo
            {
                Id = templateInfoModel.TemplateInfoId,
                ShowName = templateInfoModel.ShowName,
                ShowType = templateInfoModel.ShowType,
                ShowGoldCost = templateInfoModel.ShowGoldCost,
                ShowLoyalty = templateInfoModel.ShowLoyalty,
                ShowText = templateInfoModel.ShowText,
                ShowAttack = templateInfoModel.ShowAttack,
                ShowDefense = templateInfoModel.ShowDefense,
                ShowDiscipline = templateInfoModel.ShowDiscipline,
                ShowArt = templateInfoModel.ShowArt,
                ShowInfo = templateInfoModel.ShowInfo,
                MaxTextBoxWidth = templateInfoModel.MaxTextBoxWidth,
                MaxTextBoxHeight = templateInfoModel.MaxTextBoxHeight,
            };
           
            return result.SyncBase(templateInfoModel);
        }
     
    }
}
