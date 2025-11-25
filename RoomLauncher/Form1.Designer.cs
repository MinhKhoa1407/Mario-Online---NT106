namespace RoomLauncher
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
            label1 = new Label();
            txtRoomName = new TextBox();
            btnCreate = new Button();
            btnJoin = new Button();
            lblRoomInfo = new Label();
            btnStartGame = new Button();
            label2 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(449, 23);
            label1.Name = "label1";
            label1.Size = new Size(77, 25);
            label1.TabIndex = 0;
            label1.Text = "ROOMS";
            label1.Click += label1_Click;
            // 
            // txtRoomName
            // 
            txtRoomName.Location = new Point(327, 83);
            txtRoomName.Name = "txtRoomName";
            txtRoomName.Size = new Size(150, 31);
            txtRoomName.TabIndex = 1;
            txtRoomName.TextChanged += txtRoomName_TextChanged;
            // 
            // btnCreate
            // 
            btnCreate.Location = new Point(247, 213);
            btnCreate.Name = "btnCreate";
            btnCreate.Size = new Size(186, 45);
            btnCreate.TabIndex = 2;
            btnCreate.Text = "Create Room";
            btnCreate.UseVisualStyleBackColor = true;
            btnCreate.Click += btnCreate_Click;
            // 
            // btnJoin
            // 
            btnJoin.Location = new Point(502, 213);
            btnJoin.Name = "btnJoin";
            btnJoin.Size = new Size(156, 45);
            btnJoin.TabIndex = 3;
            btnJoin.Text = "Join ID";
            btnJoin.UseVisualStyleBackColor = true;
            btnJoin.Click += btnJoin_Click;
            // 
            // lblRoomInfo
            // 
            lblRoomInfo.AutoSize = true;
            lblRoomInfo.Location = new Point(325, 131);
            lblRoomInfo.Name = "lblRoomInfo";
            lblRoomInfo.Size = new Size(0, 25);
            lblRoomInfo.TabIndex = 4;
            lblRoomInfo.Click += label2_Click;
            // 
            // btnStartGame
            // 
            btnStartGame.Enabled = false;
            btnStartGame.Location = new Point(546, 81);
            btnStartGame.Name = "btnStartGame";
            btnStartGame.Size = new Size(112, 34);
            btnStartGame.TabIndex = 5;
            btnStartGame.Text = "Vào Game";
            btnStartGame.UseVisualStyleBackColor = true;
            btnStartGame.Click += btnStartGame_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(190, 89);
            label2.Name = "label2";
            label2.Size = new Size(130, 25);
            label2.TabIndex = 6;
            label2.Text = "Tên phòng/ID: ";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1031, 586);
            Controls.Add(label2);
            Controls.Add(btnStartGame);
            Controls.Add(lblRoomInfo);
            Controls.Add(btnJoin);
            Controls.Add(btnCreate);
            Controls.Add(txtRoomName);
            Controls.Add(label1);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txtRoomName;
        private Button btnCreate;
        private Button btnJoin;
        private Label lblRoomInfo;
        private Button btnStartGame;
        private Label label2;
    }
}
