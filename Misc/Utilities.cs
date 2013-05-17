using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Due
{
    class Utilities
    {
        public static string TimeAgo(DateTime since, string prefix = "updated")
        {
            //if (prefix == "updated") prefix = AppResources.updated;

            TimeSpan diff = DateTime.Now.Subtract(since);
            if (diff.TotalDays >= 1)
            {
                if (diff.TotalDays >= 2)
                {
                    return String.Format("{0} {1} {2} {3}", prefix, (int)diff.TotalDays, "days", "ago");
                }
                else
                {
                    return String.Format("{0} 1 {1} {2}", prefix, "day", "ago");
                }

            }
            else
            {
                if (diff.TotalHours >= 1)
                {
                    if (diff.TotalHours >= 2)
                    {
                        return String.Format("{0} {1} {2} {3}", prefix, (int)diff.TotalHours, "hours", "ago");
                    }
                    else
                    {
                        return String.Format("{0} 1 {1} {2}", prefix, "hour", "ago");
                    }
                }
                else
                {
                    if (diff.TotalMinutes >= 2)
                    {
                        return String.Format("{0} {1} {2} {3}", prefix, (int)diff.TotalMinutes, "minutes", "ago");
                    }
                    else
                    {
                        return prefix + (prefix != "" ? " " : "") + "just added";
                    }
                }
            }
        }
    }
}
