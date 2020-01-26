using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace DLCListEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string gta5Directory;
        private string modsDirectory;
        private string vanillaDirectory;
        private Dictionary<string, DLCPack> dlcPacks;
        private readonly string defaultInstallLocation = "C:\\Program Files (x86)\\Steam\\SteamApps\\common\\Grand Theft Auto V\\";
        private bool isProcessed = false;
        private bool existingList = false;
        private char dirSplit = '\\';

        private XmlDocument xmlDocument;
        private XmlNode itemNode;

        private string[] platforms = new string[]
        {
            "mpBeach", "mpBusiness", "mpChristmas", "mpValentines", "mpBusiness2", "mpHipster", "mpIndependence", "mpPilot", "spUpgrade", "mpLTS"
        };
        public MainWindow()
        {
            InitializeComponent();
            if (Directory.Exists(defaultInstallLocation))
            {
                isProcessed = ProcessGTAV(defaultInstallLocation + "GTA5.exe");
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
                Filter = "GTA 5 Executable|GTA5.exe"
            };
            if (openFile.ShowDialog() == true)
            {
                isProcessed = ProcessGTAV(openFile.FileName);
            }
        }

        private bool ProcessGTAV(string gtavExe)
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
                    try
                    {
                        dlcPacks[name].InVanillaDir = true;
                    }
                    catch (KeyNotFoundException)
                    {
                        dlcPacks.Add(name, new DLCPack(name, true, false));
                    }
                    //Debug.WriteLine($"Found {name} in vanilla directory");
                }

            }

            if (Directory.Exists(modsDirectory))
            {
                foreach (var dir in Directory.GetDirectories(modsDirectory))
                {
                    string name = dir.Split('\\').Last();
                    try
                    {
                        dlcPacks[name].InModsDir = true;
                        //Debug.WriteLine($"Found {name} in both directories!");
                    }
                    catch (KeyNotFoundException)
                    {
                        dlcPacks.Add(name, new DLCPack(name, false, true));
                        //Debug.WriteLine($"Found {name} in mods directory");
                    }
                }
            }

            // now we have our dlcPacks dict populated!!!

            //foreach (var item in dlcPacks)
            //{
            //    string output = item.Value.ModName;
            //    if (item.Value.InVanillaDir)
            //        output += " (VANILLA)";
            //    if (item.Value.InModsDir)
            //        output += " (MODS)";
            //    Debug.WriteLine(output);
            //}
            if (!existingList)
            {
                foreach (var item in dlcPacks)
                {
                    item.Value.InDlcList = true;
                }
            }

            dlcGrid.ItemsSource = dlcPacks.Values.ToList();
            return true;
        }

        private void OpenDLCListItem_Click(object sender, RoutedEventArgs e)
        {
            string filename;

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "XML Document (*.xml)|*.xml"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                filename = openFileDialog.FileName;
                xmlDocument = new XmlDocument();
                xmlDocument.Load(filename);

                string dlcName;
                string[] wholePath;
                if (!isProcessed)
                {
                    dlcPacks = new Dictionary<string, DLCPack>();
                }
                else
                {

                    foreach (var item in dlcPacks)
                    {
                        item.Value.InDlcList = false;
                    }
                }
                foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes[0].ChildNodes)
                {
                    //Debug.WriteLine(xmlNode.InnerText);
                    if (xmlNode.InnerText.Contains("/"))
                        dirSplit = '/';
                    wholePath = xmlNode.InnerText.Split(dirSplit);
                    if (wholePath.Length >= 2 && wholePath.Last() == "")
                    {
                        dlcName = wholePath[wholePath.Count() - 2];
                    }
                    else
                    {
                        dlcName = wholePath.Last();
                    }
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
                                dlcPacks.Add(dlcName, new DLCPack(dlcName, false, false));
                                dlcPacks[dlcName].InDlcList = true;
                            }
                        }
                        else
                        {
                            dlcPacks.Add(dlcName, new DLCPack(dlcName, false, false));
                            dlcPacks[dlcName].InDlcList = true;
                        }
                    }
                }

                MessageBox.Show($"Opened {filename}!");
                dlcGrid.ItemsSource = dlcPacks.Values.ToList();
                existingList = true;
            }
        }

        private void NewDLCListItem_Click(object sender, RoutedEventArgs e)
        {
            if (!isProcessed)
            {
                MessageBox.Show("Select the GTA5 first probably");
            }
            else
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
                    xmlDocument = new XmlDocument();
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
        }

        private void selectAllDlcList_Click(object sender, RoutedEventArgs e)
        {
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Show();
        }
    }
}
