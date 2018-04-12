using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LuckParser.Controllers;


namespace LuckParser
{
    public partial class Form1 : Form
    {
        
        BackgroundWorker m_oWorker;
        private SettingsForm setFrm;
        //public bool[] settingArray = { true, true, true, true, true, false, true, true };
        bool completedOp = false;
        List<string> paths = new List<string>();// Environment.CurrentDirectory + "/" + "parsedhtml.html";
        Controller1 controller = new Controller1();
        public Form1()
        {
            InitializeComponent();
            
            btnCancel.Enabled = false;
            btnStartAsyncOperation.Enabled = false;
            m_oWorker = new BackgroundWorker();

            // Create a background worker thread that ReportsProgress &
            // SupportsCancellation
            // Hook up the appropriate events.
            m_oWorker.DoWork += new DoWorkEventHandler(m_oWorker_DoWork);
            m_oWorker.ProgressChanged += new ProgressChangedEventHandler
                    (m_oWorker_ProgressChanged);
            m_oWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler
                    (m_oWorker_RunWorkerCompleted);
            m_oWorker.WorkerReportsProgress = true;
            m_oWorker.WorkerSupportsCancellation = true;

            
        }
       
        // On completed do the appropriate task
        
        void m_oWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                lblStatus.Text = "Task Cancelled.";
            }
            // Check to see if an error occurred in the background process.
            else if (e.Error != null)
            {
                lblStatus.Text = "Error while performing background operation.";
            }
            else
            {
                // Everything completed normally.
                lblStatus.Text = "Task Completed";
            }

            //Change the status of the buttons on the UI accordingly
            btnStartAsyncOperation.Enabled = true;
            btnCancel.Enabled = false;
        }

        // Notification is performed here to the progress bar
        void m_oWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            string[] progstr = (string[])e.UserState;
            if (progstr[1] == "Cancel")
            {
                paths = null;
                //listView1.Items.Clear();
                completedOp = true;
                btnClear.Enabled = true;
                lblStatus.Text = "Canceled Parseing";
            } else if(progstr[1] == "Finish") {
                paths = null;
                //listView1.Items.Clear();
                completedOp = true;
                btnClear.Enabled = true;
                btnStartAsyncOperation.Enabled = false;
                lblStatus.Text = "Finished Parseing";
            }
            else {
                lblStatus.Text = "Parseing...";
                listView1.Items[Int32.Parse(progstr[0])].SubItems[1].Text = progstr[1];
            }
        }

        /// Time consuming operations go here </br>
        /// i.e. Database operations,Reporting
        void m_oWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            bool[] settingsSnap = new bool[] {Properties.Settings.Default.DPSGraphTotals,
                Properties.Settings.Default.PlayerGraphTotals,
                Properties.Settings.Default.PlayerGraphBoss,
                Properties.Settings.Default.PlayerBoonsUniversal,
                Properties.Settings.Default.PlayerBoonsImpProf,
                Properties.Settings.Default.PlayerBoonsAllProf,
                Properties.Settings.Default.PlayerRot,
                Properties.Settings.Default.PlayerRotIcons
            };
            for (int i = 0; i < listView1.Items.Count ; i++)
            {
                string[] reportObject = { i.ToString(), "Working..." };
                m_oWorker.ReportProgress(20,reportObject);

                string path = paths[i];
                int pos = path.LastIndexOf("/") + 1;
                string file = path.Substring(pos, path.Length - pos);
                int pos1 = file.LastIndexOf(".") + 1;
                string appendix = file.Substring(pos1, file.Length - pos1);
                string fileName = file.Substring(0, pos1 - 1);

                Controller1 control = new Controller1();
                if (appendix == "evtc")
                {
                    //Process evtc here
                    reportObject = new string[] { i.ToString(), "Reading Binary..." };
                    m_oWorker.ReportProgress(40, reportObject);
                    control.ParseLog(path);



                    //Creating File
                    //save location
                    string location = "";
                    if (Properties.Settings.Default.SaveAtOut || Properties.Settings.Default.OutLocation == null)
                    {
                        location = path.Substring(0, path.Length - file.Length);
                    }
                    else {
                        location = Properties.Settings.Default.OutLocation;
                    }

                    reportObject = new string[] { i.ToString(), "Createing File..." };
                    m_oWorker.ReportProgress(60, reportObject);
                    FileStream fcreate = File.Open(location + fileName + "_Report.html", FileMode.Create);

                    //return html string
                    reportObject = new string[] { i.ToString(), "Writing HTML..." };
                    m_oWorker.ReportProgress(80, reportObject);

                    
                    using (StreamWriter sw = new StreamWriter(fcreate))
                    {
                        // string htmlContent = control.CreateHTML(/*settingArray*/);
                        // sw.Write(htmlContent);
                        control.CreateHTML(sw,settingsSnap);
                        sw.Close();
                    }

                    // MessageBox.Show(path + " generated");
                    reportObject = new string[] { i.ToString(), "HTML Generated" };
                    m_oWorker.ReportProgress(100, reportObject);
                }
                else {
                    reportObject = new string[] { i.ToString(), "Not EVTC" };
                    m_oWorker.ReportProgress(100, reportObject);
                }
                if (m_oWorker.CancellationPending)
                {
                    // Set the e.Cancel flag so that the WorkerCompleted event
                    // knows that the process was cancelled.
                    e.Cancel = true;
                     reportObject = new string[] { i.ToString(), "Cancel" };
                    m_oWorker.ReportProgress(0,reportObject);
                    btnStartAsyncOperation.Enabled = false;
                    return;
                }
            }

            //Report 100% completion on operation completed
           string[] rO = new string[] {0.ToString(), "Finish" };
            m_oWorker.ReportProgress(100, rO);
            btnStartAsyncOperation.Enabled = false;
        }
       
        private void btnStartAsyncOperation_Click(object sender, EventArgs e)
        {
            //Change the status of the buttons on the UI accordingly
            //The start button is disabled as soon as the background operation is started
            //The Cancel button is enabled so that the user can stop the operation 
            //at any point of time during the execution
            if (paths != null)
            {
                btnStartAsyncOperation.Enabled = false;
                btnCancel.Enabled = true;
                btnClear.Enabled = false;

                m_oWorker.RunWorkerAsync();
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (m_oWorker.IsBusy)
            {
                // Notify the worker thread that a cancel has been requested.
                // The cancel will not actually happen until the thread in the
                // DoWork checks the m_oWorker.CancellationPending flag. 

                m_oWorker.CancelAsync();
            }
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            btnStartAsyncOperation.Enabled = true;
            if (completedOp) {
                listView1.Items.Clear();
                completedOp = false;
            }
            //Get files as list
            string[] filesArray = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            List<string> files = new List<string>();
            foreach (string file in filesArray)
            {
                files.Add(file);
            }
            //Add files to paths
            if (paths == null)
            {
                paths = files;
                
            }
            else
            {
                //Dont add doubles
                for (int f = 0; f < files.Count(); f++)
                {
                    for (int p = 0; p < paths.Count(); p++)
                    {
                        if (paths[p] == files[f])
                        {
                            files.Remove(files[f]);
                        }
                    }
                }
                paths.AddRange(files);
            }
            //Show in listbox
            foreach (string file in files)
            {
              
                ListViewItem item = new ListViewItem(file);
                item.SubItems.Add(" ");
                listView1.Items.Add(item);

            }
        }

        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void listView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Yellow, e.Bounds);
            e.DrawText();
        }
       
        private void settingsbtn_Click(object sender, EventArgs e)
        {
            setFrm = new SettingsForm(/*settingArray,this*/);
            setFrm.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
          
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            btnCancel.Enabled = false;
            btnStartAsyncOperation.Enabled = false;
            paths = null;
            listView1.Items.Clear();
        }
    }
}
