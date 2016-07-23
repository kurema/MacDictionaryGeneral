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

using CefSharp;

namespace MacDictionaryWpfTest
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Cef.Initialize(new CefSettings());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            var dic = new MacDictionaryGeneral.MacDictionary(TextBoxPath.Text.Replace("\"", ""));
            var text = dic.GetBodyDataSingle(0x60, 0);
            var html = dic.GetFullHtml(dic.GetBodyDataArray(dic.GetKeyword(new bool[4] { true, true, true, true }, TextBoxKeyword.Text, (a, b) => a.ToLower() == b.ToLower())));
            //WebBrowserBody.NavigateToString(html);
            Browser.LoadHtml(html, "http://www.google.com/");
        }
    }
}
