using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FirstOhm;

namespace TaskTrayApplication
{
    public partial class FormWriteToExcel : Form
    {
        public FormWriteToExcel()
        {
            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            try
            {
                switch (comboTargetData.Text)
                {
                    case "請假資料":
                        string sourceTemplate = Constants.getProperty("PortalExcelTemplate");
                        string destPath = Constants.getProperty("PortalExcel");
                        CommonApp.writeToExcelFromDB(sourceTemplate, destPath);
                        break;
                }
            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "請假資料", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void FormWriteToExcel_Load(object sender, EventArgs e)
        {

        }
    }
}
