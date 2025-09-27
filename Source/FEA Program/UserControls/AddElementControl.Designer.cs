namespace FEA_Program.UserControls
{
    partial class AddElementControl
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
            _Label1 = new Label();
            _Button_Accept = new Button();
            _Button_Cancel = new Button();
            _Label2 = new Label();
            _ComboBox_ElemType = new ComboBox();
            _ComboBox_Material = new ComboBox();
            _Label3 = new Label();
            SuspendLayout();
            // 
            // _Label1
            // 
            _Label1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _Label1.AutoSize = true;
            _Label1.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _Label1.Location = new Point(4, 6);
            _Label1.Margin = new Padding(4, 0, 4, 0);
            _Label1.Name = "_Label1";
            _Label1.Size = new Size(112, 20);
            _Label1.TabIndex = 0;
            _Label1.Text = "Add Element";
            // 
            // _Button_Accept
            // 
            _Button_Accept.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _Button_Accept.FlatStyle = FlatStyle.Popup;
            _Button_Accept.Location = new Point(155, 5);
            _Button_Accept.Margin = new Padding(4, 3, 4, 3);
            _Button_Accept.Name = "_Button_Accept";
            _Button_Accept.Size = new Size(37, 27);
            _Button_Accept.TabIndex = 1;
            _Button_Accept.Text = "Y";
            _Button_Accept.UseVisualStyleBackColor = true;
            // 
            // _Button_Cancel
            // 
            _Button_Cancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _Button_Cancel.FlatStyle = FlatStyle.Popup;
            _Button_Cancel.Location = new Point(200, 5);
            _Button_Cancel.Margin = new Padding(4, 3, 4, 3);
            _Button_Cancel.Name = "_Button_Cancel";
            _Button_Cancel.Size = new Size(37, 27);
            _Button_Cancel.TabIndex = 2;
            _Button_Cancel.Text = "N";
            _Button_Cancel.UseVisualStyleBackColor = true;
            // 
            // _Label2
            // 
            _Label2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _Label2.AutoSize = true;
            _Label2.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            _Label2.Location = new Point(5, 53);
            _Label2.Margin = new Padding(4, 0, 4, 0);
            _Label2.Name = "_Label2";
            _Label2.Size = new Size(91, 16);
            _Label2.TabIndex = 3;
            _Label2.Text = "Element Type";
            // 
            // _ComboBox_ElemType
            // 
            _ComboBox_ElemType.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _ComboBox_ElemType.DropDownStyle = ComboBoxStyle.DropDownList;
            _ComboBox_ElemType.FormattingEnabled = true;
            _ComboBox_ElemType.Location = new Point(8, 75);
            _ComboBox_ElemType.Margin = new Padding(4, 3, 4, 3);
            _ComboBox_ElemType.Name = "_ComboBox_ElemType";
            _ComboBox_ElemType.Size = new Size(228, 23);
            _ComboBox_ElemType.TabIndex = 4;
            // 
            // _ComboBox_Material
            // 
            _ComboBox_Material.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _ComboBox_Material.DropDownStyle = ComboBoxStyle.DropDownList;
            _ComboBox_Material.FormattingEnabled = true;
            _ComboBox_Material.Location = new Point(8, 133);
            _ComboBox_Material.Margin = new Padding(4, 3, 4, 3);
            _ComboBox_Material.Name = "_ComboBox_Material";
            _ComboBox_Material.Size = new Size(228, 23);
            _ComboBox_Material.TabIndex = 5;
            // 
            // _Label3
            // 
            _Label3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _Label3.AutoSize = true;
            _Label3.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            _Label3.Location = new Point(9, 111);
            _Label3.Margin = new Padding(4, 0, 4, 0);
            _Label3.Name = "_Label3";
            _Label3.Size = new Size(55, 16);
            _Label3.TabIndex = 6;
            _Label3.Text = "Material";
            // 
            // AddElementControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(_Label3);
            Controls.Add(_ComboBox_Material);
            Controls.Add(_ComboBox_ElemType);
            Controls.Add(_Label2);
            Controls.Add(_Button_Cancel);
            Controls.Add(_Button_Accept);
            Controls.Add(_Label1);
            Margin = new Padding(4, 3, 4, 3);
            Name = "AddElementControl";
            Size = new Size(245, 493);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label _Label1;
        private Label _Label2;
        private Label _Label3;
        private Button _Button_Accept;
        private Button _Button_Cancel;
        private ComboBox _ComboBox_Material;
        private ComboBox _ComboBox_ElemType;
    }
}
