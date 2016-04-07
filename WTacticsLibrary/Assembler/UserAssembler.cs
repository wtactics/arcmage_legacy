using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WTacticsDAL;
using WTacticsLibrary.Model;

namespace WTacticsLibrary.Assembler
{
    public static class UserAssembler
    {
        public static User FromDal(this UserModel userModel, bool includeRole = false, bool includeDecks = false, bool includeCards = false)
        {
            if (userModel == null) return null;
            var result = new User
            {
                Id = userModel.UserId,
                Name = userModel.Name,
                Email = userModel.Email,
                LastLoginTime = userModel.LastLoginTime,
                CreateTime = userModel.CreateTime
                
            };
            if (includeRole)
            {
                result.Role = userModel.Role.FromDal(true);
            }

            if (includeDecks)
            {
                userModel.Decks.ForEach(x => result.Decks.Add(x.FromDal()));
            }
            if (includeCards)
            {
                userModel.Cards.ForEach(x=> result.Cards.Add(x.FromDal()));
            }
            return result;
        }
    }
}
