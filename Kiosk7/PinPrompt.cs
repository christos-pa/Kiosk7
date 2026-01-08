using System;
using System.Drawing;
using System.Windows.Forms;

namespace Kiosk7
{
    public class PinPrompt : Form
    {
        private readonly TextBox _tb;
        private readonly Button _ok;
        private readonly Button _cancel;
        private readonly Button _eye;
        private readonly Label _caps;
        private readonly Panel _header;
        private readonly PictureBox _logo;

        public string PinText => _tb.Text;

        public PinPrompt()
        {
            // ===== Window =====
            Text = "Exit kiosk";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            MinimizeBox = MaximizeBox = false;
            TopMost = true;
            AutoScaleMode = AutoScaleMode.Dpi;
            Font = new Font("Segoe UI", 10f);
            BackColor = Color.White;
            Padding = new Padding(16);
            ClientSize = new Size(360, 220);
            KeyPreview = true;

            // ===== Layout constants =====
            int LeftPad = 16;
            int RightPad = 80;
            int HeaderHeight = 64;
            int FooterHeight = 72;

            // ===== Header =====
            _header = new Panel
            {
                BackColor = Color.FromArgb(255, 210, 0),
                Dock = DockStyle.Top,
                Height = HeaderHeight,
                Padding = new Padding(16, 10, 16, 8)
            };

            var title = new Label
            {
                AutoSize = true,
                Text = "Enter PIN",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(16, 18)
            };
            _header.Controls.Add(title);

            _logo = new PictureBox
            {
                Size = new Size(48, 48),
                SizeMode = PictureBoxSizeMode.Zoom,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.Transparent
            };
            try { _logo.ImageLocation = "snb_logo.png"; } catch { }
            _header.Controls.Add(_logo);

            Controls.Add(_header);

            void PositionLogo()
            {
                _logo.Location = new Point(_header.Width - _logo.Width - 10, 8);
            }
            _header.Resize += (_, __) => PositionLogo();
            PositionLogo();

            // ===== PIN textbox =====
            int contentTop = _header.Height + 24;

            _tb = new TextBox
            {
                UseSystemPasswordChar = true,
                Font = new Font("Segoe UI", 11f),
                Location = new Point(LeftPad, contentTop),
                Size = new Size(ClientSize.Width - LeftPad - RightPad - 36, 28)
            };
            Controls.Add(_tb);

            // ===== Eye toggle =====
            _eye = new Button
            {
                Text = "\uD83D\uDC41",
                Font = new Font("Segoe UI Symbol", 11f),
                Size = new Size(34, _tb.Height + 2),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                TabStop = false
            };
            _eye.FlatAppearance.BorderSize = 0;
            _eye.Click += (_, __) => _tb.UseSystemPasswordChar = !_tb.UseSystemPasswordChar;
            Controls.Add(_eye);

            // ===== Caps Lock Indicator =====
            _caps = new Label
            {
                Text = "Caps Lock is ON",
                ForeColor = Color.DarkRed,
                AutoSize = true,
                Visible = Control.IsKeyLocked(Keys.CapsLock),
                Font = new Font("Segoe UI", 9f, FontStyle.Italic),
                Location = new Point(LeftPad, _tb.Bottom + 8)
            };
            Controls.Add(_caps);

            // ===== Buttons =====
            _ok = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Size = new Size(96, 34),
                Font = new Font("Segoe UI Semibold", 10f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(255, 210, 0),
                ForeColor = Color.Black
            };
            _ok.FlatAppearance.BorderSize = 0;

            _cancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Size = new Size(96, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.Black
            };
            _cancel.FlatAppearance.BorderSize = 1;
            _cancel.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);

            Controls.Add(_ok);
            Controls.Add(_cancel);

            // ===== Positioning =====
            void Position()
            {
                _tb.Width = ClientSize.Width - LeftPad - RightPad - 36;
                _eye.Location = new Point(_tb.Right + 2, _tb.Top - 1);

                int footerTop = ClientSize.Height - FooterHeight;

                _ok.Location = new Point(ClientSize.Width - 180, footerTop + 18);
                _cancel.Location = new Point(_ok.Left - _cancel.Width - 10, footerTop + 18);

                PositionLogo();
            }

            Resize += (_, __) => Position();
            Position();

            // ===== Behavior =====
            AcceptButton = _ok;
            CancelButton = _cancel;

            Shown += (_, __) =>
            {
                _tb.Focus();
                UpdateCaps();
            };

            KeyDown += (_, e) => { if (e.KeyCode == Keys.CapsLock) UpdateCaps(); };
            KeyUp += (_, e) => { if (e.KeyCode == Keys.CapsLock) UpdateCaps(); };
            _tb.KeyDown += (_, e) => { if (e.KeyCode == Keys.CapsLock) UpdateCaps(); };
            _tb.KeyUp += (_, e) => { if (e.KeyCode == Keys.CapsLock) UpdateCaps(); };
        }

        private void UpdateCaps()
        {
            _caps.Visible = Control.IsKeyLocked(Keys.CapsLock);
        }
    }
}
