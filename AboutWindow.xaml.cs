using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DLCListEditor
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            versionText.Text = $"v{App.VERSION:0.00}";
            emailLink.NavigateUri = App.EMAILINK;
            emailText.Text = App.EMAIL.ToString(); ;
            sourceLink.NavigateUri = App.SOURCE;
            authorText.Text = App.AUTHOR;
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Debug.WriteLine($"Launching {e.Uri.AbsoluteUri}");
            Process.Start(e.Uri.AbsoluteUri);
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            LicenseWindow window = new LicenseWindow
            {
                Owner = this
            };
            window.Show();
        }
    }
}