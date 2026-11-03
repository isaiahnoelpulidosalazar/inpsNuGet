using System.ComponentModel;

namespace inpsNuGet;

/// <summary>Vertical, scrollable list of <see cref="ClickableElement"/>. Windows Forms only (net8.0-windows).</summary>
[ToolboxItem(true)]
public class VerticalList : FlowLayoutPanel
{
    const int WS_HSCROLL = 0x00100000;
    const int MinItemWidth = 50;

    public VerticalList()
    {
        BorderStyle = BorderStyle.FixedSingle;
        AutoScroll = true;
        FlowDirection = FlowDirection.TopDown;
        WrapContents = false;
        Padding = new Padding(0, 0, 0, 6); // replaces the per-item "last item gets a bottom margin" pass
        DoubleBuffered = true;
    }

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.Style &= ~WS_HSCROLL; // never show a horizontal scrollbar
            return cp;
        }
    }

    int TargetWidth => Math.Max(MinItemWidth, ClientSize.Width - 6);

    public void AddItem(string title) => AddItem(new ClickableElement(title));

    public void AddItem(string title, Action onClick) => AddItem(new ClickableElement(title).SetEvent(onClick));

    public void AddItem(ClickableElement item)
    {
        ArgumentNullException.ThrowIfNull(item);
        item.Width = TargetWidth;
        Controls.Add(item);

        if (IsHandleCreated && Visible)
        {
            PerformLayout();
            ScrollToBottom();
        }
    }

    public void ScrollToBottom() => AutoScrollPosition = new Point(0, DisplayRectangle.Height);

    protected override void OnClientSizeChanged(EventArgs e)
    {
        base.OnClientSizeChanged(e);
        int target = TargetWidth;

        SuspendLayout();
        try
        {
            foreach (Control c in Controls)
                if (c.Width != target) c.Width = target;
        }
        finally { ResumeLayout(true); }
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        BeginInvoke((MethodInvoker)ScrollToBottom);
    }
}
