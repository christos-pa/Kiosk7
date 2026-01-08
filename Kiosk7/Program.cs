using System;
using System.IO;
using System.Windows.Forms;

namespace Kiosk7
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            string cfgPath = Path.Combine(AppContext.BaseDirectory, "settings.cfg");

            // If config exists → load it and start kiosk directly
            if (File.Exists(cfgPath))
            {
                var cfg = Config.Load(AppContext.BaseDirectory);
                Application.Run(new Form1(cfg));
                return;
            }

            // Otherwise → run SetupForm first
            using var setup = new SetupForm("", "1234", null, false);
            if (setup.ShowDialog() != DialogResult.OK)
                return;

            // Build config from SetupForm
            var newCfg = new Config
            {
                Url = setup.SelectedUrl,
                Pin = setup.SelectedPin,
                Allowlist = setup.SelectedAllowlist,
                ShowExitButton = setup.SelectedShowExit
            };

            // Save config to settings.cfg
            newCfg.Save(AppContext.BaseDirectory);

            // Start kiosk with saved config
            Application.Run(new Form1(newCfg));
        }
    }
}
