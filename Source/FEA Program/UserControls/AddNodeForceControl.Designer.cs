namespace FEA_Program.UserControls
{
    partial class AddNodeForceControl
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
            Label1 = new Label();
            Button_Accept = new Button();
            Button_Cancel = new Button();
            Label2 = new Label();
            CheckedListBox_ApplyNodes = new CheckedListBox();
            SuspendLayout();
            // 
            // Label1
            // 
            Label1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Label1.AutoSize = true;
            Label1.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Label1.Location = new Point(4, 6);
            Label1.Margin = new Padding(4, 0, 4, 0);
            Label1.Name = "Label1";
            Label1.Size = new Size(139, 20);
            Label1.TabIndex = 0;
            Label1.Text = "Add Node Force";
            // 
            // Button_Accept
            // 
            Button_Accept.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Button_Accept.FlatStyle = FlatStyle.Popup;
            Button_Accept.Location = new Point(175, 5);
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
            Button_Cancel.Location = new Point(219, 5);
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
            Label2.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label2.Location = new Point(5, 60);
            Label2.Margin = new Padding(4, 0, 4, 0);
            Label2.Name = "Label2";
            Label2.Size = new Size(88, 15);
            Label2.TabIndex = 3;
            Label2.Text = "Nodes to Apply";
            // 
            // CheckedListBox_ApplyNodes
            // 
            CheckedListBox_ApplyNodes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            CheckedListBox_ApplyNodes.CheckOnClick = true;
            CheckedListBox_ApplyNodes.FormattingEnabled = true;
            CheckedListBox_ApplyNodes.Location = new Point(8, 81);
            CheckedListBox_ApplyNodes.Margin = new Padding(4, 3, 4, 3);
            CheckedListBox_ApplyNodes.Name = "CheckedListBox_ApplyNodes";
            CheckedListBox_ApplyNodes.Size = new Size(248, 400);
            CheckedListBox_ApplyNodes.TabIndex = 4;
            CheckedListBox_ApplyNodes.MouseUp += CheckedListBox_ApplyNodes_MouseUp;
            // 
            // AddNodeForceControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(CheckedListBox_ApplyNodes);
            Controls.Add(Label2);
            Controls.Add(Button_Cancel);
            Controls.Add(Button_Accept);
            Controls.Add(Label1);
            Margin = new Padding(4, 3, 4, 3);
            Name = "AddNodeForceControl";
            Size = new Size(265, 493);
            KeyDown += UserControl_AddElement_KeyDown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        internal Label Label1;
        internal Button Button_Accept;
        internal Button Button_Cancel;
        internal Label Label2;
        internal CheckedListBox CheckedListBox_ApplyNodes;
    }
}
