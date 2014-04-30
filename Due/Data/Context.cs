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

        /// <summary>
        /// THIS METHOD SHOULD ALSO BE COPIED IN THE AGENT
        /// </summary>
        public void UpdateTileCount()
        {
            ShellTile appTile = ShellTile.ActiveTiles.First();
            if (appTile != null)
            {
                var items = (from Todo item in todos where item.Completed == false && item.DueDate <= DateTime.Today orderby Guid.NewGuid() select item).ToList();
                var count = items.Count;

                IconicTileData newTile = new IconicTileData
                {
                    Title = "Due",
                    Count = count,
                    SmallIconImage = new Uri("/Resources/iconic-small.png", UriKind.Relative),
                    IconImage = new Uri("/Resources/iconic.png", UriKind.Relative),
                    BackgroundColor = new System.Windows.Media.Color{ A = 0, R = 0, G = 0, B = 0 }
                };

                if (count > 0)
                {
                    newTile.WideContent1 = items[0].Title;
                }

                if (count > 1)
                {
                    newTile.WideContent2 = items[1].Title;
                }

                if (count > 2)
                {
                    newTile.WideContent3 = items[2].Title;
                }

                appTile.Update(newTile);
            }
        }
    }
}
