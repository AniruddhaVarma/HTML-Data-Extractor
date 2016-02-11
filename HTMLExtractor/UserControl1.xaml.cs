using HtmlAgilityPack;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace HTMLExtractor
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string HTMLContent = tbHTMLPattern.Text;
            XDocument doc = XDocument.Parse(HTMLContent, LoadOptions.None);
            string RootName = doc.Document.Root.Name.LocalName;
            IEnumerable<XElement> Children = doc.Document.Root.Elements();
            IEnumerable<string> ChildrenNames = doc.Document.Root.Elements().Select(x=>x.Name.LocalName);

            //get HTML
            //get tag which matches RootName
            //get children from RootName
            //  check ChildrenNames are all included in the RootName

            

            ;
        }
    }
}
