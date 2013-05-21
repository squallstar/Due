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
                }

                appTile.Update(newTile);
            }
        }
    }
}
