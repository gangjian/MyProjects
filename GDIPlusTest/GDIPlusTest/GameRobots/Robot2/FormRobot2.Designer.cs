namespace GDIPlusTest.GameRobots.Robot2
{
    partial class FormRobot2
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnLoadPic = new System.Windows.Forms.Button();
            this.btnFindRocks = new System.Windows.Forms.Button();
            this.btnFindTrees = new System.Windows.Forms.Button();
            this.btnFindBricks = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(645, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(82, 34);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(285, 255);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // btnLoadPic
            // 
            this.btnLoadPic.Location = new System.Drawing.Point(557, 12);
            this.btnLoadPic.Name = "btnLoadPic";
            this.btnLoadPic.Size = new System.Drawing.Size(82, 34);
            this.btnLoadPic.TabIndex = 0;
            this.btnLoadPic.Text = "loadPicture";
            this.btnLoadPic.UseVisualStyleBackColor = true;
            this.btnLoadPic.Click += new System.EventHandler(this.btnLoadPic_Click);
            // 
            // btnFindRocks
            // 
            this.btnFindRocks.Location = new System.Drawing.Point(557, 61);
            this.btnFindRocks.Name = "btnFindRocks";
            this.btnFindRocks.Size = new System.Drawing.Size(82, 34);
            this.btnFindRocks.TabIndex = 0;
            this.btnFindRocks.Text = "findRocks";
            this.btnFindRocks.UseVisualStyleBackColor = true;
            this.btnFindRocks.Click += new System.EventHandler(this.btnFindRocks_Click);
            // 
            // btnFindTrees
            // 
            this.btnFindTrees.Location = new System.Drawing.Point(557, 101);
            this.btnFindTrees.Name = "btnFindTrees";
            this.btnFindTrees.Size = new System.Drawing.Size(82, 34);
            this.btnFindTrees.TabIndex = 0;
            this.btnFindTrees.Text = "findTrees";
            this.btnFindTrees.UseVisualStyleBackColor = true;
            this.btnFindTrees.Click += new System.EventHandler(this.btnFindTrees_Click);
            // 
            // btnFindBricks
            // 
            this.btnFindBricks.Location = new System.Drawing.Point(557, 141);
            this.btnFindBricks.Name = "btnFindBricks";
            this.btnFindBricks.Size = new System.Drawing.Size(82, 34);
            this.btnFindBricks.TabIndex = 0;
            this.btnFindBricks.Text = "findBricks";
            this.btnFindBricks.UseVisualStyleBackColor = true;
            this.btnFindBricks.Click += new System.EventHandler(this.btnFindBricks_Click);
            // 
            // FormRobot2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(739, 540);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnLoadPic);
            this.Controls.Add(this.btnFindBricks);
            this.Controls.Add(this.btnFindTrees);
            this.Controls.Add(this.btnFindRocks);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormRobot2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormRobot2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormRobot2_FormClosing);
            this.Load += new System.EventHandler(this.FormRobot2_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnLoadPic;
        private System.Windows.Forms.Button btnFindRocks;
        private System.Windows.Forms.Button btnFindTrees;
        private System.Windows.Forms.Button btnFindBricks;
    }
}