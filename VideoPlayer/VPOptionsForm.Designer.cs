namespace VideoPlayer
{
    partial class VPOptionsForm
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
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.sceneDurationLabel = new System.Windows.Forms.Label();
            this.shotGenerationLabel = new System.Windows.Forms.Label();
            this.keyFrameLabel = new System.Windows.Forms.Label();
            this.summaryDuration = new System.Windows.Forms.Label();
            this.colorThresholdingButton = new System.Windows.Forms.RadioButton();
            this.audioExcitationButton = new System.Windows.Forms.RadioButton();
            this.shotGenerationGroupBox = new System.Windows.Forms.GroupBox();
            this.keyFrameGroupBox = new System.Windows.Forms.GroupBox();
            this.summaryTrackBar = new System.Windows.Forms.TrackBar();
            this.sceneTrackBar = new System.Windows.Forms.TrackBar();
            this.motionAnalysisButton = new System.Windows.Forms.RadioButton();
            this.shotGenerationGroupBox.SuspendLayout();
            this.keyFrameGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.summaryTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sceneTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.BackColor = System.Drawing.Color.Peru;
            this.okButton.Location = new System.Drawing.Point(118, 278);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = false;
            // 
            // cancelButton
            // 
            this.cancelButton.BackColor = System.Drawing.Color.Peru;
            this.cancelButton.Location = new System.Drawing.Point(197, 278);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = false;
            // 
            // sceneDurationLabel
            // 
            this.sceneDurationLabel.AutoSize = true;
            this.sceneDurationLabel.Location = new System.Drawing.Point(9, 162);
            this.sceneDurationLabel.Name = "sceneDurationLabel";
            this.sceneDurationLabel.Size = new System.Drawing.Size(124, 13);
            this.sceneDurationLabel.TabIndex = 2;
            this.sceneDurationLabel.Text = "Scene Duration: 3 (secs)";
            // 
            // shotGenerationLabel
            // 
            this.shotGenerationLabel.AutoSize = true;
            this.shotGenerationLabel.BackColor = System.Drawing.Color.LightSeaGreen;
            this.shotGenerationLabel.Location = new System.Drawing.Point(77, 9);
            this.shotGenerationLabel.Name = "shotGenerationLabel";
            this.shotGenerationLabel.Size = new System.Drawing.Size(135, 13);
            this.shotGenerationLabel.TabIndex = 3;
            this.shotGenerationLabel.Text = "Shot Generation Algorithms";
            // 
            // keyFrameLabel
            // 
            this.keyFrameLabel.AutoSize = true;
            this.keyFrameLabel.BackColor = System.Drawing.Color.LightSeaGreen;
            this.keyFrameLabel.Location = new System.Drawing.Point(88, 94);
            this.keyFrameLabel.Name = "keyFrameLabel";
            this.keyFrameLabel.Size = new System.Drawing.Size(108, 13);
            this.keyFrameLabel.TabIndex = 4;
            this.keyFrameLabel.Text = "Key Frame Algorithms";
            // 
            // summaryDuration
            // 
            this.summaryDuration.AutoSize = true;
            this.summaryDuration.Location = new System.Drawing.Point(9, 220);
            this.summaryDuration.Name = "summaryDuration";
            this.summaryDuration.Size = new System.Drawing.Size(119, 13);
            this.summaryDuration.TabIndex = 5;
            this.summaryDuration.Text = "Summary Duration: 50%";
            // 
            // colorThresholdingButton
            // 
            this.colorThresholdingButton.AutoSize = true;
            this.colorThresholdingButton.BackColor = System.Drawing.Color.Peru;
            this.colorThresholdingButton.Checked = true;
            this.colorThresholdingButton.Location = new System.Drawing.Point(6, 12);
            this.colorThresholdingButton.Name = "colorThresholdingButton";
            this.colorThresholdingButton.Size = new System.Drawing.Size(163, 17);
            this.colorThresholdingButton.TabIndex = 6;
            this.colorThresholdingButton.TabStop = true;
            this.colorThresholdingButton.Text = "Color Histogram Thresholding";
            this.colorThresholdingButton.UseVisualStyleBackColor = false;
            // 
            // audioExcitationButton
            // 
            this.audioExcitationButton.AutoSize = true;
            this.audioExcitationButton.BackColor = System.Drawing.Color.Peru;
            this.audioExcitationButton.Checked = true;
            this.audioExcitationButton.Location = new System.Drawing.Point(6, 13);
            this.audioExcitationButton.Name = "audioExcitationButton";
            this.audioExcitationButton.Size = new System.Drawing.Size(101, 17);
            this.audioExcitationButton.TabIndex = 7;
            this.audioExcitationButton.TabStop = true;
            this.audioExcitationButton.Text = "Audio Excitation";
            this.audioExcitationButton.UseVisualStyleBackColor = false;
            // 
            // shotGenerationGroupBox
            // 
            this.shotGenerationGroupBox.Controls.Add(this.motionAnalysisButton);
            this.shotGenerationGroupBox.Controls.Add(this.colorThresholdingButton);
            this.shotGenerationGroupBox.Location = new System.Drawing.Point(12, 24);
            this.shotGenerationGroupBox.Name = "shotGenerationGroupBox";
            this.shotGenerationGroupBox.Size = new System.Drawing.Size(260, 59);
            this.shotGenerationGroupBox.TabIndex = 16;
            this.shotGenerationGroupBox.TabStop = false;
            // 
            // keyFrameGroupBox
            // 
            this.keyFrameGroupBox.Controls.Add(this.audioExcitationButton);
            this.keyFrameGroupBox.Location = new System.Drawing.Point(12, 110);
            this.keyFrameGroupBox.Name = "keyFrameGroupBox";
            this.keyFrameGroupBox.Size = new System.Drawing.Size(260, 37);
            this.keyFrameGroupBox.TabIndex = 17;
            this.keyFrameGroupBox.TabStop = false;
            // 
            // summaryTrackBar
            // 
            this.summaryTrackBar.LargeChange = 10;
            this.summaryTrackBar.Location = new System.Drawing.Point(12, 237);
            this.summaryTrackBar.Maximum = 100;
            this.summaryTrackBar.Name = "summaryTrackBar";
            this.summaryTrackBar.Size = new System.Drawing.Size(260, 45);
            this.summaryTrackBar.TabIndex = 18;
            this.summaryTrackBar.TabStop = false;
            this.summaryTrackBar.TickFrequency = 10;
            this.summaryTrackBar.Value = 50;
            // 
            // sceneTrackBar
            // 
            this.sceneTrackBar.LargeChange = 2;
            this.sceneTrackBar.Location = new System.Drawing.Point(12, 179);
            this.sceneTrackBar.Maximum = 8;
            this.sceneTrackBar.Minimum = 1;
            this.sceneTrackBar.Name = "sceneTrackBar";
            this.sceneTrackBar.Size = new System.Drawing.Size(260, 45);
            this.sceneTrackBar.TabIndex = 19;
            this.sceneTrackBar.TabStop = false;
            this.sceneTrackBar.Value = 3;
            // 
            // motionAnalysisButton
            // 
            this.motionAnalysisButton.AutoSize = true;
            this.motionAnalysisButton.BackColor = System.Drawing.Color.Peru;
            this.motionAnalysisButton.Location = new System.Drawing.Point(6, 35);
            this.motionAnalysisButton.Name = "motionAnalysisButton";
            this.motionAnalysisButton.Size = new System.Drawing.Size(98, 17);
            this.motionAnalysisButton.TabIndex = 9;
            this.motionAnalysisButton.TabStop = true;
            this.motionAnalysisButton.Text = "Motion Analysis";
            this.motionAnalysisButton.UseVisualStyleBackColor = false;
            // 
            // VPOptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSeaGreen;
            this.ClientSize = new System.Drawing.Size(284, 313);
            this.Controls.Add(this.keyFrameGroupBox);
            this.Controls.Add(this.summaryDuration);
            this.Controls.Add(this.keyFrameLabel);
            this.Controls.Add(this.shotGenerationLabel);
            this.Controls.Add(this.sceneDurationLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.shotGenerationGroupBox);
            this.Controls.Add(this.summaryTrackBar);
            this.Controls.Add(this.sceneTrackBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VPOptionsForm";
            this.Text = "Summary Options";
            this.shotGenerationGroupBox.ResumeLayout(false);
            this.shotGenerationGroupBox.PerformLayout();
            this.keyFrameGroupBox.ResumeLayout(false);
            this.keyFrameGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.summaryTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sceneTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label sceneDurationLabel;
        private System.Windows.Forms.Label shotGenerationLabel;
        private System.Windows.Forms.Label keyFrameLabel;
        private System.Windows.Forms.Label summaryDuration;
        private System.Windows.Forms.RadioButton colorThresholdingButton;
        private System.Windows.Forms.RadioButton audioExcitationButton;
        private System.Windows.Forms.GroupBox shotGenerationGroupBox;
        private System.Windows.Forms.GroupBox keyFrameGroupBox;
        private System.Windows.Forms.TrackBar summaryTrackBar;
        private System.Windows.Forms.TrackBar sceneTrackBar;
        private System.Windows.Forms.RadioButton motionAnalysisButton;
    }
}