using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WeddingCombat
{
    public class Ball : GameObject
    {
        private double _radius = 0.01;
        private double _centerX = XYRatio/2;
        private double _centerY = 0.5;

        private double _velocity = 1.0 / (30 * Fps);
        private double _angle = new Random().Next(-1, 3) * Math.PI / 2 - Math.PI / 4 ;

        
        
        public Ball()
        {
        }

        public double Top
        {
            get => _centerY - _radius;
            set => _centerY = value + _radius;
        }

        public double Bottom
        {
            get => _centerY + _radius;
            set => _centerY = value - _radius;
        }

        public double Left => _centerX - _radius;

        public override void Paint(Canvas canvas)
        {
            var rect = new Ellipse
            {
                Width = TranslateWidth(_radius * 2 ),
                Height = TranslateHeight(_radius * 2),
                Fill = Brushes.White,
            };
            Canvas.SetTop(rect, TranslateY(Top));
            Canvas.SetLeft(rect, TranslateX(Left));
            canvas.Children.Add(rect);
        }

        protected override CollisionKind? UpdateNextFrame(long deltaTime, IEnumerable<GameObject> objects)
        {
            _centerX += deltaTime * _velocity * Math.Cos(_angle);
            _centerY += deltaTime * _velocity * Math.Sin(_angle);

            CollisionKind? res = null;
            
            if (Top < 0)
            {
                Top = -Top;
                _angle = -_angle;
                res = CollisionKind.Boundary;
            }

            if (Bottom > 1.0)
            {
                Bottom = 2.0 - Bottom;
                _angle = -_angle;
                res = CollisionKind.Boundary;
            }
            

            foreach (var p in objects.OfType<Paddle>())
            {
                if (
                    (
                        Math.Abs(_angle) > Math.PI / 2
                        && p.Right < 0.5 //left paddle
                        && p.Right > _centerX - _radius //intersects left paddle line
                        && p.CenterX < _centerX // but not lost yet
                        
                        ||
                        Math.Abs(_angle) < Math.PI / 2
                        && p.Left > 0.5 //right paddle 
                        && p.Left < _centerX + _radius //intersects left paddle line
                        && p.CenterX > _centerX //but not lost yet
                        
                    )
                    && p.Top < Bottom && p.Bottom > Top) //todo collide it correcly
                {
                    _angle = Math.Sign(_angle) * Math.PI -_angle;
                    _velocity *= 1.3;
                    res = CollisionKind.Paddle;
                }
            }

            return res;
        }


        public EndOfRound? IsEndOfRound()
        {
            if (0 > _centerX + _radius)
                return EndOfRound.LeftLost;

            if (XYRatio < _centerX - _radius)
                return EndOfRound.RightLost;

            return null;
        }
    }

    public enum CollisionKind
    {
        Boundary,
        Paddle
    }
    
    public enum EndOfRound
    {
        LeftLost,
        RightLost
    }
}