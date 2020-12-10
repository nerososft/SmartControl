using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Debug : Form
    {
        private SetVisiableHandler change;
        private DataSendHandler sender;

        public Debug(SetVisiableHandler view,DataSendHandler send)
        {
            InitializeComponent();
            change = view;
            sender = send;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (this.change!=null) {
                this.change();
                this.Hide();
            }
        }

        private void Debug_Load(object sender, EventArgs e)
        {
            richTextBox1.BackColor = Color.FromArgb(255, 59,59, 59);
          
           
        }
        private void exit_P_o(object sender, EventArgs e)
        {
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
        }
        private void exit_P_f(object sender, EventArgs e)
        {
            pictureBox1.BorderStyle = BorderStyle.None;
        }

        private void pictureBox3_mousein(object sender, EventArgs e)
        {
            pictureBox3.Image = global::WindowsFormsApp1.Properties.Resources.send2;
        }
        private void pictureBox3_leave(object sender, EventArgs e)
        {
            pictureBox3.Image = global::WindowsFormsApp1.Properties.Resources.sned1;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (sender!=null) {
                this.sender(richTextBox1.Text);
            }
        }
    }
}
