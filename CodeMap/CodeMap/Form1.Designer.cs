namespace CodeMap
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
            this.tbxRootFolder = new System.Windows.Forms.TextBox();
            this.btnPrjFdr = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.lbStatus = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tbxRootFolder
            // 
            this.tbxRootFolder.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tbxRootFolder.Location = new System.Drawing.Point(12, 31);
            this.tbxRootFolder.Name = "tbxRootFolder";
            this.tbxRootFolder.Size = new System.Drawing.Size(427, 20);
            this.tbxRootFolder.TabIndex = 0;
            // 
            // btnPrjFdr
            // 
            this.btnPrjFdr.Location = new System.Drawing.Point(445, 30);
            this.btnPrjFdr.Name = "btnPrjFdr";
            this.btnPrjFdr.Size = new System.Drawing.Size(75, 22);
            this.btnPrjFdr.TabIndex = 1;
            this.btnPrjFdr.Text = "...";
            this.btnPrjFdr.UseVisualStyleBackColor = true;
            this.btnPrjFdr.Click += new System.EventHandler(this.btnPrjFdr_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "Project Folder";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(547, 16);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 36);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // lbStatus
            // 
            this.lbStatus.AutoSize = true;
            this.lbStatus.Location = new System.Drawing.Point(12, 731);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(37, 12);
            this.lbStatus.TabIndex = 4;
            this.lbStatus.Text = "Ready";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Black;
            this.pictureBox1.Location = new System.Drawing.Point(12, 65);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1186, 663);
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1210, 752);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lbStatus);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnPrjFdr);
            this.Controls.Add(this.tbxRootFolder);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "CodeMap";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbxRootFolder;
        private System.Windows.Forms.Button btnPrjFdr;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

