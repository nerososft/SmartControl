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
    public partial class StreamView : Form
    {
        private SetVisiableHandler change;
        public StreamView(SetVisiableHandler view)
        {
            InitializeComponent();
            this.change = view;
        }

        private void StreamView_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(255, 59, 59, 59);
            pictureBox2.Left = this.Width - pictureBox2.Width - 10;
            richTextBox1.Height = this.Height / 5;
            richTextBox1.BackColor = Color.FromArgb(255, 59, 59, 59);
            richTextBox2.Height = this.Height / 6;
            richTextBox2.BackColor = Color.FromArgb(255, 59, 59, 59);
            richTextBox3.Height = this.Height / 6;
            richTextBox3.BackColor = Color.FromArgb(255, 59, 59, 59);
            remote_data.Height = this.Height / 5;
            remote_data.BackColor = Color.FromArgb(255, 59, 59, 59);
            richTextBox1.Width = this.Width - 20;
            richTextBox2.Width = this.Width - 20;
            richTextBox3.Width = this.Width - 20;
            remote_data.Width = this.Width - 20;
            richTextBox1.Left =  10;
            richTextBox2.Left = 10;

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (this.change != null)
            {
                this.change();
                this.Hide();
            }
        }
        private void exit_P_o(object sender, EventArgs e)
        {
            pictureBox2.BorderStyle = BorderStyle.FixedSingle;
        }
        private void exit_P_f(object sender, EventArgs e)
        {
            pictureBox2.BorderStyle = BorderStyle.None;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
