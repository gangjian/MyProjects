namespace SourceOutsight
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
			this.tbxPath = new System.Windows.Forms.TextBox();
			this.tbxLog = new System.Windows.Forms.TextBox();
			this.btnOpen = new System.Windows.Forms.Button();
			this.btnStart = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// tbxPath
			// 
			this.tbxPath.Location = new System.Drawing.Point(12, 12);
			this.tbxPath.Name = "tbxPath";
			this.tbxPath.ReadOnly = true;
			this.tbxPath.Size = new System.Drawing.Size(755, 22);
			this.tbxPath.TabIndex = 0;
			this.tbxPath.TabStop = false;
			// 
			// tbxLog
			// 
			this.tbxLog.Location = new System.Drawing.Point(12, 40);
			this.tbxLog.Multiline = true;
			this.tbxLog.Name = "tbxLog";
			this.tbxLog.ReadOnly = true;
			this.tbxLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbxLog.Size = new System.Drawing.Size(755, 596);
			this.tbxLog.TabIndex = 1;
			this.tbxLog.WordWrap = false;
			// 
			// btnOpen
			// 
			this.btnOpen.Location = new System.Drawing.Point(773, 11);
			this.btnOpen.Name = "btnOpen";
			this.btnOpen.Size = new System.Drawing.Size(75, 23);
			this.btnOpen.TabIndex = 2;
			this.btnOpen.Text = "...";
			this.btnOpen.UseVisualStyleBackColor = true;
			this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
			// 
			// btnStart
			// 
			this.btnStart.Location = new System.Drawing.Point(773, 40);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(75, 37);
			this.btnStart.TabIndex = 2;
			this.btnStart.Text = "start";
			this.btnStart.UseVisualStyleBackColor = true;
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(860, 648);
			this.Controls.Add(this.btnStart);
			this.Controls.Add(this.btnOpen);
			this.Controls.Add(this.tbxLog);
			this.Controls.Add(this.tbxPath);
			this.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox tbxPath;
		private System.Windows.Forms.TextBox tbxLog;
		private System.Windows.Forms.Button btnOpen;
		private System.Windows.Forms.Button btnStart;
	}
}

