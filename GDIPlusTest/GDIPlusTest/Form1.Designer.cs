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
            this.pbxSrcImg = new System.Windows.Forms.PictureBox();
            this.btnOpenSrc = new System.Windows.Forms.Button();
            this.btnOpenSub = new System.Windows.Forms.Button();
            this.btnRobot1 = new System.Windows.Forms.Button();
            this.btnWebTest = new System.Windows.Forms.Button();
            this.buttonCapture = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.pbxSubImg = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRobot2 = new System.Windows.Forms.Button();
            this.btnZoom = new System.Windows.Forms.Button();
            this.btnDumpSubImg = new System.Windows.Forms.Button();
            this.btnAutoKeys = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbxSrcImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxSubImg)).BeginInit();
            this.SuspendLayout();
            // 
            // pbxSrcImg
            // 
            this.pbxSrcImg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbxSrcImg.Location = new System.Drawing.Point(11, 11);
            this.pbxSrcImg.Margin = new System.Windows.Forms.Padding(2);
            this.pbxSrcImg.Name = "pbxSrcImg";
            this.pbxSrcImg.Size = new System.Drawing.Size(835, 683);
            this.pbxSrcImg.TabIndex = 0;
            this.pbxSrcImg.TabStop = false;
            this.pbxSrcImg.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            // 
            // btnOpenSrc
            // 
            this.btnOpenSrc.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnOpenSrc.Location = new System.Drawing.Point(884, 11);
            this.btnOpenSrc.Margin = new System.Windows.Forms.Padding(2);
            this.btnOpenSrc.Name = "btnOpenSrc";
            this.btnOpenSrc.Size = new System.Drawing.Size(80, 42);
            this.btnOpenSrc.TabIndex = 1;
            this.btnOpenSrc.Text = "OpenSrcImg";
            this.btnOpenSrc.UseVisualStyleBackColor = true;
            this.btnOpenSrc.Click += new System.EventHandler(this.btnOpenSrc_Click);
            // 
            // btnOpenSub
            // 
            this.btnOpenSub.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnOpenSub.Location = new System.Drawing.Point(979, 11);
            this.btnOpenSub.Margin = new System.Windows.Forms.Padding(2);
            this.btnOpenSub.Name = "btnOpenSub";
            this.btnOpenSub.Size = new System.Drawing.Size(80, 42);
            this.btnOpenSub.TabIndex = 2;
            this.btnOpenSub.Text = "OpenSubImg";
            this.btnOpenSub.UseVisualStyleBackColor = true;
            this.btnOpenSub.Click += new System.EventHandler(this.buttonOpenSub_Click);
            // 
            // btnRobot1
            // 
            this.btnRobot1.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnRobot1.Location = new System.Drawing.Point(884, 290);
            this.btnRobot1.Name = "btnRobot1";
            this.btnRobot1.Size = new System.Drawing.Size(80, 30);
            this.btnRobot1.TabIndex = 7;
            this.btnRobot1.Text = "Robot1";
            this.btnRobot1.UseVisualStyleBackColor = true;
            this.btnRobot1.Click += new System.EventHandler(this.btnRobot1_Click);
            // 
            // btnWebTest
            // 
            this.btnWebTest.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnWebTest.Location = new System.Drawing.Point(979, 125);
            this.btnWebTest.Name = "btnWebTest";
            this.btnWebTest.Size = new System.Drawing.Size(80, 30);
            this.btnWebTest.TabIndex = 6;
            this.btnWebTest.Text = "WebTest";
            this.btnWebTest.UseVisualStyleBackColor = true;
            this.btnWebTest.Click += new System.EventHandler(this.btnWebTest_Click);
            // 
            // buttonCapture
            // 
            this.buttonCapture.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonCapture.Location = new System.Drawing.Point(979, 80);
            this.buttonCapture.Margin = new System.Windows.Forms.Padding(2);
            this.buttonCapture.Name = "buttonCapture";
            this.buttonCapture.Size = new System.Drawing.Size(80, 30);
            this.buttonCapture.TabIndex = 5;
            this.buttonCapture.Text = "Capture";
            this.buttonCapture.UseVisualStyleBackColor = true;
            this.buttonCapture.Click += new System.EventHandler(this.buttonCapture_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnSearch.Location = new System.Drawing.Point(884, 80);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(2);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(80, 30);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // pbxSubImg
            // 
            this.pbxSubImg.Location = new System.Drawing.Point(884, 514);
            this.pbxSubImg.Name = "pbxSubImg";
            this.pbxSubImg.Size = new System.Drawing.Size(100, 95);
            this.pbxSubImg.TabIndex = 8;
            this.pbxSubImg.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 704);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "label1";
            // 
            // btnRobot2
            // 
            this.btnRobot2.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnRobot2.Location = new System.Drawing.Point(979, 290);
            this.btnRobot2.Name = "btnRobot2";
            this.btnRobot2.Size = new System.Drawing.Size(80, 30);
            this.btnRobot2.TabIndex = 10;
            this.btnRobot2.Text = "Robot2";
            this.btnRobot2.UseVisualStyleBackColor = true;
            this.btnRobot2.Click += new System.EventHandler(this.btnRobot2_Click);
            // 
            // btnZoom
            // 
            this.btnZoom.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnZoom.Location = new System.Drawing.Point(884, 125);
            this.btnZoom.Name = "btnZoom";
            this.btnZoom.Size = new System.Drawing.Size(80, 30);
            this.btnZoom.TabIndex = 6;
            this.btnZoom.Text = "Zoom";
            this.btnZoom.UseVisualStyleBackColor = true;
            this.btnZoom.Click += new System.EventHandler(this.btnZoom_Click);
            // 
            // btnDumpSubImg
            // 
            this.btnDumpSubImg.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnDumpSubImg.Location = new System.Drawing.Point(884, 664);
            this.btnDumpSubImg.Name = "btnDumpSubImg";
            this.btnDumpSubImg.Size = new System.Drawing.Size(100, 30);
            this.btnDumpSubImg.TabIndex = 11;
            this.btnDumpSubImg.Text = "DumpSubImg";
            this.btnDumpSubImg.UseVisualStyleBackColor = true;
            this.btnDumpSubImg.Click += new System.EventHandler(this.btnDumpSubImg_Click);
            // 
            // btnAutoKeys
            // 
            this.btnAutoKeys.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnAutoKeys.Location = new System.Drawing.Point(884, 180);
            this.btnAutoKeys.Name = "btnAutoKeys";
            this.btnAutoKeys.Size = new System.Drawing.Size(80, 41);
            this.btnAutoKeys.TabIndex = 12;
            this.btnAutoKeys.Text = "AutoKeys";
            this.btnAutoKeys.UseVisualStyleBackColor = true;
            this.btnAutoKeys.Click += new System.EventHandler(this.btnAutoKeys_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1079, 725);
            this.Controls.Add(this.btnAutoKeys);
            this.Controls.Add(this.btnDumpSubImg);
            this.Controls.Add(this.btnRobot2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pbxSubImg);
            this.Controls.Add(this.btnRobot1);
            this.Controls.Add(this.pbxSrcImg);
            this.Controls.Add(this.btnZoom);
            this.Controls.Add(this.btnWebTest);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.buttonCapture);
            this.Controls.Add(this.btnOpenSrc);
            this.Controls.Add(this.btnOpenSub);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pbxSrcImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxSubImg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbxSrcImg;
        private System.Windows.Forms.Button btnOpenSrc;
        private System.Windows.Forms.Button btnOpenSub;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button buttonCapture;
        private System.Windows.Forms.Button btnWebTest;
        private System.Windows.Forms.Button btnRobot1;
        private System.Windows.Forms.PictureBox pbxSubImg;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnRobot2;
        private System.Windows.Forms.Button btnZoom;
        private System.Windows.Forms.Button btnDumpSubImg;
        private System.Windows.Forms.Button btnAutoKeys;

    }
}

