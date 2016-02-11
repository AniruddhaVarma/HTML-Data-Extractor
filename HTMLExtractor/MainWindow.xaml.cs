/*
 * Target Data : 1) HTML Element's inner content 2)  HTML attribute's value
 * 
 * Filters to pinpoint right data
 * ================================
 * Target Data 1 -> 1) The HTML Element MUST have an attribute named 'x'
 *  
*/

using HtmlAgilityPack;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// regex pattern searcher
        /// </summary>
        private BackgroundWorker workerRegexPattern;

        /// <summary>
        /// When loading from local folder, all file paths inside are stored here
        /// </summary>
        static IEnumerable<string> LocalFolderFiles=null;

        /// <summary>
        /// Full path to local folder loaded (used to re-load folder files when top-level/deep file selectin changed)
        /// </summary>
        string SelectedFolderPath = null;

        public MainWindow()
        {
            InitializeComponent();

        }

        private void PressNetExtraction(object sender, RoutedEventArgs e)
        {

            HtmlDocument doc = new HtmlDocument();
            //doc.LoadHtml(File.ReadAllText("f:\\Projects\\1.Random\\htmlpage.txt"));
            doc.LoadHtml(File.ReadAllText(@"f:\research\Australian Kickstarter _ How to use Kickstarter Outside the USA _ robyward.com.htm"));
            var inText = doc.DocumentNode.Descendants("link").Select(x => x.ChildAttributes("href").First());
            MessageBox.Show(inText.Count()+" results found.");
            SourceArea.Text = "";
            foreach(var innerTag in inText)
            {
                SourceArea.Text += innerTag.Value;
                SourceArea.Text += Environment.NewLine + "---" + Environment.NewLine;
            }

            return;


            WebClient web = new WebClient();
            try
            {
                string webSource = web.DownloadString(AddressBar.Text);
            }
            catch(Exception exc)
            {
                SourceArea.Text = exc.ToString() + exc.Message;
            }
        }

        /// <summary>
        /// Reload folder path from textbox, and re-calculate number of files
        /// Happens in Browse; top-level or all-files changed; when directly clicked on button
        /// </summary>
        private void LoadFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(GUIPathToFolder.Text=="")
                {
                    ShowError("No folder currently selected. Please Browse to a folder first.");
                    return;
                }

                SelectedFolderPath = GUIPathToFolder.Text;

                //Calculate number of files based on whether top-level only or all-files
                if (IsTopLevelFilesOnly())
                    LocalFolderFiles = Directory.GetFiles(SelectedFolderPath, "*.*", SearchOption.TopDirectoryOnly).Where(s => s.ToLower().EndsWith(".txt") || s.ToLower().EndsWith(".html") || s.ToLower().EndsWith(".htm"));
                else
                    LocalFolderFiles = Directory.GetFiles(SelectedFolderPath, "*.*", SearchOption.AllDirectories).Where(s => s.ToLower().EndsWith(".txt") || s.ToLower().EndsWith(".html") || s.ToLower().EndsWith(".htm"));

                GUINumberOfFiles.Text = LocalFolderFiles.Count().ToString() + " files";

                //Now 

            }
            catch(Exception exc)
            {
                ShowError(exc.Message);
            }
        }


        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (LocalFolderFiles == null)
                {
                    //not loaded folder yet
                    return;
                }
                FolderArea.Text = "";
                foreach (string FilePath in LocalFolderFiles)
                {
                    string FileContents = File.ReadAllText(FilePath);
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(FileContents);
                    //IEnumerable<string> innerData = doc.DocumentNode.Descendants(TargetElement.Text).Select(x => x.InnerHtml);
                    //if (innerData.Count() == 0)
                    //{
                    //    //no results with selected HTML Element
                    //    continue;
                    //}
                    //FolderArea.Text += "===Results from file [" + FilePath + "]===";
                    //foreach (string ResultLoop in innerData)
                    //{
                    //    FolderArea.Text += ResultLoop;
                    //}
                    //FolderArea.Text += Environment.NewLine + "===End Of File===" + Environment.NewLine;
                }
            }
            catch(Exception exc)
            {
                FolderArea.Text += exc.Message;
            }
        
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<WebPage> links = WebPage.GetAllPagesUnder(new Uri("http://www.cell-phonestores.com/stores/newyork/Cooperstown.htm"));
            int count = links.Count();
            MessageBox.Show(count.ToString());
            ;
        }

        async private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //Search for the regex pattern in each file in LocalFolderFiles
            //Perform the search on a BackGroundWorker to keep the UI free
            BackgroundWorker RegexSearcher = new BackgroundWorker();
            RegexSearcher.DoWork += RegexSearcher_DoWork;
            RegexSearcher.ProgressChanged += (s, args) => { FolderArea.Text = args.ProgressPercentage.ToString(); };
            RegexSearcher.WorkerReportsProgress = true;
            RegexSearcher.RunWorkerCompleted += (s, args) => { FolderArea.Text = "Completed"; };
            RegexSearcher.RunWorkerAsync("<a.*");
        }

        /// <summary>
        /// Done on background thread. Loop through all files, check for regex matches and keep
        /// adding to a string. The number of files checked is reported regularly with ReportProgress(i).
        /// (Pass the file paths in selected folder as strings)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RegexSearcher_DoWork(object sender, DoWorkEventArgs e)
        {
            string ResultText = "";
            try
            {
                string RegexPattern = e.Argument as string;
                int totalNumberOfFiles = MainWindow.LocalFolderFiles.Count();
                int currentFile = 0;

                
                foreach (string filePath in MainWindow.LocalFolderFiles)
                {
                    if (workerRegexPattern.CancellationPending)
                    {
                        e.Result = ResultText;
                        e.Cancel = true;
                        return;
                    }


                    string fileContents = File.ReadAllText(filePath);
                    List<Match> Matches = Extensions.MultipleRegexMatches(RegexPattern, fileContents);
                    foreach (Match matchLoop in Matches)
                    {
                        ResultText += matchLoop.Value + Environment.NewLine;
                    }
                    System.Threading.Thread.Sleep(10);
                    ((BackgroundWorker)sender).ReportProgress(++currentFile);
                    
                }

                e.Result = ResultText;
            }
            catch(Exception exc)
            {
                e.Result = ResultText;
                e.Cancel = true;
            }
        }

        void InBetweenStrings_DoWork(object sender, DoWorkEventArgs e)
        {
            //arg1 string 1
            //arg2 string 2
            List<object> args = e.Argument as List<object>;
            string strLeft = args[0] as string;
            string strRigt = args[1] as string;

            string ResultText = "";
            try
            {
                string RegexPattern = strLeft + "((.|\n)*)" + strRigt;
                //string RegexPattern = strLeft + ".*" + strRigt;
                int totalNumberOfFiles = MainWindow.LocalFolderFiles.Count();
                int currentFile = 0;


                foreach (string filePath in MainWindow.LocalFolderFiles)
                {
                    if (workerRegexPattern.CancellationPending)
                    {
                        e.Result = ResultText;
                        e.Cancel = true;
                        return;
                    }


                    string fileContents = File.ReadAllText(filePath);
                    List<Match> Matches = Extensions.MultipleRegexMatches(RegexPattern, fileContents);
                    foreach (Match matchLoop in Matches)
                    {
                        string Match = matchLoop.Value;
                        
                        Extensions.ReplacePatterns(strLeft, strRigt, ref Match);

                        if(strLeft!="") Match = Match.Replace(strLeft,"");
                        if(strRigt!="") Match = Match.Replace(strRigt, "");
                        ResultText += Match + Environment.NewLine;
                    }
                    System.Threading.Thread.Sleep(10);
                    ((BackgroundWorker)sender).ReportProgress(++currentFile);

                }

                e.Result = ResultText;
            }
            catch (Exception exc)
            {
                e.Result = ResultText;
                e.Cancel = true;
            }
        }


        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.FolderBrowserDialog selectFolder = new System.Windows.Forms.FolderBrowserDialog();
                System.Windows.Forms.DialogResult sel = selectFolder.ShowDialog();
                if (selectFolder.SelectedPath == "")
                {
                    //did not select
                    return;
                }

                SelectedFolderPath = selectFolder.SelectedPath;
                GUIPathToFolder.Text = SelectedFolderPath;

                //Use GetFiles() on the local folder
                LoadFolder_Click(null,null);
                
            }
            catch(Exception exc)
            {
                ShowError(exc.Message);   
            }
        }

        private bool IsTopLevelFilesOnly()
        {
            if ((bool)RadioTopLevel.IsChecked)
                return true;
            else
                return false;
        }

        public void ShowError(string text)
        {
            FolderArea.Text = "Exception : \n"+text;
        }


        /// <summary>
        /// Radio button 'Top-Level' checked or unchecked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioTopLevel_Checked(object sender, RoutedEventArgs e)
        {
            if(FolderArea!=null)
            LoadFolder_Click(null,null);
        }

        private void GUIRegexSearch_Click(object sender, RoutedEventArgs e)
        {
            InitiateWorker(SearchType.RegexPattern);
        }

        private void GUIStringSearch_Click(object sender, RoutedEventArgs e)
        {
            //Retrieve the text IN BETWEEN two strings.
            InitiateWorker(SearchType.BetweenStrings);
        }

        public enum SearchType { BetweenStrings, RegexPattern, BetweenRegex }
        private bool IsSearchInProgress=false;
        public void InitiateWorker(SearchType type)
        {
            if(IsSearchInProgress)
            {
                MessageBox.Show("A Search is in progress. Please cancel the current search operation before starting another.");
                return;
            }
            if(LocalFolderFiles==null)
            {
                MessageBox.Show("No files have been loaded yet. Please browse to a a folder.");
                return;
            }

            //By now it is established that one of the search methods are going to be used.
            //Set up the common parameters for the vars, worker method etc
            int TotalNumberFiles = LocalFolderFiles.Count();
            GUIProgressBar.Maximum = TotalNumberFiles;

            if(type==SearchType.BetweenStrings)
            {
                string string1 = GUIString1.Text;
                string string2 = GUIString2.Text;
                List<object> ArgumentToPass = new List<object>();
                ArgumentToPass.Add(string1);
                ArgumentToPass.Add(string2);

                IsSearchInProgress = true;
                workerRegexPattern = new BackgroundWorker();
                workerRegexPattern.DoWork += InBetweenStrings_DoWork;
                workerRegexPattern.ProgressChanged += (s, args) => { GUIProgressNumber.Text = args.ProgressPercentage.ToString() + " / " + TotalNumberFiles.ToString(); GUIProgressBar.Value = args.ProgressPercentage; };
                workerRegexPattern.WorkerReportsProgress = true;
                workerRegexPattern.WorkerSupportsCancellation = true;
                workerRegexPattern.RunWorkerCompleted += (s, args) => 
                { 
                    if (args.Cancelled == true)
                    {
                        FolderArea.Text = "(The Search Operation was cancelled mid-way)";
                        IsSearchInProgress = false;
                        return; 
                    } 
                    FolderArea.Text = args.Result as string;
                    IsSearchInProgress = false;
                };
                workerRegexPattern.RunWorkerAsync(ArgumentToPass);
            }

            else if (type == SearchType.RegexPattern)
            {
                string _regexQuery = GUIRegexQueryBox.Text;
                workerRegexPattern = new BackgroundWorker();
                workerRegexPattern.DoWork += RegexSearcher_DoWork;
                workerRegexPattern.ProgressChanged += (s, args) => { GUIProgressNumber.Text = args.ProgressPercentage.ToString() + " / " + TotalNumberFiles.ToString(); GUIProgressBar.Value = args.ProgressPercentage; };
                workerRegexPattern.WorkerReportsProgress = true;
                workerRegexPattern.WorkerSupportsCancellation = true;
                workerRegexPattern.RunWorkerCompleted += (s, args) => 
                { 
                    if (args.Cancelled == true)
                    {
                        FolderArea.Text = "(The Search Operation was cancelled mid-way)";
                        IsSearchInProgress = false;
                        return; 
                    } 
                    FolderArea.Text = args.Result as string;
                    IsSearchInProgress = false;
                };
                workerRegexPattern.RunWorkerAsync(_regexQuery);
                IsSearchInProgress = true;            
            }
        }

        private void GUICancelSearch_Click(object sender, RoutedEventArgs e)
        {
           if (workerRegexPattern != null)
               if(workerRegexPattern.CancellationPending==false)
                    workerRegexPattern.CancelAsync();
            
        }


        public class ExcelSaveFormat
        {
            public string Results { get; set; }
        }
        private void GUIExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            List<ExcelSaveFormat> someList = new List<ExcelSaveFormat>();
            
            string[] strArray = FolderArea.Text.Split(new string[]{Environment.NewLine},StringSplitOptions.None);
            List<string> strList = strArray.ToList();
            
            foreach(string strLoop in strList)
            {
                someList.Add(new ExcelSaveFormat() { Results = strLoop });
            }

            System.Data.DataTable dt = ExportToExcel.CreateExcelFile.ListToDataTable<ExcelSaveFormat>(someList);

            SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "ExtractedData"; // Default file name
            dlg.DefaultExt = ".xlsx"; // Default file extension
            dlg.Filter = "Excel Spreadsheet (.xlsx)|*.xlsx"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                ExportToExcel.CreateExcelFile.CreateExcelDocument(dt, filename);
            }
        }

        private void GUIExportToTextFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "ExtractedData"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text File (.txt)|*.txt"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                File.WriteAllText(filename, FolderArea.Text);                
            }

        }

        
        private void GUITagBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string tag = (sender as TextBox).Text;
            GUISampleShow.Text = "<"+tag+">...(Get this Data)..."+"</"+tag+">"; 
        }

        private void TagContentSearch_Click(object sender, RoutedEventArgs e)
        {
            string TagName = GUITagBox.Text;
            string[] Attributes = GUITagAttributeBox.Text.Split(new string[]{" "},StringSplitOptions.RemoveEmptyEntries); ;

            List<HtmlNode> acceptedNodes = new List<HtmlNode>(); // those among matchingNodes that have all required attributes
            FolderArea.Text = "";

            foreach(string filePath in LocalFolderFiles)
            {
                string fileContents = File.ReadAllText(filePath);
                HtmlAgilityPack.HtmlDocument theHTML = new HtmlDocument();
                theHTML.LoadHtml(fileContents);

                IEnumerable<HtmlNode> matchingNodes = theHTML.DocumentNode.Descendants(TagName);
                
                foreach(var matchingNode in matchingNodes)
                {
                    HtmlAttributeCollection matchingAttrs = matchingNode.Attributes;
                    bool AcceptThisNode = true;
                    foreach(string attribute in Attributes)
                    {
                        HtmlAttribute attr = matchingAttrs[0];
                        int Number = matchingAttrs.Where(x=>x.Name==attribute).Count();

                        if (Number == 0)
                        {
                            //this matchingNode did not make it - go to next outer loop
                            AcceptThisNode = false;
                            break;
                        }
                    }
                    if(AcceptThisNode)
                    {
                        if(matchingNode.InnerHtml!=""&&matchingNode.InnerHtml!=null)
                        acceptedNodes.Add(matchingNode);
                    }
                }
                
                
            }

            if (acceptedNodes.Count() == 0)
            {
                FolderArea.Text = "No matching tags were found.";
            }
            else
            {
                foreach (HtmlNode node in acceptedNodes)
                {
                    FolderArea.Text += node.InnerHtml;
                }
            }


        }//end fn

        
    }
}
