using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace _2DCollisionDetection
{
    class MyPolygon
    {
        public Polygon Polygon { get; set; }
        public Line[] lines { get; set; }
        public TranslateTransform TranslateTransform { get; set; }
        public BoundingVolumeHierarchy BVH { get; set; }
        private Canvas myCanvas;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="verticies"></param>
        /// <param name="minx"></param>
        /// <param name="maxx"></param>
        /// <param name="miny"></param>
        /// <param name="maxy"></param>
        public MyPolygon(Canvas canvas, int verticies, int minx, int maxx, int miny, int maxy)
        {
            myCanvas = canvas;
            Polygon = new Polygon();
            BVH = new BoundingVolumeHierarchy();
            TranslateTransform = new TranslateTransform(0, 0);
            Polygon.RenderTransform = TranslateTransform;
            BuildPolygon(verticies, minx, maxx, miny, maxy);
            BuildBVH();            
            DrawBVH();
        }

        public void Translate(int directionX, int directionY)
        {
            TranslateTransform.X += directionX;
            TranslateTransform.Y += directionY;

            AABB[] tree = BVH.GetTree();
            foreach (AABB node in tree)
            {
                node.MinX += directionX;
                node.MinY += directionY;
                node.Location = new Point(node.Location.X + directionX, node.Location.Y + directionY);
            }

            foreach (Line line in lines)
            {
                line.X1 += directionX;
                line.Y1 += directionY;
                line.X2 += directionX;
                line.Y2 += directionY;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="verticies"></param>
        /// <param name="minx"></param>
        /// <param name="maxx"></param>
        /// <param name="miny"></param>
        /// <param name="maxy"></param>
        private void BuildPolygon(int verticies, int minx, int maxx, int miny, int maxy)
        {
            PointCollection pointCollection = new PointCollection();
            Random rand = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
            Line line = null;
            Point[] points = new Point[verticies];
            lines = new Line[verticies];

            // generate random points and add to point collection and points array
            for (int i = 0; i < verticies; i++)
            {
                Point point = new Point(rand.Next(minx, maxx), rand.Next(miny, maxy));
                pointCollection.Add(point);
                points[i] = point;
            }

            Polygon.Points = pointCollection;

            // go through all verticies and find all the lines, add to line array
            // line array will be used later for collision detection
            for (int i = 0, j = 1; j <= points.Length; i++, j++)
            {
                if (j < points.Length)
                {
                    line = new Line();
                    line.X1 = points[i].X;
                    line.Y1 = points[i].Y;
                    line.X2 = points[j].X;
                    line.Y2 = points[j].Y;
                }
                else
                {
                    line = new Line();
                    line.X1 = points[i].X;
                    line.Y1 = points[i].Y;
                    line.X2 = points[0].X;
                    line.Y2 = points[0].Y;
                }
                lines[i] = line;
            }

            myCanvas.Children.Add(Polygon);
        }

        /// <summary>
        /// 
        /// </summary>
        private void BuildBVH()
        {
            AABB root = new AABB();
            PointCollection pc = Polygon.Points;

            float minY = float.MaxValue;
            float minX = float.MaxValue;
            float maxY = 0;
            float maxX = 0;
            foreach (Point p in pc)
            {
                if (p.Y < minY)
                {
                    minY = (float)p.Y;
                }
                if (p.X < minX)
                {
                    minX = (float)p.X;
                }
                if (p.Y > maxY)
                {
                    maxY = (float)p.Y;
                }
                if (p.X > maxX)
                {
                    maxX = (float)p.X;
                }
            }

            root.Location = new Point(minX, minY);
            root.Width = maxX - minX;
            root.Height = maxY - minY;
            root.MinX = minX;
            root.MinY = minY;
            BVH.Add(root);

            GenerateChildren(root, pc);
        }

        private void GenerateChildren(AABB node, PointCollection pc)
        {

            if (node.Width < 20 || node.Height < 20)
                return;

            float avgX = 0;
            float avgY = 0;
            foreach (Point p in pc)
            {
                avgX += (float)p.X;
                avgY += (float)p.Y;
            }
            avgX /= pc.Count;
            avgY /= pc.Count;

            Line line = new Line();
            line.Stroke = Brushes.Black;
            line.StrokeThickness = 1;

            if (node.Height >= node.Width)
            {
                line.X1 = node.MinX - 20;
                line.Y1 = avgY;
                line.X2 = node.MinX + 20 + node.Width;
                line.Y2 = avgY;
            }
            else
            {
                line.X1 = avgX;
                line.Y1 = node.MinY - 20;
                line.X2 = avgX;
                line.Y2 = node.MinY + 20 + node.Width;
            }

            PointCollection newPoints = new PointCollection();
            foreach (Line l in lines)
            {
                if (CollisionDetection.DoLinesIntersect(l, line))
                {
                    //Line1
                    float A1 = (float)(l.Y2 - l.Y1);
                    float B1 = (float)(l.X1 - l.X2);
                    float C1 = (float)(A1 * l.X1 + B1 * l.Y1);

                    //Line2
                    float A2 = (float)(line.Y2 - line.Y1);
                    float B2 = (float)(line.X1 - line.X2);
                    float C2 = (float)(A2 * line.X1 + B2 * line.Y1);

                    float det = A1 * B2 - A2 * B1;

                    float x = (B2 * C1 - B1 * C2) / det;
                    float y = (A1 * C2 - A2 * C1) / det;

                    if ((x <= (node.MinX + node.Width) && x >= node.MinX) && (y < (node.MinY + node.Height) && y > node.MinY))
                    {
                        newPoints.Add(new Point(x, y));
                    }
                }
            }

            PointCollection topPoints = new PointCollection();
            PointCollection bottomPoints = new PointCollection();

            foreach (Point p in newPoints)
            {
                topPoints.Add(p);
                bottomPoints.Add(p);
            }

            foreach (Point p in pc)
            {
                if (p.Y < avgY)
                {
                    topPoints.Add(p);
                }
                else
                {
                    bottomPoints.Add(p);
                }
            }

            float minY = float.MaxValue;
            float minX = float.MaxValue;
            float maxY = 0;
            float maxX = 0;
            foreach (Point p in topPoints)
            {
                if (p.Y < minY)
                {
                    minY = (float)p.Y;
                }
                if (p.X < minX)
                {
                    minX = (float)p.X;
                }
                if (p.Y > maxY)
                {
                    maxY = (float)p.Y;
                }
                if (p.X > maxX)
                {
                    maxX = (float)p.X;
                }
            }

            AABB leftChild = new AABB();
            leftChild.Location = new Point(minX, minY);
            leftChild.Width = maxX - minX;
            leftChild.Height = maxY - minY;
            leftChild.MinX = minX;
            leftChild.MinY = minY;
            if (PointsInsideAABB(leftChild, newPoints))
            {
                BVH.Add(leftChild);
                GenerateChildren(leftChild, topPoints);
            }
            else
            {
                return;
            }

            minY = float.MaxValue;
            minX = float.MaxValue;
            maxY = 0;
            maxX = 0;
            foreach (Point p in bottomPoints)
            {
                if (p.Y < minY)
                {
                    minY = (float)p.Y;
                }
                if (p.X < minX)
                {
                    minX = (float)p.X;
                }
                if (p.Y > maxY)
                {
                    maxY = (float)p.Y;
                }
                if (p.X > maxX)
                {
                    maxX = (float)p.X;
                }
            }

            AABB rightChild = new AABB();
            rightChild.Location = new Point(minX, minY);
            rightChild.Width = maxX - minX;
            rightChild.Height = maxY - minY;
            rightChild.MinX = minX;
            rightChild.MinY = minY;
            if (PointsInsideAABB(rightChild, newPoints))
            {
                BVH.Add(rightChild);
                GenerateChildren(rightChild, bottomPoints);
            }
            else
            {
                return;
            }
        }

        private bool PointsInsideAABB(AABB node, PointCollection points)
        {
            float minX = node.MinX;
            float maxX = node.MinX + node.Width;
            float minY = node.MinY;
            float maxY = node.MinY + node.Height;
            bool pointIsInside = false;

            foreach (Point p in points)
            {
                if (p.X >= minX && p.X <= maxX && p.Y >= minY && p.Y <= maxY)
                {
                    pointIsInside = true;
                }
            }
            return pointIsInside;
        }

        private void DrawBVH()
        {
            AABB[] tree = BVH.GetTree();
            Random rand = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
            SolidColorBrush[] colors =
            {
                Brushes.Black,
                Brushes.Aqua,
                Brushes.Blue,
                Brushes.Coral,
                Brushes.Cyan,
                Brushes.DarkOrange,
                Brushes.Yellow,
                Brushes.Violet,
                Brushes.Purple
            };

            int count = 0;
            foreach (AABB node in tree)
            {
                if (count == 9)
                {
                    count = 0;
                }
                Rectangle rect = new Rectangle
                {
                    Stroke = colors[count++],
                    StrokeThickness = 2,
                    Width = node.Width,
                    Height = node.Height,
                    RenderTransform = TranslateTransform
                };

                Canvas.SetTop(rect, node.MinY);
                Canvas.SetLeft(rect, node.MinX);
                myCanvas.Children.Add(rect);
            }
        }
    }
}
