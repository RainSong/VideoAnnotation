namespace VideoAnnotation
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.Menu = new System.Windows.Forms.MenuStrip();
            this.MenuItemFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemOpenFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemOpenFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemAddAnnotation = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.FolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.DataGridFiles = new System.Windows.Forms.DataGridView();
            this.ColID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColFileFullName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.vlcPlayer = new Vlc.DotNet.Forms.VlcControl();
            this.panelRight = new System.Windows.Forms.Panel();
            this.panelRightTop = new System.Windows.Forms.Panel();
            this.panelPlayer = new System.Windows.Forms.Panel();
            this.panelPlayControl = new System.Windows.Forms.Panel();
            this.labelVideoPosition = new System.Windows.Forms.Label();
            this.trackBarPosition = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.panelRightBottom = new System.Windows.Forms.Panel();
            this.listViewAnnotation = new System.Windows.Forms.ListView();
            this.Menu.SuspendLayout();
            this.panelLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridFiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.vlcPlayer)).BeginInit();
            this.panelRight.SuspendLayout();
            this.panelRightTop.SuspendLayout();
            this.panelPlayer.SuspendLayout();
            this.panelPlayControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarPosition)).BeginInit();
            this.panelRightBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // Menu
            // 
            this.Menu.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Menu.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemFile,
            this.MenuItemAddAnnotation});
            this.Menu.Location = new System.Drawing.Point(0, 0);
            this.Menu.Name = "Menu";
            this.Menu.Size = new System.Drawing.Size(1146, 25);
            this.Menu.TabIndex = 0;
            this.Menu.Text = "menuStrip1";
            // 
            // MenuItemFile
            // 
            this.MenuItemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemOpen});
            this.MenuItemFile.Name = "MenuItemFile";
            this.MenuItemFile.Size = new System.Drawing.Size(44, 21);
            this.MenuItemFile.Text = "文件";
            // 
            // MenuItemOpen
            // 
            this.MenuItemOpen.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemOpenFile,
            this.MenuItemOpenFolder});
            this.MenuItemOpen.Name = "MenuItemOpen";
            this.MenuItemOpen.Size = new System.Drawing.Size(100, 22);
            this.MenuItemOpen.Text = "打开";
            // 
            // MenuItemOpenFile
            // 
            this.MenuItemOpenFile.Name = "MenuItemOpenFile";
            this.MenuItemOpenFile.Size = new System.Drawing.Size(112, 22);
            this.MenuItemOpenFile.Text = "文件";
            this.MenuItemOpenFile.Click += new System.EventHandler(this.MenuItemOpenFile_Click);
            // 
            // MenuItemOpenFolder
            // 
            this.MenuItemOpenFolder.Name = "MenuItemOpenFolder";
            this.MenuItemOpenFolder.Size = new System.Drawing.Size(112, 22);
            this.MenuItemOpenFolder.Text = "文件夹";
            this.MenuItemOpenFolder.Click += new System.EventHandler(this.MenuItemOpenFolder_Click);
            // 
            // MenuItemAddAnnotation
            // 
            this.MenuItemAddAnnotation.Name = "MenuItemAddAnnotation";
            this.MenuItemAddAnnotation.Size = new System.Drawing.Size(68, 21);
            this.MenuItemAddAnnotation.Text = "添加注解";
            this.MenuItemAddAnnotation.Click += new System.EventHandler(this.MenuItemAddAnnotation_Click);
            // 
            // FolderBrowserDialog
            // 
            this.FolderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // panelLeft
            // 
            this.panelLeft.Controls.Add(this.DataGridFiles);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 25);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(310, 657);
            this.panelLeft.TabIndex = 1;
            // 
            // DataGridFiles
            // 
            this.DataGridFiles.AllowUserToAddRows = false;
            this.DataGridFiles.AllowUserToDeleteRows = false;
            this.DataGridFiles.AllowUserToResizeColumns = false;
            this.DataGridFiles.AllowUserToResizeRows = false;
            this.DataGridFiles.BackgroundColor = System.Drawing.SystemColors.Control;
            this.DataGridFiles.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.DataGridFiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridFiles.ColumnHeadersVisible = false;
            this.DataGridFiles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColID,
            this.ColFileFullName,
            this.colFileName});
            this.DataGridFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataGridFiles.Location = new System.Drawing.Point(0, 0);
            this.DataGridFiles.MultiSelect = false;
            this.DataGridFiles.Name = "DataGridFiles";
            this.DataGridFiles.RowHeadersVisible = false;
            this.DataGridFiles.RowTemplate.Height = 23;
            this.DataGridFiles.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.DataGridFiles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DataGridFiles.Size = new System.Drawing.Size(310, 657);
            this.DataGridFiles.TabIndex = 0;
            this.DataGridFiles.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridFiles_CellDoubleClick);
            // 
            // ColID
            // 
            this.ColID.DataPropertyName = "id";
            this.ColID.HeaderText = "FieldID";
            this.ColID.Name = "ColID";
            this.ColID.ReadOnly = true;
            this.ColID.Visible = false;
            // 
            // ColFileFullName
            // 
            this.ColFileFullName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColFileFullName.DataPropertyName = "file_full_name";
            this.ColFileFullName.HeaderText = "FileFullName";
            this.ColFileFullName.Name = "ColFileFullName";
            this.ColFileFullName.ReadOnly = true;
            this.ColFileFullName.Visible = false;
            // 
            // colFileName
            // 
            this.colFileName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colFileName.DataPropertyName = "file_name";
            this.colFileName.HeaderText = "FileName";
            this.colFileName.Name = "colFileName";
            this.colFileName.ReadOnly = true;
            // 
            // vlcPlayer
            // 
            this.vlcPlayer.BackColor = System.Drawing.Color.Black;
            this.vlcPlayer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vlcPlayer.Location = new System.Drawing.Point(0, 0);
            this.vlcPlayer.Name = "vlcPlayer";
            this.vlcPlayer.Size = new System.Drawing.Size(836, 443);
            this.vlcPlayer.Spu = -1;
            this.vlcPlayer.TabIndex = 0;
            this.vlcPlayer.Text = "vlcControl1";
            this.vlcPlayer.VlcLibDirectory = null;
            this.vlcPlayer.VlcMediaplayerOptions = null;
            this.vlcPlayer.VlcLibDirectoryNeeded += new System.EventHandler<Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs>(this.vlcPlayer_VlcLibDirectoryNeeded);
            this.vlcPlayer.PositionChanged += new System.EventHandler<Vlc.DotNet.Core.VlcMediaPlayerPositionChangedEventArgs>(this.vlcPlayer_PositionChanged);
            // 
            // panelRight
            // 
            this.panelRight.Controls.Add(this.panelRightTop);
            this.panelRight.Controls.Add(this.panelRightBottom);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRight.Location = new System.Drawing.Point(310, 25);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(836, 657);
            this.panelRight.TabIndex = 2;
            // 
            // panelRightTop
            // 
            this.panelRightTop.Controls.Add(this.panelPlayer);
            this.panelRightTop.Controls.Add(this.panelPlayControl);
            this.panelRightTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRightTop.Location = new System.Drawing.Point(0, 0);
            this.panelRightTop.Name = "panelRightTop";
            this.panelRightTop.Size = new System.Drawing.Size(836, 493);
            this.panelRightTop.TabIndex = 1;
            // 
            // panelPlayer
            // 
            this.panelPlayer.Controls.Add(this.vlcPlayer);
            this.panelPlayer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPlayer.Location = new System.Drawing.Point(0, 0);
            this.panelPlayer.Name = "panelPlayer";
            this.panelPlayer.Size = new System.Drawing.Size(836, 443);
            this.panelPlayer.TabIndex = 1;
            // 
            // panelPlayControl
            // 
            this.panelPlayControl.Controls.Add(this.labelVideoPosition);
            this.panelPlayControl.Controls.Add(this.trackBarPosition);
            this.panelPlayControl.Controls.Add(this.label1);
            this.panelPlayControl.Controls.Add(this.btnNext);
            this.panelPlayControl.Controls.Add(this.btnPrevious);
            this.panelPlayControl.Controls.Add(this.btnStartStop);
            this.panelPlayControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelPlayControl.Location = new System.Drawing.Point(0, 443);
            this.panelPlayControl.Name = "panelPlayControl";
            this.panelPlayControl.Size = new System.Drawing.Size(836, 50);
            this.panelPlayControl.TabIndex = 0;
            // 
            // labelVideoPosition
            // 
            this.labelVideoPosition.AutoSize = true;
            this.labelVideoPosition.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelVideoPosition.Location = new System.Drawing.Point(676, 16);
            this.labelVideoPosition.Name = "labelVideoPosition";
            this.labelVideoPosition.Size = new System.Drawing.Size(126, 14);
            this.labelVideoPosition.TabIndex = 6;
            this.labelVideoPosition.Text = "0:00:00 / 0:00:00";
            // 
            // trackBarPosition
            // 
            this.trackBarPosition.Location = new System.Drawing.Point(246, 14);
            this.trackBarPosition.Maximum = 100;
            this.trackBarPosition.Name = "trackBarPosition";
            this.trackBarPosition.Size = new System.Drawing.Size(424, 45);
            this.trackBarPosition.TabIndex = 4;
            this.trackBarPosition.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarPosition.Scroll += new System.EventHandler(this.trackBarPosition_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(211, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "进度";
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(145, 11);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(50, 25);
            this.btnNext.TabIndex = 2;
            this.btnNext.Text = "下一个";
            this.btnNext.UseVisualStyleBackColor = true;
            // 
            // btnPrevious
            // 
            this.btnPrevious.Location = new System.Drawing.Point(33, 11);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(50, 25);
            this.btnPrevious.TabIndex = 1;
            this.btnPrevious.Text = "上一个";
            this.btnPrevious.UseVisualStyleBackColor = true;
            // 
            // btnStartStop
            // 
            this.btnStartStop.Location = new System.Drawing.Point(89, 11);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(50, 25);
            this.btnStartStop.TabIndex = 0;
            this.btnStartStop.Tag = "START";
            this.btnStartStop.Text = "开始";
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // panelRightBottom
            // 
            this.panelRightBottom.Controls.Add(this.listViewAnnotation);
            this.panelRightBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelRightBottom.Location = new System.Drawing.Point(0, 493);
            this.panelRightBottom.Name = "panelRightBottom";
            this.panelRightBottom.Size = new System.Drawing.Size(836, 164);
            this.panelRightBottom.TabIndex = 0;
            // 
            // listViewAnnotation
            // 
            this.listViewAnnotation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewAnnotation.FullRowSelect = true;
            this.listViewAnnotation.GridLines = true;
            this.listViewAnnotation.Location = new System.Drawing.Point(0, 0);
            this.listViewAnnotation.Name = "listViewAnnotation";
            this.listViewAnnotation.Size = new System.Drawing.Size(836, 164);
            this.listViewAnnotation.TabIndex = 0;
            this.listViewAnnotation.UseCompatibleStateImageBehavior = false;
            this.listViewAnnotation.View = System.Windows.Forms.View.List;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1146, 682);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.Menu);
            this.MainMenuStrip = this.Menu;
            this.Name = "MainForm";
            this.Text = "视频注解";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Menu.ResumeLayout(false);
            this.Menu.PerformLayout();
            this.panelLeft.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DataGridFiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.vlcPlayer)).EndInit();
            this.panelRight.ResumeLayout(false);
            this.panelRightTop.ResumeLayout(false);
            this.panelPlayer.ResumeLayout(false);
            this.panelPlayControl.ResumeLayout(false);
            this.panelPlayControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarPosition)).EndInit();
            this.panelRightBottom.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip Menu;
        private System.Windows.Forms.ToolStripMenuItem MenuItemFile;
        private System.Windows.Forms.ToolStripMenuItem MenuItemOpen;
        private System.Windows.Forms.ToolStripMenuItem MenuItemOpenFile;
        private System.Windows.Forms.ToolStripMenuItem MenuItemOpenFolder;
        private System.Windows.Forms.OpenFileDialog OpenFileDialog;
        private System.Windows.Forms.FolderBrowserDialog FolderBrowserDialog;
        private System.Windows.Forms.Panel panelLeft;
        private Vlc.DotNet.Forms.VlcControl vlcPlayer;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.Panel panelRightTop;
        private System.Windows.Forms.Panel panelPlayer;
        private System.Windows.Forms.Panel panelPlayControl;
        private System.Windows.Forms.TrackBar trackBarPosition;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrevious;
        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.Panel panelRightBottom;
        private System.Windows.Forms.ListView listViewAnnotation;
        private System.Windows.Forms.Label labelVideoPosition;
        private System.Windows.Forms.ToolStripMenuItem MenuItemAddAnnotation;
        private System.Windows.Forms.DataGridView DataGridFiles;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColFileFullName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFileName;
    }
}

