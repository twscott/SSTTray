namespace TaskTrayApplication
{
    partial class HtmlToPdf
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
            this.btnConvert = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.txtSourceFile = new System.Windows.Forms.TextBox();
            this.radioPortaratr = new System.Windows.Forms.RadioButton();
            this.radioAllFiles = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioSingleFile = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.comboUDPSubName = new System.Windows.Forms.ComboBox();
            this.radioHorizontalPrint = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnConvert
            // 
            this.btnConvert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConvert.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnConvert.Location = new System.Drawing.Point(593, 43);
            this.btnConvert.Margin = new System.Windows.Forms.Padding(4);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(189, 119);
            this.btnConvert.TabIndex = 12;
            this.btnConvert.Text = "開始轉換";
            this.btnConvert.UseVisualStyleBackColor = true;
            this.btnConvert.Click += new System.EventHandler(this.btnConvert_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(9, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(166, 24);
            this.label1.TabIndex = 10;
            this.label1.Text = "Html 檔案路徑";
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Font = new System.Drawing.Font("標楷體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnOpenFile.Location = new System.Drawing.Point(433, 90);
            this.btnOpenFile.Margin = new System.Windows.Forms.Padding(4);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(152, 42);
            this.btnOpenFile.TabIndex = 17;
            this.btnOpenFile.Text = "選取檔案";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // txtSourceFile
            // 
            this.txtSourceFile.Font = new System.Drawing.Font("標楷體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtSourceFile.Location = new System.Drawing.Point(13, 43);
            this.txtSourceFile.Margin = new System.Windows.Forms.Padding(4);
            this.txtSourceFile.Name = "txtSourceFile";
            this.txtSourceFile.Size = new System.Drawing.Size(572, 39);
            this.txtSourceFile.TabIndex = 16;
            this.txtSourceFile.Text = "D:\\temp";
            // 
            // radioPortaratr
            // 
            this.radioPortaratr.AutoSize = true;
            this.radioPortaratr.Checked = true;
            this.radioPortaratr.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioPortaratr.Location = new System.Drawing.Point(395, 8);
            this.radioPortaratr.Name = "radioPortaratr";
            this.radioPortaratr.Size = new System.Drawing.Size(79, 28);
            this.radioPortaratr.TabIndex = 18;
            this.radioPortaratr.TabStop = true;
            this.radioPortaratr.Text = "直印";
            this.radioPortaratr.UseVisualStyleBackColor = true;
            // 
            // radioAllFiles
            // 
            this.radioAllFiles.AutoSize = true;
            this.radioAllFiles.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioAllFiles.Location = new System.Drawing.Point(6, 48);
            this.radioAllFiles.Name = "radioAllFiles";
            this.radioAllFiles.Size = new System.Drawing.Size(343, 28);
            this.radioAllFiles.TabIndex = 19;
            this.radioAllFiles.Text = "本目錄下的所有檔案全部轉換";
            this.radioAllFiles.UseVisualStyleBackColor = true;
            this.radioAllFiles.CheckedChanged += new System.EventHandler(this.radioAllFiles_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioSingleFile);
            this.groupBox1.Controls.Add(this.radioAllFiles);
            this.groupBox1.Location = new System.Drawing.Point(13, 90);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(414, 86);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "轉檔範圍";
            // 
            // radioSingleFile
            // 
            this.radioSingleFile.AutoSize = true;
            this.radioSingleFile.Checked = true;
            this.radioSingleFile.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioSingleFile.Location = new System.Drawing.Point(6, 15);
            this.radioSingleFile.Name = "radioSingleFile";
            this.radioSingleFile.Size = new System.Drawing.Size(175, 28);
            this.radioSingleFile.TabIndex = 19;
            this.radioSingleFile.TabStop = true;
            this.radioSingleFile.Text = "僅選取的檔案";
            this.radioSingleFile.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(15, 199);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(226, 24);
            this.label2.TabIndex = 22;
            this.label2.Text = "指定來源檔案副檔名";
            this.label2.Visible = false;
            // 
            // comboUDPSubName
            // 
            this.comboUDPSubName.AutoCompleteCustomSource.AddRange(new string[] {
            "列印 RO QRCode",
            "HtmlToPdf"});
            this.comboUDPSubName.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboUDPSubName.FormattingEnabled = true;
            this.comboUDPSubName.Items.AddRange(new object[] {
            "*.*",
            "html",
            "htm",
            "xlsx",
            "xls"});
            this.comboUDPSubName.Location = new System.Drawing.Point(257, 196);
            this.comboUDPSubName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboUDPSubName.Name = "comboUDPSubName";
            this.comboUDPSubName.Size = new System.Drawing.Size(252, 31);
            this.comboUDPSubName.TabIndex = 75;
            this.comboUDPSubName.Text = "html";
            this.comboUDPSubName.Visible = false;
            // 
            // radioHorizontalPrint
            // 
            this.radioHorizontalPrint.AutoSize = true;
            this.radioHorizontalPrint.Checked = true;
            this.radioHorizontalPrint.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioHorizontalPrint.Location = new System.Drawing.Point(489, 8);
            this.radioHorizontalPrint.Name = "radioHorizontalPrint";
            this.radioHorizontalPrint.Size = new System.Drawing.Size(79, 28);
            this.radioHorizontalPrint.TabIndex = 76;
            this.radioHorizontalPrint.TabStop = true;
            this.radioHorizontalPrint.Text = "橫印";
            this.radioHorizontalPrint.UseVisualStyleBackColor = true;
            // 
            // HtmlToPdf
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(795, 245);
            this.Controls.Add(this.radioHorizontalPrint);
            this.Controls.Add(this.comboUDPSubName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.radioPortaratr);
            this.Controls.Add(this.btnOpenFile);
            this.Controls.Add(this.txtSourceFile);
            this.Controls.Add(this.btnConvert);
            this.Controls.Add(this.label1);
            this.Name = "HtmlToPdf";
            this.Text = "HtmlToPdf";
            this.Load += new System.EventHandler(this.HtmlToPdf_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnConvert;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.TextBox txtSourceFile;
        private System.Windows.Forms.RadioButton radioPortaratr;
        private System.Windows.Forms.RadioButton radioAllFiles;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioSingleFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboUDPSubName;
        private System.Windows.Forms.RadioButton radioHorizontalPrint;
    }
}