using System;
using System.Windows;
using System.Windows.Threading;

namespace SoftphoneKiosk
{
    public partial class CallWindow : Window
    {
        private DispatcherTimer timer;
        private int seconds = 0;

        public CallWindow(string targetName)
        {
            InitializeComponent();
            CallTarget.Text = $"משדר כריזה אל: {targetName}"; // הצגת השם במקום המספר
            StartTimer();
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
            this.Close();
        }
    }
}