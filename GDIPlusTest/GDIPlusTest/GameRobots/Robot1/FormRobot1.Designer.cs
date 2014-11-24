namespace GDIPlusTest.GameRobots.Robot1
{
    partial class FormRobot1
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
            this.button1 = new System.Windows.Forms.Button();
            this.tbxLogOutPut = new System.Windows.Forms.TextBox();
            this.btnKeyDown = new System.Windows.Forms.Button();
            this.btnKeyUp = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(214, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tbxLogOutPut
            // 
            this.tbxLogOutPut.Location = new System.Drawing.Point(12, 14);
            this.tbxLogOutPut.Multiline = true;
            this.tbxLogOutPut.Name = "tbxLogOutPut";
            this.tbxLogOutPut.ReadOnly = true;
            this.tbxLogOutPut.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbxLogOutPut.Size = new System.Drawing.Size(193, 320);
            this.tbxLogOutPut.TabIndex = 1;
            // 
            // btnKeyDown
            // 
            this.btnKeyDown.Location = new System.Drawing.Point(214, 70);
            this.btnKeyDown.Name = "btnKeyDown";
            this.btnKeyDown.Size = new System.Drawing.Size(75, 23);
            this.btnKeyDown.TabIndex = 2;
            this.btnKeyDown.Text = "keydown";
            this.btnKeyDown.UseVisualStyleBackColor = true;
            this.btnKeyDown.Click += new System.EventHandler(this.btnKeyDown_Click);
            // 
            // btnKeyUp
            // 
            this.btnKeyUp.Location = new System.Drawing.Point(214, 117);
            this.btnKeyUp.Name = "btnKeyUp";
            this.btnKeyUp.Size = new System.Drawing.Size(75, 23);
            this.btnKeyUp.TabIndex = 3;
            this.btnKeyUp.Text = "keyup";
            this.btnKeyUp.UseVisualStyleBackColor = true;
            this.btnKeyUp.Click += new System.EventHandler(this.btnKeyUp_Click);
            // 
            // FormGameRobot
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 353);
            this.Controls.Add(this.btnKeyUp);
            this.Controls.Add(this.btnKeyDown);
            this.Controls.Add(this.tbxLogOutPut);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FormGameRobot";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GameRobot";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tbxLogOutPut;
        private System.Windows.Forms.Button btnKeyDown;
        private System.Windows.Forms.Button btnKeyUp;
    }
}