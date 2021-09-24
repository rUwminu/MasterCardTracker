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

        static int VALIDATION_DELAY = 1500;
        System.Threading.Timer timer = null;

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
            dt.Clear();
            dgv1.Refresh();

            string woallsql = "SELECT demo.masterc_record.mcr_no as msno, demo.masterc_record.mcr_location as mslocation, demo.masterc_record.mcr_datetime as msdate, demo.workorder.id as wono, demo.masterc_record.mcr_status as msstatus " +
                              "FROM demo.workorder " +
                              "LEFT JOIN demo.masterc_record ON demo.workorder.WO_master = demo.masterc_record.mcr_no " +
                              "ORDER BY demo.masterc_record.id DESC";
                              

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

        public void GetByWOHistory()
        {
            dt.Clear();
            dgv1.Refresh();

            string woallsql = "SELECT demo.masterc_record.mcr_no as msno, demo.masterc_record.mcr_location as mslocation, demo.masterc_record.mcr_datetime as msdate, demo.workorder.id as wono, demo.masterc_record.mcr_status as msstatus " +
                              "FROM demo.workorder " +
                              "LEFT JOIN demo.masterc_record ON demo.workorder.WO_master = demo.masterc_record.mcr_no " +
                              "WHERE demo.workorder.id = '" + textBox1.Text + "' " +
                              "ORDER BY demo.masterc_record.id DESC";


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

            string woallsql = "SELECT demo.masterc_record.mcr_no as msno, demo.masterc_record.mcr_location as mslocation, demo.masterc_record.mcr_datetime as msdate, demo.workorder.id as wono, demo.masterc_record.mcr_status as msstatus " +
                              "FROM demo.workorder " +
                              "LEFT JOIN demo.masterc_record ON demo.workorder.WO_master = demo.masterc_record.mcr_no " +
                              "WHERE DATE(demo.masterc_record.mcr_datetime) = '" + dateTimePicker1.Text + "' " +
                              "ORDER BY demo.masterc_record.id DESC";


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

            string woallsql = "SELECT demo.masterc_record.mcr_no as msno, demo.masterc_record.mcr_location as mslocation, demo.masterc_record.mcr_datetime as msdate, demo.masterc_record.mcr_wo_no as wono, demo.masterc_record.mcr_status as msstatus " +
                              "FROM demo.workorder " +
                              "LEFT JOIN demo.masterc_record ON demo.workorder.WO_master = demo.masterc_record.mcr_no " +
                              "WHERE demo.masterc_record.mcr_wo_no = '" + textBox1.Text + "' AND DATE(demo.masterc_record.mcr_datetime) = '" + dateTimePicker1.Text + "' " +
                              "ORDER BY demo.masterc_record.id DESC";


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
                    GetByWOHistory();
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
