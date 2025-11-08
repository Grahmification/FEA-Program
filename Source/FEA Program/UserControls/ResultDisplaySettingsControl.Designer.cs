namespace FEA_Program.UserControls
{
    partial class ResultDisplaySettingsControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            checkBox_displaced = new CheckBox();
            label2 = new Label();
            trackBar_displace = new TrackBar();
            label3 = new Label();
            numericUpDown_scalingfactor = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)trackBar_displace).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_scalingfactor).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(4, 7);
            label1.Name = "label1";
            label1.Size = new Size(130, 15);
            label1.TabIndex = 0;
            label1.Text = "Results Display Settings";
            // 
            // checkBox_displaced
            // 
            checkBox_displaced.AutoSize = true;
            checkBox_displaced.Location = new Point(23, 36);
            checkBox_displaced.Name = "checkBox_displaced";
            checkBox_displaced.Size = new Size(112, 19);
            checkBox_displaced.TabIndex = 5;
            checkBox_displaced.Text = "Draw Displaced?";
            checkBox_displaced.UseVisualStyleBackColor = true;
            checkBox_displaced.CheckedChanged += checkBox_displaced_CheckedChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(18, 66);
            label2.Name = "label2";
            label2.Size = new Size(95, 15);
            label2.TabIndex = 4;
            label2.Text = "Displacement %:";
            // 
            // trackBar_displace
            // 
            trackBar_displace.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBar_displace.Location = new Point(12, 89);
            trackBar_displace.Maximum = 100;
            trackBar_displace.Name = "trackBar_displace";
            trackBar_displace.Size = new Size(231, 45);
            trackBar_displace.TabIndex = 3;
            trackBar_displace.TickStyle = TickStyle.None;
            trackBar_displace.ValueChanged += trackBar_displace_ValueChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(18, 122);
            label3.Name = "label3";
            label3.Size = new Size(123, 15);
            label3.TabIndex = 6;
            label3.Text = "Displacement Scaling:";
            // 
            // numericUpDown_scalingfactor
            // 
            numericUpDown_scalingfactor.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            numericUpDown_scalingfactor.Increment = new decimal(new int[] { 100, 0, 0, 0 });
            numericUpDown_scalingfactor.Location = new Point(23, 145);
            numericUpDown_scalingfactor.Maximum = new decimal(new int[] { 1874919424, 2328306, 0, 0 });
            numericUpDown_scalingfactor.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown_scalingfactor.Name = "numericUpDown_scalingfactor";
            numericUpDown_scalingfactor.Size = new Size(208, 23);
            numericUpDown_scalingfactor.TabIndex = 7;
            numericUpDown_scalingfactor.Value = new decimal(new int[] { 1000, 0, 0, 0 });
            numericUpDown_scalingfactor.ValueChanged += numericUpDown_scalingfactor_ValueChanged;
            // 
            // ResultDisplaySettingsControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightGray;
            Controls.Add(numericUpDown_scalingfactor);
            Controls.Add(label3);
            Controls.Add(checkBox_displaced);
            Controls.Add(label2);
            Controls.Add(trackBar_displace);
            Controls.Add(label1);
            Name = "ResultDisplaySettingsControl";
            Size = new Size(258, 233);
            ((System.ComponentModel.ISupportInitialize)trackBar_displace).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_scalingfactor).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private CheckBox checkBox_displaced;
        private Label label2;
        private TrackBar trackBar_displace;
        private Label label3;
        private NumericUpDown numericUpDown_scalingfactor;
    }
}
