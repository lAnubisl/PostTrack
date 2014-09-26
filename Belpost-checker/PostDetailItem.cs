using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace belpost_checker
{
    public class PostDetailItem : IComparable<PostDetailItem>
    {
        private static CultureInfo provider = new CultureInfo("ru-RU");
        public PostDetailItem() { }
        internal PostDetailItem(Match match) 
        {
            Date = DateTime.Parse(match.Groups["date"].Value.Trim(), provider);
            Action = match.Groups["action"].Value.Trim();
            Place = match.Groups["place"].Value.Trim();
        }

        public DateTime Date { get; set; }
        public string Action { get; set; }
        public string Place { get; set; }

        public int CompareTo(PostDetailItem another)
        {
            if (another == null) return 1;
            return this.Date > another.Date ? 1 : -1;
        }
    }
}