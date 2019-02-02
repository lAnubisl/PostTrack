using System;

namespace Posttrack.Data.Interfaces.DTO
{
    public class PackageHistoryItemDTO
    {
        public DateTime Date { get; set; }

        public string Action { get; set; }

        public string Place { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return Equals(obj as PackageHistoryItemDTO);
        }

        public bool Equals(PackageHistoryItemDTO another)
        {
            if (another == null)
            {
                return false;
            }

            return
                Equals(Action, another.Action) &&
                Equals(Place, another.Place);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 24;
                hash = (hash * 17) + Action.GetHashCode();
                hash = (hash * 17) + Place.GetHashCode();
                return hash;
            }
        }
    }
}