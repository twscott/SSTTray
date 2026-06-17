namespace TaskTrayApplication
{
    partial class Form電鍍報表cs
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
            this.dgv1 = new System.Windows.Forms.DataGridView();
            this.btnLblQuery = new System.Windows.Forms.Button();
            this.btnLblPrint = new System.Windows.Forms.Button();
            this.btnRptQuery = new System.Windows.Forms.Button();
            this.btnExcel = new System.Windows.Forms.Button();
            this.lblDataDate = new System.Windows.Forms.Label();
            this.btnRptPrint = new System.Windows.Forms.Button();
            this.txtFinalCommand = new System.Windows.Forms.TextBox();
            this.dtp1 = new System.Windows.Forms.DateTimePicker();
            this.comboSupplier = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtUdpServer = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgv1)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv1
            // 
            this.dgv1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv1.Location = new System.Drawing.Point(5, 118);
            this.dgv1.Name = "dgv1";
            this.dgv1.RowTemplate.Height = 27;
            this.dgv1.Size = new System.Drawing.Size(793, 233);
            this.dgv1.TabIndex = 0;
            // 
            // btnLblQuery
            // 
            this.btnLblQuery.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnLblQuery.Location = new System.Drawing.Point(5, 60);
            this.btnLblQuery.Margin = new System.Windows.Forms.Padding(4);
            this.btnLblQuery.Name = "btnLblQuery";
            this.btnLblQuery.Size = new System.Drawing.Size(189, 51);
            this.btnLblQuery.TabIndex = 9;
            this.btnLblQuery.Text = "標籤查詢";
            this.btnLblQuery.UseVisualStyleBackColor = true;
            this.btnLblQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // btnLblPrint
            // 
            this.btnLblPrint.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnLblPrint.Location = new System.Drawing.Point(199, 59);
            this.btnLblPrint.Margin = new System.Windows.Forms.Padding(4);
            this.btnLblPrint.Name = "btnLblPrint";
            this.btnLblPrint.Size = new System.Drawing.Size(189, 51);
            this.btnLblPrint.TabIndex = 10;
            this.btnLblPrint.Text = "列印標籤";
            this.btnLblPrint.UseVisualStyleBackColor = true;
            this.btnLblPrint.Click += new System.EventHandler(this.btnPrintLabel_Click);
            // 
            // btnRptQuery
            // 
            this.btnRptQuery.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnRptQuery.Location = new System.Drawing.Point(409, 59);
            this.btnRptQuery.Margin = new System.Windows.Forms.Padding(4);
            this.btnRptQuery.Name = "btnRptQuery";
            this.btnRptQuery.Size = new System.Drawing.Size(189, 51);
            this.btnRptQuery.TabIndex = 11;
            this.btnRptQuery.Text = "列印驗收報表";
            this.btnRptQuery.UseVisualStyleBackColor = true;
            // 
            // btnExcel
            // 
            this.btnExcel.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnExcel.Location = new System.Drawing.Point(603, 0);
            this.btnExcel.Margin = new System.Windows.Forms.Padding(4);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(180, 51);
            this.btnExcel.TabIndex = 12;
            this.btnExcel.Text = "匯出";
            this.btnExcel.UseVisualStyleBackColor = true;
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // lblDataDate
            // 
            this.lblDataDate.AutoSize = true;
            this.lblDataDate.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblDataDate.Location = new System.Drawing.Point(7, 16);
            this.lblDataDate.Name = "lblDataDate";
            this.lblDataDate.Size = new System.Drawing.Size(106, 24);
            this.lblDataDate.TabIndex = 15;
            this.lblDataDate.Text = "資料日期";
            // 
            // btnRptPrint
            // 
            this.btnRptPrint.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnRptPrint.Location = new System.Drawing.Point(603, 59);
            this.btnRptPrint.Margin = new System.Windows.Forms.Padding(4);
            this.btnRptPrint.Name = "btnRptPrint";
            this.btnRptPrint.Size = new System.Drawing.Size(189, 51);
            this.btnRptPrint.TabIndex = 17;
            this.btnRptPrint.Text = "列印驗收報表";
            this.btnRptPrint.UseVisualStyleBackColor = true;
            // 
            // txtFinalCommand
            // 
            this.txtFinalCommand.Enabled = false;
            this.txtFinalCommand.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtFinalCommand.Location = new System.Drawing.Point(5, 401);
            this.txtFinalCommand.Margin = new System.Windows.Forms.Padding(4);
            this.txtFinalCommand.Multiline = true;
            this.txtFinalCommand.Name = "txtFinalCommand";
            this.txtFinalCommand.Size = new System.Drawing.Size(787, 101);
            this.txtFinalCommand.TabIndex = 24;
            this.txtFinalCommand.Text = "[\"plattingNote\", \"[{\'Key\':\'Supplier\',\'Value\':\'CS\'}]\"]";
            // 
            // dtp1
            // 
            this.dtp1.CalendarFont = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dtp1.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dtp1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtp1.Location = new System.Drawing.Point(120, 4);
            this.dtp1.Margin = new System.Windows.Forms.Padding(4);
            this.dtp1.Name = "dtp1";
            this.dtp1.Size = new System.Drawing.Size(188, 39);
            this.dtp1.TabIndex = 52;
            this.dtp1.Value = new System.DateTime(2020, 12, 14, 0, 0, 0, 0);
            // 
            // comboSupplier
            // 
            this.comboSupplier.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboSupplier.FormattingEnabled = true;
            this.comboSupplier.Items.AddRange(new object[] {
            "EG",
            "CS",
            "KJ"});
            this.comboSupplier.Location = new System.Drawing.Point(479, 10);
            this.comboSupplier.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboSupplier.Name = "comboSupplier";
            this.comboSupplier.Size = new System.Drawing.Size(93, 31);
            this.comboSupplier.TabIndex = 53;
            this.comboSupplier.Text = "CS";
            this.comboSupplier.SelectedIndexChanged += new System.EventHandler(this.comboSupplier_SelectedIndexChanged_1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(413, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 24);
            this.label1.TabIndex = 54;
            this.label1.Text = "廠商";
            // 
            // txtUdpServer
            // 
            this.txtUdpServer.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtUdpServer.Location = new System.Drawing.Point(168, 357);
            this.txtUdpServer.Margin = new System.Windows.Forms.Padding(4);
            this.txtUdpServer.Name = "txtUdpServer";
            this.txtUdpServer.Size = new System.Drawing.Size(252, 36);
            this.txtUdpServer.TabIndex = 56;
            this.txtUdpServer.Text = "127.0.0.1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(7, 367);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(130, 24);
            this.label3.TabIndex = 55;
            this.label3.Text = "UPD Server";
            // 
            // Form電鍍報表cs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 515);
            this.Controls.Add(this.txtUdpServer);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboSupplier);
            this.Controls.Add(this.dtp1);
            this.Controls.Add(this.txtFinalCommand);
            this.Controls.Add(this.btnRptPrint);
            this.Controls.Add(this.lblDataDate);
            this.Controls.Add(this.btnExcel);
            this.Controls.Add(this.btnRptQuery);
            this.Controls.Add(this.btnLblPrint);
            this.Controls.Add(this.btnLblQuery);
            this.Controls.Add(this.dgv1);
            this.Name = "Form電鍍報表cs";
            this.Text = "電鍍報表cs";
            this.Load += new System.EventHandler(this.Form電鍍報表cs_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv1;
        private System.Windows.Forms.Button btnLblQuery;
        private System.Windows.Forms.Button btnLblPrint;
        private System.Windows.Forms.Button btnRptQuery;
        private System.Windows.Forms.Button btnExcel;
        private System.Windows.Forms.Label lblDataDate;
        private System.Windows.Forms.Button btnRptPrint;
        private System.Windows.Forms.TextBox txtFinalCommand;
        private System.Windows.Forms.DateTimePicker dtp1;
        private System.Windows.Forms.ComboBox comboSupplier;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtUdpServer;
        private System.Windows.Forms.Label label3;
    }
}