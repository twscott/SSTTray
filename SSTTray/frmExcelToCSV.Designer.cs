
namespace TaskTrayApplication
{
    partial class frmExcelToCSV
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
            this.label9 = new System.Windows.Forms.Label();
            this.txtWorkSheet = new System.Windows.Forms.TextBox();
            this.btnUdpSend = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.comboDelimeter = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtUdpToSend = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.btnExcelConvert = new System.Windows.Forms.Button();
            this.txtJson = new System.Windows.Forms.TextBox();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.textScriptPath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCSV = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label9.Location = new System.Drawing.Point(635, 16);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(202, 24);
            this.label9.TabIndex = 68;
            this.label9.Text = "Excel 工作表名稱";
            // 
            // txtWorkSheet
            // 
            this.txtWorkSheet.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtWorkSheet.Location = new System.Drawing.Point(857, 13);
            this.txtWorkSheet.Margin = new System.Windows.Forms.Padding(4);
            this.txtWorkSheet.Name = "txtWorkSheet";
            this.txtWorkSheet.Size = new System.Drawing.Size(264, 36);
            this.txtWorkSheet.TabIndex = 67;
            this.txtWorkSheet.Text = "test";
            // 
            // btnUdpSend
            // 
            this.btnUdpSend.Font = new System.Drawing.Font("標楷體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnUdpSend.Location = new System.Drawing.Point(947, 109);
            this.btnUdpSend.Margin = new System.Windows.Forms.Padding(4);
            this.btnUdpSend.Name = "btnUdpSend";
            this.btnUdpSend.Size = new System.Drawing.Size(174, 52);
            this.btnUdpSend.TabIndex = 66;
            this.btnUdpSend.Text = "UdpSend";
            this.btnUdpSend.UseVisualStyleBackColor = true;
            this.btnUdpSend.Click += new System.EventHandler(this.btnUdpSend_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label6.Location = new System.Drawing.Point(12, 271);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(226, 24);
            this.label6.TabIndex = 61;
            this.label6.Text = "讀取結果(CSV 格式)";
            // 
            // comboDelimeter
            // 
            this.comboDelimeter.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboDelimeter.FormattingEnabled = true;
            this.comboDelimeter.Items.AddRange(new object[] {
            ",",
            ";",
            "|",
            "~",
            "!",
            "@",
            "#",
            "$",
            "%",
            "^",
            "&",
            "*"});
            this.comboDelimeter.Location = new System.Drawing.Point(213, 14);
            this.comboDelimeter.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboDelimeter.Name = "comboDelimeter";
            this.comboDelimeter.Size = new System.Drawing.Size(67, 31);
            this.comboDelimeter.TabIndex = 60;
            this.comboDelimeter.Text = ",";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label5.Location = new System.Drawing.Point(12, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(106, 24);
            this.label5.TabIndex = 59;
            this.label5.Text = "分隔符號";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(12, 546);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 24);
            this.label3.TabIndex = 58;
            this.label3.Text = "UDP 指令";
            // 
            // txtUdpToSend
            // 
            this.txtUdpToSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUdpToSend.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtUdpToSend.Location = new System.Drawing.Point(13, 574);
            this.txtUdpToSend.Margin = new System.Windows.Forms.Padding(4);
            this.txtUdpToSend.Multiline = true;
            this.txtUdpToSend.Name = "txtUdpToSend";
            this.txtUdpToSend.Size = new System.Drawing.Size(1121, 117);
            this.txtUdpToSend.TabIndex = 57;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(339, 131);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(154, 24);
            this.label2.TabIndex = 56;
            this.label2.Text = "（從零起算）";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.numericUpDown1.Location = new System.Drawing.Point(213, 123);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(120, 35);
            this.numericUpDown1.TabIndex = 55;
            this.numericUpDown1.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(60, 124);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 24);
            this.label1.TabIndex = 54;
            this.label1.Text = "資料起始行";
            // 
            // btnExcelConvert
            // 
            this.btnExcelConvert.Font = new System.Drawing.Font("標楷體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnExcelConvert.Location = new System.Drawing.Point(687, 106);
            this.btnExcelConvert.Margin = new System.Windows.Forms.Padding(4);
            this.btnExcelConvert.Name = "btnExcelConvert";
            this.btnExcelConvert.Size = new System.Drawing.Size(252, 52);
            this.btnExcelConvert.TabIndex = 53;
            this.btnExcelConvert.Text = "excel轉CSV";
            this.btnExcelConvert.UseVisualStyleBackColor = true;
            this.btnExcelConvert.Click += new System.EventHandler(this.btnExcelConvert_Click);
            // 
            // txtJson
            // 
            this.txtJson.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtJson.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtJson.Location = new System.Drawing.Point(13, 299);
            this.txtJson.Margin = new System.Windows.Forms.Padding(4);
            this.txtJson.Multiline = true;
            this.txtJson.Name = "txtJson";
            this.txtJson.Size = new System.Drawing.Size(1121, 243);
            this.txtJson.TabIndex = 52;
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Font = new System.Drawing.Font("標楷體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnOpenFile.Location = new System.Drawing.Point(947, 59);
            this.btnOpenFile.Margin = new System.Windows.Forms.Padding(4);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(178, 42);
            this.btnOpenFile.TabIndex = 51;
            this.btnOpenFile.Text = "選取檔案";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // textScriptPath
            // 
            this.textScriptPath.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textScriptPath.Location = new System.Drawing.Point(213, 65);
            this.textScriptPath.Margin = new System.Windows.Forms.Padding(4);
            this.textScriptPath.Name = "textScriptPath";
            this.textScriptPath.Size = new System.Drawing.Size(726, 36);
            this.textScriptPath.TabIndex = 50;
            this.textScriptPath.Text = "D:\\temp\\e-learning.xlsx";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label4.Location = new System.Drawing.Point(10, 69);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(190, 24);
            this.label4.TabIndex = 49;
            this.label4.Text = "預設資料路徑 : ";
            // 
            // txtCSV
            // 
            this.txtCSV.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtCSV.Location = new System.Drawing.Point(212, 227);
            this.txtCSV.Margin = new System.Windows.Forms.Padding(4);
            this.txtCSV.Name = "txtCSV";
            this.txtCSV.Size = new System.Drawing.Size(726, 36);
            this.txtCSV.TabIndex = 70;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label7.Location = new System.Drawing.Point(9, 231);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(190, 24);
            this.label7.TabIndex = 69;
            this.label7.Text = "輸出的 CSV 檔案";
            // 
            // frmExcelToCSV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1134, 705);
            this.Controls.Add(this.txtCSV);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtWorkSheet);
            this.Controls.Add(this.btnUdpSend);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.comboDelimeter);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtUdpToSend);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnExcelConvert);
            this.Controls.Add(this.txtJson);
            this.Controls.Add(this.btnOpenFile);
            this.Controls.Add(this.textScriptPath);
            this.Controls.Add(this.label4);
            this.Name = "frmExcelToCSV";
            this.Text = "frmExcelToCSV";
            this.Load += new System.EventHandler(this.frmExcelToCSV_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtWorkSheet;
        private System.Windows.Forms.Button btnUdpSend;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboDelimeter;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUdpToSend;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnExcelConvert;
        private System.Windows.Forms.TextBox txtJson;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.TextBox textScriptPath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtCSV;
        private System.Windows.Forms.Label label7;
    }
}