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
        //string initialstate = "Printing";
        // string initialstate = "Cutting";
         string initialstate = "Final";

        static int VALIDATION_DELAY = 1500;
        System.Threading.Timer timer = null;

        public Form1()
        {
            InitializeComponent();

            
            conn = new MySqlConnection();
            conn.ConnectionString = "server=localhost;uid=root; pwd=CBS12345678.; database=demo; Convert Zero Datetime=True; Allow Zero Datetime=True; default command timeout=300; ";
        }
        private async void saveHistory(string MSid, string WOid, string location, string status)
        {
            // Delay This task for last task to complete sql read and connection to close *Testing Beta
            await Task.Delay(3000);

            string insertinfo = "INSERT INTO demo.masterc_record(mcr_no, mcr_wo_no, mcr_location, mcr_datetime, mcr_status) VALUES (@mcr_no, @mcr_wo_no, @mcr_location, @mcr_datetime, @mcr_status)";


            conn.Open();

            MySqlCommand upcmd = new MySqlCommand(insertinfo, conn);

            upcmd.Parameters.AddWithValue("@mcr_no", MSid);
            upcmd.Parameters.AddWithValue("@mcr_wo_no", WOid);
            upcmd.Parameters.AddWithValue("@mcr_location", location);
            upcmd.Parameters.AddWithValue("@mcr_datetime", dateTime.ToString("yyyy-MM-dd hh:mm:ss"));
            upcmd.Parameters.AddWithValue("@mcr_status", status);

            upcmd.ExecuteNonQuery();

            conn.Close();

            label3.Text = "Done Checkin. MasterCard Update to this deparment.";
            label3.ForeColor = System.Drawing.Color.Green;

            return;
        }
        private void getWOData (string query)
    {
            try
            {
                cmd = new MySqlCommand(query, conn);

                conn.Open();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if(reader.HasRows){
                        while (reader.Read())
                        {
                            string msid = reader[3].ToString();
                            string woid = reader[2].ToString();
                            string lastlocation = reader[1].ToString();
                            string proccessStep = reader[4].ToString();
                            string msStatus = reader[5].ToString(); // **Not in use yet**
                            label4.Text = msid;
                            label6.Text = lastlocation;
                            label9.Text = proccessStep;

                            //This if/else check user department and the posible Workorder can pass to this user deparment
                            if (initialstate == "Final")
                            {
                                if (msid != "" && lastlocation == initialstate)
                                {
                                    label3.Text = "MasterCard Already Here.";
                                    label3.ForeColor = System.Drawing.Color.Green;                                    

                                    return;
                                }
                                else if (lastlocation != initialstate)
                                {
                                    label3.Text = "New MasterCard Come In, Doing Check-In...";
                                    label3.ForeColor = System.Drawing.Color.Red;
                                    label4.Text = msid;

                                    saveHistory(msid, woid, initialstate, "IN");
                                    saveHistory(msid, woid, lastlocation, "OUT");

                                    return;
                                }
                            } 
                            else if (initialstate == "Extrusion")
                            {
                                // This if/else check the mastercard last location and compare with current location to let user know is mastercard here or will update to here
                                if (msid != "" && lastlocation == initialstate)
                                {
                                    label3.Text = "MasterCard Already Here.";
                                    label3.ForeColor = System.Drawing.Color.Green;

                                    return;
                                }
                                else if (lastlocation != initialstate)
                                {
                                    label3.Text = "New MasterCard Come In, Doing Check-In...";
                                    label3.ForeColor = System.Drawing.Color.Red;
                                    label4.Text = msid;

                                    saveHistory(msid, woid, initialstate, "IN");
                                    saveHistory(msid, woid, lastlocation, "OUT");

                                    return;
                                }
                            }
                            else if (initialstate == "Printing")
                            {
                                if (msid != "" && lastlocation == initialstate)
                                {
                                    label3.Text = "MasterCard Already Here.";
                                    label3.ForeColor = System.Drawing.Color.Green;

                                    return;
                                }
                                else if (lastlocation != initialstate)
                                {
                                    label3.Text = "New MasterCard Come In, Doing Check-In...";
                                    label3.ForeColor = System.Drawing.Color.Red;
                                    label4.Text = msid;

                                    saveHistory(msid, woid, initialstate, "IN");
                                    saveHistory(msid, woid, lastlocation, "OUT");

                                    return;
                                }
                            }
                            else if (initialstate == "Cutting")
                            {
                                if (msid != "" && lastlocation == initialstate)
                                {
                                    label3.Text = "MasterCard Already Here.";
                                    label3.ForeColor = System.Drawing.Color.Green;

                                    return;
                                }
                                else if (lastlocation != initialstate)
                                {
                                    label3.Text = "New MasterCard Come In, Doing Check-In...";
                                    label3.ForeColor = System.Drawing.Color.Red;
                                    label4.Text = msid;

                                    saveHistory(msid, woid, initialstate, "IN");
                                    saveHistory(msid, woid, lastlocation, "OUT");

                                    return;
                                }
                            }
                            else
                            {
                                label3.Text = "MasterCard Doesn't Have This Process";
                                label3.ForeColor = System.Drawing.Color.Red;
                            }
                        }
                    } else
                    {
                        label3.Text = "No Workorder with this id!";
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("An error occurred {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } finally
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
            //if (textBox1.Text != "")
            //{
            //    string woallsql = "SELECT demo.masterc_record.mcr_no, demo.masterc_record.mcr_location, demo.workorder.id, demo.workorder.WO_master, demo.workorder.WO_proccess, demo.masterc_record.mcr_status " +
            //                      "FROM demo.workorder " +
            //                      "LEFT JOIN demo.masterc_record ON demo.workorder.WO_master = demo.masterc_record.mcr_no " +
            //                      "WHERE demo.workorder.id = '" + textBox1.Text + "' " +
            //                      "ORDER BY demo.masterc_record.id DESC";

            //    getWOData(woallsql);
            //    loaddata();
            //}

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
                if (textBox1.Text != "")
                {
                    string woallsql = "SELECT demo.masterc_record.mcr_no, demo.masterc_record.mcr_location, demo.workorder.id, demo.workorder.WO_master, demo.workorder.WO_proccess, demo.masterc_record.mcr_status " +
                                      "FROM demo.workorder " +
                                      "LEFT JOIN demo.masterc_record ON demo.workorder.WO_master = demo.masterc_record.mcr_no " +
                                      "WHERE demo.workorder.id = '" + textBox1.Text + "' " +
                                      "ORDER BY demo.masterc_record.id DESC";

                    getWOData(woallsql);
                    loaddata();
                }
            }
            ));
        }

        public void loaddata()
        {
            string woallsql = "SELECT demo.masterc_record.mcr_no as msno, demo.masterc_record.mcr_location as mslocation, demo.workorder.id as wono, demo.workorder.WO_master as womsno, demo.masterc_record.mcr_datetime as msdate, demo.masterc_record.mcr_status as msstatus " +
                "FROM demo.workorder " +
                "LEFT JOIN demo.masterc_record ON demo.workorder.WO_master = demo.masterc_record.mcr_no " +
                "WHERE demo.workorder.id = '" + textBox1.Text + "' " +
                "ORDER BY demo.masterc_record.id DESC";

            getMSCHistory(woallsql);
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if(textBox1.Text != "")
            {
                loaddata();
            } 
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2(this);
            frm.Show();
        }
    }
}
