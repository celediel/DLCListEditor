using System;
using System.Windows;

namespace DLCListEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static double VERSION = 1.10;
        public static string AUTHOR = "celediel";
        public static string EMAIL = "celediel813@gmail.com";
        public static Uri EMAILINK = new Uri($"mailto:{EMAIL}");
        public static Uri SOURCE = new Uri("https://gitlab.com/celediel/dlclisteditor");
    }
}
