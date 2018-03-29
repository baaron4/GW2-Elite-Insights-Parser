using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LuckParser.Controllers;

namespace LuckParser
{
    public partial class Form1 : Form
    {
        List<string> paths = new List<string>();// Environment.CurrentDirectory + "/" + "parsedhtml.html";
        Controller1 controller = new Controller1();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (paths != null)
            {
                foreach (string path in paths)
                {
                    int pos = path.LastIndexOf("/") + 1;
                    string file = path.Substring(pos, path.Length - pos);
                    int pos1 = file.LastIndexOf(".") + 1;
                    string appendix = file.Substring(pos1, file.Length - pos1);
                    string fileName = file.Substring(0,pos1-1);
                    if (appendix == "evtc")
                    {
                        //Process evtc here
                        controller.ParseLog(path);
                        //return html string
                        string htmlContent = controller.CreateHTML(0);
                        FileStream fcreate = File.Open(path.Substring(0, path.Length - file.Length) + fileName +"_AB.html", FileMode.Create);
                        using (StreamWriter sw = new StreamWriter(fcreate))
                        {
                            sw.Write(htmlContent);
                            sw.Close();
                        }
                        MessageBox.Show(path + " generated");
                    }
                    else {
                        MessageBox.Show(path + " File not evtc");
                    }
                }
            } else {
                MessageBox.Show("EVTC file(s) not selected");
            }
            paths = null;
            listBox1.Items.Clear();
        }

        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            //Get files as list
            string[] filesArray = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            List<string> files = new List<string>();
            foreach (string file in filesArray) {
                files.Add(file);
            }
            //Add files to paths
            if (paths == null)
            {
                paths = files;
            }else {
                //Dont add doubles
                for (int f = 0;f < files.Count();f++) {
                    for (int p = 0; p < paths.Count(); p++) {
                        if (paths[p] == files[f]) {
                            files.Remove(files[f]);
                        }
                    }
                }
                paths.AddRange(files);
            }
           //Show in listbox
            foreach (string file in files)
            {
                listBox1.Items.Add(file);
               
            }
        }

        private void listBox1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }
    }
}
