using inpsNuGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inpsNuGetTestForm
{
    public class VerticalFlowLayoutPanel : FlowLayoutPanel
    {
        bool IsAdjustingWidths = false;

        public VerticalFlowLayoutPanel()
        {
            BorderStyle = BorderStyle.FixedSingle;
            AutoScroll = true;
            FlowDirection = FlowDirection.TopDown;
            WrapContents = false;
            Padding = new Padding(0, 0, 0, 3);
        }

        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                System.Windows.Forms.CreateParams cp = base.CreateParams;
                cp.Style &= ~0x00100000;
                return cp;
            }
        }

        public void AddItem(string Title)
        {
            Panel Panel = new Panel
            {
                Height = 38,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(3, 3, 3, 0),
                BackColor = SystemColors.Control,
                Cursor = Cursors.Hand
            };

            int InitialWidth = Math.Max(50, ClientSize.Width - 6);
            Panel.Width = InitialWidth;

            Label Label = new Label
            {
                Text = string.IsNullOrEmpty(Title) ? "Title" : Title,
                Location = new Point(5, 11),
                Size = new Size(130, 18),
                AutoEllipsis = true,
                Cursor = Cursors.Hand
            };

            Panel.Controls.Add(Label);
            Controls.Add(Panel);

            UpdateItemMargins();
            AdjustItemWidths();
            PerformLayout();
            ScrollControlIntoView(Panel);
        }

        void UpdateItemMargins()
        {
            for (int a = 0; a < Controls.Count; a++)
            {
                Control Control = Controls[a];
                bool IsLastItem = (a == Controls.Count - 1);

                int bottomMargin = IsLastItem ? 3 : 0;
                if (Control.Margin.Bottom != bottomMargin)
                {
                    Control.Margin = new Padding(3, 3, 3, bottomMargin);
                }
            }
        }

        void AdjustItemWidths()
        {
            if (IsAdjustingWidths)
            {
                return;
            }

            IsAdjustingWidths = true;

            SuspendLayout();

            try
            {
                int ClientWidth = ClientSize.Width;
                int TargetWidth = ClientWidth - 6;

                if (TargetWidth < 50)
                {
                    TargetWidth = 50;
                }

                foreach (Control Control in Controls)
                {
                    if (Control is Panel Panel)
                    {
                        if (Panel.Width != TargetWidth)
                        {
                            Panel.Width = TargetWidth;
                            AdjustInternalItemControls(Panel, TargetWidth);
                        }
                    }
                }
            }
            finally
            {
                ResumeLayout(true);
                IsAdjustingWidths = false;
            }
        }

        void AdjustInternalItemControls(Panel Panel, int TargetWidth)
        {
            Label CurrentLabel = null;

            foreach (Control Control in Panel.Controls)
            {
                if (Control is Label Label)
                {
                    CurrentLabel = Label;
                }
            }

            if (CurrentLabel != null)
            {
                CurrentLabel.Width = CurrentLabel.Width - 6;
            }
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            UpdateItemMargins();
            base.OnLayout(levent);
            AdjustItemWidths();
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            AdjustItemWidths();
        }
    }
}
