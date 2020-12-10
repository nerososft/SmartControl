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
    public partial class About : Form
    {
        private SetVisiableHandler change;

        public About(SetVisiableHandler view)
        {
            InitializeComponent();
            this.change = view;
        }

        private void About_Load(object sender, EventArgs e)
        {
            label1.BackColor = Color.FromArgb(255, 59, 59, 59);
        }

        private Point mPoint = new Point();
       
        private void mm_down(object sender, MouseEventArgs e)
        {
            mPoint.X = e.X;
            mPoint.Y = e.Y;
        }
        private void mm_move(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point myPosittion = MousePosition;
                myPosittion.Offset(-mPoint.X, -mPoint.Y);
                Location = myPosittion;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
           
        }
        private void exit_P_o(object sender, EventArgs e)
        {
            pictureBox1.Image = global::WindowsFormsApp1.Properties.Resources.ggb;
        }
        private void exit_P_f(object sender, EventArgs e)
        {
            pictureBox1.Image = global::WindowsFormsApp1.Properties.Resources.gg;
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            if (change != null)
            {
                this.change();
            }
        }
    }
}
