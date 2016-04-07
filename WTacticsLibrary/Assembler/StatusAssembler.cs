using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WTacticsDAL;
using WTacticsLibrary.Model;

namespace WTacticsLibrary.Assembler
{
    public static class StatusAssembler
    {
        public static Status FromDal(this StatusModel statusModel)
        {
            if (statusModel == null) return null;
            var result = new Status()
            {
                Id = statusModel.StatusId,
                Name = statusModel.Name
            };
            return result.SyncBase(statusModel);
        }

        public static void Patch(this StatusModel statusModel, Status status, UserModel user)
        {
            if (statusModel == null) return;
            statusModel.Name = status.Name;
            statusModel.Patch(user);
        }
    }
}
