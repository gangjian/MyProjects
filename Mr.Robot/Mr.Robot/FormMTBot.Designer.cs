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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMTBot));
			this.label1 = new System.Windows.Forms.Label();
			this.tbxRootPath = new System.Windows.Forms.TextBox();
			this.btnOpenRoot = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.tbxSourcePath = new System.Windows.Forms.TextBox();
			this.btnOpenSource = new System.Windows.Forms.Button();
			this.btnStart = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.lvDetailList = new System.Windows.Forms.ListView();
			this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.tbxLog = new System.Windows.Forms.TextBox();
			this.btnSaveDetail2CSV = new System.Windows.Forms.Button();
			this.lvSummaryList = new System.Windows.Forms.ListView();
			this.columnHeaderIdx = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeaderMacroName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeaderDefined = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeaderValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeaderDefFile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.label6 = new System.Windows.Forms.Label();
			this.btnSaveSummary2CSV = new System.Windows.Forms.Button();
			this.treeViewSrcFile = new System.Windows.Forms.TreeView();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.tbxOutputLog = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(86, 21);
			this.label1.TabIndex = 0;
			this.label1.Text = "Root Path";
			// 
			// tbxRootPath
			// 
			this.tbxRootPath.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tbxRootPath.Location = new System.Drawing.Point(16, 33);
			this.tbxRootPath.Name = "tbxRootPath";
			this.tbxRootPath.ReadOnly = true;
			this.tbxRootPath.Size = new System.Drawing.Size(736, 23);
			this.tbxRootPath.TabIndex = 1;
			// 
			// btnOpenRoot
			// 
			this.btnOpenRoot.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
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
			this.label2.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label2.Location = new System.Drawing.Point(12, 59);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(99, 21);
			this.label2.TabIndex = 0;
			this.label2.Text = "Target Path";
			// 
			// tbxSourcePath
			// 
			this.tbxSourcePath.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tbxSourcePath.Location = new System.Drawing.Point(16, 83);
			this.tbxSourcePath.Name = "tbxSourcePath";
			this.tbxSourcePath.ReadOnly = true;
			this.tbxSourcePath.Size = new System.Drawing.Size(736, 23);
			this.tbxSourcePath.TabIndex = 1;
			// 
			// btnOpenSource
			// 
			this.btnOpenSource.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.btnOpenSource.Location = new System.Drawing.Point(758, 83);
			this.btnOpenSource.Name = "btnOpenSource";
			this.btnOpenSource.Size = new System.Drawing.Size(75, 23);
			this.btnOpenSource.TabIndex = 2;
			this.btnOpenSource.Text = "...";
			this.btnOpenSource.UseVisualStyleBackColor = true;
			this.btnOpenSource.Click += new System.EventHandler(this.btnOpenSource_Click);
			// 
			// btnStart
			// 
			this.btnStart.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnStart.Location = new System.Drawing.Point(851, 33);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(103, 54);
			this.btnStart.TabIndex = 3;
			this.btnStart.Text = "Start";
			this.btnStart.UseVisualStyleBackColor = true;
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(452, 352);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(108, 19);
			this.label4.TabIndex = 0;
			this.label4.Text = "Detail List";
			// 
			// lvDetailList
			// 
			this.lvDetailList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader8,
            this.columnHeader6,
            this.columnHeader7});
			this.lvDetailList.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.lvDetailList.FullRowSelect = true;
			this.lvDetailList.GridLines = true;
			this.lvDetailList.Location = new System.Drawing.Point(456, 374);
			this.lvDetailList.Name = "lvDetailList";
			this.lvDetailList.Size = new System.Drawing.Size(498, 310);
			this.lvDetailList.TabIndex = 4;
			this.lvDetailList.UseCompatibleStateImageBehavior = false;
			this.lvDetailList.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "idx";
			this.columnHeader3.Width = 35;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "file";
			this.columnHeader4.Width = 220;
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "line";
			this.columnHeader5.Width = 55;
			// 
			// columnHeader8
			// 
			this.columnHeader8.Text = "expression";
			this.columnHeader8.Width = 260;
			// 
			// columnHeader6
			// 
			this.columnHeader6.Text = "macro";
			this.columnHeader6.Width = 230;
			// 
			// columnHeader7
			// 
			this.columnHeader7.Text = "value";
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
			this.tbxLog.Location = new System.Drawing.Point(527, 113);
			this.tbxLog.Multiline = true;
			this.tbxLog.Name = "tbxLog";
			this.tbxLog.ReadOnly = true;
			this.tbxLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbxLog.Size = new System.Drawing.Size(427, 146);
			this.tbxLog.TabIndex = 6;
			// 
			// btnSaveDetail2CSV
			// 
			this.btnSaveDetail2CSV.Font = new System.Drawing.Font("Consolas", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSaveDetail2CSV.Location = new System.Drawing.Point(858, 343);
			this.btnSaveDetail2CSV.Name = "btnSaveDetail2CSV";
			this.btnSaveDetail2CSV.Size = new System.Drawing.Size(96, 28);
			this.btnSaveDetail2CSV.TabIndex = 7;
			this.btnSaveDetail2CSV.Text = "Save CSV";
			this.btnSaveDetail2CSV.UseVisualStyleBackColor = true;
			this.btnSaveDetail2CSV.Click += new System.EventHandler(this.btnSaveDetail2CSV_Click);
			// 
			// lvSummaryList
			// 
			this.lvSummaryList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderIdx,
            this.columnHeaderMacroName,
            this.columnHeaderDefined,
            this.columnHeaderValue,
            this.columnHeaderDefFile});
			this.lvSummaryList.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.lvSummaryList.FullRowSelect = true;
			this.lvSummaryList.GridLines = true;
			this.lvSummaryList.Location = new System.Drawing.Point(16, 374);
			this.lvSummaryList.Name = "lvSummaryList";
			this.lvSummaryList.Size = new System.Drawing.Size(421, 310);
			this.lvSummaryList.TabIndex = 4;
			this.lvSummaryList.UseCompatibleStateImageBehavior = false;
			this.lvSummaryList.View = System.Windows.Forms.View.Details;
			// 
			// columnHeaderIdx
			// 
			this.columnHeaderIdx.Text = "idx";
			this.columnHeaderIdx.Width = 30;
			// 
			// columnHeaderMacroName
			// 
			this.columnHeaderMacroName.Text = "macro";
			this.columnHeaderMacroName.Width = 180;
			// 
			// columnHeaderDefined
			// 
			this.columnHeaderDefined.Text = "def";
			this.columnHeaderDefined.Width = 35;
			// 
			// columnHeaderValue
			// 
			this.columnHeaderValue.Text = "value";
			this.columnHeaderValue.Width = 55;
			// 
			// columnHeaderDefFile
			// 
			this.columnHeaderDefFile.Text = "path";
			this.columnHeaderDefFile.Width = 120;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(12, 352);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(117, 19);
			this.label6.TabIndex = 0;
			this.label6.Text = "Summary List";
			// 
			// btnSaveSummary2CSV
			// 
			this.btnSaveSummary2CSV.Font = new System.Drawing.Font("Consolas", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSaveSummary2CSV.Location = new System.Drawing.Point(341, 343);
			this.btnSaveSummary2CSV.Name = "btnSaveSummary2CSV";
			this.btnSaveSummary2CSV.Size = new System.Drawing.Size(96, 28);
			this.btnSaveSummary2CSV.TabIndex = 7;
			this.btnSaveSummary2CSV.Text = "Save CSV";
			this.btnSaveSummary2CSV.UseVisualStyleBackColor = true;
			this.btnSaveSummary2CSV.Click += new System.EventHandler(this.btnSaveSummary2CSV_Click);
			// 
			// treeViewSrcFile
			// 
			this.treeViewSrcFile.CheckBoxes = true;
			this.treeViewSrcFile.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.treeViewSrcFile.ImageIndex = 2;
			this.treeViewSrcFile.ImageList = this.imageList1;
			this.treeViewSrcFile.Location = new System.Drawing.Point(16, 113);
			this.treeViewSrcFile.Name = "treeViewSrcFile";
			this.treeViewSrcFile.SelectedImageIndex = 2;
			this.treeViewSrcFile.Size = new System.Drawing.Size(492, 219);
			this.treeViewSrcFile.TabIndex = 8;
			this.treeViewSrcFile.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeViewSrcFile_AfterCheck);
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "1490786290_page_white_c.ico");
			this.imageList1.Images.SetKeyName(1, "1490786648_page_white_h.ico");
			this.imageList1.Images.SetKeyName(2, "1490786476_folder.ico");
			// 
			// tbxOutputLog
			// 
			this.tbxOutputLog.Location = new System.Drawing.Point(527, 265);
			this.tbxOutputLog.Multiline = true;
			this.tbxOutputLog.Name = "tbxOutputLog";
			this.tbxOutputLog.ReadOnly = true;
			this.tbxOutputLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbxOutputLog.Size = new System.Drawing.Size(427, 67);
			this.tbxOutputLog.TabIndex = 6;
			// 
			// FormMTBot
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(966, 722);
			this.Controls.Add(this.treeViewSrcFile);
			this.Controls.Add(this.btnSaveSummary2CSV);
			this.Controls.Add(this.btnSaveDetail2CSV);
			this.Controls.Add(this.tbxOutputLog);
			this.Controls.Add(this.tbxLog);
			this.Controls.Add(this.progressBar1);
			this.Controls.Add(this.lvSummaryList);
			this.Controls.Add(this.lvDetailList);
			this.Controls.Add(this.btnStart);
			this.Controls.Add(this.btnOpenSource);
			this.Controls.Add(this.btnOpenRoot);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.tbxSourcePath);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.tbxRootPath);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "FormMTBot";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "MTBot v0.1.8";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMTBot_FormClosing);
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
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ListView lvDetailList;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ColumnHeader columnHeader6;
		private System.Windows.Forms.ColumnHeader columnHeader7;
		private System.Windows.Forms.TextBox tbxLog;
		private System.Windows.Forms.ColumnHeader columnHeader8;
		private System.Windows.Forms.Button btnSaveDetail2CSV;
		private System.Windows.Forms.ListView lvSummaryList;
		private System.Windows.Forms.ColumnHeader columnHeaderIdx;
		private System.Windows.Forms.ColumnHeader columnHeaderMacroName;
		private System.Windows.Forms.ColumnHeader columnHeaderValue;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button btnSaveSummary2CSV;
		private System.Windows.Forms.ColumnHeader columnHeaderDefFile;
		private System.Windows.Forms.TreeView treeViewSrcFile;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.TextBox tbxOutputLog;
		private System.Windows.Forms.ColumnHeader columnHeaderDefined;
	}
}