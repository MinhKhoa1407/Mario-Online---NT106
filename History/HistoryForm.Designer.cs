using System.Drawing;

namespace History
{
    partial class HistoryForm
    {
        private System.ComponentModel.IContainer components = null;

        // 🔥 KHAI BÁO PANEL ĐÂY
        private System.Windows.Forms.FlowLayoutPanel panelHistory;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.panelHistory = new System.Windows.Forms.FlowLayoutPanel();
            this.title = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // panelHistory
            // 
            this.panelHistory.AutoScroll = true;
            this.panelHistory.BackColor = System.Drawing.Color.White;
            this.panelHistory.Location = new System.Drawing.Point(13, 57);
            this.panelHistory.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panelHistory.Name = "panelHistory";
            this.panelHistory.Size = new System.Drawing.Size(1325, 838);
            this.panelHistory.TabIndex = 1;
            this.panelHistory.AutoScroll = true;
            this.panelHistory.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.panelHistory.WrapContents = false;
            // 
            // title
            // 
            this.title.AutoSize = true;
            this.title.Font = new System.Drawing.Font("Segoe UI Black", 26F);
            this.title.ForeColor = System.Drawing.Color.Black;
            this.title.Location = new System.Drawing.Point(30, 16);
            this.title.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(221, 60);
            this.title.TabIndex = 0;
            this.title.Text = "HISTORY";
            // 
            // HistoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1351, 909);
            this.Controls.Add(this.title);
            this.Controls.Add(this.panelHistory);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "HistoryForm";
            this.Text = "History";
            this.Load += new System.EventHandler(this.HistoryForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label title;
    }
}
