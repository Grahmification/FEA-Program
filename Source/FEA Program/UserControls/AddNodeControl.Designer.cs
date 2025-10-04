namespace FEA_Program.UserControls
{
    partial class AddNodeControl
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
            Label_title = new Label();
            Button_Accept = new Button();
            Button_Cancel = new Button();
            Label2 = new Label();
            Label4 = new Label();
            Button_FixAll = new Button();
            Button_UnfixAll = new Button();
            SuspendLayout();
            // 
            // Label_title
            // 
            Label_title.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Label_title.AutoSize = true;
            Label_title.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Label_title.Location = new Point(4, 6);
            Label_title.Margin = new Padding(4, 0, 4, 0);
            Label_title.Name = "Label_title";
            Label_title.Size = new Size(88, 20);
            Label_title.TabIndex = 0;
            Label_title.Text = "Add Node";
            // 
            // Button_Accept
            // 
            Button_Accept.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Button_Accept.FlatStyle = FlatStyle.Popup;
            Button_Accept.Location = new Point(113, 5);
            Button_Accept.Margin = new Padding(4, 3, 4, 3);
            Button_Accept.Name = "Button_Accept";
            Button_Accept.Size = new Size(37, 27);
            Button_Accept.TabIndex = 1;
            Button_Accept.Text = "Y";
            Button_Accept.UseVisualStyleBackColor = true;
            Button_Accept.Click += ButtonAccept_Click;
            // 
            // Button_Cancel
            // 
            Button_Cancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Button_Cancel.FlatStyle = FlatStyle.Popup;
            Button_Cancel.Location = new Point(158, 5);
            Button_Cancel.Margin = new Padding(4, 3, 4, 3);
            Button_Cancel.Name = "Button_Cancel";
            Button_Cancel.Size = new Size(37, 27);
            Button_Cancel.TabIndex = 2;
            Button_Cancel.Text = "N";
            Button_Cancel.UseVisualStyleBackColor = true;
            Button_Cancel.Click += ButtonAccept_Click;
            // 
            // Label2
            // 
            Label2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Label2.AutoSize = true;
            Label2.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label2.Location = new Point(35, 42);
            Label2.Margin = new Padding(4, 0, 4, 0);
            Label2.Name = "Label2";
            Label2.Size = new Size(88, 16);
            Label2.TabIndex = 3;
            Label2.Text = "Position (mm)";
            // 
            // Label4
            // 
            Label4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Label4.AutoSize = true;
            Label4.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label4.Location = new Point(164, 42);
            Label4.Margin = new Padding(4, 0, 4, 0);
            Label4.Name = "Label4";
            Label4.Size = new Size(24, 16);
            Label4.TabIndex = 5;
            Label4.Text = "Fix";
            // 
            // Button_FixAll
            // 
            Button_FixAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Button_FixAll.Location = new Point(113, 74);
            Button_FixAll.Margin = new Padding(4, 3, 4, 3);
            Button_FixAll.Name = "Button_FixAll";
            Button_FixAll.Size = new Size(88, 27);
            Button_FixAll.TabIndex = 6;
            Button_FixAll.Text = "Fix All";
            Button_FixAll.UseVisualStyleBackColor = true;
            Button_FixAll.Click += Button_FixFloat_Click;
            // 
            // Button_UnfixAll
            // 
            Button_UnfixAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Button_UnfixAll.Location = new Point(113, 105);
            Button_UnfixAll.Margin = new Padding(4, 3, 4, 3);
            Button_UnfixAll.Name = "Button_UnfixAll";
            Button_UnfixAll.Size = new Size(88, 27);
            Button_UnfixAll.TabIndex = 7;
            Button_UnfixAll.Text = "Unfix All";
            Button_UnfixAll.UseVisualStyleBackColor = true;
            Button_UnfixAll.Click += Button_FixFloat_Click;
            // 
            // AddNodeControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(Button_UnfixAll);
            Controls.Add(Button_FixAll);
            Controls.Add(Label4);
            Controls.Add(Label2);
            Controls.Add(Button_Cancel);
            Controls.Add(Button_Accept);
            Controls.Add(Label_title);
            Margin = new Padding(4, 3, 4, 3);
            Name = "AddNodeControl";
            Size = new Size(203, 436);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        internal Label Label_title;
        internal Button Button_Accept;
        internal Button Button_Cancel;
        internal Label Label2;
        internal Label Label4;
        internal Button Button_FixAll;
        internal Button Button_UnfixAll;
    }
}
