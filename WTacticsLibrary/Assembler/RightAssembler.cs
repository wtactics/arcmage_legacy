using WTacticsDAL;
using WTacticsLibrary.Model;

namespace WTacticsLibrary.Assembler
{
    public static class RightAssembler
    {
        public static Right FromDal(this RightModel rightModel)
        {
            if (rightModel == null) return null;
            var result = new Right
            {
                Id = rightModel.RightId,
                Name = rightModel.Name,
            };
            return result.SyncBase(rightModel);
        }
    }
}
