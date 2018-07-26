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
using Fluent;
using DevZest;
using Newtonsoft.Json;
using System.IO;
using WpfApp2;
using DevZest.Windows.Docking;


using System.Xml;
using System.Windows.Markup;

namespace WpfApp2

{
    /// <summary>
    /// Represents the main window of the application
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// 

        string rfp = "backup\\backup.txt";
        string dfp = "backup\\default.txt";


        public MainWindow()
        {
            this.InitializeComponent();


        }


        private void BtnQueryUser_Click(object sender, RoutedEventArgs e)
        {
            //string s = JsonConvert.SerializeObject(t1);
            //   FileStream xjFileStream = new FileStream(rfp, FileMode.Create, FileAccess.Write);
            //  StreamWriter xjStreamWriter = new StreamWriter(xjFileStream, Encoding.Default);
            //  xjStreamWriter.WriteLine(s);
            //  xjStreamWriter.WriteLine(XamlWriter.Save(layout));
            //  xjStreamWriter.Close();
            //  xjFileStream.Close();

            Textbox1.Activate();

        }

        private void BtnAddUser_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void BtnModifyUser_Click(object sender, RoutedEventArgs e)
        {
          
        }

        private void BtnQueryDept_Click(object sender, RoutedEventArgs e)
        {
            Jishu.Activate();
        }

        private void BtnAddDept_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void BtnModifyDept_Click(object sender, RoutedEventArgs e)
        {
      
        }

        private void BtnModifyPassword_Click(object sender, RoutedEventArgs e)
        {
            FileExplorer.Activate();
        }

        private void BtnSecManager_Click(object sender, RoutedEventArgs e)
        {
            Xitong.Activate();
        }

        private void SAVE_Click(object sender, RoutedEventArgs e)
        {
            DevZest.Windows.Docking.DockLayout layout = c1.Save();
            // XmlWriterSettings settings = new XmlWriterSettings();
            // settings.Indent = true;
            // settings.IndentChars = new string(' ', 4);
            // StringBuilder strbuild = new StringBuilder();
            // XmlWriter xmlWriter = XmlWriter.Create(strbuild, settings);
            // XamlWriter.Save(layout, xmlWriter);

            string s = WpfApp2.XamlWriter.Save(layout);
            // Textbox3.textBox.Text = WpfApp2.XamlWriter.Save(layout);



            // string json = JsonConvert.SerializeObject(layout);
            FileStream xjFileStream = new FileStream(rfp, FileMode.Create, FileAccess.Write);
            StreamWriter xjStreamWriter = new StreamWriter(xjFileStream, Encoding.Default);
            //   Console.WriteLine(s);
            xjStreamWriter.WriteLine(s);
            // xjStreamWriter.WriteLine(strbuild.ToString());
            //  xjStreamWriter.WriteLine(XamlWriter.Save(layout));
            xjStreamWriter.Close();
            xjFileStream.Close();
        }

        private void READ_Click(object sender, RoutedEventArgs e)
        {
            loadlayout(rfp);

            //StreamReader sr = new StreamReader(rfp, Encoding.Default);
            //string s = sr.ReadToEnd();
            // StringBuilder sb = new StringBuilder();
            // sb.Append(sr.ReadToEnd());
            //sr.Close();
            //DockLayout layout = JsonConvert.DeserializeObject<DockLayout>(json);
            // XmlReaderSettings settings = new XmlReaderSettings();
            //DockLayout layout = (DockLayout)XamlReader.Load(new XmlTextReader(new StringReader(s)));
            // DockLayout layout = (DockLayout)XamlReader.Load(XmlReader.Create(sb.ToString()));
            //  Console.WriteLine(s);
            //DockLayout layout = (DockLayout)XamlReader.Load(new XmlTextReader(new StringReader(Textbox3.textBox.Text)));
            // DockLayout layout = (DockLayout)XamlReader.Load(new System.Xml.XmlTextReader(s));
            //c1.Load(layout, LoadDockItem);

            //CloseAll();

           // c1.Load(layout, LoadDockItem);



            //DevZest.Windows.Docking.DockLayout sl = JsonConvert.DeserializeObject(json);
            //DevZest.Windows.Docking.DockLayout layout = new DevZest.Windows.Docking.DockLayout();
            // string json = JsonConvert.SerializeObject(layout);
            // FileStream xjFileStream = new FileStream(rfp, FileMode.OpenOrCreate, FileAccess.Write);
            // StreamWriter xjStreamWriter = new StreamWriter(xjFileStream, Encoding.Default);
            // xjStreamWriter.WriteLine(json);
            // xjStreamWriter.Close();
            // xjFileStream.Close();
        }

        private void DEFAULT_Click(object sender, RoutedEventArgs e)
        {
            loadlayout(dfp);
        }


        private void CloseAll()
        {
            for (int i = c1.DockItems.Count - 1; i >= 0; i--)
            {
                DockItem item = c1.DockItems[i];
                item.Close();
            }
        }

        private DockItem LoadDockItem(object obj)
        {
            if (Textbox1.GetType().ToString().Equals(obj))
            {
                return Textbox1;
            }
            else if (FileExplorer.GetType().ToString().Equals(obj))
            {
                return FileExplorer;
            }
            else if (Jishu.GetType().ToString().Equals(obj))
                return Jishu;
            else if (Xitong.GetType().ToString().Equals(obj))
                return Xitong;
            else
            {
                return obj as DockItem;
            }
        }


        private void loadlayout(string spath)
        {
             StreamReader sr = new StreamReader(spath, Encoding.Default);
             string s = sr.ReadToEnd();
             sr.Close();
             DockLayout layout = (DockLayout)XamlReader.Load(new XmlTextReader(new StringReader(s)));
       
             CloseAll();

             c1.Load(layout, LoadDockItem);
        }

        private void savelayout()
        {
            DevZest.Windows.Docking.DockLayout layout = c1.Save();
            string s = WpfApp2.XamlWriter.Save(layout);
            FileStream xjFileStream = new FileStream(rfp, FileMode.Create, FileAccess.Write);
            StreamWriter xjStreamWriter = new StreamWriter(xjFileStream, Encoding.Default);
            xjStreamWriter.WriteLine(s);
            xjStreamWriter.Close();
            xjFileStream.Close();
        }

        private void RibbonWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            savelayout();
        }

        private void RibbonWindow_Activated(object sender, EventArgs e)
        {
        }

        private void c1_Loaded(object sender, RoutedEventArgs e)
        {
            loadlayout(rfp);
        }

       
    }
}
