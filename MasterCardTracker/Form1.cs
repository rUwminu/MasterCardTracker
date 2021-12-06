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
         string initialstate = "Printing";
        // string initialstate = "Cutting";
        // string initialstate = "Planning";

        static int VALIDATION_DELAY = 1500;
        System.Threading.Timer timer = null;

        string msid;
        string woid;
        string lastlocation;
        string proccessStep;

        public Form1()
        {
            InitializeComponent();
            
            conn = new MySqlConnection();
            conn.ConnectionString = "server=localhost;uid=root; pwd=CBS12345678.; database=plastic; Convert Zero Datetime=True; Allow Zero Datetime=True; default command timeout=300; ";
        }

        public async void saveHistory(string MSid, string WOid, string location, string status, bool valid, int delay)
        {
            // Delay This task for last task to complete sql read and connection to close *Testing Beta
            await Task.Delay(delay);

            string insertinfo = "INSERT INTO plastic.masterc_record(mcr_no, mcr_wo_no, mcr_location, mcr_datetime, mcr_status) VALUES (@mcr_no, @mcr_wo_no, @mcr_location, @mcr_datetime, @mcr_status)";

            conn.Open();

            MySqlCommand upcmd = new MySqlCommand(insertinfo, conn);

            upcmd.Parameters.AddWithValue("@mcr_no", MSid);
            upcmd.Parameters.AddWithValue("@mcr_wo_no", WOid);
            upcmd.Parameters.AddWithValue("@mcr_location", location);
            upcmd.Parameters.AddWithValue("@mcr_datetime", dateTime.ToString("yyyy-MM-dd hh:mm:ss"));
            upcmd.Parameters.AddWithValue("@mcr_status", status);

            upcmd.ExecuteNonQuery();

            conn.Close();

            if(valid)
            {
                label3.Text = "Done Checkin. MasterCard Update to this deparment.";
                label3.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                label3.Text = "Workorder Don't have this process";
                label3.ForeColor = System.Drawing.Color.Red;
            }

            return;
        }

        private async void getWOData ()
        {
            Console.WriteLine("Running Checking");

            bool isCheck = false;
            var splitted = textBox1.Text.Split('-');
            string pono = splitted[0];

            string query = "SELECT plastic.masterc_record.mcr_no, plastic.masterc_record.mcr_location, plastic.wo.PO, plastic.masterc.mascNo, plastic.masterc_record.mcr_status " +
                            "FROM plastic.wo " +
                            "LEFT JOIN plastic.masterc ON plastic.wo.MASCID = plastic.masterc.ID " +
                            "LEFT JOIN plastic.masterc_record ON plastic.masterc.mascNo = plastic.masterc_record.mcr_no " +
                            "LEFT JOIN plastic.woitem ON plastic.wo.ID = plastic.woitem.woId " +
                            "LEFT JOIN ( SELECT * FROM plastic.woplan ) AS b ON plastic.woitem.id = b.woitemid " +
                            "WHERE plastic.wo.PO = '" + pono + "' AND NOT plastic.masterc_record.mcr_status = 'Invalid' " +
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
                    if (reader.HasRows){
                        while (reader.Read())
                        {
                            msid = reader[3].ToString();
                            woid = reader[2].ToString();
                            lastlocation = reader[1].ToString();
                            proccessStep = reader[4].ToString();
                            //string msStatus = reader[5].ToString(); // **Not in use yet**
                            label4.Text = msid;
                            label6.Text = lastlocation;
                            label9.Text = proccessStep;

                            //This if/else check user department and the posible Workorder can pass to this user deparment
                            if (initialstate == "Final")
                            {
                                if (msid != "" && lastlocation == initialstate && pono == woid)
                                {
                                    label3.Text = "MasterCard Already Here.";
                                    label3.ForeColor = System.Drawing.Color.Green;                                    

                                    return;
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
                                else if (lastlocation != initialstate)
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
                                else if (lastlocation != initialstate)
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
                                else if (lastlocation != initialstate)
                                {
                                    isCheck = true;
                                    label3.Text = "New MasterCard Come In, Doing Check-In...";
                                    label3.ForeColor = System.Drawing.Color.Red;
                                    label4.Text = msid;
                                }
                            }
                            else
                            {
                                label3.Text = "MasterCard Doesn't Have This Process";
                                label3.ForeColor = System.Drawing.Color.Red;
                            }
                        }

                        reader.Close();
                        conn.Close();
                    } 

                    if (!reader.HasRows)
                    {
                        // If is new workorder pass down to production, this function run and record
                        getNewWoDataAndSave();
                        return;
                    }                  
                }

                if(isCheck == true)
                {
                    saveHistory(msid, woid, initialstate, "IN", true, 500);
                    saveHistory(msid, woid, lastlocation, "OUT", true, 300);                                     
                }   
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

        public async void getNewWoDataAndSave()
        {
            await Task.Delay(300);

            var splitted = textBox1.Text.Split('-');
            string pono = splitted[0];
            string item = splitted[1];

            // Workorder scan, record mastercard location.
            string query = "SELECT plastic.wo.PO, plastic.masterc.mascNo " +
                           "FROM plastic.wo " +
                           "LEFT JOIN plastic.masterc ON plastic.wo.MASCID = plastic.masterc.ID " +
                           "LEFT JOIN plastic.woitem ON plastic.wo.ID = plastic.woitem.woId " +
                           "LEFT JOIN ( SELECT * FROM plastic.woplan ) AS b ON plastic.woitem.id = b.woitemid " +
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

                saveHistory(msid, woid, initialstate, "IN", true, 500);

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
                    try
                    {
                        var splitted = textBox1.Text.Split('-');
                        string pono = splitted[0];
                        string item = splitted[1];

                        timer.Dispose();                        
                        //loaddata(pono);

                        // Query to check is the wo/master card have this process
                        string woprocesssql = "SELECT plastic.wo.PO, plastic.parat.code, plastic.masterc.mascNo " +
                                              "FROM plastic.wo " +
                                              "LEFT JOIN plastic.masterc ON plastic.wo.MASCID = plastic.masterc.ID " +
                                              "LEFT JOIN plastic.planwoprocess ON plastic.wo.MASCID = plastic.planwoprocess.mascid " +
                                              "LEFT JOIN plastic.parat ON plastic.planwoprocess.paratId = plastic.parat.id " +
                                              "WHERE plastic.wo.PO = '" + pono + "' ";


                        getAllWoProcess(woprocesssql);
                    }
                    catch (Exception ex)
                    {
                        timer.Dispose();
                        MessageBox.Show(string.Format("Invalid Workoreder Number"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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


        public void getAllWoProcess(string query)
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

                // Run function to check the wo process step have process with current department
                checkProcessStepHaveStepInDepartment(tempArr[0].wopo.ToString(), tempArr[0].masterc.ToString(), proccessStep.ToArray());
            } 
            catch (Exception ex)
            {
                timer.Dispose();
                // MessageBox.Show(string.Format("Invalid Workoreder Number"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } finally
            {
                conn.Close();
            }
        }

        public void checkProcessStepHaveStepInDepartment (string wopo, string masc, string[] arr)
        {
            int i, j;
            string[] stepInDepart;
            List<string> termsList = new List<string>();

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

            // Last or Return Mastercard to Admin/Planner/CS, just record the info.
            if (initialstate == "Planning")
            {
                saveHistory(masc, wopo, initialstate, "IN", true, 300);
                return;
            }

            stepInDepart = termsList.ToArray();

            // Console writeline possible process step
            foreach(var item in arr)
            {
                Console.WriteLine(item);
            }

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
            saveHistory(masc, wopo, initialstate, "Invalid", false, 300);
            MessageBox.Show(string.Format("This Workorder Doesn't Have This Process, Please Return The Workorder To Last Deparment!"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return; 
        }

        public void loaddata(string pono)
        {
            // TODO change db string
            string woallsql = "SELECT plastic.masterc_record.mcr_no as msno, plastic.masterc_record.mcr_wo_no as wono, plastic.masterc_record.mcr_location as mslocation, plastic.masterc_record.mcr_datetime as msdate, plastic.masterc_record.mcr_status as msstatus " +
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
                    if (Convert.ToString(myRow.Cells[4].Value) != null)
                    {
                        if (Convert.ToString(myRow.Cells[4].Value) == "IN")
                        {
                            // HightLight row with background color
                            //myRow.DefaultCellStyle.BackColor = Color.ForestGreen;

                            // HightLight selected cell in row with background color
                            myRow.Cells["Status"].Style.BackColor = Color.ForestGreen;
                        }
                        else if (Convert.ToString(myRow.Cells[4].Value) == "OUT")
                        {
                            //myRow.DefaultCellStyle.BackColor = Color.RoyalBlue;
                            myRow.Cells["Status"].Style.BackColor = Color.RoyalBlue;

                        }
                        else if (Convert.ToString(myRow.Cells[4].Value) == "Invalid")
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
    }
}
