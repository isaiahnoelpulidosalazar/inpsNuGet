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
        Controls.Add(new ClickableElement(Title));

        UpdateItemMargins();
        AdjustItemWidths();
        PerformLayout();

        if (IsHandleCreated && Visible)
        {
            ScrollToBottom();
        }
    }

    public void AddItem(string Title, Action Event)
    {
        Controls.Add(new ClickableElement(Title).SetEvent(Event));

        UpdateItemMargins();
        AdjustItemWidths();
        PerformLayout();

        if (IsHandleCreated && Visible)
        {
            ScrollToBottom();
        }
    }

    public void AddItem(ClickableElement ClickableElement)
    {
        Controls.Add(ClickableElement);

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