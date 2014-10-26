namespace VisualizeMyLife
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.buttonTomatoTimer = new System.Windows.Forms.Button();
            this.buttonDiagram = new System.Windows.Forms.Button();
            this.buttonDailyRecord = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonTomatoTimer
            // 
            this.buttonTomatoTimer.Font = new System.Drawing.Font("SimHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonTomatoTimer.Location = new System.Drawing.Point(253, 12);
            this.buttonTomatoTimer.Name = "buttonTomatoTimer";
            this.buttonTomatoTimer.Size = new System.Drawing.Size(113, 44);
            this.buttonTomatoTimer.TabIndex = 1;
            this.buttonTomatoTimer.Text = "TomatoTimer";
            this.buttonTomatoTimer.UseVisualStyleBackColor = true;
            this.buttonTomatoTimer.Click += new System.EventHandler(this.buttonTomatoTimer_Click);
            // 
            // buttonDiagram
            // 
            this.buttonDiagram.Font = new System.Drawing.Font("SimHei", 9F);
            this.buttonDiagram.Location = new System.Drawing.Point(253, 78);
            this.buttonDiagram.Name = "buttonDiagram";
            this.buttonDiagram.Size = new System.Drawing.Size(113, 44);
            this.buttonDiagram.TabIndex = 2;
            this.buttonDiagram.Text = "Diagram";
            this.buttonDiagram.UseVisualStyleBackColor = true;
            this.buttonDiagram.Click += new System.EventHandler(this.buttonDiagram_Click);
            // 
            // buttonDailyRecord
            // 
            this.buttonDailyRecord.Font = new System.Drawing.Font("SimHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDailyRecord.Location = new System.Drawing.Point(253, 144);
            this.buttonDailyRecord.Name = "buttonDailyRecord";
            this.buttonDailyRecord.Size = new System.Drawing.Size(113, 44);
            this.buttonDailyRecord.TabIndex = 3;
            this.buttonDailyRecord.Text = "DailyRecord";
            this.buttonDailyRecord.UseVisualStyleBackColor = true;
            this.buttonDailyRecord.Click += new System.EventHandler(this.buttonDailyRecord_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(378, 311);
            this.Controls.Add(this.buttonDailyRecord);
            this.Controls.Add(this.buttonDiagram);
            this.Controls.Add(this.buttonTomatoTimer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "VisualizeMyLife";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonTomatoTimer;
        private System.Windows.Forms.Button buttonDiagram;
        private System.Windows.Forms.Button buttonDailyRecord;
    }
}

