using System;
using System.Drawing;
using System.Windows.Forms;

namespace Kiosk7
{
    public class AccessDeniedForm : Form
    {
        public AccessDeniedForm()
        {
            // ==== Base window ====
            Text = "Access denied";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            BackColor = Color.White;
            ClientSize = new Size(360, 180);
            Font = new Font("Segoe UI", 10f);
            TopMost = true;

            // ==== Header ====
            var header = new Panel
            {
                BackColor = Color.FromArgb(255, 210, 0), // S&B yellow
                Dock = DockStyle.Top,
                Height = 60
            };
            Controls.Add(header);

            var lblTitle = new Label
            {
                Text = "Access Denied",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(16, 18)
            };
            header.Controls.Add(lblTitle);

            // ==== Icon ====
            var icon = new PictureBox
            {
                Image = SystemIcons.Error.ToBitmap(),
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(40, 40),
                Location = new Point(28, 80)
            };
            Controls.Add(icon);

            // ==== Message ====
            var lblMsg = new Label
            {
                Text = "Wrong PIN. Try again.",
                AutoSize = true,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 11f, FontStyle.Regular),
                Location = new Point(80, 92)
            };
            Controls.Add(lblMsg);

            // ==== OK button ====
            var okBtn = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                BackColor = Color.FromArgb(255, 210, 0),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 10f),
                Size = new Size(100, 34),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            okBtn.FlatAppearance.BorderSize = 0;
            okBtn.Location = new Point(ClientSize.Width - okBtn.Width - 24, ClientSize.Height - okBtn.Height - 20);
            Controls.Add(okBtn);

            AcceptButton = okBtn;
        }

        public static void ShowDialogBox(IWin32Window owner = null)
        {
            using var dlg = new AccessDeniedForm();
            dlg.ShowDialog(owner);
        }
    }
}
