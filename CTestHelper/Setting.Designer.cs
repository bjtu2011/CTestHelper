namespace CTestHelper
{
    partial class Setting
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
            this.components = new System.ComponentModel.Container();
            this.DataFolder = new CCWin.SkinControl.SkinTextBox();
            this.skinLabel1 = new CCWin.SkinControl.SkinLabel();
            this.skinPictureBox1 = new CCWin.SkinControl.SkinPictureBox();
            this.skinButton1 = new CCWin.SkinControl.SkinButton();
            this.skinLabel2 = new CCWin.SkinControl.SkinLabel();
            this.ChooseInstrument = new CCWin.SkinControl.SkinComboBox();
            this.skinLabel3 = new CCWin.SkinControl.SkinLabel();
            this.MonitorFileType = new CCWin.SkinControl.SkinComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.skinPictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // DataFolder
            // 
            this.DataFolder.BackColor = System.Drawing.Color.Transparent;
            this.DataFolder.CausesValidation = false;
            this.DataFolder.DownBack = null;
            this.DataFolder.Enabled = false;
            this.DataFolder.Icon = null;
            this.DataFolder.IconIsButton = false;
            this.DataFolder.IconMouseState = CCWin.SkinClass.ControlState.Normal;
            this.DataFolder.IsPasswordChat = '\0';
            this.DataFolder.IsSystemPasswordChar = false;
            this.DataFolder.Lines = new string[0];
            this.DataFolder.Location = new System.Drawing.Point(126, 102);
            this.DataFolder.Margin = new System.Windows.Forms.Padding(0);
            this.DataFolder.MaxLength = 32767;
            this.DataFolder.MinimumSize = new System.Drawing.Size(28, 28);
            this.DataFolder.MouseBack = null;
            this.DataFolder.MouseState = CCWin.SkinClass.ControlState.Normal;
            this.DataFolder.Multiline = false;
            this.DataFolder.Name = "DataFolder";
            this.DataFolder.NormlBack = null;
            this.DataFolder.Padding = new System.Windows.Forms.Padding(5);
            this.DataFolder.ReadOnly = true;
            this.DataFolder.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.DataFolder.Size = new System.Drawing.Size(297, 28);
            // 
            // 
            // 
            this.DataFolder.SkinTxt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.DataFolder.SkinTxt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataFolder.SkinTxt.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.DataFolder.SkinTxt.Location = new System.Drawing.Point(5, 5);
            this.DataFolder.SkinTxt.Name = "BaseText";
            this.DataFolder.SkinTxt.ReadOnly = true;
            this.DataFolder.SkinTxt.Size = new System.Drawing.Size(287, 18);
            this.DataFolder.SkinTxt.TabIndex = 0;
            this.DataFolder.SkinTxt.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.DataFolder.SkinTxt.WaterText = "选择存储数据的文件夹";
            this.DataFolder.TabIndex = 0;
            this.DataFolder.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.DataFolder.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.DataFolder.WaterText = "选择存储数据的文件夹";
            this.DataFolder.WordWrap = true;
            // 
            // skinLabel1
            // 
            this.skinLabel1.AutoSize = true;
            this.skinLabel1.BackColor = System.Drawing.Color.Transparent;
            this.skinLabel1.BorderColor = System.Drawing.Color.White;
            this.skinLabel1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinLabel1.Location = new System.Drawing.Point(34, 108);
            this.skinLabel1.Name = "skinLabel1";
            this.skinLabel1.Size = new System.Drawing.Size(80, 17);
            this.skinLabel1.TabIndex = 1;
            this.skinLabel1.Text = "数据文件夹：";
            // 
            // skinPictureBox1
            // 
            this.skinPictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.skinPictureBox1.Image = global::CTestHelper.Properties.Resources.folder_ico;
            this.skinPictureBox1.Location = new System.Drawing.Point(426, 102);
            this.skinPictureBox1.Name = "skinPictureBox1";
            this.skinPictureBox1.Size = new System.Drawing.Size(33, 28);
            this.skinPictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.skinPictureBox1.TabIndex = 2;
            this.skinPictureBox1.TabStop = false;
            this.skinPictureBox1.Click += new System.EventHandler(this.skinPictureBox1_Click);
            // 
            // skinButton1
            // 
            this.skinButton1.BackColor = System.Drawing.Color.Transparent;
            this.skinButton1.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinButton1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.skinButton1.DownBack = null;
            this.skinButton1.Location = new System.Drawing.Point(191, 193);
            this.skinButton1.MouseBack = null;
            this.skinButton1.Name = "skinButton1";
            this.skinButton1.NormlBack = null;
            this.skinButton1.Size = new System.Drawing.Size(86, 39);
            this.skinButton1.TabIndex = 3;
            this.skinButton1.Text = "确定";
            this.skinButton1.UseVisualStyleBackColor = false;
            this.skinButton1.Click += new System.EventHandler(this.skinButton1_Click);
            // 
            // skinLabel2
            // 
            this.skinLabel2.AutoSize = true;
            this.skinLabel2.BackColor = System.Drawing.Color.Transparent;
            this.skinLabel2.BorderColor = System.Drawing.Color.White;
            this.skinLabel2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinLabel2.Location = new System.Drawing.Point(34, 68);
            this.skinLabel2.Name = "skinLabel2";
            this.skinLabel2.Size = new System.Drawing.Size(68, 17);
            this.skinLabel2.TabIndex = 4;
            this.skinLabel2.Text = "仪器设备：";
            // 
            // ChooseInstrument
            // 
            this.ChooseInstrument.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.ChooseInstrument.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ChooseInstrument.FormattingEnabled = true;
            this.ChooseInstrument.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChooseInstrument.Items.AddRange(new object[] {
            "W110",
            "W140"});
            this.ChooseInstrument.Location = new System.Drawing.Point(126, 68);
            this.ChooseInstrument.Name = "ChooseInstrument";
            this.ChooseInstrument.Size = new System.Drawing.Size(297, 22);
            this.ChooseInstrument.TabIndex = 5;
            this.ChooseInstrument.WaterText = "";
            // 
            // skinLabel3
            // 
            this.skinLabel3.AutoSize = true;
            this.skinLabel3.BackColor = System.Drawing.Color.Transparent;
            this.skinLabel3.BorderColor = System.Drawing.Color.White;
            this.skinLabel3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinLabel3.Location = new System.Drawing.Point(34, 149);
            this.skinLabel3.Name = "skinLabel3";
            this.skinLabel3.Size = new System.Drawing.Size(92, 17);
            this.skinLabel3.TabIndex = 6;
            this.skinLabel3.Text = "监控文件类型：";
            // 
            // MonitorFileType
            // 
            this.MonitorFileType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.MonitorFileType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MonitorFileType.FormattingEnabled = true;
            this.MonitorFileType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.MonitorFileType.Items.AddRange(new object[] {
            "*.mdb",
            "*.csv"});
            this.MonitorFileType.Location = new System.Drawing.Point(126, 144);
            this.MonitorFileType.Name = "MonitorFileType";
            this.MonitorFileType.Size = new System.Drawing.Size(297, 22);
            this.MonitorFileType.TabIndex = 7;
            this.MonitorFileType.WaterText = "";
            // 
            // Setting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 258);
            this.Controls.Add(this.MonitorFileType);
            this.Controls.Add(this.skinLabel3);
            this.Controls.Add(this.ChooseInstrument);
            this.Controls.Add(this.skinLabel2);
            this.Controls.Add(this.skinButton1);
            this.Controls.Add(this.skinPictureBox1);
            this.Controls.Add(this.skinLabel1);
            this.Controls.Add(this.DataFolder);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Setting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "设置";
            this.Load += new System.EventHandler(this.Setting_Load);
            ((System.ComponentModel.ISupportInitialize)(this.skinPictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CCWin.SkinControl.SkinTextBox DataFolder;
        private CCWin.SkinControl.SkinLabel skinLabel1;
        private CCWin.SkinControl.SkinPictureBox skinPictureBox1;
        private CCWin.SkinControl.SkinButton skinButton1;
        private CCWin.SkinControl.SkinLabel skinLabel2;
        private CCWin.SkinControl.SkinComboBox ChooseInstrument;
        private CCWin.SkinControl.SkinLabel skinLabel3;
        private CCWin.SkinControl.SkinComboBox MonitorFileType;
    }
}