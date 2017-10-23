using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;


namespace Perceptron
{
    public partial class Form1 : Form
    {
        public Form1(string first, string second, double rate)
        {
            InitializeComponent();
            _learningRate = rate;
            _firstSymbol = first;
            _secondSymbol = second;
        }

        private const int _splitValue = 5;
        private bool _isMouseDown;
        private Pen _pen;
        private Point _fromPoint;
        private Point _toPoint;
        private Bitmap _bitMap;
        private int[] _cells;
        private double[] _weights;
        private int _rowCellsCount;
        private int _columnCellsCount;
        private int _cellsCount;
        private int _result;
        private double _learningRate;
        private string _firstSymbol;
        private string _secondSymbol;

        private Dictionary<Point, Color> _pixels;

        private void Form1_Load(object sender, EventArgs e)
        {
            _pen = new Pen(Color.Black, 2.0f);
            _bitMap = new Bitmap(pictureBox.Size.Width, pictureBox.Size.Height);
            _pixels = new Dictionary<Point, Color>();
            pictureBox.Image = _bitMap;
            pictureBox.MouseDown += PictureBox_MouseDown;
            pictureBox.MouseMove += PictureBox_MouseMove;
            pictureBox.MouseUp += PictureBox_MouseUp;
            _rowCellsCount = pictureBox.Size.Width / _splitValue;
            _columnCellsCount = pictureBox.Size.Height / _splitValue;
            _cellsCount = _rowCellsCount * _columnCellsCount;
            _cells = new int[_cellsCount];
            _weights = new double[_cellsCount + 1];
            var random = new Random();
            for (int i = 0; i < _cellsCount + 1; i++)
            {
                _weights[i] = random.NextDouble() * (_learningRate - (-_learningRate)) + (-_learningRate);
            }
        }

        private int ActivationFunction()
        {
            double sum = 0;
            sum += _weights[0];
            for (int i = 1; i < _cellsCount+1; i++)
            {
                sum += _cells[i-1] * _weights[i];
            }

            return sum >= 0 ? 1 : 0;
        }

        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            _isMouseDown = false;
            _pixels.Clear();

            for (int i = 0; i < _bitMap.Width; i++)
                for (int j = 0; j < _bitMap.Height; j++)
                {
                    var color = _bitMap.GetPixel(i, j);
                    if (color.A == 255 && color.R == 0 && color.G == 0 && color.B == 0)
                    {
                        _pixels.Add(new Point(i, j), color);
                    }
                }

            if (_pixels.Count == 0)
            {
                return;
            }
            var leftTop = new Point(_pixels.Min(p => p.Key.X), _pixels.Min(p => p.Key.Y));
            var rightBottom = new Point(_pixels.Max(p => p.Key.X), _pixels.Max(p => p.Key.Y));

            var boundBitmap = new Bitmap(rightBottom.X - leftTop.X + 1, rightBottom.Y - leftTop.Y + 1);
            foreach (var pixel in _pixels)
            {
                boundBitmap.SetPixel(pixel.Key.X - leftTop.X, pixel.Key.Y - leftTop.Y, pixel.Value);
            }

            var stretchX = boundBitmap.Width / (float)pictureBox.Width;
            var stretchY = boundBitmap.Height / (float)pictureBox.Height;

            var stretchedBitmap = new Bitmap(100, 100);

            for (int i = 0; i < 100; i++)
                for (int j = 0; j < 100; j++)
                {
                    var color = boundBitmap.GetPixel((int)(i * stretchX), (int)(j * stretchY));
                    stretchedBitmap.SetPixel(i, j, color);
                }

            var cellValues = new int[_rowCellsCount * _rowCellsCount];

            for (int i = 0; i < stretchedBitmap.Width; i++)
            {
                for (int j = 0; j < stretchedBitmap.Height; j++)
                {
                    var color = stretchedBitmap.GetPixel(i, j);
                    if (color.A == 255 && color.R == 0 && color.G == 0 && color.B == 0)
                    {
                        cellValues[(i / _splitValue) * _rowCellsCount + j / _splitValue]++;
                    }
                }
            }

            for (int i = 0; i < _rowCellsCount * _rowCellsCount; i++)
            {
                if (cellValues[i] > 5)
                {
                    _cells[i] = 1;
                }
            }
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                _toPoint = new Point(e.X, e.Y);
                {
                    DrawCurrent();
                }
                _fromPoint = _toPoint;
            }
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            _isMouseDown = true;
            _fromPoint = new Point(e.X, e.Y);
        }

        private void DrawCurrent()
        {
            var graphics = Graphics.FromImage(_bitMap);
            graphics.DrawLine(_pen, _fromPoint, _toPoint);
            pictureBox.Image = _bitMap;
        }

        private void Learn_Click(object sender, EventArgs e)
        {
            LearnForm learnForm = new LearnForm(_firstSymbol, _secondSymbol);
            learnForm.ShowDialog();
            Recalculate(learnForm.Result);
        }

        private void Recalculate(int result)
        {
            var delta = result - _result;
            _weights[0] = _weights[0] + _learningRate * delta;
            for (int i = 1; i < _cellsCount + 1; i++)
            {
                _weights[i] = _weights[i] + _learningRate * delta * _cells[i - 1];
            }
        }

        private void Recognize_Click(object sender, EventArgs e)
        {
            _result = ActivationFunction();
            var value = _result == 0 ? _firstSymbol : _secondSymbol;
            MessageBox.Show(value);
            
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < _cellsCount; i++)
            {
                _cells[i] = 0;
            }
            _bitMap = new Bitmap(pictureBox.Width, pictureBox.Height);
            pictureBox.Image = _bitMap;
        }
    }
}
