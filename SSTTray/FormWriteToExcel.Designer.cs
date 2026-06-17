namespace TaskTrayApplication
{
    partial class FormWriteToExcel
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
            this.label5 = new System.Windows.Forms.Label();
            this.comboTargetData = new System.Windows.Forms.ComboBox();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label5.Location = new System.Drawing.Point(37, 45);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(106, 24);
            this.label5.TabIndex = 17;
            this.label5.Text = "目標資料";
            // 
            // comboTargetData
            // 
            this.comboTargetData.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboTargetData.FormattingEnabled = true;
            this.comboTargetData.Items.AddRange(new object[] {
            "請假資料"});
            this.comboTargetData.Location = new System.Drawing.Point(149, 42);
            this.comboTargetData.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboTargetData.Name = "comboTargetData";
            this.comboTargetData.Size = new System.Drawing.Size(392, 31);
            this.comboTargetData.TabIndex = 18;
            this.comboTargetData.Text = "請假資料";
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Font = new System.Drawing.Font("標楷體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnOpenFile.Location = new System.Drawing.Point(352, 107);
            this.btnOpenFile.Margin = new System.Windows.Forms.Padding(4);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(189, 42);
            this.btnOpenFile.TabIndex = 19;
            this.btnOpenFile.Text = "輸出到 Excel";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // FormWriteToExcel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 177);
            this.Controls.Add(this.btnOpenFile);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.comboTargetData);
            this.Name = "FormWriteToExcel";
            this.Text = "FormWriteToExcel";
            this.Load += new System.EventHandler(this.FormWriteToExcel_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboTargetData;
        private System.Windows.Forms.Button btnOpenFile;
    }
}