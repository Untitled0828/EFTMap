namespace EFTMap
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            panel1 = new Panel();
            Label_state = new Label();
            comboBox1 = new ComboBox();
            CB_AutoRemove = new CheckBox();
            CB_AutoSelect = new CheckBox();
            webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)webView).BeginInit();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Location = new Point(-4, -23);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1263, 961);
            tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(panel1);
            tabPage1.Controls.Add(webView);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1255, 933);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "맵";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.Controls.Add(Label_state);
            panel1.Controls.Add(comboBox1);
            panel1.Controls.Add(CB_AutoRemove);
            panel1.Controls.Add(CB_AutoSelect);
            panel1.Location = new Point(5, 4);
            panel1.Name = "panel1";
            panel1.Size = new Size(1900, 35);
            panel1.TabIndex = 7;
            // 
            // Label_state
            // 
            Label_state.AutoSize = true;
            Label_state.Location = new Point(409, 13);
            Label_state.Name = "Label_state";
            Label_state.Size = new Size(27, 15);
            Label_state.TabIndex = 7;
            Label_state.Text = "End";
            // 
            // comboBox1
            // 
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FlatStyle = FlatStyle.Popup;
            comboBox1.Font = new Font("맑은 고딕", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 129);
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(3, 3);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(156, 33);
            comboBox1.TabIndex = 3;
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            // 
            // CB_AutoRemove
            // 
            CB_AutoRemove.AutoSize = true;
            CB_AutoRemove.Font = new Font("맑은 고딕", 12F);
            CB_AutoRemove.Location = new Point(272, 7);
            CB_AutoRemove.Name = "CB_AutoRemove";
            CB_AutoRemove.Size = new Size(131, 25);
            CB_AutoRemove.TabIndex = 6;
            CB_AutoRemove.Text = "Auto Remove";
            CB_AutoRemove.UseVisualStyleBackColor = true;
            // 
            // CB_AutoSelect
            // 
            CB_AutoSelect.AutoSize = true;
            CB_AutoSelect.Font = new Font("맑은 고딕", 12F);
            CB_AutoSelect.Location = new Point(160, 7);
            CB_AutoSelect.Name = "CB_AutoSelect";
            CB_AutoSelect.Size = new Size(116, 25);
            CB_AutoSelect.TabIndex = 5;
            CB_AutoSelect.Text = "Auto Select";
            CB_AutoSelect.UseVisualStyleBackColor = true;
            // 
            // webView
            // 
            webView.AllowExternalDrop = true;
            webView.CreationProperties = null;
            webView.DefaultBackgroundColor = Color.White;
            webView.Location = new Point(5, 5);
            webView.Name = "webView";
            webView.Size = new Size(136, 80);
            webView.TabIndex = 0;
            webView.ZoomFactor = 1D;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1264, 941);
            Controls.Add(tabControl1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form1";
            Text = "EFTMap";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)webView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabPage1;
        private ComboBox comboBox1;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private CheckBox CB_AutoSelect;
        private CheckBox CB_AutoRemove;
        private Panel panel1;
        private Label Label_state;
    }
}
