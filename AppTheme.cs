using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GitRecon
{
    public class AppTheme
    {
        public static void ChangeTheme(Uri themeuri)
        {
            ResourceDictionary Theme = new ResourceDictionary() { Source = themeuri };

            App.Current.Resources.Clear();
            App.Current.Resources.MergedDictionaries.Add(Theme);
        }
    }
}
