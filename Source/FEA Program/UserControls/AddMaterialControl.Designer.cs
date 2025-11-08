namespace FEA_Program.UserControls
{
    partial class AddMaterialControl
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
            TextBox_Name = new TextBox();
            Label5 = new Label();
            ComboBox_SubTypes = new ComboBox();
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
            Label1.Size = new Size(110, 20);
            Label1.TabIndex = 0;
            Label1.Text = "Add Material";
            // 
            // Button_Accept
            // 
            Button_Accept.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Button_Accept.Enabled = false;
            Button_Accept.FlatStyle = FlatStyle.Popup;
            Button_Accept.Location = new Point(138, 5);
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
            Button_Cancel.Location = new Point(182, 5);
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
            Label2.Location = new Point(1, 77);
            Label2.Margin = new Padding(4, 0, 4, 0);
            Label2.Name = "Label2";
            Label2.Size = new Size(44, 16);
            Label2.TabIndex = 4;
            Label2.Text = "Name";
            // 
            // TextBox_Name
            // 
            TextBox_Name.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            TextBox_Name.Location = new Point(56, 76);
            TextBox_Name.Margin = new Padding(4, 3, 4, 3);
            TextBox_Name.Name = "TextBox_Name";
            TextBox_Name.Size = new Size(163, 23);
            TextBox_Name.TabIndex = 5;
            TextBox_Name.TextChanged += TextBox_Name_TextChanged;
            // 
            // Label5
            // 
            Label5.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Label5.AutoSize = true;
            Label5.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Label5.Location = new Point(2, 46);
            Label5.Margin = new Padding(4, 0, 4, 0);
            Label5.Name = "Label5";
            Label5.Size = new Size(39, 16);
            Label5.TabIndex = 8;
            Label5.Text = "Type";
            // 
            // ComboBox_SubTypes
            // 
            ComboBox_SubTypes.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_SubTypes.FormattingEnabled = true;
            ComboBox_SubTypes.Location = new Point(56, 45);
            ComboBox_SubTypes.Margin = new Padding(4, 3, 4, 3);
            ComboBox_SubTypes.Name = "ComboBox_SubTypes";
            ComboBox_SubTypes.Size = new Size(163, 23);
            ComboBox_SubTypes.TabIndex = 12;
            // 
            // AddMaterialControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(ComboBox_SubTypes);
            Controls.Add(Label5);
            Controls.Add(TextBox_Name);
            Controls.Add(Label2);
            Controls.Add(Button_Cancel);
            Controls.Add(Button_Accept);
            Controls.Add(Label1);
            Margin = new Padding(4, 3, 4, 3);
            Name = "AddMaterialControl";
            Size = new Size(227, 436);
            KeyDown += UserControl_AddElement_KeyDown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        internal Label Label1;
        internal Button Button_Accept;
        internal Button Button_Cancel;
        internal Label Label2;
        internal TextBox TextBox_Name;
        internal Label Label5;
        internal ComboBox ComboBox_SubTypes;
    }
}
