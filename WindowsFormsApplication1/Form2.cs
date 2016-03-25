using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        private string provinces="";
        private string provinceIDs = "";
        public string getProvinces()
        {
            return this.provinces;
        }
        public string getProvinceIDs()
        {
            return this.provinceIDs;
        }
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            provinceIDs = "110";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int count = 0;
            foreach (Control c in groupBox1.Controls)
            {
                if (c is CheckBox)
                {
                    if ((c as CheckBox).Checked == true)
                    {
                        provinces = provinces + c.Text + ",";
                        provinceIDs = provinceIDs + c.Tag + ",";
                        count++;
                    }
                }
            }
            if (count > 10)
            {
                MessageBox.Show("所选省份不能超过十个！");
                return; 
            }
            if (count == 0)
            {
                MessageBox.Show("尚未选择省份！");
                return;
            }
            provinces = provinces.Substring(0, provinces.Length - 1);
            provinceIDs = provinceIDs.Substring(0, provinceIDs.Length - 1);
            this.DialogResult = DialogResult.OK;
        }
    }
}
