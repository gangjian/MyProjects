namespace CreeperGUI
{
	partial class Form1
	{
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows 窗体设计器生成的代码

		/// <summary>
		/// 设计器支持所需的方法 - 不要
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			this.tbxPrjPath = new System.Windows.Forms.TextBox();
			this.btnOpenPrjPath = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.tbxFileName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.btnOpenFileName = new System.Windows.Forms.Button();
			this.btnStart = new System.Windows.Forms.Button();
			this.tbxLog = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// tbxPrjPath
			// 
			this.tbxPrjPath.Location = new System.Drawing.Point(12, 25);
			this.tbxPrjPath.Name = "tbxPrjPath";
			this.tbxPrjPath.ReadOnly = true;
			this.tbxPrjPath.Size = new System.Drawing.Size(412, 22);
			this.tbxPrjPath.TabIndex = 0;
			this.tbxPrjPath.TabStop = false;
			// 
			// btnOpenPrjPath
			// 
			this.btnOpenPrjPath.Location = new System.Drawing.Point(430, 24);
			this.btnOpenPrjPath.Name = "btnOpenPrjPath";
			this.btnOpenPrjPath.Size = new System.Drawing.Size(57, 23);
			this.btnOpenPrjPath.TabIndex = 1;
			this.btnOpenPrjPath.Text = "...";
			this.btnOpenPrjPath.UseVisualStyleBackColor = true;
			this.btnOpenPrjPath.Click += new System.EventHandler(this.btnOpenPrjPath_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(91, 14);
			this.label1.TabIndex = 2;
			this.label1.Text = "Project path";
			// 
			// tbxFileName
			// 
			this.tbxFileName.Location = new System.Drawing.Point(498, 25);
			this.tbxFileName.Name = "tbxFileName";
			this.tbxFileName.ReadOnly = true;
			this.tbxFileName.Size = new System.Drawing.Size(349, 22);
			this.tbxFileName.TabIndex = 0;
			this.tbxFileName.TabStop = false;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(495, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(70, 14);
			this.label2.TabIndex = 2;
			this.label2.Text = "File name";
			// 
			// btnOpenFileName
			// 
			this.btnOpenFileName.Location = new System.Drawing.Point(853, 24);
			this.btnOpenFileName.Name = "btnOpenFileName";
			this.btnOpenFileName.Size = new System.Drawing.Size(57, 23);
			this.btnOpenFileName.TabIndex = 1;
			this.btnOpenFileName.Text = "...";
			this.btnOpenFileName.UseVisualStyleBackColor = true;
			this.btnOpenFileName.Click += new System.EventHandler(this.btnOpenFileName_Click);
			// 
			// btnStart
			// 
			this.btnStart.Location = new System.Drawing.Point(853, 64);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(57, 37);
			this.btnStart.TabIndex = 1;
			this.btnStart.Text = "Start";
			this.btnStart.UseVisualStyleBackColor = true;
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// tbxLog
			// 
			this.tbxLog.Location = new System.Drawing.Point(12, 64);
			this.tbxLog.Multiline = true;
			this.tbxLog.Name = "tbxLog";
			this.tbxLog.ReadOnly = true;
			this.tbxLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbxLog.Size = new System.Drawing.Size(835, 617);
			this.tbxLog.TabIndex = 0;
			this.tbxLog.TabStop = false;
			this.tbxLog.WordWrap = false;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(922, 693);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.tbxLog);
			this.Controls.Add(this.tbxFileName);
			this.Controls.Add(this.btnStart);
			this.Controls.Add(this.btnOpenFileName);
			this.Controls.Add(this.btnOpenPrjPath);
			this.Controls.Add(this.tbxPrjPath);
			this.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "Form1";
			this.Text = "CreeperGUI";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox tbxPrjPath;
		private System.Windows.Forms.Button btnOpenPrjPath;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbxFileName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnOpenFileName;
		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.TextBox tbxLog;
	}
}

