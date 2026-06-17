
namespace TaskTrayApplication
{
    partial class 問卷調查結果分析
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbQuestionnaireTitle = new System.Windows.Forms.ComboBox();
            this.btmQuery = new System.Windows.Forms.Button();
            this.dgv1 = new System.Windows.Forms.DataGridView();
            this.btnPrintExcel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgv1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "問卷名稱:";
            // 
            // cmbQuestionnaireTitle
            // 
            this.cmbQuestionnaireTitle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbQuestionnaireTitle.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.cmbQuestionnaireTitle.FormattingEnabled = true;
            this.cmbQuestionnaireTitle.Items.AddRange(new object[] {
            "---------請選擇問卷---------",
            "111年度員工旅遊滿意度問卷"});
            this.cmbQuestionnaireTitle.Location = new System.Drawing.Point(108, 14);
            this.cmbQuestionnaireTitle.Name = "cmbQuestionnaireTitle";
            this.cmbQuestionnaireTitle.Size = new System.Drawing.Size(295, 32);
            this.cmbQuestionnaireTitle.TabIndex = 1;
            // 
            // btmQuery
            // 
            this.btmQuery.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btmQuery.Location = new System.Drawing.Point(409, 14);
            this.btmQuery.Name = "btmQuery";
            this.btmQuery.Size = new System.Drawing.Size(75, 32);
            this.btmQuery.TabIndex = 2;
            this.btmQuery.Text = "查詢";
            this.btmQuery.UseVisualStyleBackColor = true;
            this.btmQuery.Click += new System.EventHandler(this.btmQuery_Click);
            // 
            // dgv1
            // 
            this.dgv1.AllowUserToAddRows = false;
            this.dgv1.AllowUserToDeleteRows = false;
            this.dgv1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgv1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv1.Location = new System.Drawing.Point(12, 59);
            this.dgv1.Name = "dgv1";
            dataGridViewCellStyle4.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dgv1.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgv1.RowTemplate.Height = 24;
            this.dgv1.Size = new System.Drawing.Size(933, 621);
            this.dgv1.TabIndex = 3;
            // 
            // btnPrintExcel
            // 
            this.btnPrintExcel.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnPrintExcel.Location = new System.Drawing.Point(490, 14);
            this.btnPrintExcel.Name = "btnPrintExcel";
            this.btnPrintExcel.Size = new System.Drawing.Size(135, 32);
            this.btnPrintExcel.TabIndex = 4;
            this.btnPrintExcel.Text = "匯出EXCEL";
            this.btnPrintExcel.UseVisualStyleBackColor = true;
            this.btnPrintExcel.Click += new System.EventHandler(this.btnPrintExcel_Click);
            // 
            // 問卷調查結果分析
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(957, 692);
            this.Controls.Add(this.btnPrintExcel);
            this.Controls.Add(this.dgv1);
            this.Controls.Add(this.btmQuery);
            this.Controls.Add(this.cmbQuestionnaireTitle);
            this.Controls.Add(this.label1);
            this.Name = "問卷調查結果分析";
            this.Text = "問卷調查結果分析";
            this.Load += new System.EventHandler(this.問卷調查結果分析_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbQuestionnaireTitle;
        private System.Windows.Forms.Button btmQuery;
        private System.Windows.Forms.DataGridView dgv1;
        private System.Windows.Forms.Button btnPrintExcel;
    }
}