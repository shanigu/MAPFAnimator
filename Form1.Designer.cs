namespace MAPFAnimator
{
    partial class MAPFAnimator
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            picMap = new PictureBox();
            btnLoad = new Button();
            lblMapName = new Label();
            btnStart = new Button();
            chkStop = new CheckBox();
            tbSpeed = new TrackBar();
            label1 = new Label();
            label2 = new Label();
            lblStep = new Label();
            ((System.ComponentModel.ISupportInitialize)picMap).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tbSpeed).BeginInit();
            SuspendLayout();
            // 
            // picMap
            // 
            picMap.Location = new Point(12, 48);
            picMap.Name = "picMap";
            picMap.Size = new Size(800, 800);
            picMap.TabIndex = 0;
            picMap.TabStop = false;
            picMap.Paint += picMap_Paint;
            // 
            // btnLoad
            // 
            btnLoad.Location = new Point(12, 8);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(112, 34);
            btnLoad.TabIndex = 1;
            btnLoad.Text = "Load";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += btnLoad_Click;
            // 
            // lblMapName
            // 
            lblMapName.AutoSize = true;
            lblMapName.Location = new Point(130, 13);
            lblMapName.Name = "lblMapName";
            lblMapName.Size = new Size(97, 25);
            lblMapName.TabIndex = 2;
            lblMapName.Text = "map name";
            // 
            // btnStart
            // 
            btnStart.Location = new Point(10, 873);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(112, 34);
            btnStart.TabIndex = 3;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // chkStop
            // 
            chkStop.AutoSize = true;
            chkStop.Location = new Point(653, 873);
            chkStop.Name = "chkStop";
            chkStop.Size = new Size(155, 29);
            chkStop.TabIndex = 4;
            chkStop.Text = "Stop on replan";
            chkStop.UseVisualStyleBackColor = true;
            // 
            // tbSpeed
            // 
            tbSpeed.Location = new Point(357, 872);
            tbSpeed.Maximum = 20;
            tbSpeed.Name = "tbSpeed";
            tbSpeed.Size = new Size(191, 69);
            tbSpeed.TabIndex = 5;
            tbSpeed.Value = 10;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(299, 877);
            label1.Name = "label1";
            label1.Size = new Size(65, 25);
            label1.TabIndex = 6;
            label1.Text = "Slower";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(543, 877);
            label2.Name = "label2";
            label2.Size = new Size(58, 25);
            label2.TabIndex = 7;
            label2.Text = "Faster";
            // 
            // lblStep
            // 
            lblStep.AutoSize = true;
            lblStep.Location = new Point(146, 877);
            lblStep.Name = "lblStep";
            lblStep.Size = new Size(47, 25);
            lblStep.TabIndex = 8;
            lblStep.Text = "Step";
            // 
            // MAPFAnimator
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(820, 919);
            Controls.Add(lblStep);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(tbSpeed);
            Controls.Add(chkStop);
            Controls.Add(btnStart);
            Controls.Add(lblMapName);
            Controls.Add(btnLoad);
            Controls.Add(picMap);
            Name = "MAPFAnimator";
            Text = "MAPF Animator";
            Load += MAPFAnimator_Load;
            ((System.ComponentModel.ISupportInitialize)picMap).EndInit();
            ((System.ComponentModel.ISupportInitialize)tbSpeed).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox picMap;
        private Button btnLoad;
        private Label lblMapName;
        private Button btnStart;
        private CheckBox chkStop;
        private TrackBar tbSpeed;
        private Label label1;
        private Label label2;
        private Label lblStep;
    }
}