using inpsNuGet;
using System.IO.Compression;
using System.Reflection;

namespace inpsNuGetTestForm
{
    public partial class MainForm : Form
    {
        private bool isAdjustingWidths = false;

        public MainForm()
        {
            InitializeComponent();

            for (int a = 0; a < 15; a++)
            {
                verticalFlowLayoutPanel2.AddItem($"Test{a + 1}");

                Panel tabItem = new Panel
                {
                    Height = 38,
                    BorderStyle = BorderStyle.FixedSingle,
                    Margin = new Padding(3, 3, 3, 0),
                    BackColor = SystemColors.Control,
                    Cursor = Cursors.Hand
                };

                int initialWidth = Math.Max(50, verticalFlowLayoutPanel1.ClientSize.Width - 6);
                tabItem.Width = initialWidth;

                Label lblTitle = new Label
                {
                    Text = "Loading...",
                    Location = new Point(5, 11),
                    Size = new Size(130, 18),
                    AutoEllipsis = true,
                    Cursor = Cursors.Hand
                };

                Button btnClose = new Button
                {
                    Text = "×",
                    Font = new Font("Arial", 9.5F, FontStyle.Bold),
                    Size = new Size(20, 20),
                    Location = new Point(148, 8),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btnClose.FlatAppearance.BorderSize = 0;

                //tabItem.Tag = webView;
                //lblTitle.Tag = tabItem;
                //btnClose.Tag = tabItem;

                //tabItem.Click += TabItem_Click;
                //lblTitle.Click += TabItem_Click;
                //btnClose.Click += BtnClose_Click;

                tabItem.Controls.Add(lblTitle);
                tabItem.Controls.Add(btnClose);

                AdjustInternalTabControls(tabItem, initialWidth);
                verticalFlowLayoutPanel1.Controls.Add(tabItem);

                AdjustTabWidths();
                verticalFlowLayoutPanel1.PerformLayout();
                verticalFlowLayoutPanel1.ScrollControlIntoView(tabItem);
            }
        }

        private void AdjustTabWidths()
        {
            if (isAdjustingWidths) return;
            isAdjustingWidths = true;

            verticalFlowLayoutPanel1.SuspendLayout();
            try
            {
                int clientWidth = verticalFlowLayoutPanel1.ClientSize.Width;
                int targetWidth = clientWidth - 6;

                if (targetWidth < 50) targetWidth = 50;

                foreach (Control ctrl in verticalFlowLayoutPanel1.Controls)
                {
                    if (ctrl is Panel tabItem)
                    {
                        if (tabItem.Width != targetWidth)
                        {
                            tabItem.Width = targetWidth;
                            AdjustInternalTabControls(tabItem, targetWidth);
                        }
                    }
                }
            }
            finally
            {
                verticalFlowLayoutPanel1.ResumeLayout(true);
                isAdjustingWidths = false;
            }
        }

        private void AdjustInternalTabControls(Panel tabItem, int targetWidth)
        {
            Label lbl = null;
            Button btn = null;

            foreach (Control c in tabItem.Controls)
            {
                if (c is Label label) lbl = label;
                else if (c is Button button) btn = button;
            }

            if (btn != null)
            {
                btn.Left = targetWidth - btn.Width - 6;
            }

            if (lbl != null && btn != null)
            {
                lbl.Width = btn.Left - lbl.Left - 4;
            }
        }

        private void TabPanel_Layout(object sender, LayoutEventArgs e)
        {
            AdjustTabWidths();
        }

        private void TabPanel_Resize(object sender, EventArgs e)
        {
            AdjustTabWidths();
        }
    }
}
