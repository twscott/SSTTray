
namespace TaskTrayApplication
{
    partial class FormDoSST
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
            this.label2 = new System.Windows.Forms.Label();
            this.txtEndTime = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnDoSST = new System.Windows.Forms.Button();
            this.txtStartTime = new System.Windows.Forms.TextBox();
            this.txtStackTrace = new System.Windows.Forms.TextBox();
            this.txtStackFlow = new System.Windows.Forms.Label();
            this.txtLastErrorTime = new System.Windows.Forms.TextBox();
            this.txtDayErrCnt = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtErrorCnt = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtLogoutTime = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtLoginTime = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(8, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(142, 24);
            this.label2.TabIndex = 10;
            this.label2.Text = "登入時間 : ";
            // 
            // txtEndTime
            // 
            this.txtEndTime.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtEndTime.Location = new System.Drawing.Point(239, 170);
            this.txtEndTime.Margin = new System.Windows.Forms.Padding(4);
            this.txtEndTime.Name = "txtEndTime";
            this.txtEndTime.Size = new System.Drawing.Size(392, 36);
            this.txtEndTime.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(8, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 24);
            this.label1.TabIndex = 12;
            this.label1.Text = "登出時間 : ";
            // 
            // btnDoSST
            // 
            this.btnDoSST.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDoSST.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnDoSST.Location = new System.Drawing.Point(673, 5);
            this.btnDoSST.Margin = new System.Windows.Forms.Padding(4);
            this.btnDoSST.Name = "btnDoSST";
            this.btnDoSST.Size = new System.Drawing.Size(187, 46);
            this.btnDoSST.TabIndex = 14;
            this.btnDoSST.Text = "Do SST";
            this.btnDoSST.UseVisualStyleBackColor = true;
            this.btnDoSST.Click += new System.EventHandler(this.btnDoSST_Click);
            // 
            // txtStartTime
            // 
            this.txtStartTime.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtStartTime.Location = new System.Drawing.Point(239, 117);
            this.txtStartTime.Margin = new System.Windows.Forms.Padding(4);
            this.txtStartTime.Name = "txtStartTime";
            this.txtStartTime.Size = new System.Drawing.Size(392, 36);
            this.txtStartTime.TabIndex = 11;
            // 
            // txtStackTrace
            // 
            this.txtStackTrace.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtStackTrace.Location = new System.Drawing.Point(673, 107);
            this.txtStackTrace.Margin = new System.Windows.Forms.Padding(4);
            this.txtStackTrace.Multiline = true;
            this.txtStackTrace.Name = "txtStackTrace";
            this.txtStackTrace.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtStackTrace.Size = new System.Drawing.Size(765, 241);
            this.txtStackTrace.TabIndex = 41;
            // 
            // txtStackFlow
            // 
            this.txtStackFlow.AutoSize = true;
            this.txtStackFlow.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtStackFlow.Location = new System.Drawing.Point(669, 78);
            this.txtStackFlow.Name = "txtStackFlow";
            this.txtStackFlow.Size = new System.Drawing.Size(178, 24);
            this.txtStackFlow.TabIndex = 40;
            this.txtStackFlow.Text = "最後 StackFlow";
            // 
            // txtLastErrorTime
            // 
            this.txtLastErrorTime.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtLastErrorTime.Location = new System.Drawing.Point(1034, 63);
            this.txtLastErrorTime.Margin = new System.Windows.Forms.Padding(4);
            this.txtLastErrorTime.Name = "txtLastErrorTime";
            this.txtLastErrorTime.Size = new System.Drawing.Size(404, 36);
            this.txtLastErrorTime.TabIndex = 37;
            // 
            // txtDayErrCnt
            // 
            this.txtDayErrCnt.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtDayErrCnt.Location = new System.Drawing.Point(239, 268);
            this.txtDayErrCnt.Margin = new System.Windows.Forms.Padding(4);
            this.txtDayErrCnt.Name = "txtDayErrCnt";
            this.txtDayErrCnt.Size = new System.Drawing.Size(159, 36);
            this.txtDayErrCnt.TabIndex = 35;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label6.Location = new System.Drawing.Point(13, 271);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(202, 24);
            this.label6.TabIndex = 34;
            this.label6.Text = "本日處理錯誤次數";
            // 
            // txtErrorCnt
            // 
            this.txtErrorCnt.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtErrorCnt.Location = new System.Drawing.Point(239, 223);
            this.txtErrorCnt.Margin = new System.Windows.Forms.Padding(4);
            this.txtErrorCnt.Name = "txtErrorCnt";
            this.txtErrorCnt.Size = new System.Drawing.Size(159, 36);
            this.txtErrorCnt.TabIndex = 33;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label5.Location = new System.Drawing.Point(13, 226);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(202, 24);
            this.label5.TabIndex = 32;
            this.label5.Text = "本次處理錯誤次數";
            // 
            // txtLogoutTime
            // 
            this.txtLogoutTime.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtLogoutTime.Location = new System.Drawing.Point(239, 65);
            this.txtLogoutTime.Margin = new System.Windows.Forms.Padding(4);
            this.txtLogoutTime.Name = "txtLogoutTime";
            this.txtLogoutTime.Size = new System.Drawing.Size(404, 36);
            this.txtLogoutTime.TabIndex = 31;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label4.Location = new System.Drawing.Point(13, 120);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(154, 24);
            this.label4.TabIndex = 30;
            this.label4.Text = "開始處理時間";
            // 
            // txtLoginTime
            // 
            this.txtLoginTime.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtLoginTime.Location = new System.Drawing.Point(239, 12);
            this.txtLoginTime.Margin = new System.Windows.Forms.Padding(4);
            this.txtLoginTime.Name = "txtLoginTime";
            this.txtLoginTime.Size = new System.Drawing.Size(404, 36);
            this.txtLoginTime.TabIndex = 29;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(13, 173);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(154, 24);
            this.label3.TabIndex = 28;
            this.label3.Text = "完成處理時間";
            // 
            // FormDoSST
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1450, 363);
            this.Controls.Add(this.txtStackTrace);
            this.Controls.Add(this.txtStackFlow);
            this.Controls.Add(this.txtLastErrorTime);
            this.Controls.Add(this.txtDayErrCnt);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtErrorCnt);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtLogoutTime);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtLoginTime);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnDoSST);
            this.Controls.Add(this.txtEndTime);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtStartTime);
            this.Controls.Add(this.label2);
            this.Name = "FormDoSST";
            this.Text = "FormDoSST";
            this.Load += new System.EventHandler(this.FormDoSST_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox txtEndTime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnDoSST;
        public System.Windows.Forms.TextBox txtStartTime;
        public System.Windows.Forms.TextBox txtStackTrace;
        public System.Windows.Forms.Label txtStackFlow;
        public System.Windows.Forms.TextBox txtLastErrorTime;
        public System.Windows.Forms.TextBox txtDayErrCnt;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.TextBox txtErrorCnt;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.TextBox txtLogoutTime;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox txtLoginTime;
        private System.Windows.Forms.Label label3;
    }
}