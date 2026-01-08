using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Kiosk7
{
    public partial class SetupForm : Form
    {
        public string SelectedUrl { get; private set; } = "";
        public string SelectedPin { get; private set; } = "";
        public List<string> SelectedAllowlist { get; private set; } = new();
        public bool SelectedShowExit { get; private set; } = false;

        public SetupForm(string defaultUrl, string defaultPin, IEnumerable<string>? defaultAllowlist, bool defaultShowExit)
        {
            InitializeComponent();

            headerIcon.Image = System.Drawing.SystemIcons.Information.ToBitmap();

            tbUrl.Text = string.IsNullOrWhiteSpace(defaultUrl) ? "https://www.bbc.com" : defaultUrl;
            tbPin.Text = string.IsNullOrWhiteSpace(defaultPin) ? "1234" : defaultPin;
            tbAllow.Text = string.Join(Environment.NewLine, defaultAllowlist ?? Array.Empty<string>());
            cbShowExit.Checked = defaultShowExit;
        }

        private void btnSaveStart_Click(object sender, EventArgs e)
        {
            string url = tbUrl.Text.Trim();
            string pin = tbPin.Text.Trim();

            if (!Uri.TryCreate(url, UriKind.Absolute, out var u) ||
                (u.Scheme != Uri.UriSchemeHttp && u.Scheme != Uri.UriSchemeHttps))
            {
                MessageBox.Show("URL must start with http:// or https://");
                tbUrl.Focus();
                tbUrl.SelectAll();
                return;
            }

            if (string.IsNullOrWhiteSpace(pin))
            {
                MessageBox.Show("PIN cannot be empty.");
                tbPin.Focus();
                return;
            }

            SelectedUrl = url;
            SelectedPin = pin;

            SelectedAllowlist = tbAllow.Text
                .Split(new[] { '\r', '\n', ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            SelectedShowExit = cbShowExit.Checked;

            DialogResult = DialogResult.OK;
        }
    }
}
