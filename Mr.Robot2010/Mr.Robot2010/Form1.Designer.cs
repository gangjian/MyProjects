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
            this.btnStartFile = new System.Windows.Forms.Button();
            this.labelStatus = new System.Windows.Forms.Label();
            this.cbxFile = new System.Windows.Forms.CheckBox();
            this.btnStartFunction = new System.Windows.Forms.Button();
            this.cbxFunction = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // tbxRootPath
            // 
            this.tbxRootPath.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxRootPath.Location = new System.Drawing.Point(12, 33);
            this.tbxRootPath.Name = "tbxRootPath";
            this.tbxRootPath.ReadOnly = true;
            this.tbxRootPath.Size = new System.Drawing.Size(413, 23);
            this.tbxRootPath.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 14);
            this.label1.TabIndex = 1;
            this.label1.Text = "Root path";
            // 
            // btnSetFolder
            // 
            this.btnSetFolder.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSetFolder.Location = new System.Drawing.Point(450, 33);
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
            this.lvFileList.Location = new System.Drawing.Point(12, 77);
            this.lvFileList.MultiSelect = false;
            this.lvFileList.Name = "lvFileList";
            this.lvFileList.Size = new System.Drawing.Size(331, 157);
            this.lvFileList.TabIndex = 3;
            this.lvFileList.UseCompatibleStateImageBehavior = false;
            this.lvFileList.View = System.Windows.Forms.View.Details;
            this.lvFileList.SelectedIndexChanged += new System.EventHandler(this.lvFileList_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 14);
            this.label2.TabIndex = 1;
            this.label2.Text = "Source files";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 236);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 14);
            this.label3.TabIndex = 1;
            this.label3.Text = "Functions";
            // 
            // lvFunctionList
            // 
            this.lvFunctionList.CheckBoxes = true;
            this.lvFunctionList.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvFunctionList.FullRowSelect = true;
            this.lvFunctionList.GridLines = true;
            this.lvFunctionList.Location = new System.Drawing.Point(12, 250);
            this.lvFunctionList.MultiSelect = false;
            this.lvFunctionList.Name = "lvFunctionList";
            this.lvFunctionList.Size = new System.Drawing.Size(331, 280);
            this.lvFunctionList.TabIndex = 3;
            this.lvFunctionList.UseCompatibleStateImageBehavior = false;
            this.lvFunctionList.View = System.Windows.Forms.View.Details;
            this.lvFunctionList.SelectedIndexChanged += new System.EventHandler(this.lvFunctionList_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(447, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 14);
            this.label4.TabIndex = 1;
            this.label4.Text = "Viable";
            // 
            // lvVariableList
            // 
            this.lvVariableList.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvVariableList.FullRowSelect = true;
            this.lvVariableList.GridLines = true;
            this.lvVariableList.Location = new System.Drawing.Point(450, 77);
            this.lvVariableList.MultiSelect = false;
            this.lvVariableList.Name = "lvVariableList";
            this.lvVariableList.Size = new System.Drawing.Size(331, 233);
            this.lvVariableList.TabIndex = 3;
            this.lvVariableList.UseCompatibleStateImageBehavior = false;
            this.lvVariableList.View = System.Windows.Forms.View.Details;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(447, 313);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(50, 14);
            this.label5.TabIndex = 1;
            this.label5.Text = "Branch";
            // 
            // lvBranchList
            // 
            this.lvBranchList.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvBranchList.FullRowSelect = true;
            this.lvBranchList.GridLines = true;
            this.lvBranchList.Location = new System.Drawing.Point(450, 326);
            this.lvBranchList.MultiSelect = false;
            this.lvBranchList.Name = "lvBranchList";
            this.lvBranchList.Size = new System.Drawing.Size(331, 204);
            this.lvBranchList.TabIndex = 3;
            this.lvBranchList.UseCompatibleStateImageBehavior = false;
            this.lvBranchList.View = System.Windows.Forms.View.Details;
            // 
            // btnStartFile
            // 
            this.btnStartFile.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartFile.Location = new System.Drawing.Point(349, 77);
            this.btnStartFile.Name = "btnStartFile";
            this.btnStartFile.Size = new System.Drawing.Size(76, 32);
            this.btnStartFile.TabIndex = 2;
            this.btnStartFile.Text = "Start";
            this.btnStartFile.UseVisualStyleBackColor = true;
            this.btnStartFile.Click += new System.EventHandler(this.btnStartFile_Click);
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(9, 545);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(46, 14);
            this.labelStatus.TabIndex = 4;
            this.labelStatus.Text = "Ready";
            // 
            // cbxFile
            // 
            this.cbxFile.AutoSize = true;
            this.cbxFile.Location = new System.Drawing.Point(349, 119);
            this.cbxFile.Name = "cbxFile";
            this.cbxFile.Size = new System.Drawing.Size(79, 18);
            this.cbxFile.TabIndex = 5;
            this.cbxFile.Text = "check all";
            this.cbxFile.UseVisualStyleBackColor = true;
            this.cbxFile.CheckedChanged += new System.EventHandler(this.cbxFile_CheckedChanged);
            // 
            // btnStartFunction
            // 
            this.btnStartFunction.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartFunction.Location = new System.Drawing.Point(349, 250);
            this.btnStartFunction.Name = "btnStartFunction";
            this.btnStartFunction.Size = new System.Drawing.Size(76, 32);
            this.btnStartFunction.TabIndex = 2;
            this.btnStartFunction.Text = "Start";
            this.btnStartFunction.UseVisualStyleBackColor = true;
            this.btnStartFunction.Click += new System.EventHandler(this.btnStartFunction_Click);
            // 
            // cbxFunction
            // 
            this.cbxFunction.AutoSize = true;
            this.cbxFunction.Location = new System.Drawing.Point(349, 292);
            this.cbxFunction.Name = "cbxFunction";
            this.cbxFunction.Size = new System.Drawing.Size(79, 18);
            this.cbxFunction.TabIndex = 5;
            this.cbxFunction.Text = "check all";
            this.cbxFunction.UseVisualStyleBackColor = true;
            this.cbxFunction.CheckedChanged += new System.EventHandler(this.cbxFunction_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(793, 564);
            this.Controls.Add(this.cbxFunction);
            this.Controls.Add(this.cbxFile);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.lvBranchList);
            this.Controls.Add(this.lvVariableList);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lvFunctionList);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lvFileList);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnStartFunction);
            this.Controls.Add(this.btnStartFile);
            this.Controls.Add(this.btnSetFolder);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbxRootPath);
            this.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
		private System.Windows.Forms.Button btnStartFile;
		private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.CheckBox cbxFile;
        private System.Windows.Forms.Button btnStartFunction;
        private System.Windows.Forms.CheckBox cbxFunction;
	}
}

