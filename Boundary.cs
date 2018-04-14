using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WeddingCombat
{
    public class Boundary : GameObject
    {
        private readonly double _topY;

        public Boundary(double topY)
        {
            _topY = topY;
        }


        public const double Height = 0.005;
        
        public override void Paint(Canvas canvas)
        {
            var rect = new Rectangle
            {
                Fill = Brushes.White,
                Width = TranslateWidth(XYRatio),
                Height = TranslateHeight(Height)
            };
            Canvas.SetTop(rect, TranslateY(_topY));
            canvas.Children.Add(rect);
        }

        protected override CollisionKind? UpdateNextFrame(long deltaTime, IEnumerable<GameObject> objects)
        {
            //nothing to do
            return null;
        }
    }
}