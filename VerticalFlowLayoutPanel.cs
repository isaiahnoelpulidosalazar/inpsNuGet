using System.ComponentModel;

namespace inpsNuGet;

[ToolboxItem(true)]
#if Windows
public class VerticalFlowLayoutPanel : FlowLayoutPanel
{
    protected override System.Windows.Forms.CreateParams CreateParams
    {
        get
        {
            System.Windows.Forms.CreateParams cp = base.CreateParams;
            cp.Style &= ~0x00100000;
            return cp;
        }
    }
}
#else
public class VerticalFlowLayoutPanel
{
}
#endif