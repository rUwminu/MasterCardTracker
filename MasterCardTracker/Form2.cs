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
            conn.ConnectionString = "server=localhost;uid=root; pwd=CBS12345678.; database=plastic; Convert Zero Datetime=True; Allow Zero Datetime=True; default command timeout=300; ";
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
                            GetByWOHistory(reader[0].ToString());
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
            await Task.Delay(3000);

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
            dt.Clear();
            dgv1.Refresh();

            string woallsql = "SELECT plastic.masterc_record.mcr_no as msno, plastic.masterc_record.mcr_wo_no as wono, plastic.masterc_record.mcr_location as mslocation, plastic.masterc_record.mcr_datetime as msdate, plastic.masterc_record.mcr_status as msstatus " +
                                "FROM plastic.masterc_record " +
                                "WHERE plastic.masterc_record.mcr_wo_no = '" + label3.Text + "' " +
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

        private void dgv1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            GetByDateHistory();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetByWOnDateHistory();
        }
    }
}
