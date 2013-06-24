using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Due
{
    class Utilities
    {
        public static DateTime Someday
        {
            get
            {
                return DateTime.Today.AddDays(60);
            }
        }

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
                        return prefix + (prefix != "" ? " " : "") + "few seconds ago";
                    }
                }
            }
        }

        public static void SetMainViewIndex(DateTime dt)
        {
            var today = DateTime.Today;
            if (dt == today || DateTime.Compare(dt, today) <= 0) App.JumpToView = "today";
            else if (dt == today.AddDays(1)) App.JumpToView = "tomorrow";
            else App.JumpToView = "someday";
        }

        public static string TimeInFuture(DateTime when, string prefix = "due")
        {
            TimeSpan diff = when.Subtract(DateTime.Now);

            if (diff.TotalDays > 7)
            {
                return String.Format("{0} on {1}", prefix, when.ToString("MMM d"));
            }
            else if (diff.TotalDays >= 2)
            {
                return String.Format("{0} in {1} {2}", prefix, (int)diff.TotalDays, "days");
            }
            else if (diff.TotalDays == 1)
            {
                return String.Format("{0} tomorrow", prefix);
            }
            else
            {
                return String.Format("{0} today", prefix);
            }
        }
    }
}
