using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTacticsDAL
{
    public class UserModel
    {

        [Key]
        public int UserId { get; set; }

        public Guid Guid { get; set; }

        public DateTime CreateTime { get; set; }
        
        public DateTime LastLoginTime { get; set; }

        public string Token { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public virtual RoleModel Role { get; set; }

        public virtual List<CardModel> Cards { get; set; }

        public virtual List<DeckModel>  Decks { get; set; }


    }
}
