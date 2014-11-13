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

            return Object.Equals(this.Date, another.Date) &&
                Object.Equals(this.Action, another.Action) &&
                Object.Equals(this.Place, another.Place);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 24;
                hash = hash * 17 + this.Date.GetHashCode();
                hash = hash * 17 + this.Action.GetHashCode();
                hash = hash * 17 + this.Place.GetHashCode();
                return hash;
            }
        }
    }
}