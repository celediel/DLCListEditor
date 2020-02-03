using System.Diagnostics;
using System.IO;
using System.Windows;
using Markdig.Wpf;

namespace DLCListEditor
{
    /// <summary>
    /// Interaction logic for ReadMeWindow.xaml
    /// </summary>
    public partial class ReadMeWindow : Window
    {
        public ReadMeWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Viewer.Markdown = File.ReadAllText("README.md");
        }

        private void OpenHyperlink(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            Process.Start(e.Parameter.ToString());
        }

        private void ClickOnImage(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            MessageBox.Show($"URL: {e.Parameter}");
        }
    }
}
