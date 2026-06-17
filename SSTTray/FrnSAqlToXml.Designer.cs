
namespace TaskTrayApplication
{
    partial class FrnSAqlToXml
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrnSAqlToXml));
            this.txtResult = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.txtConnStr = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSql = new System.Windows.Forms.TextBox();
            this.txtDressing = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.brnGenXml = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtResult
            // 
            this.txtResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResult.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtResult.Location = new System.Drawing.Point(178, 455);
            this.txtResult.Margin = new System.Windows.Forms.Padding(4);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResult.Size = new System.Drawing.Size(1119, 169);
            this.txtResult.TabIndex = 24;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label21.Location = new System.Drawing.Point(31, 455);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(142, 24);
            this.label21.TabIndex = 23;
            this.label21.Text = "備份結果 : ";
            // 
            // txtConnStr
            // 
            this.txtConnStr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConnStr.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtConnStr.Location = new System.Drawing.Point(178, 7);
            this.txtConnStr.Margin = new System.Windows.Forms.Padding(4);
            this.txtConnStr.Name = "txtConnStr";
            this.txtConnStr.Size = new System.Drawing.Size(1119, 36);
            this.txtConnStr.TabIndex = 22;
            this.txtConnStr.Text = "Data Source=192.168.1.33;Password=2u4u 2u04yj3;User ID=firstohm;Database=conutris" +
    "tore;port=3306;charset=utf8;convert zero datetime=True;SslMode=None;";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(7, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(166, 24);
            this.label2.TabIndex = 21;
            this.label2.Text = "DB 連線設定：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(43, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 24);
            this.label1.TabIndex = 25;
            this.label1.Text = "SQL 指令：";
            // 
            // txtSql
            // 
            this.txtSql.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSql.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtSql.Location = new System.Drawing.Point(178, 49);
            this.txtSql.Margin = new System.Windows.Forms.Padding(4);
            this.txtSql.Multiline = true;
            this.txtSql.Name = "txtSql";
            this.txtSql.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtSql.Size = new System.Drawing.Size(1119, 308);
            this.txtSql.TabIndex = 26;
            this.txtSql.Text = resources.GetString("txtSql.Text");
            // 
            // txtDressing
            // 
            this.txtDressing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDressing.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtDressing.Location = new System.Drawing.Point(178, 365);
            this.txtDressing.Margin = new System.Windows.Forms.Padding(4);
            this.txtDressing.Multiline = true;
            this.txtDressing.Name = "txtDressing";
            this.txtDressing.Size = new System.Drawing.Size(1119, 82);
            this.txtDressing.TabIndex = 28;
            this.txtDressing.Text = "DocumentXm:<?xml version=\"1.0\" encoding=\"utf 8\"?>;TableTag:Products;RecordTag:Pro" +
    "duct;";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(43, 368);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(130, 24);
            this.label3.TabIndex = 27;
            this.label3.Text = "XML 轉換：";
            // 
            // brnGenXml
            // 
            this.brnGenXml.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.brnGenXml.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.brnGenXml.Location = new System.Drawing.Point(26, 491);
            this.brnGenXml.Margin = new System.Windows.Forms.Padding(4);
            this.brnGenXml.Name = "brnGenXml";
            this.brnGenXml.Size = new System.Drawing.Size(144, 81);
            this.brnGenXml.TabIndex = 29;
            this.brnGenXml.Text = "產生 XML";
            this.brnGenXml.UseVisualStyleBackColor = true;
            this.brnGenXml.Click += new System.EventHandler(this.brnGenXml_Click);
            // 
            // FrnSAqlToXml
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1310, 632);
            this.Controls.Add(this.brnGenXml);
            this.Controls.Add(this.txtDressing);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtSql);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.txtConnStr);
            this.Controls.Add(this.label2);
            this.Name = "FrnSAqlToXml";
            this.Text = "將 SQL 轉成 Xml";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox txtConnStr;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSql;
        private System.Windows.Forms.TextBox txtDressing;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button brnGenXml;
    }
}