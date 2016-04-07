using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTacticsLibrary
{
    public static class PredefinedGuids
    {
        // roles
        public static Guid Role = GuidUtility.Create("Role");

        public static Guid Administrator = GuidUtility.Create(Role, "Administrator");

        public static Guid Developer = GuidUtility.Create(Role, "Developer");
        
        public static Guid Contributer = GuidUtility.Create(Role, "Contributer");

        public static Guid ServiceUser = GuidUtility.Create(Role, "ServiceUser");

        // rights
        // no rights defined yet

        // status of cards and series
        public static Guid Status = GuidUtility.Create("Status");

        public static Guid Draft = GuidUtility.Create(Status, "Draft");

        public static Guid Final = GuidUtility.Create(Status, "Final");

        // card types
        public static Guid CardType = GuidUtility.Create("CardType");

        public static Guid NoCardType = GuidUtility.Create(CardType, "NoCardType");

        public static Guid Creature = GuidUtility.Create(CardType, "Creature");

        public static Guid Event = GuidUtility.Create(CardType, "Event");

        public static Guid Equipment = GuidUtility.Create(CardType, "Equipment");

        public static Guid Magic = GuidUtility.Create(CardType, "Magic");

        public static Guid Enchantment = GuidUtility.Create(CardType, "Enchantment");

        public static Guid City = GuidUtility.Create(CardType, "City");

        public static Guid Player = GuidUtility.Create(CardType, "Player");


        // factions
        public static Guid Faction = GuidUtility.Create("Faction");

        public static Guid NoFaction = GuidUtility.Create(Faction, "NoFaction");

        public static Guid Gaian = GuidUtility.Create(Faction, "Gaian");

        public static Guid DarkLegion = GuidUtility.Create(Faction, "DarkLegion");

        public static Guid RedBanner = GuidUtility.Create(Faction, "RedBanner");

        public static Guid HouseOfNobels = GuidUtility.Create(Faction, "HouseOfNobels");

        public static Guid Empire = GuidUtility.Create(Faction, "Empire");

        // series
        public static Guid Serie = GuidUtility.Create("Serie");

        public static Guid NoSerie = GuidUtility.Create(Serie, "NoSerie");

        public static Guid Rebirth = GuidUtility.Create(Serie, "Rebirth");



    }
}
