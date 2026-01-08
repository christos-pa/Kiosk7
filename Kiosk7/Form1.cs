using System;
using System.Diagnostics;
using System.Drawing;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace Kiosk7
{
    public partial class Form1 : Form
    {
        // =======================
        //   SECRET 5-TAP UNLOCK
        // =======================
        private int _tapCount = 0;
        private DateTime _tapWindowStart;
        private const double TapWindowSeconds = 1.5;
        private const int RequiredTaps = 5;

        private ClickThroughPanel _tapOverlay;

        private const double HotspotWidthPercent = 0.10;
        private const double HotspotHeightPercent = 0.15;

        // =======================
        //  SECRET TAP LOGIC
        // =======================
        private void CheckSecretTap(Point p)
        {
            int hotspotX = (int)(ClientSize.Width * (1.0 - HotspotWidthPercent));
            int hotspotY = (int)(ClientSize.Height * (1.0 - HotspotHeightPercent));
            int hotspotWidth = (int)(ClientSize.Width * HotspotWidthPercent);
            int hotspotHeight = (int)(ClientSize.Height * HotspotHeightPercent);

            var rect = new Rectangle(hotspotX, hotspotY, hotspotWidth, hotspotHeight);
            if (!rect.Contains(p))
                return;

            var now = DateTime.Now;

            if ((now - _tapWindowStart).TotalSeconds > TapWindowSeconds)
            {
                _tapWindowStart = now;
                _tapCount = 0;
            }

            _tapCount++;

            if (_tapCount >= RequiredTaps)
            {
                _tapCount = 0;

                // SECRET ROUTE → uses MASTER PIN (sb4711) with looping retry
                if (ConfirmMasterPin_Looping())
                {
                    _allowClose = true;
                    ShowSystemUI();
                    UninstallKeyboardHook();
                    Application.Exit();
                }
            }
        }

        // =======================
        //  LOOPING MASTER PIN (for 5-tap)
        // =======================
        private bool ConfirmMasterPin_Looping()
        {
            while (true)
            {
                using var dlg = new PinPrompt();
                var result = dlg.ShowDialog(this);

                if (result != DialogResult.OK)
                    return false;

                // MASTER PIN
                if (dlg.PinText == "sb4711")
                    return true;

                SystemSounds.Hand.Play();
                AccessDeniedForm.ShowDialogBox(this);
            }
        }

        // =======================
        //   EXISTING CODE BELOW
        // =======================

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        private Config _cfg;
        private bool _allowClose = false;
        private KeyboardForm? _keyboard;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        private IntPtr _hTaskbar;
        private IntPtr _hSecondaryTaskbar;
        private IntPtr _hStartButton;

        private void HideSystemUI()
        {
            _hTaskbar = FindWindow("Shell_TrayWnd", null);
            if (_hTaskbar != IntPtr.Zero) ShowWindow(_hTaskbar, SW_HIDE);

            _hSecondaryTaskbar = FindWindow("Shell_SecondaryTrayWnd", null);
            if (_hSecondaryTaskbar != IntPtr.Zero) ShowWindow(_hSecondaryTaskbar, SW_HIDE);

            _hStartButton = FindWindow("Button", "Start");
            if (_hStartButton != IntPtr.Zero) ShowWindow(_hStartButton, SW_HIDE);
        }

        private void ShowSystemUI()
        {
            if (_hTaskbar != IntPtr.Zero) ShowWindow(_hTaskbar, SW_SHOW);
            if (_hSecondaryTaskbar != IntPtr.Zero) ShowWindow(_hSecondaryTaskbar, SW_SHOW);
            if (_hStartButton != IntPtr.Zero) ShowWindow(_hStartButton, SW_SHOW);
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")] private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll")] private static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll")] private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll")] private static extern IntPtr GetModuleHandle(string lpModuleName);
        [DllImport("user32.dll")] private static extern short GetAsyncKeyState(int vKey);

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;

        private const int VK_TAB = 0x09;
        private const int VK_ESCAPE = 0x1B;
        private const int VK_SPACE = 0x20;
        private const int VK_F4 = 0x73;
        private const int VK_F11 = 0x7A;
        private const int VK_LWIN = 0x5B;
        private const int VK_RWIN = 0x5C;
        private const int VK_APPS = 0x5D;

        private const int VK_SHIFT = 0x10;
        private const int VK_CONTROL = 0x11;
        private const int VK_MENU = 0x12;

        private IntPtr _kbHook = IntPtr.Zero;
        private LowLevelKeyboardProc _kbProc;

        private static bool IsKeyDown(int vk) => (GetAsyncKeyState(vk) & 0x8000) != 0;

        private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);

                bool alt = IsKeyDown(VK_MENU);
                bool ctrl = IsKeyDown(VK_CONTROL);
                bool shift = IsKeyDown(VK_SHIFT);

                bool block = false;

                if (vkCode == VK_LWIN || vkCode == VK_RWIN || vkCode == VK_APPS)
                    block = true;

                if (alt && (vkCode == VK_TAB || vkCode == VK_ESCAPE || vkCode == VK_F4 || vkCode == VK_SPACE))
                    block = true;

                if (vkCode == VK_F11) block = true;

                if ((ctrl && vkCode == VK_ESCAPE) || (ctrl && shift && vkCode == VK_ESCAPE))
                    block = true;

                if (block) return (IntPtr)1;
            }
            return CallNextHookEx(_kbHook, nCode, wParam, lParam);
        }

        private void InstallKeyboardHook()
        {
            _kbProc = KeyboardHookCallback;
            using var curProcess = Process.GetCurrentProcess();
            using var curModule = curProcess.MainModule!;
            _kbHook = SetWindowsHookEx(WH_KEYBOARD_LL, _kbProc, GetModuleHandle(curModule.ModuleName), 0);
        }

        private void UninstallKeyboardHook()
        {
            if (_kbHook != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_kbHook);
                _kbHook = IntPtr.Zero;
            }
        }

        private void SendToWeb(string keySpec)
        {
            try
            {
                webView21.Focus();
                SendKeys.SendWait(keySpec);
            }
            catch { }
        }

        private void ApplyRounded(Control c, int radius)
        {
            IntPtr rgn = CreateRoundRectRgn(0, 0, c.Width, c.Height, radius, radius);
            c.Region = Region.FromHrgn(rgn);
            DeleteObject(rgn);
        }

        private void WireButtonStates(Button btn, Color baseColor)
        {
            void Set(Color col) => btn.BackColor = col;

            btn.BackColor = baseColor;
            btn.MouseEnter += (_, __) => Set(ControlPaint.Light(baseColor, 0.10f));
            btn.MouseLeave += (_, __) => Set(baseColor);
            btn.MouseDown += (_, __) => Set(ControlPaint.Dark(baseColor, 0.10f));
            btn.MouseUp += (_, __) => Set(ControlPaint.Light(baseColor, 0.10f));

            btn.Resize += (_, __) => ApplyRounded(btn, 18);
        }

        private bool ConfirmAdminPin()
        {
            while (true)
            {
                using var dlg = new PinPrompt();
                var result = dlg.ShowDialog(this);

                if (result != DialogResult.OK)
                    return false;

                if (dlg.PinText == _cfg.Pin)
                    return true;

                SystemSounds.Hand.Play();
                AccessDeniedForm.ShowDialogBox(this);
            }
        }

        // ============================
        //   MAIN CONSTRUCTOR
        // ============================
        public Form1(Config cfg)
        {
            InitializeComponent();
            _cfg = cfg;

            // Invisible overlay
            _tapOverlay = new ClickThroughPanel();
            _tapOverlay.BackColor = Color.Transparent;
            _tapOverlay.Dock = DockStyle.Fill;
            _tapOverlay.Enabled = true;
            Controls.Add(_tapOverlay);

            _tapOverlay.BringToFront();
            _tapOverlay.MouseDown += (s, e) => CheckSecretTap(e.Location);

            // Fullscreen
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            TopMost = true;
            KeyPreview = true;

            webView21.Dock = DockStyle.Fill;

            // EXIT BUTTON
            if (_cfg.ShowExitButton)
            {
                var exitBtn = new Button
                {
                    Name = "ExitButton",
                    Text = "\U0001F513  EXIT",
                    UseCompatibleTextRendering = true,
                    Font = new Font("Segoe UI Emoji", 16f, FontStyle.Bold),
                    BackColor = Color.FromArgb(220, 35, 50),
                    ForeColor = Color.White,
                    Size = new Size(160, 56),
                    FlatStyle = FlatStyle.Flat,
                    TabStop = false,
                    Cursor = Cursors.Hand,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left
                };
                exitBtn.FlatAppearance.BorderSize = 0;
                exitBtn.Location = new Point(60, 60);

                ApplyRounded(exitBtn, 18);
                WireButtonStates(exitBtn, Color.FromArgb(220, 35, 50));

                exitBtn.Click += (_, __) =>
                {
                    if (ConfirmAdminPin())
                    {
                        _allowClose = true;
                        ShowSystemUI();
                        UninstallKeyboardHook();
                        Application.Exit();
                    }
                };

                Controls.Add(exitBtn);
                exitBtn.BringToFront();
            }

            // KEYBOARD BUTTON
            var kbBtn = new Button
            {
                Name = "KeyboardButton",
                Text = "\u2328  Keyboard",
                UseCompatibleTextRendering = true,
                Font = new Font("Segoe UI Emoji", 16f, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                Size = new Size(160, 56),
                FlatStyle = FlatStyle.Flat,
                TabStop = false,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            kbBtn.FlatAppearance.BorderSize = 0;
            Controls.Add(kbBtn);
            kbBtn.BringToFront();

            kbBtn.Location = new Point(ClientSize.Width - kbBtn.Width - 60, 60);
            this.Resize += (_, __) =>
            {
                if (!IsDisposed && Controls.Contains(kbBtn))
                    kbBtn.Location = new Point(ClientSize.Width - kbBtn.Width - 60, 60);
            };

            ApplyRounded(kbBtn, 18);
            WireButtonStates(kbBtn, Color.FromArgb(0, 122, 204));

            kbBtn.Click += (_, __) =>
            {
                if (_keyboard == null || _keyboard.IsDisposed)
                    _keyboard = new KeyboardForm(SendToWeb) { TopMost = true, Owner = this };

                if (_keyboard.Visible) _keyboard.Hide();
                else
                {
                    _keyboard.Show(this);
                    _keyboard.BringToFront();
                }
            };

            // OS PROTECTION
            Shown += (_, __) =>
            {
                HideSystemUI();
                InstallKeyboardHook();
                Activate();
                TopMost = true;
            };

            Activated += (_, __) => { TopMost = true; };
            Deactivate += (_, __) => { Activate(); };

            FormClosing += (s, e) =>
            {
                if (!_allowClose)
                {
                    if (ConfirmAdminPin())
                    {
                        _allowClose = true;
                        ShowSystemUI();
                        UninstallKeyboardHook();
                    }
                    else
                    {
                        e.Cancel = true;
                        SystemSounds.Hand.Play();
                    }
                }
            };

            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.F10)
                {
                    e.Handled = true;
                    if (ConfirmAdminPin())
                    {
                        _allowClose = true;
                        ShowSystemUI();
                        UninstallKeyboardHook();
                        Application.Exit();
                    }
                }
            };

            webView21.PreviewKeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.F10)
                {
                    e.IsInputKey = true;
                    BeginInvoke(new Action(() =>
                    {
                        if (ConfirmAdminPin())
                        {
                            _allowClose = true;
                            ShowSystemUI();
                            UninstallKeyboardHook();
                            Application.Exit();
                        }
                    }));
                }
            };

            Load += Form1_Load;
        }

        public Form1() : this(Config.Load(AppContext.BaseDirectory))
        {
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await webView21.EnsureCoreWebView2Async(null);

            webView21.CoreWebView2.NewWindowRequested += (s, ev) =>
            {
                var uri = ev.Uri;
                if (!_cfg.IsAllowed(uri))
                {
                    ev.Handled = true;
                    return;
                }
                ev.Handled = true;
                webView21.CoreWebView2.Navigate(uri);
            };

            webView21.CoreWebView2.DownloadStarting += (s, ev) => ev.Cancel = true;

            webView21.CoreWebView2.NavigationStarting += (s, ev) =>
            {
                if (!_cfg.IsAllowed(ev.Uri))
                    ev.Cancel = true;
            };

            var s2 = webView21.CoreWebView2.Settings;
            s2.AreDefaultContextMenusEnabled = false;
            s2.AreDevToolsEnabled = false;
            s2.IsStatusBarEnabled = false;
            s2.IsZoomControlEnabled = false;

            webView21.CoreWebView2.NavigationCompleted += async (_, __) =>
            {
                try
                {
                    await webView21.ExecuteScriptAsync(
                        "document.addEventListener('contextmenu', e => e.preventDefault(), {capture:true});"
                    );
                }
                catch { }
            };

            webView21.CoreWebView2.Navigate(_cfg.Url);
        }
    }
}
