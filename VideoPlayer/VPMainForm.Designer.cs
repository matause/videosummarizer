namespace VideoPlayer
{
    partial class VPMainForm
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
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileOpenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileOpenSeperator = new System.Windows.Forms.ToolStripSeparator();
            this.fileCloseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileCloseSeperator = new System.Windows.Forms.ToolStripSeparator();
            this.fileExitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutAboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displaySplitContainer = new System.Windows.Forms.SplitContainer();
            this.playShotsButton = new System.Windows.Forms.Button();
            this.summarizeButton = new System.Windows.Forms.Button();
            this.playButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.timelineBar = new System.Windows.Forms.TrackBar();
            this.mainMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.displaySplitContainer)).BeginInit();
            this.displaySplitContainer.Panel2.SuspendLayout();
            this.displaySplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timelineBar)).BeginInit();
            this.SuspendLayout();
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem,
            this.aboutMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(422, 24);
            this.mainMenuStrip.TabIndex = 0;
            this.mainMenuStrip.Text = "mainMenuStrip";
            // 
            // fileMenuItem
            // 
            this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileOpenMenuItem,
            this.fileOpenSeperator,
            this.fileCloseMenuItem,
            this.fileCloseSeperator,
            this.fileExitMenuItem});
            this.fileMenuItem.Name = "fileMenuItem";
            this.fileMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileMenuItem.Text = "File";
            // 
            // fileOpenMenuItem
            // 
            this.fileOpenMenuItem.Name = "fileOpenMenuItem";
            this.fileOpenMenuItem.Size = new System.Drawing.Size(112, 22);
            this.fileOpenMenuItem.Text = "Open...";
            // 
            // fileOpenSeperator
            // 
            this.fileOpenSeperator.Name = "fileOpenSeperator";
            this.fileOpenSeperator.Size = new System.Drawing.Size(109, 6);
            // 
            // fileCloseMenuItem
            // 
            this.fileCloseMenuItem.Name = "fileCloseMenuItem";
            this.fileCloseMenuItem.Size = new System.Drawing.Size(112, 22);
            this.fileCloseMenuItem.Text = "Close";
            // 
            // fileCloseSeperator
            // 
            this.fileCloseSeperator.Name = "fileCloseSeperator";
            this.fileCloseSeperator.Size = new System.Drawing.Size(109, 6);
            // 
            // fileExitMenuItem
            // 
            this.fileExitMenuItem.Name = "fileExitMenuItem";
            this.fileExitMenuItem.Size = new System.Drawing.Size(112, 22);
            this.fileExitMenuItem.Text = "Exit";
            // 
            // aboutMenuItem
            // 
            this.aboutMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutAboutMenuItem});
            this.aboutMenuItem.Name = "aboutMenuItem";
            this.aboutMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutMenuItem.Text = "About";
            // 
            // aboutAboutMenuItem
            // 
            this.aboutAboutMenuItem.Name = "aboutAboutMenuItem";
            this.aboutAboutMenuItem.Size = new System.Drawing.Size(116, 22);
            this.aboutAboutMenuItem.Text = "About...";
            // 
            // displaySplitContainer
            // 
            this.displaySplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.displaySplitContainer.Location = new System.Drawing.Point(0, 24);
            this.displaySplitContainer.Name = "displaySplitContainer";
            this.displaySplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.displaySplitContainer.Panel1MinSize = 275;
            // 
            // displaySplitContainer.Panel2
            // 
            this.displaySplitContainer.Panel2.Controls.Add(this.playShotsButton);
            this.displaySplitContainer.Panel2.Controls.Add(this.summarizeButton);
            this.displaySplitContainer.Panel2.Controls.Add(this.playButton);
            this.displaySplitContainer.Panel2.Controls.Add(this.stopButton);
            this.displaySplitContainer.Panel2.Controls.Add(this.timelineBar);
            this.displaySplitContainer.Panel2MinSize = 59;
            this.displaySplitContainer.Size = new System.Drawing.Size(422, 338);
            this.displaySplitContainer.SplitterDistance = 275;
            this.displaySplitContainer.TabIndex = 1;
            this.displaySplitContainer.TabStop = false;
            // 
            // playShotsButton
            // 
            this.playShotsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.playShotsButton.Location = new System.Drawing.Point(255, 28);
            this.playShotsButton.Name = "playShotsButton";
            this.playShotsButton.Size = new System.Drawing.Size(75, 23);
            this.playShotsButton.TabIndex = 7;
            this.playShotsButton.Text = "Play Shots";
            this.playShotsButton.UseVisualStyleBackColor = true;
            // 
            // summarizeButton
            // 
            this.summarizeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.summarizeButton.Location = new System.Drawing.Point(174, 28);
            this.summarizeButton.Name = "summarizeButton";
            this.summarizeButton.Size = new System.Drawing.Size(75, 23);
            this.summarizeButton.TabIndex = 6;
            this.summarizeButton.Text = "Summarize";
            this.summarizeButton.UseVisualStyleBackColor = true;
            // 
            // playButton
            // 
            this.playButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.playButton.Location = new System.Drawing.Point(12, 29);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(75, 23);
            this.playButton.TabIndex = 4;
            this.playButton.Text = "Play";
            this.playButton.UseVisualStyleBackColor = true;
            // 
            // stopButton
            // 
            this.stopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.stopButton.Location = new System.Drawing.Point(93, 28);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 5;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            // 
            // timelineBar
            // 
            this.timelineBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.timelineBar.Location = new System.Drawing.Point(3, 2);
            this.timelineBar.Maximum = 100;
            this.timelineBar.Name = "timelineBar";
            this.timelineBar.Size = new System.Drawing.Size(416, 45);
            this.timelineBar.TabIndex = 7;
            this.timelineBar.TabStop = false;
            this.timelineBar.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // VPMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(422, 362);
            this.Controls.Add(this.displaySplitContainer);
            this.Controls.Add(this.mainMenuStrip);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.mainMenuStrip;
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "VPMainForm";
            this.Text = "Video Summarizer";
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.displaySplitContainer.Panel2.ResumeLayout(false);
            this.displaySplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.displaySplitContainer)).EndInit();
            this.displaySplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.timelineBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileOpenMenuItem;
        private System.Windows.Forms.ToolStripSeparator fileOpenSeperator;
        private System.Windows.Forms.ToolStripMenuItem fileCloseMenuItem;
        private System.Windows.Forms.ToolStripSeparator fileCloseSeperator;
        private System.Windows.Forms.ToolStripMenuItem fileExitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutAboutMenuItem;
        private System.Windows.Forms.SplitContainer displaySplitContainer;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button summarizeButton;
        private System.Windows.Forms.TrackBar timelineBar;
        private System.Windows.Forms.Button playShotsButton;
    }
}

