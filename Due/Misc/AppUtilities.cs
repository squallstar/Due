using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Due
{
    public class AppUtilities
    {
        public static void SetMainViewIndex(DateTime dt)
        {
            var today = DateTime.Today;
            if (dt == today || DateTime.Compare(dt, today) <= 0) App.JumpToView = "today";
            else if (dt == today.AddDays(1)) App.JumpToView = "tomorrow";
            else App.JumpToView = "someday";
        }
    }
}
