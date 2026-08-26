using System.ComponentModel;

namespace inpsNuGet;

#if WINDOWS
[ToolboxItem(true)]
public class VerticalList : FlowLayoutPanel
{
    bool IsAdjustingWidths = false;

    public VerticalList()
    {
        BorderStyle = BorderStyle.FixedSingle;
        AutoScroll = true;
        FlowDirection = FlowDirection.TopDown;
        WrapContents = false;
        Padding = new Padding(0, 0, 0, 3);
        DoubleBuffered = true;
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
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(3, 0, 3, 0),
            BackColor = SystemColors.Control,
            AutoEllipsis = true,
            Cursor = Cursors.Hand
        };

        Panel.MouseEnter += (s, e) =>
        {
            Panel.BackColor = Color.LightGray;
            Label.BackColor = Color.LightGray;
        };
        Panel.MouseLeave += (s, e) =>
        {
            Panel.BackColor = SystemColors.Control;
            Label.BackColor = SystemColors.Control;
        };
        Label.MouseEnter += (s, e) =>
        {
            Panel.BackColor = Color.LightGray;
            Label.BackColor = Color.LightGray;
        };
        Label.MouseLeave += (s, e) =>
        {
            Panel.BackColor = SystemColors.Control;
            Label.BackColor = SystemColors.Control;
        };

        Panel.Controls.Add(Label);
        Controls.Add(Panel);

        UpdateItemMargins();
        AdjustItemWidths();
        PerformLayout();

        if (IsHandleCreated && Visible)
        {
            ScrollToBottom();
        }
    }

    public void ScrollToBottom()
    {
        AutoScrollPosition = new Point(0, DisplayRectangle.Height);
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

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        BeginInvoke((MethodInvoker)delegate
        {
            ScrollToBottom();
        });
    }
}
#else
public class VerticalList { }
#endif