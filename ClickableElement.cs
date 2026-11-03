using System.ComponentModel;

namespace inpsNuGet;

/// <summary>A hover/press/toggle-aware clickable row. Windows Forms only (net8.0-windows).</summary>
[ToolboxItem(true)]
public class ClickableElement : Panel
{
    static readonly Color ToggledColor = Color.FromArgb(58, 204, 0);
    static readonly Color ToggledHoverColor = Color.FromArgb(53, 189, 0);
    static readonly Color ToggledPressedColor = Color.FromArgb(48, 168, 0);
    static readonly Color PressedColor = Color.FromArgb(175, 175, 175);

    readonly string _title;
    readonly Label _label;
    Action? _onClick;
    bool _hovered, _pressed, _toggled;

    public ClickableElement() : this("Title") { }

    public ClickableElement(string title)
    {
        _title = title;
        Height = 38;
        Width = 50;
        BorderStyle = BorderStyle.FixedSingle;
        Margin = new Padding(3, 3, 3, 0);
        Cursor = Cursors.Hand;

        _label = new Label
        {
            Text = string.IsNullOrEmpty(title) ? "Title" : title,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(3, 0, 3, 0),
            AutoEllipsis = true,
            Cursor = Cursors.Hand
        };

        _label.MouseEnter += (_, _) => { _hovered = true; UpdateVisualState(); };
        _label.MouseLeave += (_, _) => { _hovered = _pressed = false; UpdateVisualState(); };
        _label.MouseDown += (_, e) =>
        {
            if (e.Button != MouseButtons.Left) return;
            _pressed = true;
            UpdateVisualState();
        };
        _label.MouseUp += (_, e) =>
        {
            bool wasPressed = _pressed;
            _pressed = false;
            UpdateVisualState();
            // Fires only for a left press released inside the element.
            if (wasPressed && _label.ClientRectangle.Contains(e.Location)) _onClick?.Invoke();
        };

        Controls.Add(_label);
        UpdateVisualState();
    }

    void UpdateVisualState()
    {
        Color c = (_toggled, _pressed, _hovered) switch
        {
            (true, true, _) => ToggledPressedColor,
            (true, _, true) => ToggledHoverColor,
            (true, _, _) => ToggledColor,
            (false, true, _) => PressedColor,
            (false, _, true) => Color.LightGray,
            _ => SystemColors.Control
        };
        BackColor = c;
        _label.BackColor = c;
    }

    public ClickableElement SetEvent(Action onClick) { _onClick = onClick; return this; }

    public ClickableElement Toggle()
    {
        _toggled = !_toggled;
        UpdateVisualState();
        return this;
    }

    public string GetTitle() => _title;
    public bool IsToggled() => _toggled;
}
