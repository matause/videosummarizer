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
            this.editMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutAboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displaySplitContainer = new System.Windows.Forms.SplitContainer();
            this.slimDXRenderTarget = new System.Windows.Forms.PictureBox();
            this.videoControlsLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.stopButton = new System.Windows.Forms.Button();
            this.playButton = new System.Windows.Forms.Button();
            this.mainMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.displaySplitContainer)).BeginInit();
            this.displaySplitContainer.Panel1.SuspendLayout();
            this.displaySplitContainer.Panel2.SuspendLayout();
            this.displaySplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.slimDXRenderTarget)).BeginInit();
            this.videoControlsLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem,
            this.editMenuItem,
            this.aboutMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(624, 24);
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
            // editMenuItem
            // 
            this.editMenuItem.Name = "editMenuItem";
            this.editMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editMenuItem.Text = "Edit";
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
            // 
            // displaySplitContainer.Panel1
            // 
            this.displaySplitContainer.Panel1.Controls.Add(this.slimDXRenderTarget);
            // 
            // displaySplitContainer.Panel2
            // 
            this.displaySplitContainer.Panel2.Controls.Add(this.videoControlsLayout);
            this.displaySplitContainer.Size = new System.Drawing.Size(624, 418);
            this.displaySplitContainer.SplitterDistance = 208;
            this.displaySplitContainer.TabIndex = 1;
            // 
            // slimDXRenderTarget
            // 
            this.slimDXRenderTarget.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.slimDXRenderTarget.Dock = System.Windows.Forms.DockStyle.Fill;
            this.slimDXRenderTarget.Location = new System.Drawing.Point(0, 0);
            this.slimDXRenderTarget.Name = "slimDXRenderTarget";
            this.slimDXRenderTarget.Size = new System.Drawing.Size(624, 208);
            this.slimDXRenderTarget.TabIndex = 0;
            this.slimDXRenderTarget.TabStop = false;
            // 
            // videoControlsLayout
            // 
            this.videoControlsLayout.Controls.Add(this.playButton);
            this.videoControlsLayout.Controls.Add(this.stopButton);
            this.videoControlsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.videoControlsLayout.Location = new System.Drawing.Point(0, 0);
            this.videoControlsLayout.Name = "videoControlsLayout";
            this.videoControlsLayout.Size = new System.Drawing.Size(624, 206);
            this.videoControlsLayout.TabIndex = 0;
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(84, 3);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 3;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            // 
            // playButton
            // 
            this.playButton.Location = new System.Drawing.Point(3, 3);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(75, 23);
            this.playButton.TabIndex = 2;
            this.playButton.Text = "Play";
            this.playButton.UseVisualStyleBackColor = true;
            // 
            // VPMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 442);
            this.Controls.Add(this.displaySplitContainer);
            this.Controls.Add(this.mainMenuStrip);
            this.MainMenuStrip = this.mainMenuStrip;
            this.Name = "VPMainForm";
            this.Text = "Video Summarizer";
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.displaySplitContainer.Panel1.ResumeLayout(false);
            this.displaySplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.displaySplitContainer)).EndInit();
            this.displaySplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.slimDXRenderTarget)).EndInit();
            this.videoControlsLayout.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileOpenMenuItem;
        private System.Windows.Forms.ToolStripSeparator fileOpenSeperator;
        private System.Windows.Forms.ToolStripMenuItem fileCloseMenuItem;
        private System.Windows.Forms.ToolStripSeparator fileCloseSeperator;
        private System.Windows.Forms.ToolStripMenuItem fileExitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutAboutMenuItem;
        private System.Windows.Forms.SplitContainer displaySplitContainer;
        private System.Windows.Forms.PictureBox slimDXRenderTarget;
        private System.Windows.Forms.FlowLayoutPanel videoControlsLayout;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button playButton;
    }
}

