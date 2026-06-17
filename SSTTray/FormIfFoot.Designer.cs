namespace TaskTrayApplication
{
    partial class FormIfFoot
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
            this.txtMfono = new System.Windows.Forms.TextBox();
            this.lbldailyReport = new System.Windows.Forms.Label();
            this.btnCauculate = new System.Windows.Forms.Button();
            this.btnSync = new System.Windows.Forms.Button();
            this.dgv1 = new System.Windows.Forms.DataGridView();
            this.radioAll = new System.Windows.Forms.RadioButton();
            this.radioPart = new System.Windows.Forms.RadioButton();
            this.btnSyncRtype = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgv1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtMfono
            // 
            this.txtMfono.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtMfono.Location = new System.Drawing.Point(159, 13);
            this.txtMfono.Margin = new System.Windows.Forms.Padding(4);
            this.txtMfono.Name = "txtMfono";
            this.txtMfono.Size = new System.Drawing.Size(327, 36);
            this.txtMfono.TabIndex = 96;
            // 
            // lbldailyReport
            // 
            this.lbldailyReport.AutoSize = true;
            this.lbldailyReport.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbldailyReport.Location = new System.Drawing.Point(13, 19);
            this.lbldailyReport.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbldailyReport.Name = "lbldailyReport";
            this.lbldailyReport.Size = new System.Drawing.Size(106, 24);
            this.lbldailyReport.TabIndex = 95;
            this.lbldailyReport.Text = "工令單號";
            // 
            // btnCauculate
            // 
            this.btnCauculate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCauculate.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnCauculate.Location = new System.Drawing.Point(6, 353);
            this.btnCauculate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCauculate.Name = "btnCauculate";
            this.btnCauculate.Size = new System.Drawing.Size(192, 59);
            this.btnCauculate.TabIndex = 113;
            this.btnCauculate.Text = "計算 有腳/無腳";
            this.btnCauculate.UseVisualStyleBackColor = true;
            this.btnCauculate.Click += new System.EventHandler(this.btnCauculate_Click);
            // 
            // btnSync
            // 
            this.btnSync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSync.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSync.Location = new System.Drawing.Point(207, 353);
            this.btnSync.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(238, 59);
            this.btnSync.TabIndex = 112;
            this.btnSync.Text = "有腳/無腳 同步";
            this.btnSync.UseVisualStyleBackColor = true;
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // dgv1
            // 
            this.dgv1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv1.Location = new System.Drawing.Point(17, 67);
            this.dgv1.Name = "dgv1";
            this.dgv1.RowHeadersWidth = 51;
            this.dgv1.RowTemplate.Height = 27;
            this.dgv1.Size = new System.Drawing.Size(622, 236);
            this.dgv1.TabIndex = 114;
            // 
            // radioAll
            // 
            this.radioAll.AutoSize = true;
            this.radioAll.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioAll.Location = new System.Drawing.Point(318, 319);
            this.radioAll.Margin = new System.Windows.Forms.Padding(4);
            this.radioAll.Name = "radioAll";
            this.radioAll.Size = new System.Drawing.Size(127, 28);
            this.radioAll.TabIndex = 116;
            this.radioAll.Text = "全部重算";
            this.radioAll.UseVisualStyleBackColor = true;
            // 
            // radioPart
            // 
            this.radioPart.AutoSize = true;
            this.radioPart.Checked = true;
            this.radioPart.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioPart.Location = new System.Drawing.Point(207, 319);
            this.radioPart.Margin = new System.Windows.Forms.Padding(4);
            this.radioPart.Name = "radioPart";
            this.radioPart.Size = new System.Drawing.Size(103, 28);
            this.radioPart.TabIndex = 115;
            this.radioPart.TabStop = true;
            this.radioPart.Text = "只算-1";
            this.radioPart.UseVisualStyleBackColor = true;
            // 
            // btnSyncRtype
            // 
            this.btnSyncRtype.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSyncRtype.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSyncRtype.Location = new System.Drawing.Point(454, 353);
            this.btnSyncRtype.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSyncRtype.Name = "btnSyncRtype";
            this.btnSyncRtype.Size = new System.Drawing.Size(207, 59);
            this.btnSyncRtype.TabIndex = 117;
            this.btnSyncRtype.Text = "同步內外銷資料";
            this.btnSyncRtype.UseVisualStyleBackColor = true;
            this.btnSyncRtype.Click += new System.EventHandler(this.btnSyncRtype_Click);
            // 
            // FormIfFoot
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(665, 423);
            this.Controls.Add(this.btnSyncRtype);
            this.Controls.Add(this.radioAll);
            this.Controls.Add(this.radioPart);
            this.Controls.Add(this.dgv1);
            this.Controls.Add(this.btnCauculate);
            this.Controls.Add(this.btnSync);
            this.Controls.Add(this.txtMfono);
            this.Controls.Add(this.lbldailyReport);
            this.Name = "FormIfFoot";
            this.Text = "FormIfFoot";
            this.Load += new System.EventHandler(this.FormIfFoot_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtMfono;
        private System.Windows.Forms.Label lbldailyReport;
        private System.Windows.Forms.Button btnCauculate;
        private System.Windows.Forms.Button btnSync;
        private System.Windows.Forms.DataGridView dgv1;
        private System.Windows.Forms.RadioButton radioAll;
        private System.Windows.Forms.RadioButton radioPart;
        private System.Windows.Forms.Button btnSyncRtype;
    }
}