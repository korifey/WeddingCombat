using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SlimDX.DirectInput;
using Key = System.Windows.Input.Key;

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

        private int _invert;
        
        
        const string LeftName = "Давыдова";
        const string RightName = "Килимник";
        int _leftLostScore = 0;
        int _rightLostScore = 0;
        private int _round = 0; 

        public SolidColorBrush LeftBrush { get; } = Brushes.OrangeRed;
        public SolidColorBrush RightBrush { get; } = Brushes.Cyan;

        public string RoundText { get; private set; } = "Oops";

        public string LeftNameChangedLetter => RightName.Substring(0, _leftLostScore);
        public string LeftNameUntouchedLetter => LeftName.Substring(Math.Min(LeftName.Length, _leftLostScore));

        public string RightNameChangedLetter => LeftName.Substring(0, _rightLostScore);
        public string RightNameUntouchedLetter => RightName.Substring(Math.Min(RightName.Length, _rightLostScore));

        WMPLib.WindowsMediaPlayer _melodyPlayer = new WMPLib.WindowsMediaPlayer();
        WMPLib.WindowsMediaPlayer _collideBoundaryPlayer = new WMPLib.WindowsMediaPlayer();
        WMPLib.WindowsMediaPlayer _collidePaddlePlayer = new WMPLib.WindowsMediaPlayer();
        WMPLib.WindowsMediaPlayer _failPlayer = new WMPLib.WindowsMediaPlayer();

        private List<GameObject> _objects;

        private Joystick _joystick1;
        private Joystick _joystick2;

        private bool _event = false;

        private int _eventNumber = 0;


        private void ResetGameobject()
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
            ResetGameobject();

            CreditItems = new ObservableCollection<CreditItem>()
            {
                new CreditItem("Автор идеи", "Алексей Середкин", "Lesha"),
                new CreditItem("Сценарист", "Виталий Касьянов", "Vitalik"),
                new CreditItem("Жена сценариста", "Наталья Касьянова", "Natasha"),
                new CreditItem("Дизайнер", "Ольга Безгинова", "Olya"),
                new CreditItem("Программист", "Дмитрий Иванов", "Dima"),
                new CreditItem("Ассистент программиста", "Екатерина Иванова", "Katya"),
                new CreditItem("ВРАЧ", "Галина Пузова", "Galya"),
                new CreditItem("Ведущий", "Александр Безгинов", "Sasha"),
                new CreditItem("Ведущая", "Гузель Гарипова ", "Guzya"),
                new CreditItem("Менеджер и звукорежиссер", "Ксения Расина", "Ksyusha"),
                new CreditItem("Ассистент звукорежиссера", "Саддам Алиев", "Saddam"),
                new CreditItem("Видеомонтаж", "Никита Касьянов", "Nikita"),
                new CreditItem("Тусовщик", "Елизавета Дорофеева", "Liza"),
            };


            InitializeComponent();
            InitJoystick();
            _melodyPlayer.URL = "music/Start.mp3";
            _melodyPlayer.settings.volume = 100;
            _melodyPlayer.controls.play();
            
            _collideBoundaryPlayer.URL = "music/BallBoundary.mp3";
            _collideBoundaryPlayer.controls.stop();
            
            _collidePaddlePlayer.URL = "music/BallPaddle.mp3";
            _collidePaddlePlayer.controls.stop();
            
            _failPlayer.URL = "music/Fail.mp3";
            _failPlayer.controls.stop();
            

//            while (true)
//            {
//                AskJoysticks();
//            }
        }


        private void InitJoystick()
        {
            var directInput = new DirectInput();
            var deviceInstances = directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices);

            if (deviceInstances.Count > 0)
            {
                _joystick1 = new Joystick(directInput, deviceInstances[0].InstanceGuid);
                _joystick1.Properties.BufferSize = 128;
                _joystick1.Acquire();

                if (deviceInstances.Count > 1)
                {
                    _joystick2 = new Joystick(directInput, deviceInstances[1].InstanceGuid);
                    _joystick2.Properties.BufferSize = 128;
                    _joystick2.Acquire();
                }
            }
        }

        void AskJoysticks()
        {
            if (_joystick1 != null)
                AskJoystick(_joystick1, _paddle1, 1 << 16);
            
            if (_joystick2 != null)
                AskJoystick(_joystick2, _paddle2, 1 << 15);
        }

        private void AskJoystick(Joystick joystick, Paddle paddle, int max)
        {
//            var x = joystick.Poll();
//            var y = joystick.GetCurrentState();
            var joystickStates = joystick.GetBufferedData();
            foreach (var data in joystickStates)
            {
                if (data.Y == 0)
                    continue;
                
                if (data.Y > 0 && data.Y < max * 0.8 / 2)
                {
                    paddle.Throttle = -1*_invert;
                }
                else if (data.Y > max * 0.8 / 2 && data.Y < max * 1.2 / 2)
                {
                    paddle.Throttle = 0;
                }
                else if (data.Y > max * 1.2 / 2)
                {
                    paddle.Throttle = 1*_invert;
                }
                
            }
        }


        private async void Game()
        {
            _melodyPlayer.controls.stop();
//            _melodyPlayer.URL = "music/Game.mp3";
//            _melodyPlayer.controls.play();
            _melodyPlayer.settings.volume = 15;
            
            _melodyPlayer.controls.stop();
            _melodyPlayer.URL = "music/Game.mp3";
            _melodyPlayer.controls.play();
            _melodyPlayer.settings.volume = 15;
            
            await Round(0);
            await Round(1);
            await Round(2);
            await Round(3);
            
            GamePage.Visibility = Visibility.Collapsed;

            _event = true;
            _eventNumber = 4;
            await HandleEvent();
            
            CreditsPage.Visibility = Visibility.Visible;
            Credits();
        }

        
        
        private async Task Round(int round)
        {
            _invert = 1;
            ResetGameobject();
            
            _leftLostScore = 0;
            _rightLostScore = 0;
            OnPropertyChanged(nameof(LeftNameChangedLetter));
            OnPropertyChanged(nameof(LeftNameUntouchedLetter));
            OnPropertyChanged(nameof(RightNameChangedLetter));
            OnPropertyChanged(nameof(RightNameUntouchedLetter));

            _melodyPlayer.URL = $"music/Round{round}.mp3";
            _melodyPlayer.controls.play();
            
            RoundText = "Round "+_round;
            OnPropertyChanged(nameof(RoundText));
            RoundPage.Visibility = Visibility.Visible;
            await Task.Delay(1500);
            RoundPage.Visibility = Visibility.Collapsed;


            

            const int maxScore = 8;
            
            while (_leftLostScore < maxScore && _rightLostScore < maxScore)
            {
                
                //automatic event dispatch
                if (_leftLostScore == 1 || _rightLostScore == 1)
                {
                    if (round == 2 && _eventNumber < 1)
                    {
                        _event = true;
                        _eventNumber = 1;
                    }

                    if (round == 3 && _eventNumber < 2)
                    {
                        _event = true;
                        _eventNumber = 2;
                    }
                }
                
                
                //event handler
                if (_event)
                {
                    await HandleEvent();

                    continue;
                }
                
                //real turn
                var eor = await Turn();

                ResetGameobject();

                if (eor == EndOfRound.LeftLost)
                {
                    Canva.Background = RightBrush.Clone();
                    if (round > 0)
                    {
                        _leftLostScore++;
                    }

                    OnPropertyChanged(nameof(LeftNameChangedLetter));
                    OnPropertyChanged(nameof(LeftNameUntouchedLetter));
                }

                else
                {
                    Canva.Background = LeftBrush.Clone();
                    if (round > 0)
                    {
                        _rightLostScore++;
                    }

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

            
            if (round > 0)
            {
                _melodyPlayer.URL = "music/RoundFail.mp3";
                RoundFailPage.Visibility = Visibility.Visible;
                await Task.Delay(3000);
                RoundFailPage.Visibility = Visibility.Collapsed;
            }

            _round++;

        }

        private async Task HandleEvent()
        {
            var oldUrl = _melodyPlayer.URL;
            var oldVolume = _melodyPlayer.settings.volume;

            EventPage.Visibility = Visibility.Visible;
            switch (_eventNumber)
            {
                case 1:
                    WineTime.Visibility = Visibility.Visible;
                    _invert = -1;
                    break;


                case 2:
                    EventImageElections.Visibility = Visibility.Visible;
                    _melodyPlayer.URL = "music/Elections.mp3";
                    _melodyPlayer.settings.volume = 100;
                    break;

                case 3:
                    EventImagePutin.Visibility = Visibility.Visible;
                    _melodyPlayer.URL = "music/Putin.mp3";
                    _melodyPlayer.settings.volume = 100;
                    break;
                
                case 4:
                    EventImageGameover.Visibility = Visibility.Visible;
                    break;
    
                default:
                    break;
            }

            const int timeMs = 2000;
            const int MaxPercents = 100;
            EventPage.Opacity = 0;
            for (int persents = 0; persents < MaxPercents; persents++)
            {
                await Task.Delay(timeMs / MaxPercents);
                EventPage.Opacity = 1.0 * persents / MaxPercents;
            }

            await EvtFinished();

            WineTime.Visibility = Visibility.Collapsed;
            EventImageElections.Visibility = Visibility.Collapsed;
            EventImagePutin.Visibility = Visibility.Collapsed;
            EventImageGameover.Visibility = Visibility.Collapsed;

            //putin
            if (_eventNumber == 2)
            {
                _eventNumber++;
                _event = true;
            }

            if (_melodyPlayer.URL != oldUrl)
            {
                _melodyPlayer.URL = oldUrl;
                _melodyPlayer.settings.volume = oldVolume;
            }
        }

        private async Task EvtFinished()
        {
            while (_event)
            {
                await Task.Delay(20);
            }
        }

        private async Task<EndOfRound> Turn()
        {
            while (true)
            {
                AskJoysticks();
                Canva.Children.Clear();
                foreach (var gameObject in _objects)
                {
                    var res = gameObject.Update(_objects);
                    if (res != null)
                    {
                        if (res == CollisionKind.Boundary)
                        {
                            _collideBoundaryPlayer.controls.stop();
                            _collideBoundaryPlayer.controls.play();
                        }

                        if (res == CollisionKind.Paddle)
                        {
                            _collidePaddlePlayer.controls.stop();
                            _collidePaddlePlayer.controls.play();
                        }
                    }
                    gameObject.Paint(Canva);
                }

                var eor = _ball.IsEndOfRound();
                if (eor != null)
                {
                    _failPlayer.controls.stop();
                    _failPlayer.controls.play();
                    return eor.Value;
                }

                await Task.Delay(1000 / GameObject.Fps);
            }
        }


        async void Credits()
        {
//            _melodyPlayer.controls.stop();
            _melodyPlayer.URL = "music/Credits.mp3";
            _melodyPlayer.settings.volume = 100;
            _melodyPlayer.controls.play();
            
            foreach (var credit in CreditItems)
            {
                const int timeMs = 2000;
                const int MaxPercents = 100;
                for (int persents = 0; persents < MaxPercents; persents++)
                {
                    await Task.Delay(timeMs / MaxPercents);
                    credit.Opacity = 1.0 * persents / MaxPercents;
                }

                for (int fps = 0; fps < MaxPercents; fps++)
                {
                    await Task.Delay(timeMs / MaxPercents);
                    credit.Opacity = 1.0 - 1.0 * fps / MaxPercents;
                }

                await Task.Delay(200);
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
                    if (_paddle1.ThrottleIs(-1*_invert))
                        _paddle1.Throttle = 0;
                    break;
                case Key.S:
                    if (_paddle1.ThrottleIs(1*_invert))
                        _paddle1.Throttle = 0;
                    break;
                case Key.O:
                    if (_paddle2.ThrottleIs(-1*_invert))
                        _paddle2.Throttle = 0;
                    break;
                case Key.L:
                    if (_paddle2.ThrottleIs(1*_invert))
                        _paddle2.Throttle = 0;
                    break;
                
                case Key.Space:
                    
                    if (StartPage.IsVisible)
                    {
                        _melodyPlayer.controls.stop();
                        StartPage.Visibility = Visibility.Collapsed;
                        GamePage.Visibility = Visibility.Visible;
                        Game();
                    }
                    else
                    {
                        _leftLostScore = 999;
                    }
                    break;
                
                
                case Key.Enter:
                    
//                    if (!_event)
//                    {
//                        _event = true;
//                        _eventNumber++;
//                    }
//                    else 
                    if (_event) 
                    {
                        _event = false;
                    }
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
                    _paddle1.Throttle = -1*_invert;
                    break;
                case Key.S:
                    _paddle1.Throttle = 1*_invert;
                    break;
                case Key.O:
                    _paddle2.Throttle = -1*_invert;
                    break;
                case Key.L:
                    _paddle2.Throttle = 1*_invert;
                    break;

                default:
                    break;
            }
        }


        public ObservableCollection<CreditItem> CreditItems { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}