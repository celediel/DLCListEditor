using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using System.Xml;
using System.Diagnostics;
using Microsoft.Win32;
//using CodeWalker.GameFiles;

namespace DLCListEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // I'm too lazy/dumb right now to make these not global
        private string gta5executable;
        private string gta5Directory;
        private string modsDirectory;
        private string vanillaDirectory;
        private Dictionary<string, DLCPack> dlcPacks;
        private bool isProcessed = false;
        private bool existingList = false;

        public MainWindow()
        {
            InitializeComponent();

            string defaultInstallLocation = "C:\\Program Files (x86)\\Steam\\SteamApps\\common\\Grand Theft Auto V\\";

            // disable saving from the start
            CanUserSave(false);
            OpenFromRpfItem.IsEnabled = false;
            SaveToRpfITem.IsEnabled = false;

            // default status bar
            statusBarGtavDir.Text = "GTAV directory not selected";
            statusBarGtavDirToolTip.Text = "GTAV directory not selected";
            statusBarLoadedXml.Text = "";
            statusBarLoadedXmlToolTip.Text = "No parsed XML file yet";

            // if the default Steam install directory exists, we'll use it
            if (Directory.Exists(defaultInstallLocation))
            {
                gta5executable = defaultInstallLocation + "GTA5.exe";
                ProcessGTAV(gta5executable);
            }
        }

        private void ProcessGTAV(string gtavExe)
        {
            // Create a new Dict every time unless we've opened an existing list
            if (!existingList)
                dlcPacks = new Dictionary<string, DLCPack>();

            gta5Directory = gtavExe.Replace("GTA5.exe", "");
            modsDirectory = gta5Directory + "mods\\update\\x64\\dlcpacks\\";
            vanillaDirectory = gta5Directory + "\\update\\x64\\dlcpacks\\";

            // First add all vanilla dlcpacks to the List with inModsDir = false for now
            // we'll change it later
            if (Directory.Exists(vanillaDirectory))
            {
                foreach (string dir in Directory.GetDirectories(vanillaDirectory))
                {
                    string name = dir.Split('\\').Last();
                    // check if it already exists in the list, or add a new item
                    try
                    {
                        dlcPacks[name].InVanillaDir = true;
                    }
                    catch (KeyNotFoundException)
                    {
                        dlcPacks.Add(name, new DLCPack(name, true, false));
                    }
                }

            }

            // now we'll process the mods directory
            if (Directory.Exists(modsDirectory))
            {
                foreach (var dir in Directory.GetDirectories(modsDirectory))
                {
                    string name = dir.Split('\\').Last();
                    // check if it already exists in the list, or add a new item
                    try
                    {
                        dlcPacks[name].InModsDir = true;
                    }
                    catch (KeyNotFoundException)
                    {
                        dlcPacks.Add(name, new DLCPack(name, false, true));
                    }
                }
            }

            // now we have our dlcPacks dict populated!!!

            if (!existingList)
            {
                foreach (var item in dlcPacks)
                {
                    item.Value.InDlcList = true;
                }
            }

            dlcGrid.ItemsSource = dlcPacks.Values.ToList();
            statusBarGtavDir.Text = "GTAV directory loaded";
            statusBarGtavDirToolTip.Text = gta5Directory;
            // If we've made it this far, all should be well
            isProcessed = true;
            CanUserSave(true);
        }

        private void CanUserSave(bool canSave)
        {
            if (canSave)
            {
                NewDLCListItem.IsEnabled = true;
                //SaveToRpfITem.IsEnabled = true;
                ClearMenuItem.IsEnabled = true;
            }
            else
            {
                NewDLCListItem.IsEnabled = false;
                SaveToRpfITem.IsEnabled = false;
                ClearMenuItem.IsEnabled = false;
            }
        }

        // lambda quit oh yeah
        private void CloseMenuItem_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void SelectFolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Title = "Select GTA5.exe",
                Filter = "GTA 5 Executable|GTA5.exe"
            };
            if (openFile.ShowDialog() == true)
            {
                gta5executable = openFile.FileName;
                ProcessGTAV(gta5executable);
            }
        }

        private void OpenDLCListItem_Click(object sender, RoutedEventArgs e)
        {
            string filename;
            XmlDocument xmlDocument = new XmlDocument();
            char dirSplit = '\\';

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "XML Document (*.xml)|*.xml"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                filename = openFileDialog.FileName;

                string dlcName;
                string[] wholePath;
                if (isProcessed)
                {
                    // if we've already selected the GTA5 directory, set InDlcList to false for
                    // every existing dlcpack, so we can have a clean slate to load our XML into
                    foreach (var item in dlcPacks)
                    {
                        item.Value.InDlcList = false;
                    }
                }
                else
                {
                    // otherwise, we'll create a new list of the dlcpacks
                    dlcPacks = new Dictionary<string, DLCPack>();
                }

                // now the actual XML work begins
                xmlDocument.Load(filename);

                // but let's check if it's even a good dlclist.xml
                //if(xmlDocument.FirstChild.InnerXml)
                if (xmlDocument.DocumentElement.Name != "SMandatoryPacksData")
                {
                    MessageBox.Show("This doesn't appear to be a valid dlclist.xml??", "Invalid!", MessageBoxButton.OK);
                    return;
                }

                foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes[0].ChildNodes)
                {
                    if (xmlNode.InnerText.Contains("/"))
                        dirSplit = '/';
                    wholePath = xmlNode.InnerText.Split(dirSplit);
                    // if the path ends with / or \, the last value in the split array will be an empty string
                    if (wholePath.Length >= 2 && wholePath.Last() == "")
                    {
                        dlcName = wholePath[wholePath.Count() - 2];
                    }
                    // if the path doesn't end with / or \, the last value will be what we want
                    else
                    {
                        dlcName = wholePath.Last();
                    }
                    // ignore the <Item> tags with platform:/whatever in them
                    if (wholePath.First() != "platform:")
                    {
                        if (isProcessed)
                        {
                            try
                            {
                                dlcPacks[dlcName].InDlcList = true;
                            }
                            catch (KeyNotFoundException)
                            {
                                dlcPacks.Add(dlcName, new DLCPack(dlcName, false, false, true));
                            }
                        }
                        else
                        {
                            dlcPacks.Add(dlcName, new DLCPack(dlcName, false, false, true));
                        }
                    }
                }

                MessageBox.Show($"Opened {filename}!");
                dlcGrid.ItemsSource = dlcPacks.Values.ToList();
                statusBarLoadedXml.Text = filename.Split('\\').Last();
                statusBarLoadedXmlToolTip.Text = filename;
                existingList = true;
                CanUserSave(true);
            }
        }

        private void NewDLCListItem_Click(object sender, RoutedEventArgs e)
        {
            XmlDocument xmlDocument= new XmlDocument();
            XmlNode itemNode;

            // these are the <Item>platform>/dlcPacks/whatever/</Item> entries
            // I don't think they change so there's no reason not to hard code them
            string[] platforms = new string[]
            {
            "mpBeach", "mpBusiness", "mpChristmas", "mpValentines", "mpBusiness2", "mpHipster", "mpIndependence", "mpPilot", "spUpgrade", "mpLTS"
            };

            if (isProcessed)
            {
                // do the stuff
                string filename;
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    CheckPathExists = true,
                    Filter = "XML Document (*.xml)|*.xml"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    filename = saveFileDialog.FileName;
                    XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);

                    XmlNode smpdNode = xmlDocument.CreateElement("SMandatoryPacksData");
                    XmlNode pathNode = xmlDocument.CreateElement("Paths");

                    // Add the default platform:/ entries
                    foreach (var item in platforms)
                    {
                        itemNode = xmlDocument.CreateElement("Item");
                        itemNode.InnerText = $"platform:/dlcPacks/{item}/";
                        pathNode.AppendChild(itemNode);
                    }

                    foreach (var item in dlcPacks.Values.ToList())
                    {
                        if (item.InDlcList)
                        {
                            itemNode = xmlDocument.CreateElement("Item");
                            itemNode.InnerText = $"dlcpacks:/{item.ModName}/";
                            pathNode.AppendChild(itemNode);

                        }
                    }
                    smpdNode.AppendChild(pathNode);
                    xmlDocument.AppendChild(smpdNode);
                    xmlDocument.InsertBefore(xmlDeclaration, smpdNode);

                    xmlDocument.Save(filename);
                    MessageBox.Show($"{filename} written!");
                }
            }
            else
            {
                // theoretically we shouldn't ever get here because the button will be disabled
                // so maybe I should remove the whole if/else statement...
                MessageBox.Show("Select the GTA5 first probably");
            }
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Show();
        }

        private void OpenFromRpfItem_Click(object sender, RoutedEventArgs e)
        {
            // todo
            //RpfFile updateRpf;

        }

        private void SaveToRpfITem_Click(object sender, RoutedEventArgs e)
        {
            // also todo
            //RpfFile updateRpf;

        }

        private void ClearMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // clear the dlcpacks list, and reset the datagrid
            dlcPacks = new Dictionary<string, DLCPack>();
            dlcGrid.ItemsSource = dlcPacks.Values.ToList();

            // reset status bar
            statusBarGtavDir.Text = "GTAV directory not selected";
            statusBarGtavDirToolTip.Text = "GTAV directory not selected";
            statusBarLoadedXml.Text = "";
            statusBarLoadedXmlToolTip.Text = "No parsed XML file yet";

            // reset variables
            existingList = false;
            CanUserSave(false);

            // ask user if they want to reload GTA dir
            if (isProcessed)
            {
                MessageBoxResult result = MessageBox.Show("Reload same GTA5 directory?", "Alert!", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                    ProcessGTAV(gta5executable);
            }
            else
            {
                isProcessed = false;
            }
        }

        private void ReadmeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://gitlab.com/celediel/dlclisteditor/blob/master/README.md");
        }
    }
}
