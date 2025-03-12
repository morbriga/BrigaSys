using System;
using System.Windows;
using System.Windows.Threading;

namespace BrigaSys
{
    public partial class CallWindow : Window
    {
        private DispatcherTimer timer;
        private int seconds = 0;
        private CallHandler callHandler;
        private string extension;
        private string targetName;

        public CallWindow(string targetName, CallHandler callHandler, string extension)
        {
            InitializeComponent();
            CallTarget.Text = $"משדר כריזה אל: {targetName}";
            StartTimer();
            this.callHandler = callHandler;
            this.extension = extension;
            if (extension != null)
            {
                callHandler.DialNumber(extension, CallTarget.Text.Replace("משדר כריזה אל: ", ""));
            }

        }

        public CallWindow(string targetName, CallHandler callHandler)
        {
            InitializeComponent();
            this.targetName = targetName;
            this.callHandler = callHandler;
        }

        private void StartTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) =>
            {
                seconds++;
                CallTimer.Text = TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss");
            };
            timer.Start();
        }

        private void EndCall(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            callHandler.HangupCall();
            this.Close();
        }
    }
}