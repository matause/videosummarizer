namespace VideoPlayer
{
    partial class VPAboutForm
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
            this.aboutTitle = new System.Windows.Forms.Label();
            this.aboutAuthors = new System.Windows.Forms.Label();
            this.aboutJames = new System.Windows.Forms.Label();
            this.aboutVishak = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // aboutTitle
            // 
            this.aboutTitle.AutoSize = true;
            this.aboutTitle.Location = new System.Drawing.Point(51, 9);
            this.aboutTitle.Name = "aboutTitle";
            this.aboutTitle.Size = new System.Drawing.Size(103, 13);
            this.aboutTitle.TabIndex = 0;
            this.aboutTitle.Text = "CS 576 Final Project";
            // 
            // aboutAuthors
            // 
            this.aboutAuthors.AutoSize = true;
            this.aboutAuthors.Location = new System.Drawing.Point(81, 38);
            this.aboutAuthors.Name = "aboutAuthors";
            this.aboutAuthors.Size = new System.Drawing.Size(43, 13);
            this.aboutAuthors.TabIndex = 1;
            this.aboutAuthors.Text = "Authors";
            // 
            // aboutJames
            // 
            this.aboutJames.AutoSize = true;
            this.aboutJames.Location = new System.Drawing.Point(60, 62);
            this.aboutJames.Name = "aboutJames";
            this.aboutJames.Size = new System.Drawing.Size(84, 13);
            this.aboutJames.TabIndex = 2;
            this.aboutJames.Text = "James Lammlein";
            // 
            // aboutVishak
            // 
            this.aboutVishak.AutoSize = true;
            this.aboutVishak.Location = new System.Drawing.Point(65, 80);
            this.aboutVishak.Name = "aboutVishak";
            this.aboutVishak.Size = new System.Drawing.Size(75, 13);
            this.aboutVishak.TabIndex = 3;
            this.aboutVishak.Text = "Vishak Nag. A";
            // 
            // okButton
            // 
            this.okButton.BackColor = System.Drawing.Color.Peru;
            this.okButton.Location = new System.Drawing.Point(65, 107);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = false;
            // 
            // VPAboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSeaGreen;
            this.ClientSize = new System.Drawing.Size(205, 141);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.aboutVishak);
            this.Controls.Add(this.aboutJames);
            this.Controls.Add(this.aboutAuthors);
            this.Controls.Add(this.aboutTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(211, 169);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(211, 169);
            this.Name = "VPAboutForm";
            this.Text = "About...";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label aboutTitle;
        private System.Windows.Forms.Label aboutAuthors;
        private System.Windows.Forms.Label aboutJames;
        private System.Windows.Forms.Label aboutVishak;
        private System.Windows.Forms.Button okButton;
    }
}