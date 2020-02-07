using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace DLCListEditor
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        private int buttonCounter = 0;
        private int buttonCounter2 = 0;
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

        // I got bored and maybe a little stoned, don't ask why I left it here
        private void SneakyButton_Click(object sender, RoutedEventArgs e)
        {
            //switch (buttonCounter)
            //{
            //    case 0:
            //        ((Button)sender).Content = "BUTTON";
            //        break;
            //    case 1:
            //    case 2:
            //    case 3:
            //    case 4:
            //    case 5:
            //    case 6:
            //    case 7:
            //    case 8:
            //    case 9:
            //        ((Button)sender).Content += "!";
            //        ((Button)sender).FontSize += 1;
            //        break;
            //    default:
            //        buttonCounter = 0;
            //        ((Button)sender).Content = "button";
            //        ((Button)sender).FontSize = 8;
            //        break;
            //}
            if (buttonCounter <= 4)
            {
                ((Button)sender).Content += "!";
                //((Button)sender).FontSize += 0.5;
            }
            else
            {
                buttonCounter = 0;
                switch (buttonCounter2)
                {
                    case 0:
                        ((Button)sender).Content = "why";
                        break;
                    case 1:
                        ((Button)sender).Content = "stop";
                        break;
                    case 2:
                        ((Button)sender).Content = "pls";
                        break;
                    case 3:
                        ((Button)sender).Content = "ok";
                        break;
                    default:
                        ((Button)sender).Content = "no";
                        buttonCounter2 = -1;
                        break;
                }
                buttonCounter2++;
                //((Button)sender).FontSize = 8;
            }
            buttonCounter++;
        }
    }
}