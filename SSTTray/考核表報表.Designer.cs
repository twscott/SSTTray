namespace TaskTrayApplication
{
    partial class 考核表報表
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
            this.txtDataYear = new System.Windows.Forms.TextBox();
            this.lblDataDate = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioDown = new System.Windows.Forms.RadioButton();
            this.radioUpHalf = new System.Windows.Forms.RadioButton();
            this.dgv1 = new System.Windows.Forms.DataGridView();
            this.btnPrintRpt = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtDataYear
            // 
            this.txtDataYear.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtDataYear.Location = new System.Drawing.Point(135, 5);
            this.txtDataYear.Margin = new System.Windows.Forms.Padding(4);
            this.txtDataYear.Name = "txtDataYear";
            this.txtDataYear.Size = new System.Drawing.Size(267, 36);
            this.txtDataYear.TabIndex = 18;
            // 
            // lblDataDate
            // 
            this.lblDataDate.AutoSize = true;
            this.lblDataDate.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblDataDate.Location = new System.Drawing.Point(12, 9);
            this.lblDataDate.Name = "lblDataDate";
            this.lblDataDate.Size = new System.Drawing.Size(106, 24);
            this.lblDataDate.TabIndex = 17;
            this.lblDataDate.Text = "資料年度";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioDown);
            this.groupBox1.Controls.Add(this.radioUpHalf);
            this.groupBox1.Location = new System.Drawing.Point(427, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(293, 52);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            // 
            // radioDown
            // 
            this.radioDown.AutoSize = true;
            this.radioDown.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioDown.Location = new System.Drawing.Point(151, 17);
            this.radioDown.Margin = new System.Windows.Forms.Padding(4);
            this.radioDown.Name = "radioDown";
            this.radioDown.Size = new System.Drawing.Size(103, 28);
            this.radioDown.TabIndex = 14;
            this.radioDown.Text = "下半年";
            this.radioDown.UseVisualStyleBackColor = true;
            this.radioDown.CheckedChanged += new System.EventHandler(this.radioDown_CheckedChanged);
            // 
            // radioUpHalf
            // 
            this.radioUpHalf.AutoSize = true;
            this.radioUpHalf.Checked = true;
            this.radioUpHalf.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioUpHalf.Location = new System.Drawing.Point(7, 17);
            this.radioUpHalf.Margin = new System.Windows.Forms.Padding(4);
            this.radioUpHalf.Name = "radioUpHalf";
            this.radioUpHalf.Size = new System.Drawing.Size(103, 28);
            this.radioUpHalf.TabIndex = 12;
            this.radioUpHalf.TabStop = true;
            this.radioUpHalf.Text = "上半年";
            this.radioUpHalf.UseVisualStyleBackColor = true;
            this.radioUpHalf.CheckedChanged += new System.EventHandler(this.radioUpHalf_CheckedChanged);
            // 
            // dgv1
            // 
            this.dgv1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv1.Location = new System.Drawing.Point(2, 103);
            this.dgv1.Name = "dgv1";
            this.dgv1.RowHeadersWidth = 51;
            this.dgv1.RowTemplate.Height = 27;
            this.dgv1.Size = new System.Drawing.Size(1258, 335);
            this.dgv1.TabIndex = 21;
            // 
            // btnPrintRpt
            // 
            this.btnPrintRpt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrintRpt.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnPrintRpt.Location = new System.Drawing.Point(1104, 16);
            this.btnPrintRpt.Margin = new System.Windows.Forms.Padding(4);
            this.btnPrintRpt.Name = "btnPrintRpt";
            this.btnPrintRpt.Size = new System.Drawing.Size(148, 54);
            this.btnPrintRpt.TabIndex = 22;
            this.btnPrintRpt.Text = "列印報表";
            this.btnPrintRpt.UseVisualStyleBackColor = true;
            this.btnPrintRpt.Click += new System.EventHandler(this.btnPrintRpt_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.button1.Location = new System.Drawing.Point(980, 17);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(116, 54);
            this.button1.TabIndex = 23;
            this.button1.Text = "查詢";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.checkBox1.Location = new System.Drawing.Point(771, 22);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(164, 28);
            this.checkBox1.TabIndex = 24;
            this.checkBox1.Text = "全選/全反選";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // txtOutput
            // 
            this.txtOutput.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtOutput.Location = new System.Drawing.Point(135, 60);
            this.txtOutput.Margin = new System.Windows.Forms.Padding(4);
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.Size = new System.Drawing.Size(585, 36);
            this.txtOutput.TabIndex = 26;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(12, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 24);
            this.label1.TabIndex = 25;
            this.label1.Text = "輸出位置";
            // 
            // 考核表報表
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1265, 450);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnPrintRpt);
            this.Controls.Add(this.dgv1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtDataYear);
            this.Controls.Add(this.lblDataDate);
            this.Name = "考核表報表";
            this.Text = "考核表報表";
            this.Load += new System.EventHandler(this.考核表報表_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtDataYear;
        private System.Windows.Forms.Label lblDataDate;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioDown;
        private System.Windows.Forms.RadioButton radioUpHalf;
        private System.Windows.Forms.DataGridView dgv1;
        private System.Windows.Forms.Button btnPrintRpt;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Label label1;
    }
}