namespace TaskTrayApplication
{
    partial class Form產生Excel
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
            this.btnExec = new System.Windows.Forms.Button();
            this.txtConnectionStr = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboConnection = new System.Windows.Forms.ComboBox();
            this.txtSql = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnExec
            // 
            this.btnExec.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExec.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnExec.Location = new System.Drawing.Point(876, 168);
            this.btnExec.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnExec.Name = "btnExec";
            this.btnExec.Size = new System.Drawing.Size(175, 76);
            this.btnExec.TabIndex = 93;
            this.btnExec.Text = "執行";
            this.btnExec.UseVisualStyleBackColor = true;
            this.btnExec.Click += new System.EventHandler(this.btnExec_Click);
            // 
            // txtConnectionStr
            // 
            this.txtConnectionStr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConnectionStr.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtConnectionStr.Location = new System.Drawing.Point(16, 267);
            this.txtConnectionStr.Margin = new System.Windows.Forms.Padding(4);
            this.txtConnectionStr.Multiline = true;
            this.txtConnectionStr.Name = "txtConnectionStr";
            this.txtConnectionStr.Size = new System.Drawing.Size(1033, 94);
            this.txtConnectionStr.TabIndex = 92;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(15, 239);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(238, 24);
            this.label2.TabIndex = 91;
            this.label2.Text = "手動輸入 Connection";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(15, 183);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 24);
            this.label1.TabIndex = 89;
            this.label1.Text = "Connection";
            // 
            // comboConnection
            // 
            this.comboConnection.AutoCompleteCustomSource.AddRange(new string[] {
            "列印 RO QRCode",
            "HtmlToPdf"});
            this.comboConnection.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboConnection.FormattingEnabled = true;
            this.comboConnection.Items.AddRange(new object[] {
            "ACLConnString",
            "SampleConnection",
            "MFOFlowConnString",
            "WarehouseConnString",
            "MFOFlowConnString172_33",
            "ConutriTCConnString",
            "ConutriTCConnString"});
            this.comboConnection.Location = new System.Drawing.Point(188, 179);
            this.comboConnection.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboConnection.Name = "comboConnection";
            this.comboConnection.Size = new System.Drawing.Size(327, 31);
            this.comboConnection.TabIndex = 90;
            this.comboConnection.Text = "MFOFlowConnString";
            this.comboConnection.SelectedIndexChanged += new System.EventHandler(this.comboConnection_SelectedIndexChanged);
            // 
            // txtSql
            // 
            this.txtSql.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSql.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtSql.Location = new System.Drawing.Point(180, 24);
            this.txtSql.Margin = new System.Windows.Forms.Padding(4);
            this.txtSql.Multiline = true;
            this.txtSql.Name = "txtSql";
            this.txtSql.Size = new System.Drawing.Size(869, 136);
            this.txtSql.TabIndex = 88;
            this.txtSql.Text = "SELECT max(`PDID`),`PERFORM_ID`,`PTYPE`,`PERFORM_DATE`, count(PERFORM_ID) FROM `m" +
    "fo_perform_daily` group by `PERFORM_ID`,PERFORM_DATE having count(PERFORM_ID) > " +
    "1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(15, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(142, 24);
            this.label3.TabIndex = 87;
            this.label3.Text = "Select 指令";
            // 
            // Form產生Excel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 384);
            this.Controls.Add(this.btnExec);
            this.Controls.Add(this.txtConnectionStr);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboConnection);
            this.Controls.Add(this.txtSql);
            this.Controls.Add(this.label3);
            this.Name = "Form產生Excel";
            this.Text = "SQL 產生Excel";
            this.Load += new System.EventHandler(this.Form產生Excel_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExec;
        private System.Windows.Forms.TextBox txtConnectionStr;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboConnection;
        private System.Windows.Forms.TextBox txtSql;
        private System.Windows.Forms.Label label3;
    }
}