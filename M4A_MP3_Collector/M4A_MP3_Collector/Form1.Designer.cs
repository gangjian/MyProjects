namespace M4A_MP3_Collector
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
			this.tbxA = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.tbxB = new System.Windows.Forms.TextBox();
			this.button2 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.btnCopyM4A = new System.Windows.Forms.Button();
			this.btnReplaceM4A = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// tbxA
			// 
			this.tbxA.Location = new System.Drawing.Point(12, 33);
			this.tbxA.Name = "tbxA";
			this.tbxA.ReadOnly = true;
			this.tbxA.Size = new System.Drawing.Size(425, 22);
			this.tbxA.TabIndex = 0;
			this.tbxA.TabStop = false;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(447, 32);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "...";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// tbxB
			// 
			this.tbxB.Location = new System.Drawing.Point(12, 97);
			this.tbxB.Name = "tbxB";
			this.tbxB.ReadOnly = true;
			this.tbxB.Size = new System.Drawing.Size(425, 22);
			this.tbxB.TabIndex = 0;
			this.tbxB.TabStop = false;
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(447, 96);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 1;
			this.button2.Text = "...";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(63, 14);
			this.label1.TabIndex = 2;
			this.label1.Text = "Folder A";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(9, 80);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(63, 14);
			this.label2.TabIndex = 2;
			this.label2.Text = "Folder B";
			// 
			// btnCopyM4A
			// 
			this.btnCopyM4A.Location = new System.Drawing.Point(12, 168);
			this.btnCopyM4A.Name = "btnCopyM4A";
			this.btnCopyM4A.Size = new System.Drawing.Size(150, 42);
			this.btnCopyM4A.TabIndex = 3;
			this.btnCopyM4A.Text = "Copy M4A in A to B";
			this.btnCopyM4A.UseVisualStyleBackColor = true;
			this.btnCopyM4A.Click += new System.EventHandler(this.btnCopyM4A_Click);
			// 
			// btnReplaceM4A
			// 
			this.btnReplaceM4A.Location = new System.Drawing.Point(287, 168);
			this.btnReplaceM4A.Name = "btnReplaceM4A";
			this.btnReplaceM4A.Size = new System.Drawing.Size(150, 42);
			this.btnReplaceM4A.TabIndex = 3;
			this.btnReplaceM4A.Text = "Replace M4A in A By MP3 in B";
			this.btnReplaceM4A.UseVisualStyleBackColor = true;
			this.btnReplaceM4A.Click += new System.EventHandler(this.btnReplaceM4A_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(534, 240);
			this.Controls.Add(this.btnReplaceM4A);
			this.Controls.Add(this.btnCopyM4A);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.tbxB);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.tbxA);
			this.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "M4A_MP3_Collector";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox tbxA;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox tbxB;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnCopyM4A;
		private System.Windows.Forms.Button btnReplaceM4A;
	}
}

