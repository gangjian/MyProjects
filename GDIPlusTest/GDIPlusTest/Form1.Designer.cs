namespace GDIPlusTest
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.buttonOpen = new System.Windows.Forms.Button();
            this.buttonOpenSub = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnRobot1 = new System.Windows.Forms.Button();
            this.btnWebTest = new System.Windows.Forms.Button();
            this.buttonCapture = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(807, 509);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // buttonOpen
            // 
            this.buttonOpen.Location = new System.Drawing.Point(850, 9);
            this.buttonOpen.Margin = new System.Windows.Forms.Padding(2);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(80, 30);
            this.buttonOpen.TabIndex = 1;
            this.buttonOpen.Text = "OpenSrcImg";
            this.buttonOpen.UseVisualStyleBackColor = true;
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // buttonOpenSub
            // 
            this.buttonOpenSub.Location = new System.Drawing.Point(850, 57);
            this.buttonOpenSub.Margin = new System.Windows.Forms.Padding(2);
            this.buttonOpenSub.Name = "buttonOpenSub";
            this.buttonOpenSub.Size = new System.Drawing.Size(80, 30);
            this.buttonOpenSub.TabIndex = 2;
            this.buttonOpenSub.Text = "OpenSubImg";
            this.buttonOpenSub.UseVisualStyleBackColor = true;
            this.buttonOpenSub.Click += new System.EventHandler(this.buttonOpenSub_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnRobot1);
            this.panel1.Controls.Add(this.btnWebTest);
            this.panel1.Controls.Add(this.buttonCapture);
            this.panel1.Controls.Add(this.buttonStart);
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Controls.Add(this.buttonOpenSub);
            this.panel1.Controls.Add(this.buttonOpen);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(1, 2);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(939, 511);
            this.panel1.TabIndex = 0;
            // 
            // btnRobot1
            // 
            this.btnRobot1.Location = new System.Drawing.Point(850, 272);
            this.btnRobot1.Name = "btnRobot1";
            this.btnRobot1.Size = new System.Drawing.Size(80, 30);
            this.btnRobot1.TabIndex = 7;
            this.btnRobot1.Text = "Robot1";
            this.btnRobot1.UseVisualStyleBackColor = true;
            this.btnRobot1.Click += new System.EventHandler(this.btnRobot1_Click);
            // 
            // btnWebTest
            // 
            this.btnWebTest.Location = new System.Drawing.Point(850, 218);
            this.btnWebTest.Name = "btnWebTest";
            this.btnWebTest.Size = new System.Drawing.Size(80, 30);
            this.btnWebTest.TabIndex = 6;
            this.btnWebTest.Text = "WebTest";
            this.btnWebTest.UseVisualStyleBackColor = true;
            this.btnWebTest.Click += new System.EventHandler(this.btnWebTest_Click);
            // 
            // buttonCapture
            // 
            this.buttonCapture.Font = new System.Drawing.Font("SimHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonCapture.Location = new System.Drawing.Point(850, 164);
            this.buttonCapture.Margin = new System.Windows.Forms.Padding(2);
            this.buttonCapture.Name = "buttonCapture";
            this.buttonCapture.Size = new System.Drawing.Size(80, 30);
            this.buttonCapture.TabIndex = 5;
            this.buttonCapture.Text = "Capture";
            this.buttonCapture.UseVisualStyleBackColor = true;
            this.buttonCapture.Click += new System.EventHandler(this.buttonCapture_Click);
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(850, 112);
            this.buttonStart.Margin = new System.Windows.Forms.Padding(2);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(80, 30);
            this.buttonStart.TabIndex = 4;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(820, 435);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(110, 74);
            this.pictureBox2.TabIndex = 3;
            this.pictureBox2.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(942, 535);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button buttonOpen;
        private System.Windows.Forms.Button buttonOpenSub;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button buttonCapture;
        private System.Windows.Forms.Button btnWebTest;
        private System.Windows.Forms.Button btnRobot1;

    }
}

