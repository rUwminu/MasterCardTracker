﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MasterCardTracker
{
    public partial class Form3 : Form
    {
        Form opener;

        private MySqlConnection conn;

        DataTable dt = new DataTable();

        MySqlCommand cmd = null;

        DateTime dateTime = DateTime.Now;

        static int VALIDATION_DELAY = 1500;
        System.Threading.Timer timer = null;

        public Form3(Form parentForm)
        {
            InitializeComponent();
            opener = parentForm;

            conn = new MySqlConnection();
            conn.ConnectionString = "server=localhost;uid=root; pwd=CBS12345678.; database=plastic; Convert Zero Datetime=True; Allow Zero Datetime=True; default command timeout=300; ";
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        public void getWoMasc(string query)
        {
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
                            textBox2.Text = reader[0].ToString();
                            textBox2.ForeColor = System.Drawing.Color.Green;
                            //GetByWOHistory(reader[0].ToString());
                        }

                    }
                    else
                    {
                        textBox2.Text = "No Workorder with this No!";
                        textBox2.ForeColor = System.Drawing.Color.Red;
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

        public async void getWoInProcess(string masc)
        {
            await Task.Delay(1000);

            dt.Clear();
            dgv1.Refresh();

            string woallsql = "SELECT plastic.wo.PO as wopo, plastic.wo.COMP as wocomp, plastic.masterc.mascNo as womasc, plastic.woplan.machineid as woplan " +
                               "FROM plastic.masterc " +
                               "LEFT JOIN plastic.wo ON plastic.masterc.ID = plastic.wo.MASCID " +
                               "LEFT JOIN plastic.woplan ON plastic.wo.ID = plastic.woplan.woitemid " +
                               "WHERE plastic.masterc.mascNo = '" + masc + "' " +
                               "ORDER BY plastic.woplan.id DESC LIMIT 10 ";

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

        public void dgv1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                foreach (DataGridViewRow myRow in dgv1.Rows)
                {
                    if (Convert.ToString(myRow.Cells[1].Value) != null)
                    {
                        if (Convert.ToInt32(myRow.Cells[1].Value) > 0)
                        {
                            // HightLight row with background color
                            //myRow.DefaultCellStyle.BackColor = Color.ForestGreen;

                            // HightLight selected cell in row with background color
                            myRow.Cells["Completed"].Style.BackColor = Color.ForestGreen;
                        }
                        else if (Convert.ToInt32(myRow.Cells[1].Value) < 0)
                        {
                            //myRow.DefaultCellStyle.BackColor = Color.RoyalBlue;
                            myRow.Cells["Completed"].Style.BackColor = Color.Crimson;

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