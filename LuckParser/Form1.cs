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

public delegate void Logger(int i, string msg, int prog);
public delegate void Cancellation(int i, DoWorkEventArgs e);

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

        public Form1(string[] args)
        {
            InitializeComponent();
            listView1_AddItems(args);
            m_DoWork(log_Console, null, null);
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
                // Flash window until it recieves focus
                FlashWindow.Flash(this);
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
            }
            else if (progstr[1] == "Finish")
            {
                paths = null;
                //listView1.Items.Clear();
                completedOp = true;
                btnClear.Enabled = true;
                btnStartAsyncOperation.Enabled = false;
                lblStatus.Text = "Finished Parseing";
            }
            else
            {
                lblStatus.Text = "Parseing...";
                listView1.Items[Int32.Parse(progstr[0])].SubItems[1].Text = progstr[1];
            }
        }

        private void log_Console(int i, string msg, int prog)
        {
            Console.WriteLine(msg);
        }

        private void log_Worker(int i, string msg, int prog)
        {
            string[] reportObject = { i.ToString(), msg };
            m_oWorker.ReportProgress(prog, reportObject);
        }

        private void cancel_Worker(int i, DoWorkEventArgs e)
        {
            if (m_oWorker.CancellationPending)
            {
                // Set the e.Cancel flag so that the WorkerCompleted event
                // knows that the process was cancelled.
                e.Cancel = true;
                string[] reportObject = new string[] { i.ToString(), "Cancel" };
                m_oWorker.ReportProgress(0, reportObject);
                btnStartAsyncOperation.Enabled = false;
                return;
            }
        }

        /// Time consuming operations go here </br>
        /// i.e. Database operations,Reporting
        void m_DoWork(Logger logger, Cancellation cancel, DoWorkEventArgs e)
        {
            //globalization
            System.Globalization.CultureInfo before = System.Threading.Thread.CurrentThread.CurrentCulture;
            try

            {
                System.Threading.Thread.CurrentThread.CurrentCulture =
                    new System.Globalization.CultureInfo("en-US");
                // Proceed with specific code

                bool[] settingsSnap = new bool[] {
                    Properties.Settings.Default.DPSGraphTotals,
                    Properties.Settings.Default.PlayerGraphTotals,
                    Properties.Settings.Default.PlayerGraphBoss,
                    Properties.Settings.Default.PlayerBoonsUniversal,
                    Properties.Settings.Default.PlayerBoonsImpProf,
                    Properties.Settings.Default.PlayerBoonsAllProf,
                    Properties.Settings.Default.PlayerRot,
                    Properties.Settings.Default.PlayerRotIcons,
                    Properties.Settings.Default.EventList,
                    Properties.Settings.Default.BossSummary,
                    Properties.Settings.Default.SimpleRotation,
                     Properties.Settings.Default.ShowAutos,
                    Properties.Settings.Default.LargeRotIcons,
                    Properties.Settings.Default.ShowEstimates

                };
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    

                    string path = paths[i];
                    if (path.Contains("\\"))
                    {
                        path = path.Replace("\\", "/");
                    }
                    if (!File.Exists(path))
                    {
                        logger(i, "File does not exist", 100);
                        continue;
                    }
                    int pos = path.LastIndexOf("/") + 1;
                    //if (pos == 0) {
                    //     pos = path.LastIndexOf('\\') + 1;
                    //}
                    string file = path.Substring(pos, path.Length - pos);
                    int pos1 = file.LastIndexOf(".") + 1;
                    if (path.EndsWith(".evtc.zip", StringComparison.OrdinalIgnoreCase))
                    {
                        pos1 = pos1 - 5;
                    }

                    string appendix = file.Substring(pos1, file.Length - pos1);
                    string fileName = file.Substring(0, pos1 - 1);

                    logger(i, "Working...", 20);
                    Controller1 control = new Controller1();
                    if (path.EndsWith(".evtc", StringComparison.OrdinalIgnoreCase) ||
                        path.EndsWith(".evtc.zip", StringComparison.OrdinalIgnoreCase))
                    {
                        //Process evtc here
                        logger(i, "Reading Binary...", 40);
                        control.ParseLog(path);


                        //Creating File
                        //save location
                        string location = "";
                        if (Properties.Settings.Default.SaveAtOut || Properties.Settings.Default.OutLocation == null)
                        {
                            location = path.Substring(0, path.Length - file.Length);

                        }
                        else
                        {
                            location = Properties.Settings.Default.OutLocation + "/";
                        }
                        if (location.Contains("\\"))
                        {
                            location = location.Replace("\\", "/");
                        }
                        string bossid = control.getBossData().getID().ToString();
                        string result = "fail";
                        if (control.getLogData().getBosskill())
                        {
                            result = "kill";
                        }
                        if (Properties.Settings.Default.SaveOutHTML)
                        {
                            logger(i, "Creating File...", 60);
                            FileStream fcreate = File.Open(location + fileName + "_" + control.GetLink(bossid + "-ext") + "_" + result + ".html", FileMode.Create);

                            //return html string
                            logger(i, "Writing HTML...", 80);


                            using (StreamWriter sw = new StreamWriter(fcreate))
                            {
                                // string htmlContent = control.CreateHTML(/*settingArray*/);
                                // sw.Write(htmlContent);
                                control.CreateHTML(sw, settingsSnap);
                                sw.Close();
                            }

                            logger(i, "HTML Generated!", 100);
                        }
                        if (Properties.Settings.Default.SaveOutCSV)
                        {
                            logger(i, "Creating CSV File...", 60);
                            FileStream fcreate = File.Open(location + fileName + "_" + control.GetLink(bossid + "-ext") + "_" + result + ".csv", FileMode.Create);

                            //return html string
                            logger(i, "Writing CSV...", 80);


                            using (StreamWriter sw = new StreamWriter(fcreate))
                            {
                                // string htmlContent = control.CreateHTML(/*settingArray*/);
                                // sw.Write(htmlContent);
                                control.CreateCSV(sw,",");
                                sw.Close();
                            }

                            logger(i, "CSV Generated!", 100);
                        }
                    }
                    else
                    {
                        logger(i, "Not EVTC...", 100);
                    }
                    if (cancel != null)
                    {
                        cancel(i, e);
                    }
                }

                //Report 100% completion on operation completed
                logger(0, "Finish", 100);
                btnStartAsyncOperation.Enabled = false;
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = before;
            }
        }
        
        /// Worker job
        void m_oWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            m_DoWork(log_Worker, cancel_Worker, e);
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

        private void listView1_AddItems(string[] filesArray)
        {
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

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            btnStartAsyncOperation.Enabled = true;
            if (completedOp)
            {
                listView1.Items.Clear();
                completedOp = false;
            }
            //Get files as list
            string[] filesArray = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            listView1_AddItems(filesArray);
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

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
