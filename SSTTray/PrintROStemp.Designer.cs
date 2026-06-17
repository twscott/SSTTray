namespace TaskTrayApplication
{
    partial class PrintROStemp
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
            this.dtp1 = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.radioRo = new System.Windows.Forms.RadioButton();
            this.radioRoFeed = new System.Windows.Forms.RadioButton();
            this.chkboxReprint = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // dtp1
            // 
            this.dtp1.CalendarFont = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dtp1.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dtp1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtp1.Location = new System.Drawing.Point(171, 96);
            this.dtp1.Margin = new System.Windows.Forms.Padding(4);
            this.dtp1.Name = "dtp1";
            this.dtp1.Size = new System.Drawing.Size(192, 39);
            this.dtp1.TabIndex = 48;
            this.dtp1.Value = new System.DateTime(2018, 6, 19, 0, 0, 0, 0);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(6, 107);
            this.label1.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(154, 24);
            this.label1.TabIndex = 49;
            this.label1.Text = "RO 入庫日期*";
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Font = new System.Drawing.Font("標楷體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnOpenFile.Location = new System.Drawing.Point(93, 155);
            this.btnOpenFile.Margin = new System.Windows.Forms.Padding(4);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(223, 42);
            this.btnOpenFile.TabIndex = 50;
            this.btnOpenFile.Text = "產生RO日報表";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // radioRo
            // 
            this.radioRo.AutoSize = true;
            this.radioRo.Checked = true;
            this.radioRo.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioRo.Location = new System.Drawing.Point(70, 12);
            this.radioRo.Name = "radioRo";
            this.radioRo.Size = new System.Drawing.Size(271, 28);
            this.radioRo.TabIndex = 51;
            this.radioRo.TabStop = true;
            this.radioRo.Text = "原物料進料檢驗日報表";
            this.radioRo.UseVisualStyleBackColor = true;
            // 
            // radioRoFeed
            // 
            this.radioRoFeed.AutoSize = true;
            this.radioRoFeed.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioRoFeed.Location = new System.Drawing.Point(70, 46);
            this.radioRoFeed.Name = "radioRoFeed";
            this.radioRoFeed.Size = new System.Drawing.Size(151, 28);
            this.radioRoFeed.TabIndex = 52;
            this.radioRoFeed.Text = "進料簽收單";
            this.radioRoFeed.UseVisualStyleBackColor = true;
            // 
            // chkboxReprint
            // 
            this.chkboxReprint.AutoSize = true;
            this.chkboxReprint.Location = new System.Drawing.Point(236, 51);
            this.chkboxReprint.Name = "chkboxReprint";
            this.chkboxReprint.Size = new System.Drawing.Size(59, 19);
            this.chkboxReprint.TabIndex = 53;
            this.chkboxReprint.Text = "重印";
            this.chkboxReprint.UseVisualStyleBackColor = true;
            this.chkboxReprint.CheckedChanged += new System.EventHandler(this.chkboxReprint_CheckedChanged);
            // 
            // PrintROStemp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 210);
            this.Controls.Add(this.chkboxReprint);
            this.Controls.Add(this.radioRoFeed);
            this.Controls.Add(this.radioRo);
            this.Controls.Add(this.btnOpenFile);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dtp1);
            this.Name = "PrintROStemp";
            this.Text = "原物料進料檢驗日報表";
            this.Load += new System.EventHandler(this.PrintROStemp_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtp1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.RadioButton radioRo;
        private System.Windows.Forms.RadioButton radioRoFeed;
        private System.Windows.Forms.CheckBox chkboxReprint;
    }
}