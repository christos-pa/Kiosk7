// SetupForm.Designer.cs
using System.Windows.Forms;

namespace Kiosk7
{
    partial class SetupForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel headerBar;
        private PictureBox headerIcon;
        private PictureBox headerInfoIcon;   // new left-side info icon

        private Label headerTitle;
        private Label headerSubtitle;

        private Label lblUrl;
        private TextBox tbUrl;

        private Label lblPin;
        private TextBox tbPin;   // visible PIN

        private Label lblAllow;
        private TextBox tbAllow; // multiline allowlist

        private CheckBox cbShowExit;
        private CheckBox cbRemember;

        private Button btnCancel;
        private Button btnSaveStart;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            headerBar = new Panel();
            headerInfoIcon = new PictureBox();   // left info icon
            headerIcon = new PictureBox();       // right logo
            headerTitle = new Label();
            headerSubtitle = new Label();


            lblUrl = new Label();
            tbUrl = new TextBox();

            lblPin = new Label();
            tbPin = new TextBox();

            lblAllow = new Label();
            tbAllow = new TextBox();

            cbShowExit = new CheckBox();
            cbRemember = new CheckBox();

            btnCancel = new Button();
            btnSaveStart = new Button();

            SuspendLayout();

            // ---- Form ----
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(740, 420);
            BackColor = System.Drawing.Color.White;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Kiosk Setup";
            TopMost = true;

            // ===== Constants for layout =====
            int LeftPad = 16;
            int RightPad = 16;
            int HeaderHeight = 64;
            int FooterHeight = 88;     // reserved space for checkboxes + buttons
            int ButtonGap = 8;         // gap between Cancel and Save
            int ButtonRightMargin = 8; // ALWAYS-KEPT white space at right edge

            // ---- Header bar (S&B) ----
            headerBar.BackColor = System.Drawing.Color.FromArgb(255, 210, 0); // #FFD200
            headerBar.Dock = DockStyle.Top;
            headerBar.Height = HeaderHeight;
            headerBar.Padding = new Padding(16, 12, 16, 8);

            // Header logo (full S&B) aligned to the right
            headerIcon.SizeMode = PictureBoxSizeMode.Zoom;
            headerIcon.BackColor = System.Drawing.Color.Transparent;
            headerIcon.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            // Adjust size to taste (width x height)
            headerIcon.Size = new System.Drawing.Size(260, 40);

            // Load from the app folder (bin\...\)
            headerIcon.ImageLocation = "snb_logo2.png";

            // Place 16px from the right edge, 12px from the top inside the yellow bar
            headerIcon.Location = new System.Drawing.Point(headerBar.Width - headerIcon.Width - 16, 12);

            // Left info icon (blue circle with "i")
            headerInfoIcon.Size = new System.Drawing.Size(32, 32);
            headerInfoIcon.Location = new System.Drawing.Point(16, 16);
            headerInfoIcon.SizeMode = PictureBoxSizeMode.Zoom;
            headerInfoIcon.Image = SystemIcons.Information.ToBitmap(); // built-in Windows "info" icon
            headerBar.Controls.Add(headerInfoIcon);


            headerTitle.AutoSize = true;
            headerTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            headerTitle.ForeColor = System.Drawing.Color.Black;
            headerTitle.Text = "Configure Kiosk";
            headerTitle.Location = new System.Drawing.Point(56, 12);

            headerSubtitle.AutoSize = true;
            headerSubtitle.Font = new System.Drawing.Font("Segoe UI", 9F);
            headerSubtitle.ForeColor = System.Drawing.Color.Black;
            headerSubtitle.Text = "Enter the website, PIN, and allowed domains";
            headerSubtitle.Location = new System.Drawing.Point(58, 40);

            headerBar.Controls.Add(headerIcon);
            headerBar.Controls.Add(headerTitle);
            headerBar.Controls.Add(headerSubtitle);
            Controls.Add(headerBar);

            // ---- Content positions (don’t use Bottom at init) ----
            int contentTop = HeaderHeight + 16;

            // URL
            lblUrl.AutoSize = true;
            lblUrl.Location = new System.Drawing.Point(LeftPad, contentTop);
            lblUrl.Text = "Start URL";

            tbUrl.Location = new System.Drawing.Point(LeftPad, contentTop + 18);
            tbUrl.Size = new System.Drawing.Size(ClientSize.Width - LeftPad - RightPad, 26);
            tbUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // PIN (visible)
            int pinTop = tbUrl.Bottom + 22;
            lblPin.AutoSize = true;
            lblPin.Location = new System.Drawing.Point(LeftPad, pinTop);
            lblPin.Text = "Exit PIN (visible)";

            tbPin.Location = new System.Drawing.Point(LeftPad, pinTop + 18);
            tbPin.Size = new System.Drawing.Size(180, 26);
            tbPin.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            tbPin.UseSystemPasswordChar = false;

            // Allowlist
            int allowTop = tbPin.Bottom + 22;
            lblAllow.AutoSize = true;
            lblAllow.Location = new System.Drawing.Point(LeftPad, allowTop);
            lblAllow.Text = "Allowlist (comma or new line)";

            tbAllow.Location = new System.Drawing.Point(LeftPad, allowTop + 18);
            tbAllow.Size = new System.Drawing.Size(ClientSize.Width - LeftPad - RightPad,
                                                  ClientSize.Height - (allowTop + 18) - FooterHeight);
            tbAllow.Multiline = true;
            tbAllow.ScrollBars = ScrollBars.Vertical;
            tbAllow.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // ---- Checkboxes ----
            cbShowExit.AutoSize = true;
            cbShowExit.Text = "Show red EXIT button (for testing)";
            cbShowExit.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            cbRemember.AutoSize = true;
            cbRemember.Text = "Remember these settings for next time";
            cbRemember.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            // ---- Buttons ----
            btnCancel.Text = "Cancel";
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Size = new System.Drawing.Size(100, 34);
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(200, 200, 200);
            btnCancel.FlatAppearance.BorderSize = 1;
            btnCancel.BackColor = System.Drawing.Color.White;
            btnCancel.ForeColor = System.Drawing.Color.Black;

            btnSaveStart.Text = "Save Start";
            btnSaveStart.Size = new System.Drawing.Size(110, 34);
            btnSaveStart.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSaveStart.FlatStyle = FlatStyle.Flat;
            btnSaveStart.FlatAppearance.BorderSize = 0;
            btnSaveStart.BackColor = System.Drawing.Color.FromArgb(255, 210, 0);
            btnSaveStart.ForeColor = System.Drawing.Color.Black;
            btnSaveStart.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            btnSaveStart.Click += btnSaveStart_Click;

            // ---- Initial footer placement ----
            int footerTop = ClientSize.Height - FooterHeight + 12;

            btnSaveStart.Location = new System.Drawing.Point(
                ClientSize.Width - RightPad - btnSaveStart.Width - ButtonRightMargin,
                footerTop + 20);

            btnCancel.Location = new System.Drawing.Point(
                btnSaveStart.Left - ButtonGap - btnCancel.Width,
                footerTop + 20);

            cbShowExit.Location = new System.Drawing.Point(LeftPad, footerTop + 4);
            cbRemember.Location = new System.Drawing.Point(LeftPad + 244, footerTop + 4);

            // ---- Resize behavior (keeps right margin) ----
            Resize += (_, __) =>
            {
                headerIcon.Location = new System.Drawing.Point(headerBar.Width - headerIcon.Width - 16, 12);

                int newFooterTop = ClientSize.Height - FooterHeight + 12;

                tbUrl.Width = ClientSize.Width - LeftPad - RightPad;
                tbAllow.Width = ClientSize.Width - LeftPad - RightPad;

                int newAllowHeight = ClientSize.Height - (allowTop + 18) - FooterHeight;
                if (newAllowHeight < 60) newAllowHeight = 60;
                tbAllow.Height = newAllowHeight;

                btnSaveStart.Location = new System.Drawing.Point(
                    ClientSize.Width - RightPad - btnSaveStart.Width - ButtonRightMargin,
                    newFooterTop + 20);

                btnCancel.Location = new System.Drawing.Point(
                    btnSaveStart.Left - ButtonGap - btnCancel.Width,
                    newFooterTop + 20);

                cbShowExit.Location = new System.Drawing.Point(LeftPad, newFooterTop + 4);
                cbRemember.Location = new System.Drawing.Point(LeftPad + 244, newFooterTop + 4);
            };

            // Accept/Cancel
            AcceptButton = btnSaveStart;
            CancelButton = btnCancel;

            // ---- Add controls ----
            Controls.Add(lblUrl);
            Controls.Add(tbUrl);
            Controls.Add(lblPin);
            Controls.Add(tbPin);
            Controls.Add(lblAllow);
            Controls.Add(tbAllow);
            Controls.Add(cbShowExit);
            Controls.Add(cbRemember);
            Controls.Add(btnCancel);
            Controls.Add(btnSaveStart);

            ResumeLayout(false);
            PerformLayout();
        }
    }
}
