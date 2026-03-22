// UI/MainForm.Designer.cs
namespace SMP.UI
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private TextBox txtUrl;
        private Button btnAdd;
        private Button btnDel;
        private Button btnPlay;
        private Button btnNext;
        private Button btnStop;
        private ListBox lstPlaylist;
        private TrackBar trackVolume;
        private Label lblVolume;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            txtUrl = new TextBox();
            btnAdd = new Button();
            btnDel = new Button();
            btnPlay = new Button();
            btnNext = new Button();
            btnStop = new Button();
            lstPlaylist = new ListBox();
            trackVolume = new TrackBar();
            lblVolume = new Label();
            ((System.ComponentModel.ISupportInitialize)trackVolume).BeginInit();
            SuspendLayout();
            // 
            // txtUrl
            // 
            txtUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtUrl.Location = new Point(12, 12);
            txtUrl.Name = "txtUrl";
            txtUrl.PlaceholderText = "YouTube URL 입력";
            txtUrl.Size = new Size(360, 23);
            txtUrl.TabIndex = 0;
            // 
            // btnAdd
            // 
            btnAdd.Font = new Font("Segoe UI Emoji", 9F);
            btnAdd.Location = new Point(12, 41);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(150, 30);
            btnAdd.TabIndex = 1;
            btnAdd.Text = "➕ 추가(&A)";
            btnAdd.Click += btnAdd_Click;
            // 
            // btnDel
            // 
            btnDel.Font = new Font("Segoe UI Emoji", 9F);
            btnDel.Location = new Point(222, 41);
            btnDel.Name = "btnDel";
            btnDel.Size = new Size(150, 30);
            btnDel.TabIndex = 2;
            btnDel.Text = "🗑️ 삭제(&D)";
            btnDel.Click += btnDel_Click;
            // 
            // btnPlay
            // 
            btnPlay.Font = new Font("Segoe UI Emoji", 11F);
            btnPlay.Location = new Point(14, 258);
            btnPlay.Name = "btnPlay";
            btnPlay.Size = new Size(110, 35);
            btnPlay.TabIndex = 6;
            btnPlay.Text = "▶️ 재생";
            btnPlay.Click += btnPlay_Click;
            // 
            // btnNext
            // 
            btnNext.Font = new Font("Segoe UI Emoji", 11F);
            btnNext.Location = new Point(140, 258);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(110, 35);
            btnNext.TabIndex = 7;
            btnNext.Text = "⏭️ 다음";
            btnNext.Click += btnNext_Click;
            // 
            // btnStop
            // 
            btnStop.Font = new Font("Segoe UI Emoji", 11F);
            btnStop.Location = new Point(262, 258);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(110, 35);
            btnStop.TabIndex = 8;
            btnStop.Text = "⏹️ 정지";
            btnStop.Click += btnStop_Click;
            // 
            // lstPlaylist
            // 
            lstPlaylist.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstPlaylist.HorizontalScrollbar = true;
            lstPlaylist.Location = new Point(12, 77);
            lstPlaylist.Name = "lstPlaylist";
            lstPlaylist.Size = new Size(360, 124);
            lstPlaylist.TabIndex = 3;
            // 
            // trackVolume
            // 
            trackVolume.Location = new Point(14, 207);
            trackVolume.Maximum = 100;
            trackVolume.Name = "trackVolume";
            trackVolume.Size = new Size(280, 45);
            trackVolume.TabIndex = 4;
            trackVolume.TickFrequency = 10;
            trackVolume.Value = 50;
            trackVolume.Scroll += trackVolume_Scroll;
            // 
            // lblVolume
            // 
            lblVolume.Location = new Point(300, 207);
            lblVolume.Name = "lblVolume";
            lblVolume.Size = new Size(72, 23);
            lblVolume.TabIndex = 5;
            lblVolume.Text = "볼륨: 50%";
            // 
            // MainForm
            // 
            ClientSize = new Size(384, 300);
            Controls.Add(txtUrl);
            Controls.Add(btnAdd);
            Controls.Add(btnDel);
            Controls.Add(lstPlaylist);
            Controls.Add(trackVolume);
            Controls.Add(lblVolume);
            Controls.Add(btnPlay);
            Controls.Add(btnNext);
            Controls.Add(btnStop);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            Text = "SMP Player";
            ((System.ComponentModel.ISupportInitialize)trackVolume).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}