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
    public partial class SelectForm : Form
    {
        public string FirstSymbol;
        public string SecondSymbol;
        public double LearningRate;

        public SelectForm()
        {
            InitializeComponent();
        }
        

        private void okButton_click(object sender, EventArgs e)
        {
            FirstSymbol = textBox1.Text;
            SecondSymbol = textBox2.Text;
            LearningRate = Convert.ToDouble(textBox3.Text);
            Close();
            Application.Run(new Form1(FirstSymbol, SecondSymbol, LearningRate));
        }

        private void SelectForm_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
