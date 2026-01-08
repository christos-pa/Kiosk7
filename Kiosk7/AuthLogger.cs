using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Kiosk7
{
    internal static class AuthLogger
    {
        private static readonly object _lock = new object();

        private static string BaseDir => AppDomain.CurrentDomain.BaseDirectory;
        private static string LogDir => Path.Combine(BaseDir, "logs");

        // One log file per year
        private static string LogFile =>
            Path.Combine(LogDir, $"auth-{DateTime.Now:yyyy}.log");

        // =========================
        //  FAILED LOGOUT / UNLOCK
        // =========================
        public static void LogFailedLogout(string attemptedSecret, int attemptNumber)
        {
            try
            {
                string ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string machine = Environment.MachineName;
                // CHANGE HERE: log the full attempted secret in plain text
                string secretDisplay = attemptedSecret ?? "(null)";
                string line =
                    $"{ts} | FAILED_LOGOUT | {machine} | Attempt {attemptNumber} | ENTERED={secretDisplay}";
                WriteLine(line);
            }
            catch
            {
                // Never allow logging to crash kiosk
            }
        }

        // =========================
        //  OPTIONAL SUCCESS LOG
        // =========================
        public static void LogLogoutSuccess()
        {
            try
            {
                string ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string machine = Environment.MachineName;

                string line =
                    $"{ts} | LOGOUT_SUCCESS | {machine}";

                WriteLine(line);
            }
            catch
            {
                // ignore
            }
        }

        // =========================
        //  INTERNAL WRITE
        // =========================
        private static void WriteLine(string line)
        {
            try
            {
                lock (_lock)
                {
                    Directory.CreateDirectory(LogDir);
                    File.AppendAllText(LogFile, line + Environment.NewLine);
                }
            }
            catch
            {
                // Logging must NEVER break kiosk flow
            }
        }

        // =========================
        //  HASH (NON-REVERSIBLE)
        // =========================
        private static string ShortHash(string input)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));

                // First 8 bytes = short fingerprint (16 hex chars)
                var sb = new StringBuilder(16);
                for (int i = 0; i < 8; i++)
                    sb.Append(bytes[i].ToString("X2"));

                return sb.ToString();
            }
        }
    }
}
