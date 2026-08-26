using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inpsNuGetTestForm
{
    public class ClickableElement : Panel
    {
        Action? Event;

        public ClickableElement(string Title)
        {
            Height = 38;
            BorderStyle = BorderStyle.FixedSingle;
            Margin = new Padding(3, 3, 3, 0);
            BackColor = SystemColors.Control;
            Cursor = Cursors.Hand;

            int InitialWidth = Math.Max(50, ClientSize.Width - 6);
            Width = InitialWidth;

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

            //MouseEnter += (s, e) =>
            //{
            //    BackColor = Color.LightGray;
            //    Label.BackColor = Color.LightGray;
            //};
            //MouseLeave += (s, e) =>
            //{
            //    BackColor = SystemColors.Control;
            //    Label.BackColor = SystemColors.Control;
            //};
            //MouseDown += (s, e) =>
            //{
            //    BackColor = Color.FromArgb(175, 175, 175);
            //    Label.BackColor = Color.FromArgb(175, 175, 175);
            //};
            //MouseUp += (s, e) =>
            //{
            //    BackColor = Color.LightGray;
            //    Label.BackColor = Color.LightGray;
            //};
            Label.MouseEnter += (s, e) =>
            {
                BackColor = Color.LightGray;
                Label.BackColor = Color.LightGray;
            };
            Label.MouseLeave += (s, e) =>
            {
                BackColor = SystemColors.Control;
                Label.BackColor = SystemColors.Control;
            };
            Label.MouseDown += (s, e) =>
            {
                BackColor = Color.FromArgb(175, 175, 175);
                Label.BackColor = Color.FromArgb(175, 175, 175);
            };
            Label.MouseUp += (s, e) =>
            {
                BackColor = Color.LightGray;
                Label.BackColor = Color.LightGray;
                Event?.Invoke();
            };

            Controls.Add(Label);
        }

        public ClickableElement SetEvent(Action Event)
        {
            this.Event = Event;
            return this;
        }
    }
}
