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
    public partial class SetForm : Form
    {
        private string FirstSymbol;
        private string SecondSymbol;
        private double LearningRate;

        public SetForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                FirstSymbol = textBox1.Text;
                SecondSymbol = textBox2.Text;
                LearningRate = Convert.ToDouble(textBox3.Text);
            }
            catch 
            {
                Console.WriteLine("Error");
            }

            Form1 form = new Form1(FirstSymbol, SecondSymbol, LearningRate);
            form.Show();
            this.Hide();
        }
    }
}
