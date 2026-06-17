namespace TaskTrayApplication
{
    partial class FormGenBarCode
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnGenBar = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSubFlowID = new System.Windows.Forms.TextBox();
            this.txtSignID = new System.Windows.Forms.TextBox();
            this.txtBarCode = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.radio151 = new System.Windows.Forms.RadioButton();
            this.radio33 = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label5.Location = new System.Drawing.Point(34, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(118, 24);
            this.label5.TabIndex = 19;
            this.label5.Text = "SubFlowID";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(57, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 24);
            this.label1.TabIndex = 21;
            this.label1.Text = "SIGNID";
            // 
            // btnGenBar
            // 
            this.btnGenBar.Font = new System.Drawing.Font("標楷體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnGenBar.Location = new System.Drawing.Point(61, 153);
            this.btnGenBar.Margin = new System.Windows.Forms.Padding(4);
            this.btnGenBar.Name = "btnGenBar";
            this.btnGenBar.Size = new System.Drawing.Size(189, 42);
            this.btnGenBar.TabIndex = 23;
            this.btnGenBar.Text = "產生 BarCode";
            this.btnGenBar.UseVisualStyleBackColor = true;
            this.btnGenBar.Click += new System.EventHandler(this.btnGenBar_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(82, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 24);
            this.label2.TabIndex = 24;
            this.label2.Text = "或";
            // 
            // txtSubFlowID
            // 
            this.txtSubFlowID.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtSubFlowID.Location = new System.Drawing.Point(159, 17);
            this.txtSubFlowID.Margin = new System.Windows.Forms.Padding(4);
            this.txtSubFlowID.Name = "txtSubFlowID";
            this.txtSubFlowID.Size = new System.Drawing.Size(212, 36);
            this.txtSubFlowID.TabIndex = 25;
            this.txtSubFlowID.Text = "177483";
            // 
            // txtSignID
            // 
            this.txtSignID.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtSignID.Location = new System.Drawing.Point(159, 88);
            this.txtSignID.Margin = new System.Windows.Forms.Padding(4);
            this.txtSignID.Name = "txtSignID";
            this.txtSignID.Size = new System.Drawing.Size(212, 36);
            this.txtSignID.TabIndex = 26;
            this.txtSignID.Text = "16020";
            // 
            // txtBarCode
            // 
            this.txtBarCode.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtBarCode.Location = new System.Drawing.Point(290, 159);
            this.txtBarCode.Margin = new System.Windows.Forms.Padding(4);
            this.txtBarCode.Name = "txtBarCode";
            this.txtBarCode.Size = new System.Drawing.Size(310, 36);
            this.txtBarCode.TabIndex = 27;
            this.txtBarCode.Text = "16020";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("標楷體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.button1.Location = new System.Drawing.Point(61, 213);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(189, 42);
            this.button1.TabIndex = 28;
            this.button1.Text = "清除資料";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // radio151
            // 
            this.radio151.AutoSize = true;
            this.radio151.Checked = true;
            this.radio151.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radio151.Location = new System.Drawing.Point(430, 21);
            this.radio151.Name = "radio151";
            this.radio151.Size = new System.Drawing.Size(115, 28);
            this.radio151.TabIndex = 29;
            this.radio151.TabStop = true;
            this.radio151.Text = "172.151";
            this.radio151.UseVisualStyleBackColor = true;
            // 
            // radio33
            // 
            this.radio33.AutoSize = true;
            this.radio33.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radio33.Location = new System.Drawing.Point(430, 58);
            this.radio33.Name = "radio33";
            this.radio33.Size = new System.Drawing.Size(103, 28);
            this.radio33.TabIndex = 30;
            this.radio33.Text = "172.35";
            this.radio33.UseVisualStyleBackColor = true;
            // 
            // FormGenBarCode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(625, 259);
            this.Controls.Add(this.radio33);
            this.Controls.Add(this.radio151);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtBarCode);
            this.Controls.Add(this.txtSignID);
            this.Controls.Add(this.txtSubFlowID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnGenBar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label5);
            this.Name = "FormGenBarCode";
            this.Text = "產生 流程單 BarCode";
            this.Load += new System.EventHandler(this.FormGenBarCode_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnGenBar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSubFlowID;
        private System.Windows.Forms.TextBox txtSignID;
        private System.Windows.Forms.TextBox txtBarCode;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RadioButton radio151;
        private System.Windows.Forms.RadioButton radio33;
    }
}