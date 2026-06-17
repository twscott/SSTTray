
namespace TaskTrayApplication
{
    partial class 解開DB死結
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
            this.label6 = new System.Windows.Forms.Label();
            this.comboDBServers = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboDBToKill = new System.Windows.Forms.ComboBox();
            this.btnBackup = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dgv1 = new System.Windows.Forms.DataGridView();
            this.btnQuery = new System.Windows.Forms.Button();
            this.numericUpDown = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.dgv1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label6.Location = new System.Drawing.Point(22, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(142, 24);
            this.label6.TabIndex = 21;
            this.label6.Text = "目標 Server";
            // 
            // comboDBServers
            // 
            this.comboDBServers.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboDBServers.FormattingEnabled = true;
            this.comboDBServers.Items.AddRange(new object[] {
            "HL151",
            "HL33",
            "TP211",
            "TP33"});
            this.comboDBServers.Location = new System.Drawing.Point(212, 19);
            this.comboDBServers.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboDBServers.Name = "comboDBServers";
            this.comboDBServers.Size = new System.Drawing.Size(252, 31);
            this.comboDBServers.TabIndex = 22;
            this.comboDBServers.Text = "HL151";
            this.comboDBServers.SelectedIndexChanged += new System.EventHandler(this.comboBKServers_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(22, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 24);
            this.label1.TabIndex = 19;
            this.label1.Text = "選取資料庫";
            // 
            // comboDBToKill
            // 
            this.comboDBToKill.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboDBToKill.FormattingEnabled = true;
            this.comboDBToKill.Location = new System.Drawing.Point(212, 63);
            this.comboDBToKill.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboDBToKill.Name = "comboDBToKill";
            this.comboDBToKill.Size = new System.Drawing.Size(404, 31);
            this.comboDBToKill.TabIndex = 20;
            this.comboDBToKill.Text = "MFO_FLOW";
            // 
            // btnBackup
            // 
            this.btnBackup.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnBackup.Location = new System.Drawing.Point(555, 113);
            this.btnBackup.Margin = new System.Windows.Forms.Padding(4);
            this.btnBackup.Name = "btnBackup";
            this.btnBackup.Size = new System.Drawing.Size(177, 46);
            this.btnBackup.TabIndex = 23;
            this.btnBackup.Text = "Kill Process";
            this.btnBackup.UseVisualStyleBackColor = true;
            this.btnBackup.Click += new System.EventHandler(this.btnBackup_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(22, 113);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(154, 24);
            this.label2.TabIndex = 24;
            this.label2.Text = "執行程序秒數";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(308, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 24);
            this.label3.TabIndex = 26;
            this.label3.Text = "秒";
            // 
            // dgv1
            // 
            this.dgv1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv1.Location = new System.Drawing.Point(12, 162);
            this.dgv1.Name = "dgv1";
            this.dgv1.RowHeadersWidth = 51;
            this.dgv1.RowTemplate.Height = 27;
            this.dgv1.Size = new System.Drawing.Size(1352, 336);
            this.dgv1.TabIndex = 27;
            // 
            // btnQuery
            // 
            this.btnQuery.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnQuery.Location = new System.Drawing.Point(370, 113);
            this.btnQuery.Margin = new System.Windows.Forms.Padding(4);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(177, 46);
            this.btnQuery.TabIndex = 28;
            this.btnQuery.Text = "Query Process";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.button1_Click);
            // 
            // numericUpDown
            // 
            this.numericUpDown.Font = new System.Drawing.Font("新細明體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.numericUpDown.Increment = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numericUpDown.Location = new System.Drawing.Point(212, 107);
            this.numericUpDown.Maximum = new decimal(new int[] {
            1200,
            0,
            0,
            0});
            this.numericUpDown.Minimum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numericUpDown.Name = "numericUpDown";
            this.numericUpDown.Size = new System.Drawing.Size(90, 35);
            this.numericUpDown.TabIndex = 29;
            this.numericUpDown.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
            // 
            // 解開DB死結
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1376, 510);
            this.Controls.Add(this.numericUpDown);
            this.Controls.Add(this.btnQuery);
            this.Controls.Add(this.dgv1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnBackup);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.comboDBServers);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboDBToKill);
            this.Name = "解開DB死結";
            this.Text = "解開DB死結";
            this.Load += new System.EventHandler(this.解開DB死結_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboDBServers;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboDBToKill;
        private System.Windows.Forms.Button btnBackup;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dgv1;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.NumericUpDown numericUpDown;
    }
}