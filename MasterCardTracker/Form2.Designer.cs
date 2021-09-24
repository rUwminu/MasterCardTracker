
namespace MasterCardTracker
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.dgv1 = new System.Windows.Forms.DataGridView();
            this.mastercard_no = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.workorder_no = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ms_location = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.his_datetime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgv1)).BeginInit();
            this.SuspendLayout();
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateTimePicker1.Location = new System.Drawing.Point(281, 44);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 24);
            this.dateTimePicker1.TabIndex = 0;
            // 
            // dgv1
            // 
            this.dgv1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.mastercard_no,
            this.workorder_no,
            this.ms_location,
            this.his_datetime,
            this.status});
            this.dgv1.Location = new System.Drawing.Point(12, 102);
            this.dgv1.Name = "dgv1";
            this.dgv1.Size = new System.Drawing.Size(695, 382);
            this.dgv1.TabIndex = 8;
            // 
            // mastercard_no
            // 
            this.mastercard_no.DataPropertyName = "womsno";
            this.mastercard_no.HeaderText = "MasterCard No";
            this.mastercard_no.Name = "mastercard_no";
            this.mastercard_no.ReadOnly = true;
            this.mastercard_no.Width = 130;
            // 
            // workorder_no
            // 
            this.workorder_no.DataPropertyName = "wono";
            this.workorder_no.HeaderText = "WorkOrder No";
            this.workorder_no.Name = "workorder_no";
            this.workorder_no.ReadOnly = true;
            this.workorder_no.Width = 130;
            // 
            // ms_location
            // 
            this.ms_location.DataPropertyName = "mslocation";
            this.ms_location.HeaderText = "Last Deparment";
            this.ms_location.Name = "ms_location";
            this.ms_location.ReadOnly = true;
            this.ms_location.Width = 130;
            // 
            // his_datetime
            // 
            this.his_datetime.DataPropertyName = "msdate";
            this.his_datetime.HeaderText = "Date Time";
            this.his_datetime.Name = "his_datetime";
            this.his_datetime.ReadOnly = true;
            this.his_datetime.Width = 160;
            // 
            // status
            // 
            this.status.DataPropertyName = "msstatus";
            this.status.HeaderText = "Status";
            this.status.Name = "status";
            this.status.ReadOnly = true;
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(8, 46);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(200, 24);
            this.textBox1.TabIndex = 9;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(8, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(238, 24);
            this.label2.TabIndex = 10;
            this.label2.Text = "Sort By WO MasterCard No";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(277, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 24);
            this.label1.TabIndex = 11;
            this.label1.Text = "Sort By Date";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(504, 19);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(201, 51);
            this.button1.TabIndex = 12;
            this.button1.Text = "Sort By Date And MasterCard No";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(501, 86);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "label3";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(717, 496);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.dgv1);
            this.Controls.Add(this.dateTimePicker1);
            this.Name = "Form2";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.Form2_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.DataGridView dgv1;
        private System.Windows.Forms.DataGridViewTextBoxColumn mastercard_no;
        private System.Windows.Forms.DataGridViewTextBoxColumn workorder_no;
        private System.Windows.Forms.DataGridViewTextBoxColumn ms_location;
        private System.Windows.Forms.DataGridViewTextBoxColumn his_datetime;
        private System.Windows.Forms.DataGridViewTextBoxColumn status;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
    }
}