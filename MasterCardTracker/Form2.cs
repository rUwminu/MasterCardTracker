using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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

        string[] historyList;
        public Form2(Form parentForm)
        {
            InitializeComponent();
            opener = parentForm;
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "yyyy-MM-dd";

            conn = new MySqlConnection();
            conn.ConnectionString = "server=localhost;uid=root; pwd=CBS12345678.; database=demo; Convert Zero Datetime=True; Allow Zero Datetime=True; default command timeout=300; ";
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            GetAllHistory();
        }

        public void GetAllHistory ()
        {
            string woallsql = "SELECT demo.masterc_record.mcr_no as msno, demo.masterc_record.mcr_location as mslocation, demo.masterc_record.mcr_datetime as msdate, demo.workorder.id as wono, demo.workorder.WO_master as womsno, demo.masterc_record.mcr_status as msstatus " +
                              "FROM demo.workorder " +
                              "LEFT JOIN demo.masterc_record ON demo.workorder.WO_master = demo.masterc_record.mcr_no " +
                              "ORDER BY demo.masterc_record.id DESC";
                              

            cmd = new MySqlCommand(woallsql, conn);

            conn.Open();

            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            conn.Close();
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
            label3.Text = dateTime.ToString();
        }
    }
}
