
namespace TaskTrayApplication
{
    partial class TestDBConnection
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
            this.label1 = new System.Windows.Forms.Label();
            this.comboConnections = new System.Windows.Forms.ComboBox();
            this.textConnStr = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.textSucess = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textFail = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnConnTest = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(12, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 24);
            this.label1.TabIndex = 2;
            this.label1.Text = "選取資料庫";
            // 
            // comboConnections
            // 
            this.comboConnections.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboConnections.FormattingEnabled = true;
            this.comboConnections.Items.AddRange(new object[] {
            "全部測試",
            "MFOFlowConnString",
            "TEST_MFOFlowConnString",
            "ACLConnString",
            "SampleConnection",
            "WarehouseConnString",
            "MFOFlowConnString172_33",
            "ConutriTCConnString",
            "ProcurmentConnection"});
            this.comboConnections.Location = new System.Drawing.Point(165, 21);
            this.comboConnections.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboConnections.Name = "comboConnections";
            this.comboConnections.Size = new System.Drawing.Size(731, 31);
            this.comboConnections.TabIndex = 3;
            this.comboConnections.Text = "全部測試";
            // 
            // textConnStr
            // 
            this.textConnStr.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textConnStr.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textConnStr.Location = new System.Drawing.Point(165, 70);
            this.textConnStr.Margin = new System.Windows.Forms.Padding(4);
            this.textConnStr.Multiline = true;
            this.textConnStr.Name = "textConnStr";
            this.textConnStr.Size = new System.Drawing.Size(1040, 116);
            this.textConnStr.TabIndex = 22;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label21.Location = new System.Drawing.Point(12, 70);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(142, 24);
            this.label21.TabIndex = 21;
            this.label21.Text = "連線內容 : ";
            // 
            // textSucess
            // 
            this.textSucess.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textSucess.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textSucess.Location = new System.Drawing.Point(165, 200);
            this.textSucess.Margin = new System.Windows.Forms.Padding(4);
            this.textSucess.Multiline = true;
            this.textSucess.Name = "textSucess";
            this.textSucess.Size = new System.Drawing.Size(1040, 110);
            this.textSucess.TabIndex = 24;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(12, 200);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(142, 24);
            this.label2.TabIndex = 23;
            this.label2.Text = "連線正常 : ";
            // 
            // textFail
            // 
            this.textFail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textFail.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textFail.Location = new System.Drawing.Point(165, 327);
            this.textFail.Margin = new System.Windows.Forms.Padding(4);
            this.textFail.Multiline = true;
            this.textFail.Name = "textFail";
            this.textFail.Size = new System.Drawing.Size(1040, 110);
            this.textFail.TabIndex = 26;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(12, 334);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(142, 24);
            this.label3.TabIndex = 25;
            this.label3.Text = "連線正常 : ";
            // 
            // btnConnTest
            // 
            this.btnConnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnTest.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnConnTest.Location = new System.Drawing.Point(925, 16);
            this.btnConnTest.Margin = new System.Windows.Forms.Padding(4);
            this.btnConnTest.Name = "btnConnTest";
            this.btnConnTest.Size = new System.Drawing.Size(132, 46);
            this.btnConnTest.TabIndex = 27;
            this.btnConnTest.Text = "連線測試";
            this.btnConnTest.UseVisualStyleBackColor = true;
            this.btnConnTest.Click += new System.EventHandler(this.btnConnTest_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnRefresh.Location = new System.Drawing.Point(1065, 16);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(4);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(140, 46);
            this.btnRefresh.TabIndex = 28;
            this.btnRefresh.Text = "重新整理";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // TestDBConnection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1218, 447);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnConnTest);
            this.Controls.Add(this.textFail);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textSucess);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textConnStr);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboConnections);
            this.Name = "TestDBConnection";
            this.Text = "測試資料庫連線";
            this.Load += new System.EventHandler(this.TestDBConnection_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboConnections;
        private System.Windows.Forms.TextBox textConnStr;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox textSucess;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textFail;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnConnTest;
        private System.Windows.Forms.Button btnRefresh;
    }
}