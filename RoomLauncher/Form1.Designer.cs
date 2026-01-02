namespace RoomLauncher
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            label1 = new Label();
            panelMain = new Panel();
            label2 = new Label();
            txtRoomName = new TextBox();
            btnCreate = new Button();
            btnJoin = new Button();
            lblRoomInfo = new Label();
            btnStartGame = new Button();
            panelMain.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Dock = DockStyle.Top;
            label1.Font = new Font("Segoe UI Black", 30F);
            label1.ForeColor = Color.Gold;
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(1341, 90);
            label1.TabIndex = 0;
            label1.Text = "🎮 ROOM LAUNCHER";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panelMain
            // 
            panelMain.Anchor = AnchorStyles.None;
            panelMain.Dock = DockStyle.None;
            panelMain.BackColor = Color.FromArgb(30, 45, 70);
            panelMain.BorderStyle = BorderStyle.FixedSingle;
            panelMain.Controls.Add(label2);
            panelMain.Controls.Add(txtRoomName);
            panelMain.Controls.Add(btnCreate);
            panelMain.Controls.Add(btnJoin);
            panelMain.Controls.Add(lblRoomInfo);
            panelMain.Controls.Add(btnStartGame);
            panelMain.Location = new Point(352, 111);
            panelMain.Name = "panelMain";
            panelMain.Size = new Size(679, 328);
            panelMain.TabIndex = 1;
            panelMain.Paint += panelMain_Paint;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 13F);
            label2.ForeColor = Color.White;
            label2.Location = new Point(40, 40);
            label2.Name = "label2";
            label2.Size = new Size(162, 30);
            label2.TabIndex = 0;
            label2.Text = "Tên phòng / ID";
            // 
            // txtRoomName
            // 
            txtRoomName.Font = new Font("Segoe UI", 13F);
            txtRoomName.Location = new Point(44, 75);
            txtRoomName.Name = "txtRoomName";
            txtRoomName.Size = new Size(589, 36);
            txtRoomName.TabIndex = 1;
            txtRoomName.TextAlign = HorizontalAlignment.Center;
            txtRoomName.TextChanged += txtRoomName_TextChanged;
            // 
            // btnCreate
            // 
            btnCreate.BackColor = Color.Gold;
            btnCreate.FlatStyle = FlatStyle.Flat;
            btnCreate.Font = new Font("Segoe UI Semibold", 12F);
            btnCreate.Location = new Point(44, 135);
            btnCreate.Name = "btnCreate";
            btnCreate.Size = new Size(180, 45);
            btnCreate.TabIndex = 2;
            btnCreate.Text = "➕ Create Room";
            btnCreate.UseVisualStyleBackColor = false;
            btnCreate.Click += btnCreate_Click;
            // 
            // btnJoin
            // 
            btnJoin.BackColor = Color.Silver;
            btnJoin.FlatStyle = FlatStyle.Flat;
            btnJoin.Font = new Font("Segoe UI Semibold", 12F);
            btnJoin.Location = new Point(453, 135);
            btnJoin.Name = "btnJoin";
            btnJoin.Size = new Size(180, 45);
            btnJoin.TabIndex = 3;
            btnJoin.Text = "🚪 Join Room";
            btnJoin.UseVisualStyleBackColor = false;
            btnJoin.Click += btnJoin_Click;
            // 
            // lblRoomInfo
            // 
            lblRoomInfo.Font = new Font("Segoe UI", 11F);
            lblRoomInfo.ForeColor = Color.LightGreen;
            lblRoomInfo.Location = new Point(44, 195);
            lblRoomInfo.Name = "lblRoomInfo";
            lblRoomInfo.Size = new Size(430, 45);
            lblRoomInfo.TabIndex = 4;
            lblRoomInfo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnStartGame
            // 
            btnStartGame.BackColor = Color.FromArgb(0, 200, 100);
            btnStartGame.Enabled = false;
            btnStartGame.FlatStyle = FlatStyle.Flat;
            btnStartGame.Font = new Font("Segoe UI Black", 12F);
            btnStartGame.ForeColor = Color.White;
            btnStartGame.Location = new Point(218, 263);
            btnStartGame.Name = "btnStartGame";
            btnStartGame.Size = new Size(220, 45);
            btnStartGame.TabIndex = 5;
            btnStartGame.Text = "▶ VÀO GAME";
            btnStartGame.UseVisualStyleBackColor = false;
            btnStartGame.Click += btnStartGame_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(18, 28, 45);
            ClientSize = new Size(1341, 520);
            Controls.Add(label1);
            Controls.Add(panelMain);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Mario Online - Room";
            FormClosing += Form1_FormClosing;
            panelMain.ResumeLayout(false);
            panelMain.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtRoomName;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnJoin;
        private System.Windows.Forms.Label lblRoomInfo;
        private System.Windows.Forms.Button btnStartGame;
    }
}
