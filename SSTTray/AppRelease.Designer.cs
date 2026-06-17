
namespace TaskTrayApplication
{
    partial class AppRelease
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
            this.label1 = new System.Windows.Forms.Label();
            this.comboAppType = new System.Windows.Forms.ComboBox();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.textAppFilePath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtReleaseNote = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.dgv1 = new System.Windows.Forms.DataGridView();
            this.txtVersionNo = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgv1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(28, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 24);
            this.label1.TabIndex = 91;
            this.label1.Text = "App Type";
            // 
            // comboAppType
            // 
            this.comboAppType.AutoCompleteCustomSource.AddRange(new string[] {
            "列印 RO QRCode",
            "HtmlToPdf"});
            this.comboAppType.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboAppType.FormattingEnabled = true;
            this.comboAppType.Items.AddRange(new object[] {
            "一般製程 APP",
            "舊版製程 APP",
            "組帽 APP",
            "RO章 APP",
            "盤點 APP",
            "色碼查詢 APP "});
            this.comboAppType.Location = new System.Drawing.Point(171, 11);
            this.comboAppType.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboAppType.Name = "comboAppType";
            this.comboAppType.Size = new System.Drawing.Size(262, 31);
            this.comboAppType.TabIndex = 92;
            this.comboAppType.Text = "一般製程 APP";
            this.comboAppType.SelectedIndexChanged += new System.EventHandler(this.comboAppType_SelectedIndexChanged);
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Font = new System.Drawing.Font("標楷體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnOpenFile.Location = new System.Drawing.Point(703, 192);
            this.btnOpenFile.Margin = new System.Windows.Forms.Padding(4);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(163, 42);
            this.btnOpenFile.TabIndex = 95;
            this.btnOpenFile.Text = "選取檔案";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // textAppFilePath
            // 
            this.textAppFilePath.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textAppFilePath.Location = new System.Drawing.Point(217, 241);
            this.textAppFilePath.Margin = new System.Windows.Forms.Padding(4);
            this.textAppFilePath.Name = "textAppFilePath";
            this.textAppFilePath.Size = new System.Drawing.Size(649, 36);
            this.textAppFilePath.TabIndex = 94;
            this.textAppFilePath.Text = "D:\\DBbackup";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label4.Location = new System.Drawing.Point(26, 245);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(154, 24);
            this.label4.TabIndex = 93;
            this.label4.Text = "要發布的檔案";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(64, 290);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 24);
            this.label2.TabIndex = 96;
            this.label2.Text = "修改內容";
            // 
            // txtReleaseNote
            // 
            this.txtReleaseNote.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtReleaseNote.Location = new System.Drawing.Point(217, 290);
            this.txtReleaseNote.Margin = new System.Windows.Forms.Padding(4);
            this.txtReleaseNote.Multiline = true;
            this.txtReleaseNote.Name = "txtReleaseNote";
            this.txtReleaseNote.Size = new System.Drawing.Size(649, 190);
            this.txtReleaseNote.TabIndex = 97;
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("標楷體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnCancel.Location = new System.Drawing.Point(703, 492);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(163, 42);
            this.btnCancel.TabIndex = 98;
            this.btnCancel.Text = "取消發布";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // dgv1
            // 
            this.dgv1.AllowUserToAddRows = false;
            this.dgv1.AllowUserToDeleteRows = false;
            this.dgv1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv1.Location = new System.Drawing.Point(30, 56);
            this.dgv1.Name = "dgv1";
            this.dgv1.RowHeadersWidth = 51;
            this.dgv1.RowTemplate.Height = 27;
            this.dgv1.Size = new System.Drawing.Size(836, 119);
            this.dgv1.TabIndex = 99;
            // 
            // txtVersionNo
            // 
            this.txtVersionNo.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtVersionNo.Location = new System.Drawing.Point(217, 192);
            this.txtVersionNo.Margin = new System.Windows.Forms.Padding(4);
            this.txtVersionNo.Name = "txtVersionNo";
            this.txtVersionNo.Size = new System.Drawing.Size(467, 36);
            this.txtVersionNo.TabIndex = 101;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(74, 192);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 24);
            this.label3.TabIndex = 100;
            this.label3.Text = "版本號碼";
            // 
            // btnOK
            // 
            this.btnOK.Font = new System.Drawing.Font("標楷體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnOK.Location = new System.Drawing.Point(532, 492);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(163, 42);
            this.btnOK.TabIndex = 102;
            this.btnOK.Text = "資訊部發布";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // AppRelease
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(877, 541);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtVersionNo);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dgv1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtReleaseNote);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnOpenFile);
            this.Controls.Add(this.textAppFilePath);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboAppType);
            this.Name = "AppRelease";
            this.Text = "AppRelease";
            this.Load += new System.EventHandler(this.AppRelease_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboAppType;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.TextBox textAppFilePath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtReleaseNote;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridView dgv1;
        private System.Windows.Forms.TextBox txtVersionNo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnOK;
    }
}