using System;
using System.Text;
using System.Threading;

namespace ThousandSchnapsen.CFR.Utils
{
    public class ProgressBar : IDisposable, IProgress<(double, string)>
    {
        private const int BlockCount = 100;
        private readonly TimeSpan _animationInterval = TimeSpan.FromSeconds(1.0 / 8);
        private const string Animation = @"|/-\";

        private readonly Timer _timer;

        private double _currentProgress;
        private string _currentText = string.Empty;
        private string _currentAdditionalInfo = string.Empty;
        private bool _disposed;
        private int _animationIndex;

        public ProgressBar()
        {
            _timer = new Timer(TimerHandler);

            if (!Console.IsOutputRedirected)
            {
                ResetTimer();
            }
        }

        public void Report((double, string) data)
        {
            var (value, additionalInfo) = data;
            value = Math.Max(0, Math.Min(1, value));
            Interlocked.Exchange(ref _currentProgress, value);
            if (!String.IsNullOrEmpty(additionalInfo))
                Interlocked.Exchange(ref _currentAdditionalInfo, additionalInfo);
        }

        private void TimerHandler(object state)
        {
            lock (_timer)
            {
                if (_disposed) return;

                var progressBlockCount = (int) (_currentProgress * BlockCount);
                var percent = (int) (_currentProgress * 100);
                var text = string.Format("[{0}{1}] {2,3}% {3} {4}",
                    new string('#', progressBlockCount), new string('-', BlockCount - progressBlockCount),
                    percent,
                    Animation[_animationIndex++ % Animation.Length],
                    _currentAdditionalInfo);
                UpdateText(text);

                ResetTimer();
            }
        }

        private void UpdateText(string text)
        {
            var commonPrefixLength = 0;
            var commonLength = Math.Min(_currentText.Length, text.Length);
            while (commonPrefixLength < commonLength && text[commonPrefixLength] == _currentText[commonPrefixLength])
            {
                commonPrefixLength++;
            }

            var outputBuilder = new StringBuilder();
            outputBuilder.Append('\b', _currentText.Length - commonPrefixLength);

            outputBuilder.Append(text.Substring(commonPrefixLength));

            var overlapCount = _currentText.Length - text.Length;
            if (overlapCount > 0)
            {
                outputBuilder.Append(' ', overlapCount);
                outputBuilder.Append('\b', overlapCount);
            }

            Console.Write(outputBuilder);
            _currentText = text;
        }

        private void ResetTimer()
        {
            _timer.Change(_animationInterval, TimeSpan.FromMilliseconds(-1));
        }

        public void Dispose()
        {
            lock (_timer)
            {
                _disposed = true;
                UpdateText(string.Empty);
            }
        }
    }
}