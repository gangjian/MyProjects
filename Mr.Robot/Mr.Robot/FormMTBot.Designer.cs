namespace Mr.Robot
{
	partial class FormMTBot
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
			this.label1 = new System.Windows.Forms.Label();
			this.tbxRootPath = new System.Windows.Forms.TextBox();
			this.btnOpenRoot = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.tbxSourcePath = new System.Windows.Forms.TextBox();
			this.btnOpenSource = new System.Windows.Forms.Button();
			this.btnStart = new System.Windows.Forms.Button();
			this.lvSourceList = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.lvMacroList = new System.Windows.Forms.ListView();
			this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.tbxLog = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.btnSave2CSV = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(373, 21);
			this.label1.TabIndex = 0;
			this.label1.Text = "在这里指定所有代码的根目录(包含所有*.c, *.h文件)";
			// 
			// tbxRootPath
			// 
			this.tbxRootPath.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tbxRootPath.Location = new System.Drawing.Point(16, 33);
			this.tbxRootPath.Name = "tbxRootPath";
			this.tbxRootPath.ReadOnly = true;
			this.tbxRootPath.Size = new System.Drawing.Size(736, 23);
			this.tbxRootPath.TabIndex = 1;
			// 
			// btnOpenRoot
			// 
			this.btnOpenRoot.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.btnOpenRoot.Location = new System.Drawing.Point(758, 33);
			this.btnOpenRoot.Name = "btnOpenRoot";
			this.btnOpenRoot.Size = new System.Drawing.Size(75, 23);
			this.btnOpenRoot.TabIndex = 0;
			this.btnOpenRoot.Text = "...";
			this.btnOpenRoot.UseVisualStyleBackColor = true;
			this.btnOpenRoot.Click += new System.EventHandler(this.btnOpenRoot_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label2.Location = new System.Drawing.Point(12, 66);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(221, 21);
			this.label2.TabIndex = 0;
			this.label2.Text = "在这里指定目标*.c文件的目录";
			// 
			// tbxSourcePath
			// 
			this.tbxSourcePath.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tbxSourcePath.Location = new System.Drawing.Point(16, 90);
			this.tbxSourcePath.Name = "tbxSourcePath";
			this.tbxSourcePath.ReadOnly = true;
			this.tbxSourcePath.Size = new System.Drawing.Size(736, 23);
			this.tbxSourcePath.TabIndex = 1;
			// 
			// btnOpenSource
			// 
			this.btnOpenSource.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.btnOpenSource.Location = new System.Drawing.Point(758, 90);
			this.btnOpenSource.Name = "btnOpenSource";
			this.btnOpenSource.Size = new System.Drawing.Size(75, 23);
			this.btnOpenSource.TabIndex = 2;
			this.btnOpenSource.Text = "...";
			this.btnOpenSource.UseVisualStyleBackColor = true;
			this.btnOpenSource.Click += new System.EventHandler(this.btnOpenSource_Click);
			// 
			// btnStart
			// 
			this.btnStart.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.btnStart.Location = new System.Drawing.Point(851, 33);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(103, 54);
			this.btnStart.TabIndex = 3;
			this.btnStart.Text = "开始";
			this.btnStart.UseVisualStyleBackColor = true;
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// lvSourceList
			// 
			this.lvSourceList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
			this.lvSourceList.GridLines = true;
			this.lvSourceList.Location = new System.Drawing.Point(16, 153);
			this.lvSourceList.Name = "lvSourceList";
			this.lvSourceList.Size = new System.Drawing.Size(548, 179);
			this.lvSourceList.TabIndex = 4;
			this.lvSourceList.UseCompatibleStateImageBehavior = false;
			this.lvSourceList.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "No.";
			this.columnHeader1.Width = 45;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Path";
			this.columnHeader2.Width = 495;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label3.Location = new System.Drawing.Point(12, 129);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(93, 21);
			this.label3.TabIndex = 0;
			this.label3.Text = "*.c文件列表";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label4.Location = new System.Drawing.Point(12, 350);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(138, 21);
			this.label4.TabIndex = 0;
			this.label4.Text = "预编译宏开关列表";
			// 
			// lvMacroList
			// 
			this.lvMacroList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader8,
            this.columnHeader6,
            this.columnHeader7});
			this.lvMacroList.GridLines = true;
			this.lvMacroList.Location = new System.Drawing.Point(16, 374);
			this.lvMacroList.Name = "lvMacroList";
			this.lvMacroList.Size = new System.Drawing.Size(938, 310);
			this.lvMacroList.TabIndex = 4;
			this.lvMacroList.UseCompatibleStateImageBehavior = false;
			this.lvMacroList.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "序号";
			this.columnHeader3.Width = 40;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "文件名";
			this.columnHeader4.Width = 220;
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "行号";
			// 
			// columnHeader8
			// 
			this.columnHeader8.Text = "表达式";
			this.columnHeader8.Width = 260;
			// 
			// columnHeader6
			// 
			this.columnHeader6.Text = "宏名";
			this.columnHeader6.Width = 230;
			// 
			// columnHeader7
			// 
			this.columnHeader7.Text = "值";
			this.columnHeader7.Width = 120;
			// 
			// progressBar1
			// 
			this.progressBar1.Location = new System.Drawing.Point(16, 691);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(938, 23);
			this.progressBar1.TabIndex = 5;
			// 
			// tbxLog
			// 
			this.tbxLog.Location = new System.Drawing.Point(588, 153);
			this.tbxLog.Multiline = true;
			this.tbxLog.Name = "tbxLog";
			this.tbxLog.ReadOnly = true;
			this.tbxLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbxLog.Size = new System.Drawing.Size(366, 179);
			this.tbxLog.TabIndex = 6;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label5.Location = new System.Drawing.Point(584, 129);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(38, 21);
			this.label5.TabIndex = 0;
			this.label5.Text = "Log";
			// 
			// btnSave2CSV
			// 
			this.btnSave2CSV.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.btnSave2CSV.Location = new System.Drawing.Point(851, 338);
			this.btnSave2CSV.Name = "btnSave2CSV";
			this.btnSave2CSV.Size = new System.Drawing.Size(103, 33);
			this.btnSave2CSV.TabIndex = 7;
			this.btnSave2CSV.Text = "保存到CSV";
			this.btnSave2CSV.UseVisualStyleBackColor = true;
			this.btnSave2CSV.Click += new System.EventHandler(this.btnSave2CSV_Click);
			// 
			// FormMTBot
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(966, 722);
			this.Controls.Add(this.btnSave2CSV);
			this.Controls.Add(this.tbxLog);
			this.Controls.Add(this.progressBar1);
			this.Controls.Add(this.lvMacroList);
			this.Controls.Add(this.lvSourceList);
			this.Controls.Add(this.btnStart);
			this.Controls.Add(this.btnOpenSource);
			this.Controls.Add(this.btnOpenRoot);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.tbxSourcePath);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.tbxRootPath);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.Name = "FormMTBot";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "MTBot";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbxRootPath;
		private System.Windows.Forms.Button btnOpenRoot;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbxSourcePath;
		private System.Windows.Forms.Button btnOpenSource;
		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.ListView lvSourceList;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ListView lvMacroList;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ColumnHeader columnHeader6;
		private System.Windows.Forms.ColumnHeader columnHeader7;
		private System.Windows.Forms.TextBox tbxLog;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ColumnHeader columnHeader8;
		private System.Windows.Forms.Button btnSave2CSV;
	}
}