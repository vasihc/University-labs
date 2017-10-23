using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Perceptron
{
    public partial class LearnForm : Form
    {
        public int Result;

        public LearnForm(string first, string second)
        {
            InitializeComponent(first, second);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Result = 0;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Result = 1;
            Close();
        }
    }
}
