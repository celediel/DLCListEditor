﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using System.Xml;
//using CodeWalker.GameFiles;
using System.Diagnostics;

namespace DLCListEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string gta5executable;
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

        // these are the <Item>platform>/dlcPacks/whatever/</Item> entries
        // I don't think they change so there's no reason not to hardcode them
        private string[] platforms = new string[]
        {
            "mpBeach", "mpBusiness", "mpChristmas", "mpValentines", "mpBusiness2", "mpHipster", "mpIndependence", "mpPilot", "spUpgrade", "mpLTS"
        };

        //RpfFile updateRpf;

        public MainWindow()
        {
            InitializeComponent();
            // disable saving from the start
            CanUserSave(false);
            //OpenFromRpfItem.IsEnabled = false;

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

            if (!existingList)
            {
                foreach (var item in dlcPacks)
                {
                    item.Value.InDlcList = true;
                }
            }

            dlcGrid.ItemsSource = dlcPacks.Values.ToList();
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
                //SaveToRpfITem.IsEnabled = false;
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
                xmlDocument = new XmlDocument();
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
                existingList = true;
                CanUserSave(true);
            }
        }

        private void NewDLCListItem_Click(object sender, RoutedEventArgs e)
        {
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
            else
            {
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
        }

        private void SaveToRpfITem_Click(object sender, RoutedEventArgs e)
        {
            // also todo
        }

        private void ClearMenuItem_Click(object sender, RoutedEventArgs e)
        {
            dlcPacks = new Dictionary<string, DLCPack>();
            dlcGrid.ItemsSource = dlcPacks.Values.ToList();
            existingList = false;
            CanUserSave(false);
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
    }
}