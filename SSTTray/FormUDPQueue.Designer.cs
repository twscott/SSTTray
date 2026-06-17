namespace TaskTrayApplication
{
    partial class FormUDPQueue
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
            this.btnQuery = new System.Windows.Forms.Button();
            this.btnSync = new System.Windows.Forms.Button();
            this.chkboxSelectAll = new System.Windows.Forms.CheckBox();
            this.btnExecQueue = new System.Windows.Forms.Button();
            this.lblStartPrint = new System.Windows.Forms.Label();
            this.lblEndprintTime = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgv1)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv1
            // 
            this.dgv1.AllowUserToAddRows = false;
            this.dgv1.AllowUserToDeleteRows = false;
            this.dgv1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv1.Location = new System.Drawing.Point(1, 69);
            this.dgv1.Name = "dgv1";
            this.dgv1.RowHeadersWidth = 51;
            this.dgv1.RowTemplate.Height = 27;
            this.dgv1.Size = new System.Drawing.Size(1539, 476);
            this.dgv1.TabIndex = 0;
            // 
            // btnQuery
            // 
            this.btnQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuery.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnQuery.Location = new System.Drawing.Point(870, 5);
            this.btnQuery.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(215, 59);
            this.btnQuery.TabIndex = 115;
            this.btnQuery.Text = "查詢";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // btnSync
            // 
            this.btnSync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSync.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnSync.Location = new System.Drawing.Point(1312, 5);
            this.btnSync.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(215, 59);
            this.btnSync.TabIndex = 114;
            this.btnSync.Text = "刪除";
            this.btnSync.UseVisualStyleBackColor = true;
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // chkboxSelectAll
            // 
            this.chkboxSelectAll.AutoSize = true;
            this.chkboxSelectAll.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.chkboxSelectAll.Location = new System.Drawing.Point(681, 21);
            this.chkboxSelectAll.Name = "chkboxSelectAll";
            this.chkboxSelectAll.Size = new System.Drawing.Size(164, 28);
            this.chkboxSelectAll.TabIndex = 116;
            this.chkboxSelectAll.Text = "全選/全反選";
            this.chkboxSelectAll.UseVisualStyleBackColor = true;
            this.chkboxSelectAll.CheckedChanged += new System.EventHandler(this.chkboxSelectAll_CheckedChanged);
            // 
            // btnExecQueue
            // 
            this.btnExecQueue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExecQueue.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnExecQueue.Location = new System.Drawing.Point(1091, 5);
            this.btnExecQueue.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnExecQueue.Name = "btnExecQueue";
            this.btnExecQueue.Size = new System.Drawing.Size(215, 59);
            this.btnExecQueue.TabIndex = 117;
            this.btnExecQueue.Text = "重新執行";
            this.btnExecQueue.UseVisualStyleBackColor = true;
            this.btnExecQueue.Click += new System.EventHandler(this.btnExecQueue_Click);
            // 
            // lblStartPrint
            // 
            this.lblStartPrint.AutoSize = true;
            this.lblStartPrint.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblStartPrint.Location = new System.Drawing.Point(12, 5);
            this.lblStartPrint.Name = "lblStartPrint";
            this.lblStartPrint.Size = new System.Drawing.Size(154, 24);
            this.lblStartPrint.TabIndex = 118;
            this.lblStartPrint.Text = "開始列印時間";
            // 
            // lblEndprintTime
            // 
            this.lblEndprintTime.AutoSize = true;
            this.lblEndprintTime.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblEndprintTime.Location = new System.Drawing.Point(12, 40);
            this.lblEndprintTime.Name = "lblEndprintTime";
            this.lblEndprintTime.Size = new System.Drawing.Size(154, 24);
            this.lblEndprintTime.TabIndex = 119;
            this.lblEndprintTime.Text = "完成列印時間";
            // 
            // FormUDPQueue
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1541, 549);
            this.Controls.Add(this.lblEndprintTime);
            this.Controls.Add(this.lblStartPrint);
            this.Controls.Add(this.btnExecQueue);
            this.Controls.Add(this.chkboxSelectAll);
            this.Controls.Add(this.btnQuery);
            this.Controls.Add(this.btnSync);
            this.Controls.Add(this.dgv1);
            this.Name = "FormUDPQueue";
            this.Text = "UDP Job Queue";
            this.Load += new System.EventHandler(this.FormUDPQueue_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv1;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.Button btnSync;
        private System.Windows.Forms.CheckBox chkboxSelectAll;
        private System.Windows.Forms.Button btnExecQueue;
        private System.Windows.Forms.Label lblStartPrint;
        private System.Windows.Forms.Label lblEndprintTime;
    }
}