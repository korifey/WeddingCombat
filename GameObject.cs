using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WeddingCombat
{
    public abstract class GameObject
    {
        public static int UpperLineFromTop = 5;
        public static int LowerLineFromBottom = 50;
        protected readonly Stopwatch Watch = new Stopwatch();

        public abstract void Paint(Canvas canvas);

        private static double GameboardHeight => SystemParameters.WorkArea.Height - UpperLineFromTop - LowerLineFromBottom;

        public static int TranslateWidth(double width) => (int) (SystemParameters.WorkArea.Width * width / XYRatio);
        public static int TranslateHeight(double height) => (int) (GameboardHeight * height);

        public static int TranslateX(double x) => (int) (SystemParameters.WorkArea.Width * x / XYRatio);

        // ReSharper disable once InconsistentNaming
        public static readonly double XYRatio = SystemParameters.WorkArea.Width / GameboardHeight;


        public int TranslateY(double y)
        {
            return UpperLineFromTop + (int) (GameboardHeight * y);
        }

        public const int Fps = 100;
        
        protected abstract CollisionKind? UpdateNextFrame(long deltaTime, IEnumerable<GameObject> objects);

        public CollisionKind? Update(IEnumerable<GameObject> objects)
        {
            if (!Watch.IsRunning)
            {
                Watch.Start();
            }

            var deltaTime = Watch.ElapsedMilliseconds;
            Watch.Restart();
            
            return UpdateNextFrame(deltaTime, objects.Where(o => o != this));
        }

        public static bool Eq(double expected, double actual)
        {
            return Math.Abs(actual - expected) < 1E-8;
        }
    }
}