using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _2DCollisionDetection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {        
        MyPolygon polygon1;
        MyPolygon polygon2;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Create polygon button event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int sides1 = Int32.Parse(Shape1.Text);
            int sides2 = Int32.Parse(Shape2.Text);

            // Make polygon 1
            polygon1 = new MyPolygon(myCanvas, sides1, 20, 300, 100, 450);
            polygon1.Polygon.Stroke = Brushes.Black;
            polygon1.Polygon.Fill = Brushes.LightSeaGreen;
            polygon1.Polygon.StrokeThickness = 1;
            polygon1.Polygon.HorizontalAlignment = HorizontalAlignment.Left;
            polygon1.Polygon.VerticalAlignment = VerticalAlignment.Center;

            // Make polygon 2
            polygon2 = new MyPolygon(myCanvas, sides2, 400, 700, 200, 450);
            polygon2.Polygon.Stroke = Brushes.Black;
            polygon2.Polygon.Fill = Brushes.Aqua;
            polygon2.Polygon.StrokeThickness = 1;
            polygon2.Polygon.HorizontalAlignment = HorizontalAlignment.Left;
            polygon2.Polygon.VerticalAlignment = VerticalAlignment.Center;
        }
        
        /// <summary>
        /// Key Press listener event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DockPanel_KeyDown(object sender, KeyEventArgs e)
        {
            bool colliding = false;
            Stopwatch stopwatch;

            switch (e.Key)
            {
                case Key.J:

                    polygon1.Translate(-5, 0);
                    colliding = CollisionDetection.DetectCollisions(polygon1, polygon2, BroadPhaseTimer, NarrowPhaseTimer);
                    AddCollisionRect(colliding);
                    break;

                case Key.L:
                    polygon1.Translate(5, 0);
                    colliding = CollisionDetection.DetectCollisions(polygon1, polygon2, BroadPhaseTimer, NarrowPhaseTimer);
                    AddCollisionRect(colliding);
                    break;

                case Key.I:

                    polygon1.Translate(0, -5);
                    colliding = CollisionDetection.DetectCollisions(polygon1, polygon2, BroadPhaseTimer, NarrowPhaseTimer);
                    AddCollisionRect(colliding);
                    break;

                case Key.K:

                    polygon1.Translate(0, 5);
                    colliding = CollisionDetection.DetectCollisions(polygon1, polygon2, BroadPhaseTimer, NarrowPhaseTimer);
                    AddCollisionRect(colliding);
                    break;
            }
        }
        
        private void AddCollisionRect(bool colliding)
        {            
            if (colliding)
            {
                collisionRect.Fill = Brushes.Red;
            }
            else
            {
                collisionRect.Fill = Brushes.Green;
            }
        }   
    }
}
