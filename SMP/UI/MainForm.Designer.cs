// UI/MainForm.Designer.cs
namespace SMP.UI
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private TextBox TxtUrl;
        private Button BtnAdd;
        private Button BtnDel;
        private Button BtnPlay;
        private Button BtnNext;
        private Button BtnStop;
        private ListBox LstPlaylist;
        private TrackBar TrackVolume;
        private Label LblVolume;

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
            TxtUrl = new TextBox();
            BtnAdd = new Button();
            BtnDel = new Button();
            BtnPlay = new Button();
            BtnNext = new Button();
            BtnStop = new Button();
            LstPlaylist = new ListBox();
            TrackVolume = new TrackBar();
            LblVolume = new Label();
            BtnLoop = new Button();
            ((System.ComponentModel.ISupportInitialize)TrackVolume).BeginInit();
            SuspendLayout();
            // 
            // TxtUrl
            // 
            TxtUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            TxtUrl.Location = new Point(12, 12);
            TxtUrl.Name = "TxtUrl";
            TxtUrl.PlaceholderText = "YouTube URL 입력";
            TxtUrl.Size = new Size(360, 23);
            TxtUrl.TabIndex = 0;
            // 
            // BtnAdd
            // 
            BtnAdd.Font = new Font("Segoe UI Emoji", 9F);
            BtnAdd.Location = new Point(12, 41);
            BtnAdd.Name = "BtnAdd";
            BtnAdd.Size = new Size(150, 30);
            BtnAdd.TabIndex = 1;
            BtnAdd.Text = "➕ 추가(&A)";
            BtnAdd.Click += BtnAdd_Click;
            // 
            // BtnDel
            // 
            BtnDel.Font = new Font("Segoe UI Emoji", 9F);
            BtnDel.Location = new Point(222, 41);
            BtnDel.Name = "BtnDel";
            BtnDel.Size = new Size(150, 30);
            BtnDel.TabIndex = 2;
            BtnDel.Text = "🗑️ 삭제(&D)";
            BtnDel.Click += BtnDel_Click;
            // 
            // BtnPlay
            // 
            BtnPlay.Font = new Font("Segoe UI Emoji", 11F);
            BtnPlay.Location = new Point(14, 258);
            BtnPlay.Name = "BtnPlay";
            BtnPlay.Size = new Size(80, 35);
            BtnPlay.TabIndex = 6;
            BtnPlay.Text = "▶️ 재생";
            BtnPlay.Click += BtnPlay_Click;
            // 
            // BtnNext
            // 
            BtnNext.Font = new Font("Segoe UI Emoji", 11F);
            BtnNext.Location = new Point(106, 258);
            BtnNext.Name = "BtnNext";
            BtnNext.Size = new Size(80, 35);
            BtnNext.TabIndex = 7;
            BtnNext.Text = "⏭️ 다음";
            BtnNext.Click += BtnNext_Click;
            // 
            // BtnStop
            // 
            BtnStop.Font = new Font("Segoe UI Emoji", 11F);
            BtnStop.Location = new Point(198, 258);
            BtnStop.Name = "BtnStop";
            BtnStop.Size = new Size(80, 35);
            BtnStop.TabIndex = 8;
            BtnStop.Text = "⏹️ 정지";
            BtnStop.Click += BtnStop_Click;
            // 
            // LstPlaylist
            // 
            LstPlaylist.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            LstPlaylist.HorizontalScrollbar = true;
            LstPlaylist.Location = new Point(12, 77);
            LstPlaylist.Name = "LstPlaylist";
            LstPlaylist.Size = new Size(360, 124);
            LstPlaylist.TabIndex = 3;
            LstPlaylist.MouseClick += LstPlaylist_MouseClick;
            LstPlaylist.MouseDoubleClick += LstPlaylist_MouseDoubleClick;
            // 
            // TrackVolume
            // 
            TrackVolume.Location = new Point(14, 207);
            TrackVolume.Maximum = 100;
            TrackVolume.Name = "TrackVolume";
            TrackVolume.Size = new Size(280, 45);
            TrackVolume.TabIndex = 4;
            TrackVolume.TickFrequency = 10;
            TrackVolume.Value = 50;
            TrackVolume.Scroll += TrackVolume_Scroll;
            // 
            // LblVolume
            // 
            LblVolume.Location = new Point(300, 207);
            LblVolume.Name = "LblVolume";
            LblVolume.Size = new Size(72, 23);
            LblVolume.TabIndex = 5;
            LblVolume.Text = "볼륨: 50%";
            // 
            // BtnLoop
            // 
            BtnLoop.Font = new Font("Segoe UI Emoji", 11F);
            BtnLoop.Location = new Point(290, 258);
            BtnLoop.Name = "BtnLoop";
            BtnLoop.Size = new Size(80, 35);
            BtnLoop.TabIndex = 9;
            BtnLoop.Text = "🔁 1회";
            BtnLoop.Click += BtnLoop_Click;
            // 
            // MainForm
            // 
            ClientSize = new Size(384, 300);
            Controls.Add(BtnLoop);
            Controls.Add(TxtUrl);
            Controls.Add(BtnAdd);
            Controls.Add(BtnDel);
            Controls.Add(LstPlaylist);
            Controls.Add(TrackVolume);
            Controls.Add(LblVolume);
            Controls.Add(BtnPlay);
            Controls.Add(BtnNext);
            Controls.Add(BtnStop);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "MainForm";
            Text = "SMP Player";
            ((System.ComponentModel.ISupportInitialize)TrackVolume).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private Button BtnLoop;
    }
}