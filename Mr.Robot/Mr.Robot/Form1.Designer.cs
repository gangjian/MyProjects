namespace Mr.Robot
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
			this.tbxRootPath = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.btnSetFolder = new System.Windows.Forms.Button();
			this.lvFileList = new System.Windows.Forms.ListView();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.lvFunctionList = new System.Windows.Forms.ListView();
			this.label4 = new System.Windows.Forms.Label();
			this.lvVariableList = new System.Windows.Forms.ListView();
			this.label5 = new System.Windows.Forms.Label();
			this.lvBranchList = new System.Windows.Forms.ListView();
			this.btnStart = new System.Windows.Forms.Button();
			this.labelStatus = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// tbxRootPath
			// 
			this.tbxRootPath.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tbxRootPath.Location = new System.Drawing.Point(12, 38);
			this.tbxRootPath.Name = "tbxRootPath";
			this.tbxRootPath.ReadOnly = true;
			this.tbxRootPath.Size = new System.Drawing.Size(413, 23);
			this.tbxRootPath.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(189, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "在这里设定代码所在的根目录";
			// 
			// btnSetFolder
			// 
			this.btnSetFolder.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSetFolder.Location = new System.Drawing.Point(442, 36);
			this.btnSetFolder.Name = "btnSetFolder";
			this.btnSetFolder.Size = new System.Drawing.Size(83, 23);
			this.btnSetFolder.TabIndex = 0;
			this.btnSetFolder.Text = "...";
			this.btnSetFolder.UseVisualStyleBackColor = true;
			this.btnSetFolder.Click += new System.EventHandler(this.btnSetFolder_Click);
			// 
			// lvFileList
			// 
			this.lvFileList.CheckBoxes = true;
			this.lvFileList.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lvFileList.FullRowSelect = true;
			this.lvFileList.GridLines = true;
			this.lvFileList.Location = new System.Drawing.Point(12, 88);
			this.lvFileList.MultiSelect = false;
			this.lvFileList.Name = "lvFileList";
			this.lvFileList.Size = new System.Drawing.Size(331, 179);
			this.lvFileList.TabIndex = 3;
			this.lvFileList.UseCompatibleStateImageBehavior = false;
			this.lvFileList.View = System.Windows.Forms.View.Details;
			this.lvFileList.SelectedIndexChanged += new System.EventHandler(this.lvFileList_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(9, 72);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(63, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "文件列表";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(9, 270);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(63, 13);
			this.label3.TabIndex = 1;
			this.label3.Text = "函数列表";
			// 
			// lvFunctionList
			// 
			this.lvFunctionList.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lvFunctionList.FullRowSelect = true;
			this.lvFunctionList.GridLines = true;
			this.lvFunctionList.Location = new System.Drawing.Point(12, 286);
			this.lvFunctionList.MultiSelect = false;
			this.lvFunctionList.Name = "lvFunctionList";
			this.lvFunctionList.Size = new System.Drawing.Size(331, 319);
			this.lvFunctionList.TabIndex = 3;
			this.lvFunctionList.UseCompatibleStateImageBehavior = false;
			this.lvFunctionList.View = System.Windows.Forms.View.Details;
			this.lvFunctionList.SelectedIndexChanged += new System.EventHandler(this.lvFunctionList_SelectedIndexChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(370, 72);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(63, 13);
			this.label4.TabIndex = 1;
			this.label4.Text = "变量列表";
			// 
			// lvVariableList
			// 
			this.lvVariableList.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lvVariableList.FullRowSelect = true;
			this.lvVariableList.GridLines = true;
			this.lvVariableList.Location = new System.Drawing.Point(373, 88);
			this.lvVariableList.MultiSelect = false;
			this.lvVariableList.Name = "lvVariableList";
			this.lvVariableList.Size = new System.Drawing.Size(331, 266);
			this.lvVariableList.TabIndex = 3;
			this.lvVariableList.UseCompatibleStateImageBehavior = false;
			this.lvVariableList.View = System.Windows.Forms.View.Details;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(370, 357);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(63, 13);
			this.label5.TabIndex = 1;
			this.label5.Text = "分支列表";
			// 
			// lvBranchList
			// 
			this.lvBranchList.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lvBranchList.FullRowSelect = true;
			this.lvBranchList.GridLines = true;
			this.lvBranchList.Location = new System.Drawing.Point(373, 373);
			this.lvBranchList.MultiSelect = false;
			this.lvBranchList.Name = "lvBranchList";
			this.lvBranchList.Size = new System.Drawing.Size(331, 232);
			this.lvBranchList.TabIndex = 3;
			this.lvBranchList.UseCompatibleStateImageBehavior = false;
			this.lvBranchList.View = System.Windows.Forms.View.Details;
			// 
			// btnStart
			// 
			this.btnStart.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnStart.Location = new System.Drawing.Point(606, 29);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(98, 36);
			this.btnStart.TabIndex = 2;
			this.btnStart.Text = "Start";
			this.btnStart.UseVisualStyleBackColor = true;
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// labelStatus
			// 
			this.labelStatus.AutoSize = true;
			this.labelStatus.Location = new System.Drawing.Point(9, 623);
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Size = new System.Drawing.Size(42, 13);
			this.labelStatus.TabIndex = 4;
			this.labelStatus.Text = "Ready";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(721, 645);
			this.Controls.Add(this.labelStatus);
			this.Controls.Add(this.lvBranchList);
			this.Controls.Add(this.lvVariableList);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.lvFunctionList);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.lvFileList);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.btnStart);
			this.Controls.Add(this.btnSetFolder);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.tbxRootPath);
			this.Font = new System.Drawing.Font("SimSun-ExtB", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Mr.Robot v0.1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox tbxRootPath;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnSetFolder;
		private System.Windows.Forms.ListView lvFileList;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ListView lvFunctionList;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ListView lvVariableList;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ListView lvBranchList;
		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.Label labelStatus;
	}
}

