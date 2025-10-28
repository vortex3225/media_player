using Media_Player.Objects;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Media_Player
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //public Theme current_theme = Theme.Light;
        //public void SwitchTheme()
        //{
        //    if (current_theme == Theme.Light)
        //        current_theme = Theme.Dark;
        //    else
        //        current_theme = Theme.Light;

        //    Application.Current.Resources.MergedDictionaries[1].Source = new Uri($"/Themes/{current_theme.ToString()}.xaml", UriKind.RelativeOrAbsolute);
        //    Console.WriteLine("Changed theme to" + current_theme.ToString());
        //}
        public string current_theme = "Light";
        public string to_ret = "Light";
        public void SwitchTheme()
        {
            if (current_theme == "Light")
                current_theme = "Dark";
            else
                current_theme = "Light";

            to_ret = (current_theme == "Light" ? "/Light/" : "/Dark/");
            Application.Current.Resources.MergedDictionaries[1].Source = new Uri($"/Themes/{current_theme}.xaml", UriKind.RelativeOrAbsolute);

            MainWindow mw = Application.Current.MainWindow as MainWindow;

            mw.SwitchPlayColor(to_ret);
            CompactWindow? compactWind = mw.GetCompactWindowInstance();
            if (compactWind != null)
                compactWind.SwitchPlayColor(to_ret);
        }
    }

}
