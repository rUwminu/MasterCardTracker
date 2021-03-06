using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MasterCardTracker
{
    public partial class Form2 : Form
    {
        Form opener;

        private MySqlConnection conn;

        DataTable dt = new DataTable();

        MySqlCommand cmd = null;

        DateTime dateTime = DateTime.Now;

        static int VALIDATION_DELAY = 1500;
        System.Threading.Timer timer = null;

        public Form2(Form parentForm)
        {
            InitializeComponent();
            opener = parentForm;
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "yyyy-MM-dd";

            conn = new MySqlConnection();
            conn.ConnectionString = "server=localhost;uid=root; pwd=123456; database=plastic; Convert Zero Datetime=True; Allow Zero Datetime=True; default command timeout=300; ";
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            GetAllHistory();
        }

        public void getWoMasc (string query)
        {
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
                            label3.Text = reader[0].ToString();
                            label3.ForeColor = System.Drawing.Color.Green;
                            GetByWOHistory(reader[0].ToString());
                        }

                    } else
                    {
                        label3.Text = "No Workorder with this No!";
                        label3.ForeColor = System.Drawing.Color.Red;
                    }
                    reader.Close();
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

        public void GetAllHistory ()
        {
            dt.Clear();
            dgv1.Refresh();

            string woallsql = "SELECT plastic.masterc_record.mcr_no as msno, plastic.masterc_record.mcr_location as mslocation, plastic.masterc_record.mcr_datetime as msdate, plastic.masterc_record.mcr_wo_no as wono, plastic.masterc_record.mcr_status as msstatus " +
                              "FROM plastic.masterc_record " +
                              "ORDER BY plastic.masterc_record.id DESC";
                              

            cmd = new MySqlCommand(woallsql, conn);

            conn.Open();

            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            dgv1.DataSource = dt;
            dgv1.DataMember = dt.TableName;

            conn.Close();
        }

        public async void GetByWOHistory(string masterNo)
        {
            await Task.Delay(1000);

            dt.Clear();
            dgv1.Refresh();

            string woallsql = "SELECT plastic.masterc_record.mcr_no as msno, plastic.masterc_record.mcr_location as mslocation, plastic.masterc_record.mcr_datetime as msdate, plastic.masterc_record.mcr_wo_no as wono, plastic.masterc_record.mcr_status as msstatus " +
                              "FROM plastic.masterc_record " +
                              "WHERE plastic.masterc_record.mcr_no = '" + masterNo + "' " +
                              "ORDER BY plastic.masterc_record.id DESC";


            cmd = new MySqlCommand(woallsql, conn);

            conn.Open();

            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            dgv1.DataSource = dt;
            dgv1.DataMember = dt.TableName;

            conn.Close();
        }

        public void GetByDateHistory()
        {
            dt.Clear();
            dgv1.Refresh();

            string woallsql = "SELECT plastic.masterc_record.mcr_no as msno, plastic.masterc_record.mcr_location as mslocation, plastic.masterc_record.mcr_datetime as msdate, plastic.masterc_record.mcr_wo_no as wono, plastic.masterc_record.mcr_status as msstatus " +
                              "FROM plastic.masterc_record " +
                              "WHERE DATE(plastic.masterc_record.mcr_datetime) = '" + dateTimePicker1.Text + "' " +
                              "ORDER BY plastic.masterc_record.id DESC";


            cmd = new MySqlCommand(woallsql, conn);

            conn.Open();

            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            dgv1.DataSource = dt;
            dgv1.DataMember = dt.TableName;

            conn.Close();
        }

        public void GetByWOnDateHistory()
        {
            var splitted = textBox1.Text.Split('-');
            string pono = splitted[0];

            dt.Clear();
            dgv1.Refresh();

            string woallsql = "SELECT plastic.masterc_record.mcr_no as msno, plastic.masterc_record.mcr_wo_no as wono, plastic.masterc_record.mcr_location as mslocation, plastic.masterc_record.mcr_datetime as msdate, plastic.masterc_record.mcr_status as msstatus " +
                                "FROM plastic.masterc_record " +
                                "WHERE plastic.masterc_record.mcr_wo_no = '" + pono + "' AND DATE(plastic.masterc_record.mcr_datetime) = '" + dateTimePicker1.Text + "' " +
                                "ORDER BY plastic.masterc_record.id DESC";


            cmd = new MySqlCommand(woallsql, conn);

            conn.Open();

            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            dgv1.DataSource = dt;
            dgv1.DataMember = dt.TableName;

            conn.Close();
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
                if (textBox1.Text != "")
                {
                    var splitted = textBox1.Text.Split('-');
                    string pono = splitted[0];

                    string woallsql2 = "SELECT plastic.masterc.mascNo " +
                                       "FROM plastic.wo " +
                                       "LEFT JOIN plastic.masterc ON plastic.wo.MASCID = plastic.masterc.ID " + 
                                       "WHERE plastic.wo.PO = '" + pono + "' ";

                    getWoMasc(woallsql2);
                }
            }
            ));
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            GetByDateHistory();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetByWOnDateHistory();
        }

        // Dynamic Data Grid View Status Column Color differential
        public void dgv1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
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

        private void dgv1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
