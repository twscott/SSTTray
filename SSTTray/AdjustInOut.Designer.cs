
namespace TaskTrayApplication
{
    partial class AdjustInOut
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
            this.dtp2 = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dtp1 = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comboFlowStep = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.dgv1 = new System.Windows.Forms.DataGridView();
            this.btnQuery = new System.Windows.Forms.Button();
            this.btnAdjust = new System.Windows.Forms.Button();
            this.comboMaxLimit = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dgv2 = new System.Windows.Forms.DataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.radioAfter = new System.Windows.Forms.RadioButton();
            this.radioBefore = new System.Windows.Forms.RadioButton();
            this.btnQuerySingle = new System.Windows.Forms.Button();
            this.btnUpdateSingle = new System.Windows.Forms.Button();
            this.dgv3 = new System.Windows.Forms.DataGridView();
            this.chkboxFirstRec = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgv1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv2)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv3)).BeginInit();
            this.SuspendLayout();
            // 
            // dtp2
            // 
            this.dtp2.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dtp2.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtp2.Location = new System.Drawing.Point(407, 2);
            this.dtp2.Margin = new System.Windows.Forms.Padding(4);
            this.dtp2.Name = "dtp2";
            this.dtp2.Size = new System.Drawing.Size(181, 39);
            this.dtp2.TabIndex = 177;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(361, 9);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 24);
            this.label2.TabIndex = 176;
            this.label2.Text = "～";
            // 
            // dtp1
            // 
            this.dtp1.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dtp1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtp1.Location = new System.Drawing.Point(167, 2);
            this.dtp1.Margin = new System.Windows.Forms.Padding(4);
            this.dtp1.Name = "dtp1";
            this.dtp1.Size = new System.Drawing.Size(181, 39);
            this.dtp1.TabIndex = 175;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(1, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(154, 24);
            this.label1.TabIndex = 174;
            this.label1.Text = "製程完成日期";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(13, 59);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 24);
            this.label3.TabIndex = 178;
            this.label3.Text = "製程";
            // 
            // comboFlowStep
            // 
            this.comboFlowStep.AutoCompleteCustomSource.AddRange(new string[] {
            "列印 RO QRCode",
            "HtmlToPdf"});
            this.comboFlowStep.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboFlowStep.FormattingEnabled = true;
            this.comboFlowStep.Items.AddRange(new object[] {
            "切割",
            "底漆",
            "加壓",
            "外檢1",
            "色碼",
            "電鍍",
            "全檢1",
            "全檢2",
            "",
            ""});
            this.comboFlowStep.Location = new System.Drawing.Point(83, 57);
            this.comboFlowStep.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboFlowStep.Name = "comboFlowStep";
            this.comboFlowStep.Size = new System.Drawing.Size(130, 31);
            this.comboFlowStep.TabIndex = 179;
            this.comboFlowStep.Text = "色碼";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label4.Location = new System.Drawing.Point(325, 60);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(130, 24);
            this.label4.TabIndex = 180;
            this.label4.Text = "標準最大量";
            // 
            // dgv1
            // 
            this.dgv1.AllowUserToAddRows = false;
            this.dgv1.AllowUserToDeleteRows = false;
            this.dgv1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv1.Location = new System.Drawing.Point(6, 5);
            this.dgv1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgv1.Name = "dgv1";
            this.dgv1.RowHeadersWidth = 51;
            this.dgv1.RowTemplate.Height = 27;
            this.dgv1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv1.Size = new System.Drawing.Size(1478, 416);
            this.dgv1.TabIndex = 182;
            // 
            // btnQuery
            // 
            this.btnQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuery.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnQuery.Location = new System.Drawing.Point(1198, 11);
            this.btnQuery.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(145, 79);
            this.btnQuery.TabIndex = 183;
            this.btnQuery.Text = "查詢";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // btnAdjust
            // 
            this.btnAdjust.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdjust.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnAdjust.Location = new System.Drawing.Point(1348, 43);
            this.btnAdjust.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnAdjust.Name = "btnAdjust";
            this.btnAdjust.Size = new System.Drawing.Size(145, 45);
            this.btnAdjust.TabIndex = 184;
            this.btnAdjust.Text = "開始調整";
            this.btnAdjust.UseVisualStyleBackColor = true;
            this.btnAdjust.Click += new System.EventHandler(this.btnAdjust_Click);
            // 
            // comboMaxLimit
            // 
            this.comboMaxLimit.AutoCompleteCustomSource.AddRange(new string[] {
            "列印 RO QRCode",
            "HtmlToPdf"});
            this.comboMaxLimit.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboMaxLimit.FormattingEnabled = true;
            this.comboMaxLimit.Items.AddRange(new object[] {
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20"});
            this.comboMaxLimit.Location = new System.Drawing.Point(458, 59);
            this.comboMaxLimit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboMaxLimit.Name = "comboMaxLimit";
            this.comboMaxLimit.Size = new System.Drawing.Size(130, 31);
            this.comboMaxLimit.TabIndex = 185;
            this.comboMaxLimit.Text = "20";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(5, 111);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1498, 455);
            this.tabControl1.TabIndex = 186;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dgv1);
            this.tabPage1.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1490, 426);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "原始資料";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dgv2);
            this.tabPage2.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1490, 426);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "修改後資料";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dgv2
            // 
            this.dgv2.AllowUserToAddRows = false;
            this.dgv2.AllowUserToDeleteRows = false;
            this.dgv2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv2.Location = new System.Drawing.Point(6, 5);
            this.dgv2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgv2.Name = "dgv2";
            this.dgv2.RowHeadersWidth = 51;
            this.dgv2.RowTemplate.Height = 27;
            this.dgv2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv2.Size = new System.Drawing.Size(1478, 416);
            this.dgv2.TabIndex = 183;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.radioAfter);
            this.tabPage3.Controls.Add(this.radioBefore);
            this.tabPage3.Controls.Add(this.btnQuerySingle);
            this.tabPage3.Controls.Add(this.btnUpdateSingle);
            this.tabPage3.Controls.Add(this.dgv3);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1490, 426);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "單筆修改";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // radioAfter
            // 
            this.radioAfter.AutoSize = true;
            this.radioAfter.Checked = true;
            this.radioAfter.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioAfter.Location = new System.Drawing.Point(158, 24);
            this.radioAfter.Name = "radioAfter";
            this.radioAfter.Size = new System.Drawing.Size(151, 28);
            this.radioAfter.TabIndex = 190;
            this.radioAfter.TabStop = true;
            this.radioAfter.Text = "修改後資料";
            this.radioAfter.UseVisualStyleBackColor = true;
            // 
            // radioBefore
            // 
            this.radioBefore.AutoSize = true;
            this.radioBefore.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.radioBefore.Location = new System.Drawing.Point(22, 24);
            this.radioBefore.Name = "radioBefore";
            this.radioBefore.Size = new System.Drawing.Size(127, 28);
            this.radioBefore.TabIndex = 189;
            this.radioBefore.Text = "原始資料";
            this.radioBefore.UseVisualStyleBackColor = true;
            // 
            // btnQuerySingle
            // 
            this.btnQuerySingle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuerySingle.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnQuerySingle.Location = new System.Drawing.Point(1188, 9);
            this.btnQuerySingle.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnQuerySingle.Name = "btnQuerySingle";
            this.btnQuerySingle.Size = new System.Drawing.Size(145, 45);
            this.btnQuerySingle.TabIndex = 188;
            this.btnQuerySingle.Text = "查詢";
            this.btnQuerySingle.UseVisualStyleBackColor = true;
            this.btnQuerySingle.Click += new System.EventHandler(this.btnQuerySingle_Click);
            // 
            // btnUpdateSingle
            // 
            this.btnUpdateSingle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdateSingle.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnUpdateSingle.Location = new System.Drawing.Point(1339, 9);
            this.btnUpdateSingle.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnUpdateSingle.Name = "btnUpdateSingle";
            this.btnUpdateSingle.Size = new System.Drawing.Size(145, 45);
            this.btnUpdateSingle.TabIndex = 187;
            this.btnUpdateSingle.Text = "單筆修改";
            this.btnUpdateSingle.UseVisualStyleBackColor = true;
            this.btnUpdateSingle.Click += new System.EventHandler(this.btnUpdateSingle_Click);
            // 
            // dgv3
            // 
            this.dgv3.AllowUserToAddRows = false;
            this.dgv3.AllowUserToDeleteRows = false;
            this.dgv3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv3.Location = new System.Drawing.Point(3, 65);
            this.dgv3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgv3.Name = "dgv3";
            this.dgv3.RowHeadersWidth = 51;
            this.dgv3.RowTemplate.Height = 27;
            this.dgv3.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv3.Size = new System.Drawing.Size(1481, 356);
            this.dgv3.TabIndex = 184;
            // 
            // chkboxFirstRec
            // 
            this.chkboxFirstRec.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkboxFirstRec.AutoSize = true;
            this.chkboxFirstRec.Checked = true;
            this.chkboxFirstRec.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkboxFirstRec.Location = new System.Drawing.Point(1389, 14);
            this.chkboxFirstRec.Name = "chkboxFirstRec";
            this.chkboxFirstRec.Size = new System.Drawing.Size(104, 19);
            this.chkboxFirstRec.TabIndex = 187;
            this.chkboxFirstRec.Text = "調整第一筆";
            this.chkboxFirstRec.UseVisualStyleBackColor = true;
            // 
            // AdjustInOut
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1505, 569);
            this.Controls.Add(this.chkboxFirstRec);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.comboMaxLimit);
            this.Controls.Add(this.btnAdjust);
            this.Controls.Add(this.btnQuery);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.comboFlowStep);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dtp2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dtp1);
            this.Controls.Add(this.label1);
            this.Name = "AdjustInOut";
            this.Text = "調整製程進出量";
            this.Load += new System.EventHandler(this.AdjustInOut_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv2)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtp2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtp1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboFlowStep;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView dgv1;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.Button btnAdjust;
        private System.Windows.Forms.ComboBox comboMaxLimit;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dgv2;
        private System.Windows.Forms.Button btnUpdateSingle;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataGridView dgv3;
        private System.Windows.Forms.RadioButton radioAfter;
        private System.Windows.Forms.RadioButton radioBefore;
        private System.Windows.Forms.Button btnQuerySingle;
        private System.Windows.Forms.CheckBox chkboxFirstRec;
    }
}