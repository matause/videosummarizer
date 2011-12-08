namespace VideoPlayer
{
    partial class VPHistogramForm
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
            this.histogramControl = new VideoPlayer.HistogramControl();
            this.SuspendLayout();
            // 
            // histogramControl
            // 
            this.histogramControl.DisplayColor = System.Drawing.Color.Black;
            this.histogramControl.Location = new System.Drawing.Point(13, 13);
            this.histogramControl.Name = "histogramControl";
            this.histogramControl.Offset = 20;
            this.histogramControl.Size = new System.Drawing.Size(259, 237);
            this.histogramControl.TabIndex = 0;
            // 
            // VPHistogramForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSeaGreen;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.ControlBox = false;
            this.Controls.Add(this.histogramControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VPHistogramForm";
            this.ShowInTaskbar = false;
            this.Text = "Histogram";
            this.ResumeLayout(false);

        }

        #endregion

        private HistogramControl histogramControl;
    }
}