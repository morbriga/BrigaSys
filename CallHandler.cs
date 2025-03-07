using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace SoftphoneKiosk
{
    public class CallHandler
    {
        private string appDirectory = "C:\\SoftphoneKiosk";
        private string microSipPath;

        public CallHandler()
        {
            microSipPath = Path.Combine(appDirectory, "MicroSIP", "microsip.exe");
        }

        public void DialNumber(string extension)
        {
            if (!File.Exists(microSipPath))
            {
                MessageBox.Show("MicroSIP לא נמצא! ודא שהתוכנה מותקנת.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string command = $"\"{microSipPath}\" {extension}";

            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C {command}",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            });
        }

        public void HangupCall()
        {
            if (!File.Exists(microSipPath))
            {
                MessageBox.Show("MicroSIP לא נמצא! ודא שהתוכנה מותקנת.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string command = $"\"{microSipPath}\" /hangup";

            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C {command}",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            });
        }
    }
}
