using System.Media;
using System.Windows;
using System.Windows.Threading;

namespace Stopwatch
{
    public partial class MainWindow : Window
    {
        // Секундомер
        private DispatcherTimer stopwatchTimer;
        private TimeSpan stopwatchElapsed;
        private DateTime stopwatchStartTime;
        private bool stopwatchRunning = false;

        // Таймер
        private DispatcherTimer countdownTimer;
        private TimeSpan countdownTime;
        private TimeSpan countdownRemaining;
        private bool countdownRunning = false;

        public MainWindow()
        {
            InitializeComponent();

            stopwatchTimer = new DispatcherTimer();
            stopwatchTimer.Interval = TimeSpan.FromMilliseconds(10);
            stopwatchTimer.Tick += StopwatchTimer_Tick;

            countdownTimer = new DispatcherTimer();
            countdownTimer.Interval = TimeSpan.FromMilliseconds(10);
            countdownTimer.Tick += CountdownTimer_Tick;
        }

        #region Stopwatch

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!stopwatchRunning)
            {
                stopwatchStartTime = DateTime.Now - stopwatchElapsed;
                stopwatchTimer.Start();
                stopwatchRunning = true;

                StartButton.IsEnabled = false;
                StopButton.IsEnabled = true;
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (stopwatchRunning)
            {
                stopwatchTimer.Stop();
                stopwatchRunning = false;

                StartButton.IsEnabled = true;
                StopButton.IsEnabled = false;
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            stopwatchTimer.Stop();
            stopwatchElapsed = TimeSpan.Zero;
            StopwatchDisplay.Text = "00:00:00.000";
            stopwatchRunning = false;

            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
        }

        private void StopwatchTimer_Tick(object sender, EventArgs e)
        {
            stopwatchElapsed = DateTime.Now - stopwatchStartTime;
            StopwatchDisplay.Text = stopwatchElapsed.ToString(@"hh\:mm\:ss\.fff");
        }

        #endregion

        #region Timer

        private DateTime countdownEndTime;

        private void SetTimerButton_Click(object sender, RoutedEventArgs e)
        {
            if (countdownRunning)
                return;

            if (int.TryParse(HoursTextBox.Text, out int hours) && int.TryParse(MinutesTextBox.Text, out int minutes) && int.TryParse(SecondsTextBox.Text, out int seconds))
            {
                countdownTime = new TimeSpan(hours, minutes, seconds);
                countdownRemaining = countdownTime;
                TimerDisplay.Text = countdownRemaining.ToString(@"hh\:mm\:ss");

                StartTimerButton.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите корректные числовые значения");
            }
        }

        private void StartTimerButton_Click(object sender, RoutedEventArgs e)
        {
            if (!countdownRunning && countdownRemaining > TimeSpan.Zero)
            {
                countdownEndTime = DateTime.Now.Add(countdownRemaining);
                countdownTimer.Start();
                countdownRunning = true;

                StartTimerButton.IsEnabled = false;
                StopTimerButton.IsEnabled = true;
                SetTimerButton.IsEnabled = false;
            }
        }

        private void StopTimerButton_Click(object sender, RoutedEventArgs e)
        {
            if (countdownRunning)
            {
                countdownTimer.Stop();
                countdownRunning = false;

                countdownRemaining = countdownEndTime - DateTime.Now;
                if (countdownRemaining < TimeSpan.Zero)
                    countdownRemaining = TimeSpan.Zero;

                TimerDisplay.Text = countdownRemaining.ToString(@"hh\:mm\:ss");

                StartTimerButton.IsEnabled = true;
                StopTimerButton.IsEnabled = false;
                SetTimerButton.IsEnabled = true;
            }
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            countdownRemaining = countdownEndTime - DateTime.Now;

            if (countdownRemaining > TimeSpan.Zero)
            {
                TimerDisplay.Text = countdownRemaining.ToString(@"hh\:mm\:ss");
            }
            else
            {
                countdownTimer.Stop();
                countdownRunning = false;
                countdownRemaining = TimeSpan.Zero;
                TimerDisplay.Text = "00:00:00";

                // Оповещение через месседж бокс
                SystemSounds.Beep.Play(); // Dance Maniac: Я не могу проверить эту бипку, у меня её возможно нет в ПК.
                MessageBox.Show("Время вышло!", "Таймер", MessageBoxButton.OK, MessageBoxImage.Information);

                StartTimerButton.IsEnabled = false;
                StopTimerButton.IsEnabled = false;
                SetTimerButton.IsEnabled = true;
            }
        }

        #endregion
    }
}