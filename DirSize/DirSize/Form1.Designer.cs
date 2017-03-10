namespace DirSize
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
			this.tbxRootPath = new System.Windows.Forms.TextBox();
			this.btnBrowseFolder = new System.Windows.Forms.Button();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.SuspendLayout();
			// 
			// tbxRootPath
			// 
			this.tbxRootPath.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tbxRootPath.Location = new System.Drawing.Point(12, 27);
			this.tbxRootPath.Name = "tbxRootPath";
			this.tbxRootPath.ReadOnly = true;
			this.tbxRootPath.Size = new System.Drawing.Size(548, 23);
			this.tbxRootPath.TabIndex = 0;
			// 
			// btnBrowseFolder
			// 
			this.btnBrowseFolder.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnBrowseFolder.Location = new System.Drawing.Point(566, 26);
			this.btnBrowseFolder.Name = "btnBrowseFolder";
			this.btnBrowseFolder.Size = new System.Drawing.Size(75, 23);
			this.btnBrowseFolder.TabIndex = 1;
			this.btnBrowseFolder.Text = "...";
			this.btnBrowseFolder.UseVisualStyleBackColor = true;
			this.btnBrowseFolder.Click += new System.EventHandler(this.btnBrowseFolder_Click);
			// 
			// treeView1
			// 
			this.treeView1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.treeView1.Location = new System.Drawing.Point(12, 67);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(548, 413);
			this.treeView1.TabIndex = 2;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(656, 492);
			this.Controls.Add(this.treeView1);
			this.Controls.Add(this.btnBrowseFolder);
			this.Controls.Add(this.tbxRootPath);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox tbxRootPath;
		private System.Windows.Forms.Button btnBrowseFolder;
		private System.Windows.Forms.TreeView treeView1;
	}
}

