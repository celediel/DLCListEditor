using System.Diagnostics;
using System.Windows;

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