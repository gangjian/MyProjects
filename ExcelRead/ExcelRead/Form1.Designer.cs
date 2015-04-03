namespace ExcelRead
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
            this.btnOpenGoods = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnReadGoods = new System.Windows.Forms.Button();
            this.btnOpenExport = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.btnReadExport = new System.Windows.Forms.Button();
            this.btnOpenClose = new System.Windows.Forms.Button();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.btnReadClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbxLog = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnOpenGoods
            // 
            this.btnOpenGoods.Location = new System.Drawing.Point(488, 21);
            this.btnOpenGoods.Name = "btnOpenGoods";
            this.btnOpenGoods.Size = new System.Drawing.Size(75, 23);
            this.btnOpenGoods.TabIndex = 0;
            this.btnOpenGoods.Text = "...";
            this.btnOpenGoods.UseVisualStyleBackColor = true;
            this.btnOpenGoods.Click += new System.EventHandler(this.btnOpenGoods_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 23);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(461, 19);
            this.textBox1.TabIndex = 1;
            // 
            // btnReadGoods
            // 
            this.btnReadGoods.Location = new System.Drawing.Point(588, 21);
            this.btnReadGoods.Name = "btnReadGoods";
            this.btnReadGoods.Size = new System.Drawing.Size(75, 23);
            this.btnReadGoods.TabIndex = 1;
            this.btnReadGoods.Text = "Read";
            this.btnReadGoods.UseVisualStyleBackColor = true;
            this.btnReadGoods.Click += new System.EventHandler(this.btnReadGoods_Click);
            // 
            // btnOpenExport
            // 
            this.btnOpenExport.Location = new System.Drawing.Point(488, 63);
            this.btnOpenExport.Name = "btnOpenExport";
            this.btnOpenExport.Size = new System.Drawing.Size(75, 23);
            this.btnOpenExport.TabIndex = 2;
            this.btnOpenExport.Text = "...";
            this.btnOpenExport.UseVisualStyleBackColor = true;
            this.btnOpenExport.Click += new System.EventHandler(this.btnOpenExport_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(12, 65);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(461, 19);
            this.textBox2.TabIndex = 1;
            // 
            // btnReadExport
            // 
            this.btnReadExport.Location = new System.Drawing.Point(588, 63);
            this.btnReadExport.Name = "btnReadExport";
            this.btnReadExport.Size = new System.Drawing.Size(75, 23);
            this.btnReadExport.TabIndex = 3;
            this.btnReadExport.Text = "Read";
            this.btnReadExport.UseVisualStyleBackColor = true;
            this.btnReadExport.Click += new System.EventHandler(this.btnReadExport_Click);
            // 
            // btnOpenClose
            // 
            this.btnOpenClose.Location = new System.Drawing.Point(488, 104);
            this.btnOpenClose.Name = "btnOpenClose";
            this.btnOpenClose.Size = new System.Drawing.Size(75, 23);
            this.btnOpenClose.TabIndex = 4;
            this.btnOpenClose.Text = "...";
            this.btnOpenClose.UseVisualStyleBackColor = true;
            this.btnOpenClose.Click += new System.EventHandler(this.btnOpenClose_Click);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(12, 106);
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(461, 19);
            this.textBox3.TabIndex = 1;
            // 
            // btnReadClose
            // 
            this.btnReadClose.Location = new System.Drawing.Point(588, 104);
            this.btnReadClose.Name = "btnReadClose";
            this.btnReadClose.Size = new System.Drawing.Size(75, 23);
            this.btnReadClose.TabIndex = 5;
            this.btnReadClose.Text = "Read";
            this.btnReadClose.UseVisualStyleBackColor = true;
            this.btnReadClose.Click += new System.EventHandler(this.btnReadClose_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "Goods";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "Close";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "Export";
            // 
            // tbxLog
            // 
            this.tbxLog.Location = new System.Drawing.Point(12, 157);
            this.tbxLog.Multiline = true;
            this.tbxLog.Name = "tbxLog";
            this.tbxLog.ReadOnly = true;
            this.tbxLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbxLog.Size = new System.Drawing.Size(551, 412);
            this.tbxLog.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(679, 581);
            this.Controls.Add(this.tbxLog);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnReadClose);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.btnReadExport);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.btnOpenClose);
            this.Controls.Add(this.btnReadGoods);
            this.Controls.Add(this.btnOpenExport);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnOpenGoods);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "ExcelRead";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOpenGoods;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnReadGoods;
        private System.Windows.Forms.Button btnOpenExport;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button btnReadExport;
        private System.Windows.Forms.Button btnOpenClose;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button btnReadClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbxLog;
    }
}

