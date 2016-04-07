using WTacticsDAL;
using WTacticsLibrary.Model;

namespace WTacticsLibrary.Assembler
{
    public static class RoleAssembler
    {
        public static Role FromDal(this RoleModel roleModel, bool includeRights = false)
        {
            if (roleModel == null) return null;
            var result = new Role
            {
                Id = roleModel.RoleId,
                Name = roleModel.Name,
            };
            if (includeRights)
            {
                roleModel.Rights.ForEach(x => result.Rights.Add(x.FromDal()));
            }
            return result.SyncBase(roleModel);
        }
    }
}
