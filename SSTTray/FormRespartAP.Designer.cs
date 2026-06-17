namespace TaskTrayApplication
{
    partial class FormRespartAP
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
            this.lbldailyReport = new System.Windows.Forms.Label();
            this.txtRestartAps = new System.Windows.Forms.TextBox();
            this.btnCauculate = new System.Windows.Forms.Button();
            this.btnSync = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnShutDown = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSendMail = new System.Windows.Forms.Button();
            this.btnReceiveMail = new System.Windows.Forms.Button();
            this.txtBody = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSubject = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtEmailServer = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtEmailAdd = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbldailyReport
            // 
            this.lbldailyReport.AutoSize = true;
            this.lbldailyReport.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbldailyReport.Location = new System.Drawing.Point(20, 21);
            this.lbldailyReport.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbldailyReport.Name = "lbldailyReport";
            this.lbldailyReport.Size = new System.Drawing.Size(106, 24);
            this.lbldailyReport.TabIndex = 75;
            this.lbldailyReport.Text = "重啟程式";
            // 
            // txtRestartAps
            // 
            this.txtRestartAps.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtRestartAps.Location = new System.Drawing.Point(150, 18);
            this.txtRestartAps.Margin = new System.Windows.Forms.Padding(4);
            this.txtRestartAps.Multiline = true;
            this.txtRestartAps.Name = "txtRestartAps";
            this.txtRestartAps.Size = new System.Drawing.Size(629, 107);
            this.txtRestartAps.TabIndex = 76;
            this.txtRestartAps.Text = "D:\\DBbackup";
            // 
            // btnCauculate
            // 
            this.btnCauculate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCauculate.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnCauculate.Location = new System.Drawing.Point(22, 149);
            this.btnCauculate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCauculate.Name = "btnCauculate";
            this.btnCauculate.Size = new System.Drawing.Size(170, 59);
            this.btnCauculate.TabIndex = 115;
            this.btnCauculate.Text = "重啟程式";
            this.btnCauculate.UseVisualStyleBackColor = true;
            this.btnCauculate.Click += new System.EventHandler(this.btnCauculate_Click);
            // 
            // btnSync
            // 
            this.btnSync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSync.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSync.Location = new System.Drawing.Point(417, 149);
            this.btnSync.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(170, 59);
            this.btnSync.TabIndex = 114;
            this.btnSync.Text = "重啟電腦";
            this.btnSync.UseVisualStyleBackColor = true;
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.button1.Location = new System.Drawing.Point(216, 149);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(170, 59);
            this.button1.TabIndex = 116;
            this.button1.Text = "重啟火箭程式";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnShutDown
            // 
            this.btnShutDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnShutDown.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnShutDown.Location = new System.Drawing.Point(607, 149);
            this.btnShutDown.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnShutDown.Name = "btnShutDown";
            this.btnShutDown.Size = new System.Drawing.Size(170, 59);
            this.btnShutDown.TabIndex = 117;
            this.btnShutDown.Text = "電腦關機";
            this.btnShutDown.UseVisualStyleBackColor = true;
            this.btnShutDown.Click += new System.EventHandler(this.btnShutDown_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbldailyReport);
            this.groupBox1.Controls.Add(this.btnShutDown);
            this.groupBox1.Controls.Add(this.txtRestartAps);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.btnSync);
            this.groupBox1.Controls.Add(this.btnCauculate);
            this.groupBox1.Location = new System.Drawing.Point(17, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(856, 229);
            this.groupBox1.TabIndex = 118;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "系統測試";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnSendMail);
            this.groupBox2.Controls.Add(this.btnReceiveMail);
            this.groupBox2.Controls.Add(this.txtBody);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtSubject);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtEmailServer);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txtEmailAdd);
            this.groupBox2.Location = new System.Drawing.Point(17, 274);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(856, 306);
            this.groupBox2.TabIndex = 119;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Email 測試";
            // 
            // btnSendMail
            // 
            this.btnSendMail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSendMail.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSendMail.Location = new System.Drawing.Point(675, 23);
            this.btnSendMail.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSendMail.Name = "btnSendMail";
            this.btnSendMail.Size = new System.Drawing.Size(170, 59);
            this.btnSendMail.TabIndex = 125;
            this.btnSendMail.Text = "發信";
            this.btnSendMail.UseVisualStyleBackColor = true;
            this.btnSendMail.Click += new System.EventHandler(this.btnSendMail_Click_1);
            // 
            // btnReceiveMail
            // 
            this.btnReceiveMail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReceiveMail.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnReceiveMail.Location = new System.Drawing.Point(675, 91);
            this.btnReceiveMail.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnReceiveMail.Name = "btnReceiveMail";
            this.btnReceiveMail.Size = new System.Drawing.Size(170, 59);
            this.btnReceiveMail.TabIndex = 118;
            this.btnReceiveMail.Text = "收信";
            this.btnReceiveMail.UseVisualStyleBackColor = true;
            this.btnReceiveMail.Click += new System.EventHandler(this.btnReceiveMail_Click);
            // 
            // txtBody
            // 
            this.txtBody.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtBody.Location = new System.Drawing.Point(216, 163);
            this.txtBody.Margin = new System.Windows.Forms.Padding(4);
            this.txtBody.Multiline = true;
            this.txtBody.Name = "txtBody";
            this.txtBody.Size = new System.Drawing.Size(629, 136);
            this.txtBody.TabIndex = 118;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label4.Location = new System.Drawing.Point(59, 163);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(118, 24);
            this.label4.TabIndex = 124;
            this.label4.Text = "Mail Body";
            // 
            // txtSubject
            // 
            this.txtSubject.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtSubject.Location = new System.Drawing.Point(216, 114);
            this.txtSubject.Margin = new System.Windows.Forms.Padding(4);
            this.txtSubject.Name = "txtSubject";
            this.txtSubject.Size = new System.Drawing.Size(335, 36);
            this.txtSubject.TabIndex = 123;
            this.txtSubject.Text = "測試";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(92, 117);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 24);
            this.label3.TabIndex = 122;
            this.label3.Text = "Subject";
            // 
            // txtEmailServer
            // 
            this.txtEmailServer.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtEmailServer.Location = new System.Drawing.Point(216, 68);
            this.txtEmailServer.Margin = new System.Windows.Forms.Padding(4);
            this.txtEmailServer.Name = "txtEmailServer";
            this.txtEmailServer.Size = new System.Drawing.Size(335, 36);
            this.txtEmailServer.TabIndex = 121;
            this.txtEmailServer.Text = "mail.conutri.com";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(32, 71);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(154, 24);
            this.label2.TabIndex = 120;
            this.label2.Text = "Email Server";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(20, 28);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(166, 24);
            this.label1.TabIndex = 119;
            this.label1.Text = "Email Address";
            // 
            // txtEmailAdd
            // 
            this.txtEmailAdd.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtEmailAdd.Location = new System.Drawing.Point(216, 25);
            this.txtEmailAdd.Margin = new System.Windows.Forms.Padding(4);
            this.txtEmailAdd.Name = "txtEmailAdd";
            this.txtEmailAdd.Size = new System.Drawing.Size(335, 36);
            this.txtEmailAdd.TabIndex = 118;
            this.txtEmailAdd.Text = "ups@firstohm.com.tw";
            // 
            // FormRespartAP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(885, 592);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FormRespartAP";
            this.Text = "重啟程式/電腦";
            this.Load += new System.EventHandler(this.FormRespartAP_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lbldailyReport;
        private System.Windows.Forms.TextBox txtRestartAps;
        private System.Windows.Forms.Button btnCauculate;
        private System.Windows.Forms.Button btnSync;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnShutDown;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnSendMail;
        private System.Windows.Forms.Button btnReceiveMail;
        private System.Windows.Forms.TextBox txtBody;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSubject;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtEmailServer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtEmailAdd;
    }
}