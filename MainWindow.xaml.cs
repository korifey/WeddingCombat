using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WeddingCombat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private const double DeadZone = 0.1;
        Paddle _paddle1;
        Paddle _paddle2;
        Boundary _line1;
        Boundary _line2;
        Ball _ball;


        const string LeftName = "Давыдова";
        const string RightName = "Килимник";
        int _leftLostScore = 0;
        int _rightLostScore = 0;

        public SolidColorBrush LeftBrush { get; } = Brushes.Red;
        public SolidColorBrush RightBrush { get; } = Brushes.Cyan;

        public string LeftNameChangedLetter => RightName.Substring(0, _leftLostScore);
        public string LeftNameUntouchedLetter => LeftName.Substring(Math.Min(LeftName.Length, _leftLostScore));

        public string RightNameChangedLetter => LeftName.Substring(0, _rightLostScore);
        public string RightNameUntouchedLetter => RightName.Substring(Math.Min(RightName.Length, _rightLostScore));


        private List<GameObject> _objects;


        private void Reset()
        {
            _paddle1 = new Paddle(DeadZone, LeftBrush);
            _paddle2 = new Paddle(GameObject.XYRatio - DeadZone, RightBrush);
            _line1 = new Boundary(0 - Boundary.Height);
            _line2 = new Boundary(1);
            _ball = new Ball();

            _objects = new List<GameObject> {_paddle1, _paddle2, _line1, _line2, _ball};
        }

        public MainWindow()
        {
            Reset();
            InitializeComponent();
            Game();
        }

        private async void Game()
        {
            while (_leftLostScore < RightName.Length && _rightLostScore < LeftName.Length)
            {
                var eor = await Turn();

                Reset();
                
                if (eor == EndOfRound.LeftLost)
                {
                    Canva.Background = RightBrush.Clone();
                    _leftLostScore++;
                    OnPropertyChanged(nameof(LeftNameChangedLetter));
                    OnPropertyChanged(nameof(LeftNameUntouchedLetter));
                }

                else
                {
                    Canva.Background = LeftBrush.Clone();
                    _rightLostScore++;
                    OnPropertyChanged(nameof(RightNameChangedLetter));
                    OnPropertyChanged(nameof(RightNameUntouchedLetter));
                }
                

                var sw = Stopwatch.StartNew();
                while (sw.Elapsed < TimeSpan.FromSeconds(1))
                {
                    Canva.Background.Opacity = sw.ElapsedMilliseconds / 1000.0;
                    await Task.Delay(1000 / GameObject.Fps);
                }

                sw.Stop();
                Canva.Background = Brushes.Black;

            }
        }

        private async Task<EndOfRound> Turn()
        {
            while (true)
            {
                Canva.Children.Clear();
                foreach (var gameObject in _objects)
                {
                    gameObject.Update(_objects);
                    gameObject.Paint(Canva);
                }

                var eor = _ball.IsEndOfRound();
                if (eor != null) return eor.Value;

                await Task.Delay(1000 / GameObject.Fps);
            }
        }


        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Close();
                    break;

                case Key.W:
                    if (_paddle1.ThrottleIs(-1))
                        _paddle1.Throttle = 0;
                    break;
                case Key.S:
                    if (_paddle1.ThrottleIs(1))
                        _paddle1.Throttle = 0;
                    break;
                case Key.O:
                    if (_paddle2.ThrottleIs(-1))
                        _paddle2.Throttle = 0;
                    break;
                case Key.L:
                    if (_paddle2.ThrottleIs(1))
                        _paddle2.Throttle = 0;
                    break;


                default:
                    break;
            }
        }


        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Close();
                    break;

                case Key.W:
                    _paddle1.Throttle = -1;
                    break;
                case Key.S:
                    _paddle1.Throttle = 1;
                    break;
                case Key.O:
                    _paddle2.Throttle = -1;
                    break;
                case Key.L:
                    _paddle2.Throttle = 1;
                    break;

                default:
                    break;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}