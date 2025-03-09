using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace BrigaSys
{
    public class CallHandler
    {
        private string appDirectory = "C:\\BrigaSys";
        private string microSipPath;
        private Process currentCallProcess;

        public CallHandler()
        {
            microSipPath = Path.Combine(appDirectory, "MicroSIP", "microsip.exe");
        }

        public void DialNumber(string extension, string targetName)
        {
            if (!File.Exists(microSipPath))
            {
                MessageBox.Show("MicroSIP לא נמצא! ודא שהתוכנה מותקנת.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string command = $"\"{microSipPath}\" {extension}";

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C {command}",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            };

            currentCallProcess = Process.Start(psi);
        }

        public void HangupCall()
        {
            if (!File.Exists(microSipPath))
            {
                MessageBox.Show("MicroSIP לא נמצא! ודא שהתוכנה מותקנת.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string command = $"\"{microSipPath}\" /hangupall";

            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C {command}",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            });

            if (currentCallProcess != null && !currentCallProcess.HasExited)
            {
                currentCallProcess.Kill();
                currentCallProcess = null;
            }
        }
    }
}