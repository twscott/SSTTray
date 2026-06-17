namespace TaskTrayApplication
{
    partial class SendLineMsg
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
            this.lblDataDate = new System.Windows.Forms.Label();
            this.txtLineMsg = new System.Windows.Forms.TextBox();
            this.btnGetYieldInfo = new System.Windows.Forms.Button();
            this.btnSendMsg = new System.Windows.Forms.Button();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelResult = new System.Windows.Forms.Label();
            this.comboUser = new System.Windows.Forms.ComboBox();
            this.btnPaySendLine = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblDataDate
            // 
            this.lblDataDate.AutoSize = true;
            this.lblDataDate.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblDataDate.Location = new System.Drawing.Point(25, 30);
            this.lblDataDate.Name = "lblDataDate";
            this.lblDataDate.Size = new System.Drawing.Size(130, 24);
            this.lblDataDate.TabIndex = 19;
            this.lblDataDate.Text = "訊息接收人";
            // 
            // txtLineMsg
            // 
            this.txtLineMsg.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtLineMsg.Location = new System.Drawing.Point(29, 131);
            this.txtLineMsg.Margin = new System.Windows.Forms.Padding(4);
            this.txtLineMsg.Multiline = true;
            this.txtLineMsg.Name = "txtLineMsg";
            this.txtLineMsg.Size = new System.Drawing.Size(846, 124);
            this.txtLineMsg.TabIndex = 21;
            this.txtLineMsg.Text = "測試1\r\n測試2\r\n測試3\r\n";
            // 
            // btnGetYieldInfo
            // 
            this.btnGetYieldInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGetYieldInfo.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnGetYieldInfo.Location = new System.Drawing.Point(658, 8);
            this.btnGetYieldInfo.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetYieldInfo.Name = "btnGetYieldInfo";
            this.btnGetYieldInfo.Size = new System.Drawing.Size(205, 54);
            this.btnGetYieldInfo.TabIndex = 24;
            this.btnGetYieldInfo.Text = "取得良率資訊";
            this.btnGetYieldInfo.UseVisualStyleBackColor = true;
            this.btnGetYieldInfo.Click += new System.EventHandler(this.btnGetYieldInfo_Click);
            // 
            // btnSendMsg
            // 
            this.btnSendMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSendMsg.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSendMsg.Location = new System.Drawing.Point(369, 70);
            this.btnSendMsg.Margin = new System.Windows.Forms.Padding(4);
            this.btnSendMsg.Name = "btnSendMsg";
            this.btnSendMsg.Size = new System.Drawing.Size(262, 54);
            this.btnSendMsg.TabIndex = 25;
            this.btnSendMsg.Text = "Portal Send Line";
            this.btnSendMsg.UseVisualStyleBackColor = true;
            this.btnSendMsg.Click += new System.EventHandler(this.btnSendMsg_Click);
            // 
            // txtResult
            // 
            this.txtResult.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtResult.Location = new System.Drawing.Point(27, 312);
            this.txtResult.Margin = new System.Windows.Forms.Padding(4);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(845, 172);
            this.txtResult.TabIndex = 26;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(25, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 24);
            this.label1.TabIndex = 27;
            this.label1.Text = "傳送的訊息";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(25, 284);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(142, 24);
            this.label2.TabIndex = 28;
            this.label2.Text = "回傳結果 : ";
            // 
            // labelResult
            // 
            this.labelResult.AutoSize = true;
            this.labelResult.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelResult.Location = new System.Drawing.Point(173, 284);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(0, 24);
            this.labelResult.TabIndex = 29;
            // 
            // comboUser
            // 
            this.comboUser.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboUser.FormattingEnabled = true;
            this.comboUser.Items.AddRange(new object[] {
            "TP",
            "HL",
            "portal"});
            this.comboUser.Location = new System.Drawing.Point(177, 23);
            this.comboUser.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboUser.Name = "comboUser";
            this.comboUser.Size = new System.Drawing.Size(130, 31);
            this.comboUser.TabIndex = 30;
            this.comboUser.Text = "曾建明";
            // 
            // btnPaySendLine
            // 
            this.btnPaySendLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPaySendLine.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnPaySendLine.Location = new System.Drawing.Point(639, 69);
            this.btnPaySendLine.Margin = new System.Windows.Forms.Padding(4);
            this.btnPaySendLine.Name = "btnPaySendLine";
            this.btnPaySendLine.Size = new System.Drawing.Size(224, 54);
            this.btnPaySendLine.TabIndex = 31;
            this.btnPaySendLine.Text = "Outer Send Line";
            this.btnPaySendLine.UseVisualStyleBackColor = true;
            this.btnPaySendLine.Click += new System.EventHandler(this.btnPaySendLine_Click);
            // 
            // SendLineMsg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(885, 497);
            this.Controls.Add(this.btnPaySendLine);
            this.Controls.Add(this.comboUser);
            this.Controls.Add(this.labelResult);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.btnSendMsg);
            this.Controls.Add(this.btnGetYieldInfo);
            this.Controls.Add(this.txtLineMsg);
            this.Controls.Add(this.lblDataDate);
            this.Name = "SendLineMsg";
            this.Text = "SendLineMsg";
            this.Load += new System.EventHandler(this.SendLineMsg_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblDataDate;
        private System.Windows.Forms.TextBox txtLineMsg;
        private System.Windows.Forms.Button btnGetYieldInfo;
        private System.Windows.Forms.Button btnSendMsg;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelResult;
        private System.Windows.Forms.ComboBox comboUser;
        private System.Windows.Forms.Button btnPaySendLine;
    }
}