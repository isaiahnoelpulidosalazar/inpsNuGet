using System.ComponentModel;

namespace inpsNuGet;

#if WINDOWS
[ToolboxItem(true)]
public class ClickableElement : Panel
{
    string Title;
    Label Label;
    Action? Event;
    bool AmIHovered = false, AmIToggled = false;
    Color ToggledColor = Color.FromArgb(58, 204, 0),
        ToggledHoverColor = Color.FromArgb(53, 189, 0),
        ToggledClickedColor = Color.FromArgb(48, 168, 0);

    public ClickableElement(string Title)
    {
        this.Title = Title;
        
        Height = 38;
        BorderStyle = BorderStyle.FixedSingle;
        Margin = new Padding(3, 3, 3, 0);
        BackColor = SystemColors.Control;
        Cursor = Cursors.Hand;

        int InitialWidth = Math.Max(50, ClientSize.Width - 6);
        Width = InitialWidth;

        Label = new Label
        {
            Text = string.IsNullOrEmpty(Title) ? "Title" : Title,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(3, 0, 3, 0),
            BackColor = SystemColors.Control,
            AutoEllipsis = true,
            Cursor = Cursors.Hand
        };

        Label.MouseEnter += (s, e) =>
        {
            AmIHovered = true;
            if (AmIToggled)
            {
                BackColor = ToggledHoverColor;
                Label.BackColor = ToggledHoverColor;
            }
            else
            {
                BackColor = Color.LightGray;
                Label.BackColor = Color.LightGray;
            }
        };
        Label.MouseLeave += (s, e) =>
        {
            AmIHovered = false;
            if (AmIToggled)
            {
                BackColor = ToggledColor;
                Label.BackColor = ToggledColor;
            }
            else
            {
                BackColor = SystemColors.Control;
                Label.BackColor = SystemColors.Control;
            }
        };
        Label.MouseDown += (s, e) =>
        {
            if (AmIToggled)
            {
                BackColor = ToggledClickedColor;
                Label.BackColor = ToggledClickedColor;
            }
            else
            {
                BackColor = Color.FromArgb(175, 175, 175);
                Label.BackColor = Color.FromArgb(175, 175, 175);
            }
        };
        Label.MouseUp += (s, e) =>
        {
            if (AmIToggled)
            {
                BackColor = ToggledColor;
                Label.BackColor = ToggledColor;
            }
            else
            {
                BackColor = Color.LightGray;
                Label.BackColor = Color.LightGray;
            }
            Event?.Invoke();
        };

        Controls.Add(Label);
    }

    public ClickableElement SetEvent(Action Event)
    {
        this.Event = Event;
        return this;
    }

    public ClickableElement Toggle()
    {
        AmIToggled = !AmIToggled;
        if (AmIToggled)
        {
            BackColor = AmIHovered ? ToggledHoverColor : ToggledColor;
            Label.BackColor = AmIHovered ? ToggledHoverColor : ToggledColor;
        }
        else
        {
            BackColor = AmIHovered ? Color.LightGray : SystemColors.Control;
            Label.BackColor = AmIHovered ? Color.LightGray : SystemColors.Control;
        }
        PerformLayout();
        return this;
    }

    public string GetTitle()
    {
        return Title;
    }

    public bool IsToggled()
    {
        return AmIToggled;
    }
}
#else
public class ClickableElement { }
#endif