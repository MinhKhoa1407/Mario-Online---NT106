using System.Drawing;
using System.Windows.Forms;

namespace Ranking
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblTitle;
        private Panel panelTop3;
        private Panel card1;
        private Panel card2;
        private Panel card3;

        public Label lblTop1;
        public Label lblTop2;
        public Label lblTop3;

        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn Username;
        private DataGridViewTextBoxColumn Score;
        private DataGridViewTextBoxColumn Rank;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.panelTop3 = new Panel();
            this.card1 = new Panel();
            this.card2 = new Panel();
            this.card3 = new Panel();

            this.lblTop1 = new Label();
            this.lblTop2 = new Label();
            this.lblTop3 = new Label();

            this.dataGridView1 = new DataGridView();
            this.Username = new DataGridViewTextBoxColumn();
            this.Score = new DataGridViewTextBoxColumn();
            this.Rank = new DataGridViewTextBoxColumn();

            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();

            // FORM
            this.ClientSize = new Size(1060, 820);
            this.BackColor = Color.FromArgb(18, 28, 45);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Mario Online - Ranking";

            // TITLE
            this.lblTitle.Dock = DockStyle.Top;
            this.lblTitle.Height = 90;
            this.lblTitle.Text = "🏆 MARIO ONLINE - RANKING";
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            this.lblTitle.Font = new Font("Segoe UI Black", 32F);
            this.lblTitle.ForeColor = Color.Gold;

            // PANEL TOP 3
            this.panelTop3.Location = new Point(20, 100);
            this.panelTop3.Size = new Size(1020, 180);
            this.panelTop3.BackColor = Color.Transparent;

            this.panelTop3.Controls.Add(this.card1);
            this.panelTop3.Controls.Add(this.card2);
            this.panelTop3.Controls.Add(this.card3);

            // DATAGRIDVIEW
            this.dataGridView1.Location = new Point(20, 300);
            this.dataGridView1.Size = new Size(1020, 500);
            this.dataGridView1.BackgroundColor = Color.FromArgb(25, 38, 60);
            this.dataGridView1.BorderStyle = BorderStyle.None;
            this.dataGridView1.EnableHeadersVisualStyles = false;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            this.dataGridView1.ColumnHeadersHeight = 42;
            this.dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(35, 52, 80);
            this.dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            this.dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 13F);

            this.dataGridView1.DefaultCellStyle.BackColor = Color.FromArgb(25, 38, 60);
            this.dataGridView1.DefaultCellStyle.ForeColor = Color.White;
            this.dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 12F);

            // COLUMNS
            this.Username.HeaderText = "Username";
            this.Username.Width = 450;
            this.Score.HeaderText = "Score";
            this.Score.Width = 300;
            this.Rank.HeaderText = "Rank";
            this.Rank.Width = 120;

            this.dataGridView1.Columns.AddRange(new DataGridViewColumn[]
            {
                this.Username, this.Score, this.Rank
            });

            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.panelTop3);
            this.Controls.Add(this.dataGridView1);

            this.Load += new System.EventHandler(this.Form1_Load);

            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
