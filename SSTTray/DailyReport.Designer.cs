namespace TaskTrayApplication
{
    partial class DailyReport
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtEmpID = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnPrint = new System.Windows.Forms.Button();
            this.comboUDPCommand = new System.Windows.Forms.ComboBox();
            this.lbldailyReport = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioShift6 = new System.Windows.Forms.RadioButton();
            this.radioShift5 = new System.Windows.Forms.RadioButton();
            this.radioShift4 = new System.Windows.Forms.RadioButton();
            this.radioShift3 = new System.Windows.Forms.RadioButton();
            this.radioShift2 = new System.Windows.Forms.RadioButton();
            this.radioShift1 = new System.Windows.Forms.RadioButton();
            this.label10 = new System.Windows.Forms.Label();
            this.dtp1 = new System.Windows.Forms.DateTimePicker();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtEmpID);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.btnPrint);
            this.panel1.Controls.Add(this.comboUDPCommand);
            this.panel1.Controls.Add(this.lbldailyReport);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.dtp1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1099, 158);
            this.panel1.TabIndex = 65;
            // 
            // txtEmpID
            // 
            this.txtEmpID.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtEmpID.Location = new System.Drawing.Point(141, 85);
            this.txtEmpID.Margin = new System.Windows.Forms.Padding(4);
            this.txtEmpID.Name = "txtEmpID";
            this.txtEmpID.Size = new System.Drawing.Size(252, 36);
            this.txtEmpID.TabIndex = 76;
            this.txtEmpID.Text = "B_007";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(13, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 24);
            this.label3.TabIndex = 75;
            this.label3.Text = "員工編號";
            // 
            // btnPrint
            // 
            this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrint.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnPrint.Location = new System.Drawing.Point(850, 40);
            this.btnPrint.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(237, 59);
            this.btnPrint.TabIndex = 68;
            this.btnPrint.Text = "列印";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // comboUDPCommand
            // 
            this.comboUDPCommand.AutoCompleteCustomSource.AddRange(new string[] {
            "列印 RO QRCode",
            "HtmlToPdf"});
            this.comboUDPCommand.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboUDPCommand.FormattingEnabled = true;
            this.comboUDPCommand.Items.AddRange(new object[] {
            "列印切割日報表",
            "列印底漆日報表",
            "列印外檢底漆日報表",
            "列印加壓日報表",
            "列印色碼日報表",
            "列印全檢日報表",
            "列印貼帶日報表",
            "列印外檢色碼日報表"});
            this.comboUDPCommand.Location = new System.Drawing.Point(141, 22);
            this.comboUDPCommand.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboUDPCommand.Name = "comboUDPCommand";
            this.comboUDPCommand.Size = new System.Drawing.Size(252, 31);
            this.comboUDPCommand.TabIndex = 74;
            this.comboUDPCommand.SelectedIndexChanged += new System.EventHandler(this.comboUDPCommand_SelectedIndexChanged);
            // 
            // lbldailyReport
            // 
            this.lbldailyReport.AutoSize = true;
            this.lbldailyReport.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbldailyReport.Location = new System.Drawing.Point(4, 24);
            this.lbldailyReport.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbldailyReport.Name = "lbldailyReport";
            this.lbldailyReport.Size = new System.Drawing.Size(130, 24);
            this.lbldailyReport.TabIndex = 73;
            this.lbldailyReport.Text = "生產日報表";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioShift6);
            this.groupBox1.Controls.Add(this.radioShift5);
            this.groupBox1.Controls.Add(this.radioShift4);
            this.groupBox1.Controls.Add(this.radioShift3);
            this.groupBox1.Controls.Add(this.radioShift2);
            this.groupBox1.Controls.Add(this.radioShift1);
            this.groupBox1.Location = new System.Drawing.Point(507, 71);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Size = new System.Drawing.Size(308, 52);
            this.groupBox1.TabIndex = 71;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "班別";
            // 
            // radioShift6
            // 
            this.radioShift6.AutoSize = true;
            this.radioShift6.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioShift6.Location = new System.Drawing.Point(581, 20);
            this.radioShift6.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioShift6.Name = "radioShift6";
            this.radioShift6.Size = new System.Drawing.Size(90, 24);
            this.radioShift6.TabIndex = 74;
            this.radioShift6.Text = "大中班";
            this.radioShift6.UseVisualStyleBackColor = true;
            this.radioShift6.Visible = false;
            // 
            // radioShift5
            // 
            this.radioShift5.AutoSize = true;
            this.radioShift5.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioShift5.Location = new System.Drawing.Point(461, 18);
            this.radioShift5.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioShift5.Name = "radioShift5";
            this.radioShift5.Size = new System.Drawing.Size(90, 24);
            this.radioShift5.TabIndex = 73;
            this.radioShift5.Text = "大早班";
            this.radioShift5.UseVisualStyleBackColor = true;
            this.radioShift5.Visible = false;
            // 
            // radioShift4
            // 
            this.radioShift4.AutoSize = true;
            this.radioShift4.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioShift4.Location = new System.Drawing.Point(333, 18);
            this.radioShift4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioShift4.Name = "radioShift4";
            this.radioShift4.Size = new System.Drawing.Size(110, 24);
            this.radioShift4.TabIndex = 72;
            this.radioShift4.Text = "跨早中8H";
            this.radioShift4.UseVisualStyleBackColor = true;
            this.radioShift4.Visible = false;
            // 
            // radioShift3
            // 
            this.radioShift3.AutoSize = true;
            this.radioShift3.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioShift3.Location = new System.Drawing.Point(228, 19);
            this.radioShift3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioShift3.Name = "radioShift3";
            this.radioShift3.Size = new System.Drawing.Size(70, 24);
            this.radioShift3.TabIndex = 71;
            this.radioShift3.Text = "晚班";
            this.radioShift3.UseVisualStyleBackColor = true;
            // 
            // radioShift2
            // 
            this.radioShift2.AutoSize = true;
            this.radioShift2.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioShift2.Location = new System.Drawing.Point(129, 19);
            this.radioShift2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioShift2.Name = "radioShift2";
            this.radioShift2.Size = new System.Drawing.Size(70, 24);
            this.radioShift2.TabIndex = 70;
            this.radioShift2.Text = "中班";
            this.radioShift2.UseVisualStyleBackColor = true;
            // 
            // radioShift1
            // 
            this.radioShift1.AutoSize = true;
            this.radioShift1.Checked = true;
            this.radioShift1.Font = new System.Drawing.Font("標楷體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioShift1.Location = new System.Drawing.Point(29, 19);
            this.radioShift1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioShift1.Name = "radioShift1";
            this.radioShift1.Size = new System.Drawing.Size(70, 24);
            this.radioShift1.TabIndex = 69;
            this.radioShift1.TabStop = true;
            this.radioShift1.Text = "早班";
            this.radioShift1.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label10.Location = new System.Drawing.Point(503, 22);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(106, 24);
            this.label10.TabIndex = 58;
            this.label10.Text = "生產日期";
            // 
            // dtp1
            // 
            this.dtp1.CalendarFont = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dtp1.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dtp1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtp1.Location = new System.Drawing.Point(625, 16);
            this.dtp1.Margin = new System.Windows.Forms.Padding(4);
            this.dtp1.Name = "dtp1";
            this.dtp1.Size = new System.Drawing.Size(188, 39);
            this.dtp1.TabIndex = 51;
            this.dtp1.Value = new System.DateTime(2018, 6, 19, 0, 0, 0, 0);
            // 
            // DailyReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1099, 176);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "DailyReport";
            this.Text = "DailyReport";
            this.Load += new System.EventHandler(this.DailyReport_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioShift6;
        private System.Windows.Forms.RadioButton radioShift5;
        private System.Windows.Forms.RadioButton radioShift4;
        private System.Windows.Forms.RadioButton radioShift3;
        private System.Windows.Forms.RadioButton radioShift2;
        private System.Windows.Forms.RadioButton radioShift1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.DateTimePicker dtp1;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Label lbldailyReport;
        private System.Windows.Forms.ComboBox comboUDPCommand;
        private System.Windows.Forms.TextBox txtEmpID;
        private System.Windows.Forms.Label label3;
    }
}