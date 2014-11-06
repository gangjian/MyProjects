namespace AutoTester
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnStart = new System.Windows.Forms.Button();
            this.tbTestLog = new System.Windows.Forms.TextBox();
            this.tbxTargetFolder = new System.Windows.Forms.TextBox();
            this.btnSelTargetFolder = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.btnBuild = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.tbxMasterLogFolder = new System.Windows.Forms.TextBox();
            this.btnSelMasterLogFolder = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.label5 = new System.Windows.Forms.Label();
            this.cbxCheckAll = new System.Windows.Forms.CheckBox();
            this.listView2 = new System.Windows.Forms.ListView();
            this.label6 = new System.Windows.Forms.Label();
            this.tbxTestLogFolder = new System.Windows.Forms.TextBox();
            this.btnSelTestLogFolder = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.btnCompare = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSaveCSV = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnStart.Location = new System.Drawing.Point(621, 217);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(107, 53);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start Test";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // tbTestLog
            // 
            this.tbTestLog.Location = new System.Drawing.Point(161, 217);
            this.tbTestLog.Multiline = true;
            this.tbTestLog.Name = "tbTestLog";
            this.tbTestLog.ReadOnly = true;
            this.tbTestLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbTestLog.Size = new System.Drawing.Size(446, 245);
            this.tbTestLog.TabIndex = 1;
            // 
            // tbxTargetFolder
            // 
            this.tbxTargetFolder.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tbxTargetFolder.Location = new System.Drawing.Point(12, 33);
            this.tbxTargetFolder.Name = "tbxTargetFolder";
            this.tbxTargetFolder.ReadOnly = true;
            this.tbxTargetFolder.Size = new System.Drawing.Size(509, 23);
            this.tbxTargetFolder.TabIndex = 2;
            // 
            // btnSelTargetFolder
            // 
            this.btnSelTargetFolder.Location = new System.Drawing.Point(538, 33);
            this.btnSelTargetFolder.Name = "btnSelTargetFolder";
            this.btnSelTargetFolder.Size = new System.Drawing.Size(69, 23);
            this.btnSelTargetFolder.TabIndex = 3;
            this.btnSelTargetFolder.Text = "...";
            this.btnSelTargetFolder.UseVisualStyleBackColor = true;
            this.btnSelTargetFolder.Click += new System.EventHandler(this.btnSelTargetFolder_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(9, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(192, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "Target path(sim_integrated)";
            // 
            // treeView1
            // 
            this.treeView1.CheckBoxes = true;
            this.treeView1.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.treeView1.FullRowSelect = true;
            this.treeView1.Location = new System.Drawing.Point(12, 217);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(129, 495);
            this.treeView1.TabIndex = 5;
            this.treeView1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            // 
            // btnBuild
            // 
            this.btnBuild.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnBuild.Location = new System.Drawing.Point(621, 25);
            this.btnBuild.Name = "btnBuild";
            this.btnBuild.Size = new System.Drawing.Size(107, 38);
            this.btnBuild.TabIndex = 0;
            this.btnBuild.Text = "Build Target";
            this.btnBuild.UseVisualStyleBackColor = true;
            this.btnBuild.Click += new System.EventHandler(this.btnBuild_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 721);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(595, 23);
            this.progressBar1.TabIndex = 6;
            // 
            // tbxMasterLogFolder
            // 
            this.tbxMasterLogFolder.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tbxMasterLogFolder.Location = new System.Drawing.Point(18, 99);
            this.tbxMasterLogFolder.Name = "tbxMasterLogFolder";
            this.tbxMasterLogFolder.ReadOnly = true;
            this.tbxMasterLogFolder.Size = new System.Drawing.Size(503, 23);
            this.tbxMasterLogFolder.TabIndex = 2;
            // 
            // btnSelMasterLogFolder
            // 
            this.btnSelMasterLogFolder.Location = new System.Drawing.Point(538, 99);
            this.btnSelMasterLogFolder.Name = "btnSelMasterLogFolder";
            this.btnSelMasterLogFolder.Size = new System.Drawing.Size(69, 23);
            this.btnSelMasterLogFolder.TabIndex = 3;
            this.btnSelMasterLogFolder.Text = "...";
            this.btnSelMasterLogFolder.UseVisualStyleBackColor = true;
            this.btnSelMasterLogFolder.Click += new System.EventHandler(this.btnSelMasterLogFolder_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(3, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "Master logs path";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.Location = new System.Drawing.Point(9, 198);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "API list";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label4.Location = new System.Drawing.Point(158, 198);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(120, 16);
            this.label4.TabIndex = 4;
            this.label4.Text = "Test logs window";
            // 
            // listView1
            // 
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(161, 484);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(446, 228);
            this.listView1.TabIndex = 7;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label5.Location = new System.Drawing.Point(158, 465);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 16);
            this.label5.TabIndex = 4;
            this.label5.Text = "Statistics list";
            // 
            // cbxCheckAll
            // 
            this.cbxCheckAll.AutoSize = true;
            this.cbxCheckAll.Location = new System.Drawing.Point(71, 201);
            this.cbxCheckAll.Name = "cbxCheckAll";
            this.cbxCheckAll.Size = new System.Drawing.Size(70, 16);
            this.cbxCheckAll.TabIndex = 8;
            this.cbxCheckAll.Text = "check all";
            this.cbxCheckAll.UseVisualStyleBackColor = true;
            this.cbxCheckAll.CheckedChanged += new System.EventHandler(this.cbxCheckAll_CheckedChanged);
            // 
            // listView2
            // 
            this.listView2.GridLines = true;
            this.listView2.Location = new System.Drawing.Point(621, 555);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(113, 189);
            this.listView2.TabIndex = 9;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label6.Location = new System.Drawing.Point(618, 536);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(116, 16);
            this.label6.TabIndex = 4;
            this.label6.Text = "ChangedRegList";
            // 
            // tbxTestLogFolder
            // 
            this.tbxTestLogFolder.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tbxTestLogFolder.Location = new System.Drawing.Point(18, 150);
            this.tbxTestLogFolder.Name = "tbxTestLogFolder";
            this.tbxTestLogFolder.ReadOnly = true;
            this.tbxTestLogFolder.Size = new System.Drawing.Size(503, 23);
            this.tbxTestLogFolder.TabIndex = 2;
            // 
            // btnSelTestLogFolder
            // 
            this.btnSelTestLogFolder.Location = new System.Drawing.Point(538, 150);
            this.btnSelTestLogFolder.Name = "btnSelTestLogFolder";
            this.btnSelTestLogFolder.Size = new System.Drawing.Size(69, 23);
            this.btnSelTestLogFolder.TabIndex = 3;
            this.btnSelTestLogFolder.Text = "...";
            this.btnSelTestLogFolder.UseVisualStyleBackColor = true;
            this.btnSelTestLogFolder.Click += new System.EventHandler(this.btnSelTestLogFolder_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label7.Location = new System.Drawing.Point(3, 62);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(103, 16);
            this.label7.TabIndex = 4;
            this.label7.Text = "Test logs path";
            // 
            // btnCompare
            // 
            this.btnCompare.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnCompare.Location = new System.Drawing.Point(609, 47);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(107, 40);
            this.btnCompare.TabIndex = 0;
            this.btnCompare.Text = "Log Compare";
            this.btnCompare.UseVisualStyleBackColor = true;
            this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.btnCompare);
            this.groupBox1.Location = new System.Drawing.Point(12, 67);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(722, 124);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            // 
            // btnSaveCSV
            // 
            this.btnSaveCSV.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnSaveCSV.Location = new System.Drawing.Point(621, 484);
            this.btnSaveCSV.Name = "btnSaveCSV";
            this.btnSaveCSV.Size = new System.Drawing.Size(107, 36);
            this.btnSaveCSV.TabIndex = 0;
            this.btnSaveCSV.Text = "Save CSV";
            this.btnSaveCSV.UseVisualStyleBackColor = true;
            this.btnSaveCSV.Click += new System.EventHandler(this.btnSaveCSV_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(743, 756);
            this.Controls.Add(this.listView2);
            this.Controls.Add(this.cbxCheckAll);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSelTestLogFolder);
            this.Controls.Add(this.btnSelMasterLogFolder);
            this.Controls.Add(this.btnSelTargetFolder);
            this.Controls.Add(this.tbxTestLogFolder);
            this.Controls.Add(this.tbxMasterLogFolder);
            this.Controls.Add(this.tbxTargetFolder);
            this.Controls.Add(this.tbTestLog);
            this.Controls.Add(this.btnBuild);
            this.Controls.Add(this.btnSaveCSV);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AutoTester v0.34";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.TextBox tbTestLog;
        private System.Windows.Forms.TextBox tbxTargetFolder;
        private System.Windows.Forms.Button btnSelTargetFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button btnBuild;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox tbxMasterLogFolder;
        private System.Windows.Forms.Button btnSelMasterLogFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox cbxCheckAll;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbxTestLogFolder;
        private System.Windows.Forms.Button btnSelTestLogFolder;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnCompare;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSaveCSV;
    }
}

