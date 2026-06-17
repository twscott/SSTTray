
namespace TaskTrayApplication
{
    partial class Portal大宗費用申請
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Portal大宗費用申請));
            this.comboStandardLabel = new System.Windows.Forms.ComboBox();
            this.txtLabelData = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboStandardLabel
            // 
            this.comboStandardLabel.Font = new System.Drawing.Font("標楷體", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboStandardLabel.FormattingEnabled = true;
            this.comboStandardLabel.Location = new System.Drawing.Point(207, 7);
            this.comboStandardLabel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboStandardLabel.Name = "comboStandardLabel";
            this.comboStandardLabel.Size = new System.Drawing.Size(239, 31);
            this.comboStandardLabel.TabIndex = 36;
            // 
            // txtLabelData
            // 
            this.txtLabelData.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtLabelData.Location = new System.Drawing.Point(297, 275);
            this.txtLabelData.Margin = new System.Windows.Forms.Padding(4);
            this.txtLabelData.Multiline = true;
            this.txtLabelData.Name = "txtLabelData";
            this.txtLabelData.Size = new System.Drawing.Size(747, 114);
            this.txtLabelData.TabIndex = 35;
            this.txtLabelData.Text = resources.GetString("txtLabelData.Text");
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.label8.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label8.Location = new System.Drawing.Point(293, 247);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(130, 24);
            this.label8.TabIndex = 34;
            this.label8.Text = "Label Data";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Gainsboro;
            this.label6.Font = new System.Drawing.Font("標楷體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label6.Location = new System.Drawing.Point(12, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(190, 24);
            this.label6.TabIndex = 33;
            this.label6.Text = "標籤母版檔案 : ";
            // 
            // Portal大宗費用申請
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1336, 450);
            this.Controls.Add(this.comboStandardLabel);
            this.Controls.Add(this.txtLabelData);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Name = "Portal大宗費用申請";
            this.Text = "Portal大宗費用申請";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboStandardLabel;
        private System.Windows.Forms.TextBox txtLabelData;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
    }
}