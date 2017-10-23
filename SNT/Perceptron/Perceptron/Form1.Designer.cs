namespace Perceptron
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.Recognize = new System.Windows.Forms.Button();
            this.Learn = new System.Windows.Forms.Button();
            this.Clear = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pictureBox.Location = new System.Drawing.Point(12, 12);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(100, 100);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // Recognize
            // 
            this.Recognize.BackColor = System.Drawing.Color.White;
            this.Recognize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Recognize.ForeColor = System.Drawing.Color.Black;
            this.Recognize.Location = new System.Drawing.Point(138, 12);
            this.Recognize.Name = "Recognize";
            this.Recognize.Size = new System.Drawing.Size(94, 47);
            this.Recognize.TabIndex = 1;
            this.Recognize.Text = "Recognize";
            this.Recognize.UseVisualStyleBackColor = false;
            this.Recognize.Click += new System.EventHandler(this.Recognize_Click);
            // 
            // Learn
            // 
            this.Learn.BackColor = System.Drawing.Color.White;
            this.Learn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Learn.ForeColor = System.Drawing.Color.DarkGreen;
            this.Learn.Location = new System.Drawing.Point(138, 65);
            this.Learn.Name = "Learn";
            this.Learn.Size = new System.Drawing.Size(94, 47);
            this.Learn.TabIndex = 2;
            this.Learn.Text = "Learn";
            this.Learn.UseVisualStyleBackColor = false;
            this.Learn.Click += new System.EventHandler(this.Learn_Click);
            // 
            // Clear
            // 
            this.Clear.BackColor = System.Drawing.Color.White;
            this.Clear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Clear.ForeColor = System.Drawing.Color.DarkRed;
            this.Clear.Location = new System.Drawing.Point(12, 118);
            this.Clear.Name = "Clear";
            this.Clear.Size = new System.Drawing.Size(100, 33);
            this.Clear.TabIndex = 3;
            this.Clear.Text = "Clear";
            this.Clear.UseVisualStyleBackColor = false;
            this.Clear.Click += new System.EventHandler(this.Clear_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.ClientSize = new System.Drawing.Size(248, 189);
            this.Controls.Add(this.Clear);
            this.Controls.Add(this.Learn);
            this.Controls.Add(this.Recognize);
            this.Controls.Add(this.pictureBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "First lab";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Button Recognize;
        private System.Windows.Forms.Button Learn;
        private System.Windows.Forms.Button Clear;
    }
}

