using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using WTacticsDAL;

namespace WTacticsLibrary
{
    public static class SearchExtensions
    {
        public static T FindByGuid<T>(this DbSet<T> set, Guid guid) where T : ModelBase
        {
            return set?.SingleOrDefault(x => x.Guid == guid);
        }

        public static Task<T> FindByGuidAsync<T>(this DbSet<T> set, Guid guid) where T : ModelBase
        {
            return set?.SingleOrDefaultAsync(x => x.Guid == guid);
        }
    }
}
