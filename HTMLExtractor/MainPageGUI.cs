/*

To update the GUI Main Window, call functions from here

*/


using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace HTMLExtractor
{
    public partial class MainWindow
    {
        
        private void GUITopOrAll_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(LocalFolderFiles==null)
            {
                //no folder loaded
                return;
            }
            else
            {
                //folder loaded, re-load the same folder with either top-level or full-files selection
                //if (IsOnlyTopLevelFiles())
                //    LocalFolderFiles = Directory.GetFiles(SelectedFolderPath, "*.*", SearchOption.TopDirectoryOnly).Where(s => s.ToLower().EndsWith(".txt") || s.ToLower().EndsWith(".html") || s.ToLower().EndsWith(".htm"));
                //else
                //    LocalFolderFiles = Directory.GetFiles(SelectedFolderPath, "*.*", SearchOption.AllDirectories).Where(s => s.ToLower().EndsWith(".txt") || s.ToLower().EndsWith(".html") || s.ToLower().EndsWith(".htm"));

                //FolderArea.Text = LocalFolderFiles.Count() + " htm/html/txt files found in the folder." + Environment.NewLine + Environment.NewLine;
                //UpdateSelectedFolder(SelectedFolderPath);
                //UpdateNumberOfFiles(LocalFolderFiles.Count());
            }
        }
    }


}