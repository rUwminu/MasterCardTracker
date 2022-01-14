using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace MasterCardTracker
{
    public partial class Form1 : Form
    {
        private MySqlConnection conn;

        DataTable dt = new DataTable();        

        MySqlCommand cmd = null;

        DateTime dateTime = DateTime.Now;

        // This change according to user department, change depend on different user login
        // Example user is cutting department
        // string initialstate = "Extrusion";
        // string initialstate = "Printing";
        // string initialstate = "Cutting";
         string initialstate = "Planning";
        // string initialstate = "SomeRandom";

        static int VALIDATION_DELAY = 1500;
        System.Threading.Timer timer = null;

        string isComp;
        string msid;
        string woid;
        string lastlocation;
        string InOutStatus;

        public Form1()
        {
            InitializeComponent();
            
            conn = new MySqlConnection();
            conn.ConnectionString = "server=localhost;uid=root; pwd=123456; database=plastic; Convert Zero Datetime=True; Allow Zero Datetime=True; default command timeout=300; ";
        }

        public async void saveHistory(string MSid, string WOid, string WOitem, string location, string status, bool valid, int delay)
        {
            // Delay This task for last task to complete sql read and connection to close *Testing Beta
            await Task.Delay(delay);

            string insertinfo = "INSERT INTO plastic.masterc_record(mcr_no, mcr_wo_no, mcr_wo_no_item, mcr_location, mcr_datetime, mcr_status) VALUES (@mcr_no, @mcr_wo_no, @mcr_wo_no_item, @mcr_location, @mcr_datetime, @mcr_status)";

            conn.Open();

            MySqlCommand upcmd = new MySqlCommand(insertinfo, conn);

            upcmd.Parameters.AddWithValue("@mcr_no", MSid);
            upcmd.Parameters.AddWithValue("@mcr_wo_no", WOid);
            upcmd.Parameters.AddWithValue("@mcr_wo_no_item", WOitem);
            upcmd.Parameters.AddWithValue("@mcr_location", location);
            upcmd.Parameters.AddWithValue("@mcr_datetime", dateTime.ToString("yyyy-MM-dd hh:mm:ss"));
            upcmd.Parameters.AddWithValue("@mcr_status", status);

            upcmd.ExecuteNonQuery();

            conn.Close();

            if(valid)
            {
                label3.Text = "Done Check-in to this deparment.";
                label3.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                label3.Text = "Checking Error";
                label3.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void getWOData ()
        {
            Console.WriteLine("Running Get Workorder Details");

            try
            {
                bool isCheck = false;
                var splitted = textBox1.Text.Split('-');
                string pono = splitted[0];
                string item = splitted[1];

                string query = "SELECT plastic.masterc_record.mcr_no, plastic.masterc_record.mcr_location, plastic.wo.PO, plastic.masterc.mascNo, plastic.masterc_record.mcr_status " +
                                "FROM plastic.wo " +
                                "LEFT JOIN plastic.masterc ON plastic.wo.MASCID = plastic.masterc.ID " +
                                "LEFT JOIN plastic.masterc_record ON plastic.masterc.mascNo = plastic.masterc_record.mcr_no " +
                                "LEFT JOIN plastic.woitem ON plastic.wo.ID = plastic.woitem.woId " +
                                "LEFT JOIN plastic.woplan ON plastic.woitem.id = plastic.woplan.woitemid " +
                                "WHERE plastic.wo.PO = '" + pono + "' AND plastic.woitem.item = '" + item + "' " +
                                "AND NOT plastic.masterc_record.mcr_status = 'Invalid' " +
                                "ORDER BY plastic.masterc_record.id DESC LIMIT 1";

                try
                {
                    cmd = new MySqlCommand(query, conn);

                    conn.Open();

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Always reset the value to false
                        isCheck = false;

                        // Check record is the workorder have any pass record
                        if (reader.HasRows)
                        {
                            //Console.WriteLine("Have Row?");
                            while (reader.Read())
                            {
                                msid = reader[3].ToString();
                                woid = reader[2].ToString();
                                lastlocation = reader[1].ToString();
                                InOutStatus = reader[4].ToString();
                                //string msStatus = reader[5].ToString(); // **Not in use yet**
                                label4.Text = msid;
                                label6.Text = lastlocation;
                                label9.Text = InOutStatus;

                                // This if/else check user department and the posible Workorder can pass to this user deparment
                                if (initialstate == "Planning")
                                {
                                    if (msid != "" && lastlocation == initialstate && pono == woid)
                                    {
                                        if(InOutStatus == "IN")
                                        {
                                            // -- Prompt Message Box on Reuse Matercard that already on planner hand, *Prevent mistake on forget check-in double scan
                                            //DialogResult dialogResult = MessageBox.Show("Reuse Mastercard for New Workorder?", "Confirmation", MessageBoxButtons.YesNo);
                                            //if (dialogResult == DialogResult.Yes)
                                            //{
                                            //    saveHistory(msid, woid, item, initialstate, "OUT", true, 300);
                                            //}
                                            //else if (dialogResult == DialogResult.No)
                                            //{
                                            //    label3.Text = "MasterCard Already Here.";
                                            //    label3.ForeColor = System.Drawing.Color.Green;

                                            //    return;
                                            //}

                                            // -- Roll Back: No Prompt on reuse mastercard. *User need to remember which workorder already scanned, double scan will check out the Mastercard
                                            // -- Solution On Mistake Double Scan making MSC OUT: Just scan the workorder again, to return the MSC.
                                            saveHistory(msid, woid, item, initialstate, "OUT", true, 300);
                                        } 
                                        else 
                                        {
                                            saveHistory(msid, woid, item, initialstate, "IN", true, 300);
                                        }
                                    }
                                    else if (lastlocation != initialstate)
                                    {
                                        isCheck = true;
                                        label3.Text = "New MasterCard Come In, Doing Check-In...";
                                        label3.ForeColor = System.Drawing.Color.Red;
                                        label4.Text = msid;
                                    }
                                }
                                else if (initialstate == "Extrusion")
                                {
                                    // This if/else check the mastercard last location and compare with current location to let user know is mastercard here or will update to here
                                    if (msid != "" && lastlocation == initialstate && pono == woid)
                                    {
                                        label3.Text = "MasterCard Already Here.";
                                        label3.ForeColor = System.Drawing.Color.Green;

                                        return;
                                    }
                                    else if (lastlocation != initialstate && lastlocation != "Planning")
                                    {
                                        isCheck = true;
                                        label3.Text = "New MasterCard Come In, Doing Check-In...";
                                        label3.ForeColor = System.Drawing.Color.Red;
                                        label4.Text = msid;
                                    } 
                                    else if (lastlocation == "Planning" && InOutStatus == "IN")
                                    {
                                        label3.Text = "Planner Not Checkout Yet";
                                        label3.ForeColor = System.Drawing.Color.Red;

                                        MessageBox.Show(string.Format("Matercard Not Checkout By Planner"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                        return;
                                    }
                                    else if (lastlocation == "Planning" && InOutStatus == "OUT")
                                    {
                                        isCheck = true;
                                        label3.Text = "New MasterCard Come In, Doing Check-In...";
                                        label3.ForeColor = System.Drawing.Color.Red;
                                        label4.Text = msid;
                                    }
                                }
                                else if (initialstate == "Printing")
                                {
                                    if (msid != "" && lastlocation == initialstate && pono == woid)
                                    {
                                        label3.Text = "MasterCard Already Here.";
                                        label3.ForeColor = System.Drawing.Color.Green;

                                        return;
                                    }
                                    else if (lastlocation != initialstate && lastlocation != "Planning")
                                    {
                                        isCheck = true;
                                        label3.Text = "New MasterCard Come In, Doing Check-In...";
                                        label3.ForeColor = System.Drawing.Color.Red;
                                        label4.Text = msid;
                                    }
                                    else if (lastlocation == "Planning" && InOutStatus == "IN")
                                    {
                                        label3.Text = "Planner Not Checkout Yet";
                                        label3.ForeColor = System.Drawing.Color.Red;

                                        MessageBox.Show(string.Format("Matercard Not Checkout By Planner"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                        return;
                                    }
                                    else if (lastlocation == "Planning" && InOutStatus == "OUT")
                                    {
                                        isCheck = true;
                                        label3.Text = "New MasterCard Come In, Doing Check-In...";
                                        label3.ForeColor = System.Drawing.Color.Red;
                                        label4.Text = msid;
                                    }
                                }
                                else if (initialstate == "Cutting")
                                {
                                    if (msid != "" && lastlocation == initialstate && pono == woid)
                                    {
                                        label3.Text = "MasterCard Already Here.";
                                        label3.ForeColor = System.Drawing.Color.Green;

                                        return;
                                    }
                                    else if (lastlocation != initialstate && lastlocation != "Planning")
                                    {
                                        isCheck = true;
                                        label3.Text = "New MasterCard Come In, Doing Check-In...";
                                        label3.ForeColor = System.Drawing.Color.Red;
                                        label4.Text = msid;
                                    }
                                    else if (lastlocation == "Planning" && InOutStatus == "IN")
                                    {
                                        label3.Text = "Planner Not Checkout Yet";
                                        label3.ForeColor = System.Drawing.Color.Red;

                                        MessageBox.Show(string.Format("Matercard Not Checkout By Planner"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                        return;
                                    }
                                    else if (lastlocation == "Planning" && InOutStatus == "OUT")
                                    {
                                        isCheck = true;
                                        label3.Text = "New MasterCard Come In, Doing Check-In...";
                                        label3.ForeColor = System.Drawing.Color.Red;
                                        label4.Text = msid;
                                    }
                                }
                                else
                                {
                                    label3.Text = "You Are Not Person Involve In Process. ";
                                    label3.ForeColor = System.Drawing.Color.Red;
                                }
                            }
                        }

                        if (!reader.HasRows)
                        {
                            // It check the master record for any result, new wo will not be in master record. Then trigger this
                            // If is new workorder pass down to production, this function run and record
                            if (initialstate == "Planning")
                            {
                                getNewWoDataAndSave(pono, item, true);
                            }
                            else
                            {
                                getNewWoDataAndSave(pono, item, false);
                            }

                            return;
                        }

                        reader.Close();
                    }

                    conn.Close();

                    if (isCheck == true)
                    {
                        if (lastlocation != "Planning")
                        {
                            saveHistory(msid, woid, item, initialstate, "IN", true, 500);
                            saveHistory(msid, woid, item, lastlocation, "OUT", true, 300);
                        }
                        else
                        {
                            saveHistory(msid, woid, item, initialstate, "IN", true, 500);
                        }
                    }

                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("An error occurred {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    conn.Close();
                }
            } catch (Exception ex)
            {
                MessageBox.Show(string.Format("Invalid Workoreder No"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }      
        }

        public async void getNewWoDataAndSave(string pono, string item, bool isError)
        {
            await Task.Delay(300);

            // Get PO and mastercard No
            string query = "SELECT plastic.wo.PO, plastic.masterc.mascNo " +
                           "FROM plastic.wo " +
                           "LEFT JOIN plastic.masterc ON plastic.wo.MASCID = plastic.masterc.ID " +
                           "LEFT JOIN plastic.woitem ON plastic.wo.ID = plastic.woitem.woId " +
                           "LEFT JOIN plastic.woplan  ON plastic.woitem.id = plastic.woplan.woitemid " +
                           "WHERE plastic.wo.PO = '" + pono + "' ";

            try
            {
                cmd = new MySqlCommand(query, conn);

                conn.Open();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            msid = reader[1].ToString();
                            woid = reader[0].ToString();
                        }
                    }

                    reader.Close();
                }
                conn.Close();

                if(!isError)
                {
                    MessageBox.Show(string.Format("This Workorder Haven't Pass Down By Planner! Please Return It."), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    saveHistory(msid, woid, item, initialstate, "Invalid", false, 500);
                }
                else
                {
                    saveHistory(msid, woid, item, initialstate, "OUT", true, 500);
                }

                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("An error occurred {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        private void getMSCHistory(string query)
        {
            // Delay This task for last task to complete sql read and connection to close *Testing Beta
            //await Task.Delay(TimeSpan.FromSeconds(2));

            conn.Close();

            dt.Clear();
            dgv1.Refresh();

            try
            {
                cmd = new MySqlCommand(query, conn);

                conn.Open();

                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }

                conn.Close();

                dgv1.DataSource = dt;
                dgv1.DataMember = dt.TableName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("An error occurred {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            TextBox origin = sender as TextBox;
            if (!origin.ContainsFocus)
                return;

            DisposeTimer();
            timer = new System.Threading.Timer(TimerElapsed, null, VALIDATION_DELAY, VALIDATION_DELAY);
        }

        private void TimerElapsed(Object obj)
        {
            ActionOnCompleteInput();
            DisposeTimer();
        }

        private void DisposeTimer()
        {
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
        }

        private void ActionOnCompleteInput()
        {
            this.Invoke(new Action(() =>
            {
                label4.Text = "";
                label9.Text = "";

                if (textBox1.Text != "" )
                {
                    // Ignore check step, if new workorder with mastercard scan by planner
                    if (initialstate == "Planning")
                    {
                        timer.Dispose();

                        getWOData();
                    } 
                    else if(initialstate == "Extrusion" || initialstate == "Printing" || initialstate == "Cutting")
                    {
                        try
                        {
                            var splitted = textBox1.Text.Split('-');

                            string pono;
                            string item;

                            pono = splitted[0];

                            if (1 < splitted.Length)
                            {
                                item = splitted[0];
                            }
                            else
                            {
                                timer.Dispose();

                                MessageBox.Show(string.Format("Invalid Workoreder Or This Workorder is completed"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            timer.Dispose();
                            //loaddata(pono);

                            // Query to check is the wo/master card have this process
                            string woprocesssql = "SELECT plastic.wo.PO, plastic.parat.code, plastic.masterc.mascNo, plastic.woplan.hideBy " +
                                                  "FROM plastic.wo " +
                                                  "LEFT JOIN plastic.masterc ON plastic.wo.MASCID = plastic.masterc.ID " +
                                                  "LEFT JOIN plastic.woplan ON plastic.wo.MASCID = plastic.woplan.mascid " +
                                                  "LEFT JOIN plastic.woitem ON plastic.wo.ID = plastic.woitem.woId " +
                                                  "LEFT JOIN plastic.planwoprocess ON plastic.wo.MASCID = plastic.planwoprocess.mascid " +
                                                  "LEFT JOIN plastic.parat ON plastic.planwoprocess.paratId = plastic.parat.id " +
                                                  "WHERE plastic.wo.PO = '" + pono + "' AND plastic.woitem.item = '" + item + "' ";
                            //"AND plastic.woplan.hideBy = '0' ";                      



                            getAllWoProcess(woprocesssql, item);

                        }
                        catch (Exception)
                        {
                            timer.Dispose();
                            MessageBox.Show(string.Format("Invalid Workoreder Or This Workorder is completed"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }  
                    else
                    {
                        timer.Dispose();

                        label3.Text = "You Are Not Person Involve In Process. ";
                        label3.ForeColor = System.Drawing.Color.Red;

                        MessageBox.Show(string.Format("Mastercard don't have any process involve with your department. If you want to check the Mastercard history or job, please tab on the button on right."), "Not Person Involve", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return;
                    }
                }
            }
            ));
        }

        public class WoProcess
        {
            public string wopo;
            public string process;
            public string masterc;

            public WoProcess(string wopo, string process, string masterc)
            {
                this.wopo = wopo;
                this.process = process;
                this.masterc = masterc;
            }

            public string Wopo
            {
                get { return wopo; }
                set { wopo = value; }
            }

            public string Process
            {
                get { return process; }
                set { process = value; }
            }

            public string Masterc
            {
                get { return masterc; }
                set { masterc = value; }
            }
        }


        public void getAllWoProcess(string query, string item)
        {
            List<string> proccessStep = new List<string>();
            List<WoProcess> all = new List<WoProcess>();

            try
            {
                cmd = new MySqlCommand(query, conn);

                conn.Open();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if(reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            isComp = reader[3].ToString();
                            label4.Text = reader[2].ToString();
                            label9.Text = "Invalid";

                            Object[] values = new Object[reader.FieldCount];

                            // Get the Row with all its column values..
                            reader.GetValues(values);

                            // Check is there process for wo, if yes push all the possible process to one array of object
                            if(values[1].ToString() != null || values[1].ToString() != "")
                            {
                                all.Add(new WoProcess(values[0].ToString(), values[1].ToString(), values[2].ToString()));
                            } 
                        }
                    } 
                    reader.Close();
                }
                conn.Close();

                // Filter / Reduce the duplicate process of WO, left only one of each process
                // Exp: WO process: EXCU -> PRINT -> PRINT-> CUT-> CUT
                // Filter Result: EXCU, PRINT, CUT
                var tempArr = all.GroupBy(x => x.process).Select(grp => grp.First()).ToArray();

                foreach (var wolist in tempArr)
                {
                    //Put all process to one array
                    proccessStep.Add(wolist.process);
                }

                if(isComp == "0")
                {
                    // Run function to check the wo process step have process with current department
                    checkProcessStepHaveStepInDepartment(tempArr[0].wopo.ToString(), item, tempArr[0].masterc.ToString(), proccessStep.ToArray());
                }
                else
                {
                    MessageBox.Show(string.Format("This Workorder Already Completed, Please Return To Planner!"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    saveHistory(tempArr[0].masterc.ToString(), tempArr[0].wopo.ToString(), item, initialstate, "Invalid", true, 500);
                }      
            } 
            catch (Exception)
            {
                timer.Dispose();
                MessageBox.Show(string.Format("Invalid Workoreder Or This Workorder is completed"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } finally
            {
                conn.Close();
            }
        }

        public void checkProcessStepHaveStepInDepartment (string wopo, string itemno, string masc, string[] arr)
        {
            int i, j;
            string[] stepInDepart;
            List<string> termsList = new List<string>();

            // Add possible process to list depend on user loging
            if (initialstate == "Extrusion")
            {
                termsList.Clear();
                termsList.Add("EXTR");
            }

            if (initialstate == "Printing")
            {
                termsList.Clear();
                termsList.Add("PRINT");
            }

            if (initialstate == "Cutting")
            {
                termsList.Clear();
                termsList.Add("CUT");
                termsList.Add("SLIT");
            }

            stepInDepart = termsList.ToArray();

            // Console writeline possible process step
            foreach(var item in arr)
            {
                Console.WriteLine(item);
            }

            // Compare Department possible process with WO process
            for (i = 0; i < arr.Length; i++)
            {
                for(j = 0; j < stepInDepart.Length; j++)
                {
                    if(arr[i].ToString() == stepInDepart[j].ToString())
                    {
                        // if match any step in department array, perform checking
                        getWOData();
                        return;
                    } 
                    else
                    {
                        continue;
                    }
                }
            }

            // IF no match step in department array, prompt alert and save the scan location for record
            saveHistory(masc, wopo, itemno, initialstate, "Invalid", false, 300);
            MessageBox.Show(string.Format("This Workorder Doesn't Have This Process, Please Return The Workorder To Last Deparment!"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return; 
        }

        public void loaddata(string pono)
        {
            string woallsql = "SELECT plastic.masterc_record.mcr_no as msno, plastic.masterc_record.mcr_wo_no as wono, plastic.masterc_record.mcr_wo_no_item as poitem, plastic.masterc_record.mcr_location as mslocation, plastic.masterc_record.mcr_datetime as msdate, plastic.masterc_record.mcr_status as msstatus " +
                "FROM plastic.masterc_record " +
                "WHERE plastic.masterc_record.mcr_wo_no = '" + pono + "' " +
                "ORDER BY plastic.masterc_record.id DESC";

            getMSCHistory(woallsql);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2(this);
            frm.Show();
        }

        private void timer2_Tick_1(object sender, EventArgs e)
        {
            var splitted = textBox1.Text.Split('-');
            string pono = splitted[0];
            //string item = splitted[1];

            if (textBox1.Text != "")
            {
                loaddata(pono);
            }
        }

        // Dynamic Data Grid View Status Column Color differential
        private void dgv1_CellFormatting_1(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                foreach (DataGridViewRow myRow in dgv1.Rows)
                {
                    if (Convert.ToString(myRow.Cells[5].Value) != null)
                    {
                        if (Convert.ToString(myRow.Cells[5].Value) == "IN")
                        {
                            // HightLight row with background color
                            //myRow.DefaultCellStyle.BackColor = Color.ForestGreen;

                            // HightLight selected cell in row with background color
                            myRow.Cells["Status"].Style.BackColor = Color.ForestGreen;
                        }
                        else if (Convert.ToString(myRow.Cells[5].Value) == "OUT")
                        {
                            //myRow.DefaultCellStyle.BackColor = Color.RoyalBlue;
                            myRow.Cells["Status"].Style.BackColor = Color.RoyalBlue;

                        }
                        else if (Convert.ToString(myRow.Cells[5].Value) == "Invalid")
                        {
                            //myRow.DefaultCellStyle.BackColor = Color.Crimson;
                            myRow.Cells["Status"].Style.BackColor = Color.Crimson;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var splitted = textBox1.Text.Split('-');
            string pono = splitted[0];
            //string item = splitted[1];

            if (textBox1.Text != "")
            {
                loaddata(pono);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form3 frm = new Form3(this);
            frm.Show();
        }
    }
}
