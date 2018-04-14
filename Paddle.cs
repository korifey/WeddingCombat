using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WeddingCombat
{
    public class Paddle : GameObject
    {

        //position
        public double CenterX { get; }
        public double Width { get; } = 0.01 * XYRatio;
        public double Height { get; } = 0.18;

        public double CenterY { get; private set; } = 0.5;


        public double Top
        {
            get => CenterY - Height / 2;
            set => CenterY = value + Height / 2;
        }

        public double Bottom
        {
            get => CenterY + Height / 2;
            set => CenterY = value - Height / 2;
        }

        public double Left => CenterX - Width / 2;
        public double Right => CenterX + Width / 2;


        public double _velocity;

        private Brush _stroke;

        public Paddle(double centerX, Brush stroke)
        {
            CenterX = centerX;
            this._stroke = stroke;
        }


        private double Mass { get; } = 1;

        // [-1, +1]
        public double Throttle { get; set; } = 0;

        public bool ThrottleIs(double value) => Eq(value, Throttle);

        private const double EngineForceMax = 0.4 * 10E-6;
        private const double FrictionCoefficient = 0.5 / Fps; //friction force = 


        public double EngineForce => Throttle * EngineForceMax;
        public double FrictionForce => -FrictionCoefficient * _velocity /*8 Math.Abs(_velocity)*/;

        public double NetForce => EngineForce + FrictionForce;
        public double AccelerationMs2 => NetForce / Mass;

        protected override CollisionKind? UpdateNextFrame(long deltaTime, IEnumerable<GameObject> objects)
        {
            _velocity += AccelerationMs2 * deltaTime;

            CenterY += _velocity * deltaTime;


            if (Top < 0)
            {
                Top = -Top;
                _velocity = -_velocity;
            }

            if (Bottom > 1.0)
            {
                Bottom = 2.0 - Bottom;
                _velocity = -_velocity;
            }

            return null;
        }


        public override void Paint(Canvas canvas)
        {
            var rect = new Rectangle
            {
                Width = TranslateWidth(Width),
                Height = TranslateHeight(Height),
                Fill = Brushes.White,
                Stroke = _stroke,
                StrokeThickness = 2
            };
            Canvas.SetTop(rect, TranslateY(Top));
            Canvas.SetLeft(rect, TranslateX(Left));
            canvas.Children.Add(rect);
        }
    }
}