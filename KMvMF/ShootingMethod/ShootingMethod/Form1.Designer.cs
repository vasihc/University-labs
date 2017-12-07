namespace ShootingMethod
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.dgEnergy = new System.Windows.Forms.DataGridView();
            this.dgEigenvalues = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgEnergy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgEigenvalues)).BeginInit();
            this.SuspendLayout();
            // 
            // chart1
            // 
            chartArea3.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea3);
            legend3.Name = "Legend1";
            this.chart1.Legends.Add(legend3);
            this.chart1.Location = new System.Drawing.Point(377, 305);
            this.chart1.Name = "chart1";
            series3.ChartArea = "ChartArea1";
            series3.Legend = "Legend1";
            series3.Name = "Series1";
            this.chart1.Series.Add(series3);
            this.chart1.Size = new System.Drawing.Size(585, 388);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // dgEnergy
            // 
            this.dgEnergy.AllowUserToAddRows = false;
            this.dgEnergy.AllowUserToDeleteRows = false;
            this.dgEnergy.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dgEnergy.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgEnergy.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgEnergy.Location = new System.Drawing.Point(12, 12);
            this.dgEnergy.Name = "dgEnergy";
            this.dgEnergy.Size = new System.Drawing.Size(359, 681);
            this.dgEnergy.TabIndex = 1;
            // 
            // dgEigenvalues
            // 
            this.dgEigenvalues.AllowUserToAddRows = false;
            this.dgEigenvalues.AllowUserToDeleteRows = false;
            this.dgEigenvalues.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dgEigenvalues.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgEigenvalues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgEigenvalues.Location = new System.Drawing.Point(377, 12);
            this.dgEigenvalues.Name = "dgEigenvalues";
            this.dgEigenvalues.Size = new System.Drawing.Size(799, 266);
            this.dgEigenvalues.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(1188, 705);
            this.Controls.Add(this.dgEigenvalues);
            this.Controls.Add(this.dgEnergy);
            this.Controls.Add(this.chart1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgEnergy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgEigenvalues)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.DataGridView dgEnergy;
        private System.Windows.Forms.DataGridView dgEigenvalues;
    }
}

