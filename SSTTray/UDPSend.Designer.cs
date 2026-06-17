namespace TaskTrayApplication
{
    partial class UDPSend
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UDPSend));
            this.btnUDPSend = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTemplateFile = new System.Windows.Forms.TextBox();
            this.comboStandardReportRpt = new System.Windows.Forms.TabControl();
            this.標準標籤 = new System.Windows.Forms.TabPage();
            this.label9 = new System.Windows.Forms.Label();
            this.chkboxDataFrom = new System.Windows.Forms.CheckBox();
            this.txtPrintAttrs = new System.Windows.Forms.TextBox();
            this.btnSendStandard = new System.Windows.Forms.Button();
            this.comboStandardLabel = new System.Windows.Forms.ComboBox();
            this.txtLabelData = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textData = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtExcelLayout = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.標準報表 = new System.Windows.Forms.TabPage();
            this.button2 = new System.Windows.Forms.Button();
            this.txtStaticData = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.txtStaticLayout = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.chkboxReportDataFrom = new System.Windows.Forms.CheckBox();
            this.txtReportPrintAttrs = new System.Windows.Forms.TextBox();
            this.comboStandardReport = new System.Windows.Forms.ComboBox();
            this.txtReportData = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.textReportData = new System.Windows.Forms.TextBox();
            this.txtReportTemplateFile = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.個別標籤 = new System.Windows.Forms.TabPage();
            this.txtFinalCommand = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtUDPContent = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboUDPCommand = new System.Windows.Forms.ComboBox();
            this.tabJsonPnt = new System.Windows.Forms.TabPage();
            this.label19 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.txtJsonPrint = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.newJson = new System.Windows.Forms.TabPage();
            this.comboNewJson = new System.Windows.Forms.ComboBox();
            this.txtNewJson = new System.Windows.Forms.TextBox();
            this.btnNewUDP = new System.Windows.Forms.Button();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.textUdpIP = new System.Windows.Forms.TextBox();
            this.comboStandardReportRpt.SuspendLayout();
            this.標準標籤.SuspendLayout();
            this.標準報表.SuspendLayout();
            this.個別標籤.SuspendLayout();
            this.tabJsonPnt.SuspendLayout();
            this.newJson.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnUDPSend
            // 
            this.btnUDPSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUDPSend.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnUDPSend.Location = new System.Drawing.Point(568, 10);
            this.btnUDPSend.Margin = new System.Windows.Forms.Padding(4);
            this.btnUDPSend.Name = "btnUDPSend";
            this.btnUDPSend.Size = new System.Drawing.Size(189, 49);
            this.btnUDPSend.TabIndex = 8;
            this.btnUDPSend.Text = "發送 UDP3";
            this.btnUDPSend.UseVisualStyleBackColor = true;
            this.btnUDPSend.Click += new System.EventHandler(this.btnUDPSend_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(12, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 24);
            this.label3.TabIndex = 12;
            this.label3.Text = "UPD Peer";
            // 
            // txtTemplateFile
            // 
            this.txtTemplateFile.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtTemplateFile.Location = new System.Drawing.Point(180, 48);
            this.txtTemplateFile.Margin = new System.Windows.Forms.Padding(4);
            this.txtTemplateFile.Name = "txtTemplateFile";
            this.txtTemplateFile.Size = new System.Drawing.Size(598, 36);
            this.txtTemplateFile.TabIndex = 14;
            this.txtTemplateFile.Text = "C:\\sysTray\\母版\\成品入庫標籤母版.xlsx";
            // 
            // comboStandardReportRpt
            // 
            this.comboStandardReportRpt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboStandardReportRpt.Controls.Add(this.標準標籤);
            this.comboStandardReportRpt.Controls.Add(this.標準報表);
            this.comboStandardReportRpt.Controls.Add(this.個別標籤);
            this.comboStandardReportRpt.Controls.Add(this.tabJsonPnt);
            this.comboStandardReportRpt.Controls.Add(this.newJson);
            this.comboStandardReportRpt.Location = new System.Drawing.Point(16, 61);
            this.comboStandardReportRpt.Name = "comboStandardReportRpt";
            this.comboStandardReportRpt.SelectedIndex = 0;
            this.comboStandardReportRpt.Size = new System.Drawing.Size(801, 538);
            this.comboStandardReportRpt.TabIndex = 17;
            // 
            // 標準標籤
            // 
            this.標準標籤.Controls.Add(this.label9);
            this.標準標籤.Controls.Add(this.chkboxDataFrom);
            this.標準標籤.Controls.Add(this.txtPrintAttrs);
            this.標準標籤.Controls.Add(this.btnSendStandard);
            this.標準標籤.Controls.Add(this.comboStandardLabel);
            this.標準標籤.Controls.Add(this.txtLabelData);
            this.標準標籤.Controls.Add(this.label8);
            this.標準標籤.Controls.Add(this.label7);
            this.標準標籤.Controls.Add(this.textData);
            this.標準標籤.Controls.Add(this.txtTemplateFile);
            this.標準標籤.Controls.Add(this.label5);
            this.標準標籤.Controls.Add(this.txtExcelLayout);
            this.標準標籤.Controls.Add(this.label6);
            this.標準標籤.Location = new System.Drawing.Point(4, 25);
            this.標準標籤.Name = "標準標籤";
            this.標準標籤.Padding = new System.Windows.Forms.Padding(3);
            this.標準標籤.Size = new System.Drawing.Size(793, 509);
            this.標準標籤.TabIndex = 1;
            this.標準標籤.Text = "標準標籤";
            this.標準標籤.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(467, 27);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(167, 15);
            this.label9.TabIndex = 35;
            this.label9.Text = "(不是使用畫面上的設定)";
            // 
            // chkboxDataFrom
            // 
            this.chkboxDataFrom.AutoSize = true;
            this.chkboxDataFrom.Location = new System.Drawing.Point(520, 6);
            this.chkboxDataFrom.Name = "chkboxDataFrom";
            this.chkboxDataFrom.Size = new System.Drawing.Size(112, 19);
            this.chkboxDataFrom.TabIndex = 34;
            this.chkboxDataFrom.Tag = "";
            this.chkboxDataFrom.Text = "直接 Property";
            this.chkboxDataFrom.UseVisualStyleBackColor = true;
            // 
            // txtPrintAttrs
            // 
            this.txtPrintAttrs.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtPrintAttrs.Location = new System.Drawing.Point(413, 89);
            this.txtPrintAttrs.Margin = new System.Windows.Forms.Padding(4);
            this.txtPrintAttrs.Multiline = true;
            this.txtPrintAttrs.Name = "txtPrintAttrs";
            this.txtPrintAttrs.Size = new System.Drawing.Size(365, 101);
            this.txtPrintAttrs.TabIndex = 33;
            // 
            // btnSendStandard
            // 
            this.btnSendStandard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSendStandard.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSendStandard.Location = new System.Drawing.Point(663, 10);
            this.btnSendStandard.Margin = new System.Windows.Forms.Padding(4);
            this.btnSendStandard.Name = "btnSendStandard";
            this.btnSendStandard.Size = new System.Drawing.Size(123, 33);
            this.btnSendStandard.TabIndex = 23;
            this.btnSendStandard.Text = "發送 UDP1";
            this.btnSendStandard.UseVisualStyleBackColor = true;
            this.btnSendStandard.Click += new System.EventHandler(this.btnSendStandard_Click);
            // 
            // comboStandardLabel
            // 
            this.comboStandardLabel.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboStandardLabel.FormattingEnabled = true;
            this.comboStandardLabel.Location = new System.Drawing.Point(211, 10);
            this.comboStandardLabel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboStandardLabel.Name = "comboStandardLabel";
            this.comboStandardLabel.Size = new System.Drawing.Size(239, 31);
            this.comboStandardLabel.TabIndex = 32;
            this.comboStandardLabel.SelectedIndexChanged += new System.EventHandler(this.comboStandardLabel_SelectedIndexChanged);
            // 
            // txtLabelData
            // 
            this.txtLabelData.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtLabelData.Location = new System.Drawing.Point(21, 224);
            this.txtLabelData.Margin = new System.Windows.Forms.Padding(4);
            this.txtLabelData.Multiline = true;
            this.txtLabelData.Name = "txtLabelData";
            this.txtLabelData.Size = new System.Drawing.Size(747, 114);
            this.txtLabelData.TabIndex = 31;
            this.txtLabelData.Text = resources.GetString("txtLabelData.Text");
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.label8.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label8.Location = new System.Drawing.Point(17, 196);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(130, 24);
            this.label8.TabIndex = 30;
            this.label8.Text = "Label Data";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Gainsboro;
            this.label7.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label7.Location = new System.Drawing.Point(17, 57);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(142, 24);
            this.label7.TabIndex = 29;
            this.label7.Text = "列印參數 : ";
            // 
            // textData
            // 
            this.textData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textData.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textData.Location = new System.Drawing.Point(20, 381);
            this.textData.Margin = new System.Windows.Forms.Padding(4);
            this.textData.Multiline = true;
            this.textData.Name = "textData";
            this.textData.ReadOnly = true;
            this.textData.Size = new System.Drawing.Size(755, 114);
            this.textData.TabIndex = 28;
            this.textData.Text = "\r\n";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Gainsboro;
            this.label5.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label5.Location = new System.Drawing.Point(16, 353);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(610, 24);
            this.label5.TabIndex = 27;
            this.label5.Text = "UDP Content : UDPLib.udpSend(jsonToSend, IP, Port)";
            // 
            // txtExcelLayout
            // 
            this.txtExcelLayout.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtExcelLayout.Location = new System.Drawing.Point(20, 89);
            this.txtExcelLayout.Margin = new System.Windows.Forms.Padding(4);
            this.txtExcelLayout.Multiline = true;
            this.txtExcelLayout.Name = "txtExcelLayout";
            this.txtExcelLayout.Size = new System.Drawing.Size(385, 101);
            this.txtExcelLayout.TabIndex = 26;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Gainsboro;
            this.label6.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label6.Location = new System.Drawing.Point(16, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(190, 24);
            this.label6.TabIndex = 25;
            this.label6.Text = "標籤母版檔案 : ";
            // 
            // 標準報表
            // 
            this.標準報表.Controls.Add(this.button2);
            this.標準報表.Controls.Add(this.txtStaticData);
            this.標準報表.Controls.Add(this.label16);
            this.標準報表.Controls.Add(this.label15);
            this.標準報表.Controls.Add(this.txtStaticLayout);
            this.標準報表.Controls.Add(this.label10);
            this.標準報表.Controls.Add(this.chkboxReportDataFrom);
            this.標準報表.Controls.Add(this.txtReportPrintAttrs);
            this.標準報表.Controls.Add(this.comboStandardReport);
            this.標準報表.Controls.Add(this.txtReportData);
            this.標準報表.Controls.Add(this.label11);
            this.標準報表.Controls.Add(this.label12);
            this.標準報表.Controls.Add(this.textReportData);
            this.標準報表.Controls.Add(this.txtReportTemplateFile);
            this.標準報表.Controls.Add(this.label13);
            this.標準報表.Controls.Add(this.label14);
            this.標準報表.Location = new System.Drawing.Point(4, 25);
            this.標準報表.Name = "標準報表";
            this.標準報表.Size = new System.Drawing.Size(793, 509);
            this.標準報表.TabIndex = 2;
            this.標準報表.Text = "標準報表";
            this.標準報表.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.button2.Location = new System.Drawing.Point(649, 6);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(124, 39);
            this.button2.TabIndex = 38;
            this.button2.Text = "發送 UDP2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtStaticData
            // 
            this.txtStaticData.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtStaticData.Location = new System.Drawing.Point(463, 228);
            this.txtStaticData.Margin = new System.Windows.Forms.Padding(4);
            this.txtStaticData.Multiline = true;
            this.txtStaticData.Name = "txtStaticData";
            this.txtStaticData.Size = new System.Drawing.Size(318, 114);
            this.txtStaticData.TabIndex = 52;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.label16.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label16.Location = new System.Drawing.Point(459, 200);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(274, 24);
            this.label16.TabIndex = 51;
            this.label16.Text = "靜態資料（Dictionary）";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.BackColor = System.Drawing.Color.Gainsboro;
            this.label15.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label15.Location = new System.Drawing.Point(462, 95);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(274, 24);
            this.label15.TabIndex = 50;
            this.label15.Text = "固定資料:$變數,#C#函數";
            // 
            // txtStaticLayout
            // 
            this.txtStaticLayout.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtStaticLayout.Location = new System.Drawing.Point(463, 123);
            this.txtStaticLayout.Margin = new System.Windows.Forms.Padding(4);
            this.txtStaticLayout.Multiline = true;
            this.txtStaticLayout.Name = "txtStaticLayout";
            this.txtStaticLayout.Size = new System.Drawing.Size(322, 71);
            this.txtStaticLayout.TabIndex = 49;
            this.txtStaticLayout.Text = "{\'A4\':\'#Date\',\'A3\':\'$EmpName\',\'H3\':\'$RptDate\'}";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(468, 31);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(167, 15);
            this.label10.TabIndex = 48;
            this.label10.Text = "(不是使用畫面上的設定)";
            // 
            // chkboxReportDataFrom
            // 
            this.chkboxReportDataFrom.AutoSize = true;
            this.chkboxReportDataFrom.Location = new System.Drawing.Point(523, 10);
            this.chkboxReportDataFrom.Name = "chkboxReportDataFrom";
            this.chkboxReportDataFrom.Size = new System.Drawing.Size(112, 19);
            this.chkboxReportDataFrom.TabIndex = 47;
            this.chkboxReportDataFrom.Tag = "";
            this.chkboxReportDataFrom.Text = "直接 Property";
            this.chkboxReportDataFrom.UseVisualStyleBackColor = true;
            // 
            // txtReportPrintAttrs
            // 
            this.txtReportPrintAttrs.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtReportPrintAttrs.Location = new System.Drawing.Point(15, 96);
            this.txtReportPrintAttrs.Margin = new System.Windows.Forms.Padding(4);
            this.txtReportPrintAttrs.Multiline = true;
            this.txtReportPrintAttrs.Name = "txtReportPrintAttrs";
            this.txtReportPrintAttrs.Size = new System.Drawing.Size(440, 98);
            this.txtReportPrintAttrs.TabIndex = 46;
            this.txtReportPrintAttrs.Text = resources.GetString("txtReportPrintAttrs.Text");
            // 
            // comboStandardReport
            // 
            this.comboStandardReport.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboStandardReport.FormattingEnabled = true;
            this.comboStandardReport.Location = new System.Drawing.Point(208, 14);
            this.comboStandardReport.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboStandardReport.Name = "comboStandardReport";
            this.comboStandardReport.Size = new System.Drawing.Size(247, 31);
            this.comboStandardReport.TabIndex = 45;
            this.comboStandardReport.SelectedIndexChanged += new System.EventHandler(this.comboStandardReport_SelectedIndexChanged);
            // 
            // txtReportData
            // 
            this.txtReportData.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtReportData.Location = new System.Drawing.Point(16, 228);
            this.txtReportData.Margin = new System.Windows.Forms.Padding(4);
            this.txtReportData.Multiline = true;
            this.txtReportData.Name = "txtReportData";
            this.txtReportData.Size = new System.Drawing.Size(439, 114);
            this.txtReportData.TabIndex = 44;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.label11.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label11.Location = new System.Drawing.Point(12, 200);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(298, 24);
            this.label11.TabIndex = 43;
            this.label11.Text = "Report Data(SQL command)";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.Color.Gainsboro;
            this.label12.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label12.Location = new System.Drawing.Point(12, 61);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(142, 24);
            this.label12.TabIndex = 42;
            this.label12.Text = "列印參數 : ";
            // 
            // textReportData
            // 
            this.textReportData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textReportData.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textReportData.Location = new System.Drawing.Point(15, 385);
            this.textReportData.Margin = new System.Windows.Forms.Padding(4);
            this.textReportData.Multiline = true;
            this.textReportData.Name = "textReportData";
            this.textReportData.ReadOnly = true;
            this.textReportData.Size = new System.Drawing.Size(755, 114);
            this.textReportData.TabIndex = 41;
            this.textReportData.Text = "\r\n";
            // 
            // txtReportTemplateFile
            // 
            this.txtReportTemplateFile.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtReportTemplateFile.Location = new System.Drawing.Point(175, 52);
            this.txtReportTemplateFile.Margin = new System.Windows.Forms.Padding(4);
            this.txtReportTemplateFile.Name = "txtReportTemplateFile";
            this.txtReportTemplateFile.Size = new System.Drawing.Size(598, 36);
            this.txtReportTemplateFile.TabIndex = 36;
            this.txtReportTemplateFile.Text = "C:\\sysTray\\母版\\進料簽收單.xlsx";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.Color.Gainsboro;
            this.label13.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label13.Location = new System.Drawing.Point(11, 357);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(610, 24);
            this.label13.TabIndex = 40;
            this.label13.Text = "UDP Content : UDPLib.udpSend(jsonToSend, IP, Port)";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.Color.Gainsboro;
            this.label14.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label14.Location = new System.Drawing.Point(11, 16);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(190, 24);
            this.label14.TabIndex = 38;
            this.label14.Text = "報表母版檔案 : ";
            // 
            // 個別標籤
            // 
            this.個別標籤.Controls.Add(this.txtFinalCommand);
            this.個別標籤.Controls.Add(this.label4);
            this.個別標籤.Controls.Add(this.txtUDPContent);
            this.個別標籤.Controls.Add(this.btnUDPSend);
            this.個別標籤.Controls.Add(this.label2);
            this.個別標籤.Controls.Add(this.label1);
            this.個別標籤.Controls.Add(this.comboUDPCommand);
            this.個別標籤.Location = new System.Drawing.Point(4, 25);
            this.個別標籤.Name = "個別標籤";
            this.個別標籤.Padding = new System.Windows.Forms.Padding(3);
            this.個別標籤.Size = new System.Drawing.Size(793, 509);
            this.個別標籤.TabIndex = 0;
            this.個別標籤.Text = "個別標籤";
            this.個別標籤.UseVisualStyleBackColor = true;
            // 
            // txtFinalCommand
            // 
            this.txtFinalCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFinalCommand.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtFinalCommand.Location = new System.Drawing.Point(10, 270);
            this.txtFinalCommand.Margin = new System.Windows.Forms.Padding(4);
            this.txtFinalCommand.Multiline = true;
            this.txtFinalCommand.Name = "txtFinalCommand";
            this.txtFinalCommand.ReadOnly = true;
            this.txtFinalCommand.Size = new System.Drawing.Size(748, 231);
            this.txtFinalCommand.TabIndex = 22;
            this.txtFinalCommand.Text = resources.GetString("txtFinalCommand.Text");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label4.Location = new System.Drawing.Point(6, 242);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(178, 24);
            this.label4.TabIndex = 21;
            this.label4.Text = "UDP Content : ";
            // 
            // txtUDPContent
            // 
            this.txtUDPContent.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtUDPContent.Location = new System.Drawing.Point(167, 66);
            this.txtUDPContent.Margin = new System.Windows.Forms.Padding(4);
            this.txtUDPContent.Multiline = true;
            this.txtUDPContent.Name = "txtUDPContent";
            this.txtUDPContent.Size = new System.Drawing.Size(591, 168);
            this.txtUDPContent.TabIndex = 20;
            this.txtUDPContent.Text = resources.GetString("txtUDPContent.Text");
            this.txtUDPContent.TextChanged += new System.EventHandler(this.txtUDPContent_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(6, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(142, 24);
            this.label2.TabIndex = 19;
            this.label2.Text = "發送內容 : ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(6, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 24);
            this.label1.TabIndex = 17;
            this.label1.Text = "UDP Command";
            // 
            // comboUDPCommand
            // 
            this.comboUDPCommand.AutoCompleteCustomSource.AddRange(new string[] {
            "列印 RO QRCode",
            "HtmlToPdf"});
            this.comboUDPCommand.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboUDPCommand.FormattingEnabled = true;
            this.comboUDPCommand.Items.AddRange(new object[] {
            "Print RO QrCode",
            "Save HTML To Pdf",
            "Print RO Report",
            "列印切割日報表",
            "列印底漆日報表",
            "列印外檢日報表",
            "列印加壓日報表",
            "列印色碼日報表",
            "列印全檢日報表",
            "列印貼帶日報表",
            "RestartApp",
            "Reboot",
            "列印進料簽收單",
            "採購申請表",
            "採購請料單",
            "重啟列印",
            "重印進料簽收單",
            "列印塗裝日報表",
            "列印組帽標籤",
            "成品入庫標準標籤",
            "半成品入庫標準標籤",
            "不良品入庫標準標籤"});
            this.comboUDPCommand.Location = new System.Drawing.Point(167, 20);
            this.comboUDPCommand.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboUDPCommand.Name = "comboUDPCommand";
            this.comboUDPCommand.Size = new System.Drawing.Size(318, 31);
            this.comboUDPCommand.TabIndex = 18;
            this.comboUDPCommand.Text = "Print RO QrCode";
            this.comboUDPCommand.SelectedIndexChanged += new System.EventHandler(this.comboUDPCommand_SelectedIndexChanged);
            // 
            // tabJsonPnt
            // 
            this.tabJsonPnt.Controls.Add(this.label19);
            this.tabJsonPnt.Controls.Add(this.textBox1);
            this.tabJsonPnt.Controls.Add(this.label18);
            this.tabJsonPnt.Controls.Add(this.button1);
            this.tabJsonPnt.Controls.Add(this.txtJsonPrint);
            this.tabJsonPnt.Controls.Add(this.label17);
            this.tabJsonPnt.Location = new System.Drawing.Point(4, 25);
            this.tabJsonPnt.Name = "tabJsonPnt";
            this.tabJsonPnt.Size = new System.Drawing.Size(793, 509);
            this.tabJsonPnt.TabIndex = 3;
            this.tabJsonPnt.Text = "Json列印";
            this.tabJsonPnt.UseVisualStyleBackColor = true;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.BackColor = System.Drawing.Color.Gainsboro;
            this.label19.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label19.Location = new System.Drawing.Point(12, 319);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(70, 24);
            this.label19.TabIndex = 34;
            this.label19.Text = "範例:";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBox1.Location = new System.Drawing.Point(16, 347);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(756, 154);
            this.textBox1.TabIndex = 33;
            this.textBox1.Text = resources.GetString("textBox1.Text");
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.BackColor = System.Drawing.Color.Gainsboro;
            this.label18.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label18.Location = new System.Drawing.Point(62, 42);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(442, 24);
            this.label18.TabIndex = 32;
            this.label18.Text = "UDPLib.udpSend(jsonToSend, IP, Port)";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.button1.Location = new System.Drawing.Point(646, 19);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(124, 36);
            this.button1.TabIndex = 31;
            this.button1.Text = "發送 UDP4";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtJsonPrint
            // 
            this.txtJsonPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtJsonPrint.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtJsonPrint.Location = new System.Drawing.Point(16, 70);
            this.txtJsonPrint.Margin = new System.Windows.Forms.Padding(4);
            this.txtJsonPrint.Multiline = true;
            this.txtJsonPrint.Name = "txtJsonPrint";
            this.txtJsonPrint.Size = new System.Drawing.Size(755, 245);
            this.txtJsonPrint.TabIndex = 30;
            this.txtJsonPrint.Text = "\r\n";
            this.txtJsonPrint.TextChanged += new System.EventHandler(this.txtJsonPrint_TextChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.BackColor = System.Drawing.Color.Gainsboro;
            this.label17.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label17.Location = new System.Drawing.Point(12, 13);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(178, 24);
            this.label17.TabIndex = 29;
            this.label17.Text = "UDP Content : ";
            // 
            // newJson
            // 
            this.newJson.Controls.Add(this.comboNewJson);
            this.newJson.Controls.Add(this.txtNewJson);
            this.newJson.Controls.Add(this.btnNewUDP);
            this.newJson.Controls.Add(this.label20);
            this.newJson.Controls.Add(this.label21);
            this.newJson.Location = new System.Drawing.Point(4, 25);
            this.newJson.Name = "newJson";
            this.newJson.Size = new System.Drawing.Size(793, 509);
            this.newJson.TabIndex = 4;
            this.newJson.Text = "新版Json";
            this.newJson.UseVisualStyleBackColor = true;
            // 
            // comboNewJson
            // 
            this.comboNewJson.AutoCompleteCustomSource.AddRange(new string[] {
            "列印 RO QRCode",
            "HtmlToPdf"});
            this.comboNewJson.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboNewJson.FormattingEnabled = true;
            this.comboNewJson.Location = new System.Drawing.Point(156, 19);
            this.comboNewJson.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboNewJson.Name = "comboNewJson";
            this.comboNewJson.Size = new System.Drawing.Size(318, 31);
            this.comboNewJson.TabIndex = 27;
            this.comboNewJson.Text = "NJ_組帽拉力不合格報表";
            this.comboNewJson.SelectedIndexChanged += new System.EventHandler(this.comboNewJson_SelectedIndexChanged);
            // 
            // txtNewJson
            // 
            this.txtNewJson.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtNewJson.Location = new System.Drawing.Point(12, 106);
            this.txtNewJson.Margin = new System.Windows.Forms.Padding(4);
            this.txtNewJson.Multiline = true;
            this.txtNewJson.Name = "txtNewJson";
            this.txtNewJson.Size = new System.Drawing.Size(772, 395);
            this.txtNewJson.TabIndex = 26;
            this.txtNewJson.Text = "{\r\n\t\"udp_type\":\"newJson\",\r\n\t\"udp_func\":\"defectZuMou\",\r\n\t\"propertyName\":\"NJ_組帽拉力不合" +
    "格報表\",\r\n\t\"reportDate\":\"2022/06/08\",\r\n\t\"shiftType\":\"早班\",\r\n\t\"EmpNo\":\"C_100\",\r\n\t\"Emp" +
    "Name\":\"蕭人碩\"\r\n}\r\n";
            // 
            // btnNewUDP
            // 
            this.btnNewUDP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNewUDP.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnNewUDP.Location = new System.Drawing.Point(595, 9);
            this.btnNewUDP.Margin = new System.Windows.Forms.Padding(4);
            this.btnNewUDP.Name = "btnNewUDP";
            this.btnNewUDP.Size = new System.Drawing.Size(189, 49);
            this.btnNewUDP.TabIndex = 23;
            this.btnNewUDP.Text = "發送 UDP3";
            this.btnNewUDP.UseVisualStyleBackColor = true;
            this.btnNewUDP.Click += new System.EventHandler(this.btnNewUDP_Click);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label20.Location = new System.Drawing.Point(8, 68);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(298, 24);
            this.label20.TabIndex = 25;
            this.label20.Text = "發送內容(UDP Content) : ";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label21.Location = new System.Drawing.Point(8, 22);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(142, 24);
            this.label21.TabIndex = 24;
            this.label21.Text = "UDP Command";
            // 
            // textUdpIP
            // 
            this.textUdpIP.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textUdpIP.Location = new System.Drawing.Point(125, 18);
            this.textUdpIP.Margin = new System.Windows.Forms.Padding(4);
            this.textUdpIP.Name = "textUdpIP";
            this.textUdpIP.Size = new System.Drawing.Size(405, 36);
            this.textUdpIP.TabIndex = 18;
            this.textUdpIP.Text = "127.0.0.1";
            // 
            // UDPSend
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(820, 600);
            this.Controls.Add(this.textUdpIP);
            this.Controls.Add(this.comboStandardReportRpt);
            this.Controls.Add(this.label3);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "UDPSend";
            this.Text = "UDPSend";
            this.Load += new System.EventHandler(this.UDPSend_Load);
            this.comboStandardReportRpt.ResumeLayout(false);
            this.標準標籤.ResumeLayout(false);
            this.標準標籤.PerformLayout();
            this.標準報表.ResumeLayout(false);
            this.標準報表.PerformLayout();
            this.個別標籤.ResumeLayout(false);
            this.個別標籤.PerformLayout();
            this.tabJsonPnt.ResumeLayout(false);
            this.tabJsonPnt.PerformLayout();
            this.newJson.ResumeLayout(false);
            this.newJson.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnUDPSend;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtTemplateFile;
        private System.Windows.Forms.TabControl comboStandardReportRpt;
        private System.Windows.Forms.TabPage 個別標籤;
        private System.Windows.Forms.TextBox txtFinalCommand;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtUDPContent;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboUDPCommand;
        private System.Windows.Forms.TabPage 標準標籤;
        private System.Windows.Forms.TextBox textData;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtExcelLayout;
        private System.Windows.Forms.Button btnSendStandard;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtLabelData;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textUdpIP;
        private System.Windows.Forms.ComboBox comboStandardLabel;
        private System.Windows.Forms.TextBox txtPrintAttrs;
        private System.Windows.Forms.CheckBox chkboxDataFrom;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TabPage 標準報表;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox chkboxReportDataFrom;
        private System.Windows.Forms.TextBox txtReportPrintAttrs;
        private System.Windows.Forms.ComboBox comboStandardReport;
        private System.Windows.Forms.TextBox txtReportData;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textReportData;
        private System.Windows.Forms.TextBox txtReportTemplateFile;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtStaticLayout;
        private System.Windows.Forms.TextBox txtStaticData;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TabPage tabJsonPnt;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtJsonPrint;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TabPage newJson;
        private System.Windows.Forms.TextBox txtNewJson;
        private System.Windows.Forms.Button btnNewUDP;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.ComboBox comboNewJson;
    }
}