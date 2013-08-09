using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Windows;

namespace Due.Data
{
    public class Context : DataContext
    {
        public Context()
            : base("Data Source=isostore:/metroboard.sdf")
        { }

        public Table<Todo> todos;

        public static Context Current
        {
            get
            {
                return (Application.Current as App).db;
            }
        }

        public void UpdateTileCount()
        {
            ShellTile appTile = ShellTile.ActiveTiles.First();
            if (appTile != null)
            {
                var count = (from Todo item in todos where item.Completed == false && item.DueDate <= DateTime.Today select item).Count();

                var firstItem = (from Todo s in todos where s.Completed == false && s.DueDate <= DateTime.Today orderby s.DateInsert select s).FirstOrDefault();

                StandardTileData newTile = new StandardTileData
                {
                    Title = "Due",
                    BackgroundImage = new Uri("Background.png", UriKind.Relative),
                    Count = count
                };

                if (firstItem == null)
                {
                    newTile.BackBackgroundImage = new Uri("Background.png", UriKind.Relative);
                    newTile.BackContent = "";
                    newTile.BackTitle = "Nothing to do";
                }
                else
                {
                    newTile.BackBackgroundImage = new Uri("/Resources/BackBackground.png", UriKind.Relative);
                    newTile.BackContent = firstItem.Title;
                    newTile.BackTitle = "Due today";

                    int length = 17;
                    string title = firstItem.Title.Length > length ? firstItem.Title.Substring(0, length-1) + "..." : firstItem.Title;
                    newTile.Title = title;

                    if (count > 1)
                    {
                        var secondItem = (from Todo s in todos where s.Completed == false && s.DueDate <= DateTime.Today && s.ID != firstItem.ID orderby s.DateInsert select s).FirstOrDefault();
                        if (secondItem != null)
                        {
                            newTile.BackBackgroundImage = null;
                            newTile.BackContent = firstItem.Title + "\r\n" + secondItem.Title;
                            newTile.BackTitle = count > 2 ? "+" + (count-2) + " more" :  "Due today";
                        }

                    }
                }

                appTile.Update(newTile);
            }
        }
    }
}
