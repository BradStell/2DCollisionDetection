using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace _2DCollisionDetection
{
    static class CollisionDetection
    {
        static AABB[] tree1;
        static AABB[] tree2;
        static Stopwatch stopwatch;

        /// <summary>   ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// One Method for determining if 2 lines intersect
        /// </summary>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <returns></returns>
        public static bool DoLinesIntersect(Line line1, Line line2)
        {
            return CrossProduct(new Point(line1.X1, line1.Y1), new Point(line1.X2, line1.Y2), new Point(line2.X1, line2.Y1)) !=
                   CrossProduct(new Point(line1.X1, line1.Y1), new Point(line1.X2, line1.Y2), new Point(line2.X2, line2.Y2)) ||
                   CrossProduct(new Point(line2.X1, line2.Y1), new Point(line2.X2, line2.Y2), new Point(line1.X1, line1.Y1)) !=
                   CrossProduct(new Point(line2.X1, line2.Y1), new Point(line2.X2, line2.Y2), new Point(line1.X2, line1.Y2));
        }

        public static double CrossProduct(Point p1, Point p2, Point p3)
        {
            return (p2.X - p1.X) * (p3.Y - p1.Y) - (p3.X - p1.X) * (p2.Y - p1.Y);
        }

        public static bool DetectCollisions(MyPolygon poly1, MyPolygon poly2, Label BroadPhase, Label NarrowPhase)
        {
            bool broadPhase = false;
            bool narrowPhase = false;

            stopwatch = Stopwatch.StartNew();
            broadPhase = BroadPhaseDetection(poly1, poly2);
            stopwatch.Stop();

            BroadPhase.Content = "millis = " + stopwatch.ElapsedMilliseconds + " ticks = " + stopwatch.ElapsedTicks;

            if (broadPhase)
            {
                stopwatch = Stopwatch.StartNew();
                narrowPhase = NarrowPhaseDetection(poly1, poly2);
                stopwatch.Stop();

                NarrowPhase.Content = "millis = " + stopwatch.ElapsedMilliseconds + " ticks = " + stopwatch.ElapsedTicks; ;
            }
            else
            {
                NarrowPhase.Content = "millis = 0 ticks = 0";
            }

            return narrowPhase;
        }

        private static bool BroadPhaseDetection(MyPolygon poly1, MyPolygon poly2)
        {
            tree1 = poly1.BVH.GetTree();
            tree2 = poly2.BVH.GetTree();

            return BroadPhaseRecursion(tree1[0], tree2[0], poly1.BVH.Count, poly2.BVH.Count);
        }

        private static bool BroadPhaseRecursion(AABB node1, AABB node2, int depth1, int depth2)
        {
            if (!Colliding(node1, node2))
            {
                return false;
            }
            else
            {
                bool colliding = false;
                for (int i = 1; i < depth1; i++)
                {
                    for (int j = 1; j < depth2; j++)
                    {
                        if (Colliding(tree1[i], tree2[j]))
                        {
                            colliding = true;
                        }
                    }
                }
                if (colliding)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        private static bool NarrowPhaseDetection(MyPolygon poly1, MyPolygon poly2)
        {
            bool intersect = false;
            foreach (Line line1 in poly1.lines)
            {
                foreach (Line line2 in poly2.lines)
                {
                    intersect = DoIntersect(new Point(line1.X1, line1.Y1), new Point(line1.X2, line1.Y2), new Point(line2.X1, line2.Y1), new Point(line2.X2, line2.Y2));

                    if (intersect)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// This code was found here: http://www.geeksforgeeks.org/check-if-two-given-line-segments-intersect/
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="q1"></param>
        /// <param name="p2"></param>
        /// <param name="q2"></param>
        /// <returns></returns>
        private static bool DoIntersect(Point p1, Point q1, Point p2, Point q2)
        {
            // Find the four orientations needed for general and
            // special cases
            int o1 = orientation(p1, q1, p2);
            int o2 = orientation(p1, q1, q2);
            int o3 = orientation(p2, q2, p1);
            int o4 = orientation(p2, q2, q1);

            // General case
            if (o1 != o2 && o3 != o4)
                return true;

            // Special Cases
            // p1, q1 and p2 are colinear and p2 lies on segment p1q1
            if (o1 == 0 && onSegment(p1, p2, q1)) return true;

            // p1, q1 and p2 are colinear and q2 lies on segment p1q1
            if (o2 == 0 && onSegment(p1, q2, q1)) return true;

            // p2, q2 and p1 are colinear and p1 lies on segment p2q2
            if (o3 == 0 && onSegment(p2, p1, q2)) return true;

            // p2, q2 and q1 are colinear and q1 lies on segment p2q2
            if (o4 == 0 && onSegment(p2, q1, q2)) return true;

            return false; // Doesn't fall in any of the above cases
        }

        /// <summary>
        /// This code was found here: http://www.geeksforgeeks.org/check-if-two-given-line-segments-intersect/
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        private static int orientation(Point p, Point q, Point r)
        {
            // See http://www.geeksforgeeks.org/orientation-3-ordered-points/
            // for details of below formula.
            double val = ((q.Y - p.Y) * (r.X - q.X) -
                      (q.X - p.X) * (r.Y - q.Y));

            if (val == 0.0) return 0;  // colinear

            return (val > 0) ? 1 : 2; // clock or counterclock wise
        }

        /// <summary>
        /// This code was found here: http://www.geeksforgeeks.org/check-if-two-given-line-segments-intersect/
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        private static bool onSegment(Point p, Point q, Point r)
        {
            if (q.X <= Math.Max(p.X, r.X) && q.X >= Math.Min(p.X, r.X) &&
                q.Y <= Math.Max(p.Y, r.Y) && q.Y >= Math.Min(p.Y, r.Y))
                return true;

            return false;
        }

        /// <summary>
        /// Determines if 2 bounding volues are in collision or not
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <returns></returns>
        private static bool Colliding(AABB node1, AABB node2)
        {
            bool collidingX = false;
            bool collidingY = false;

            float minX1 = node1.MinX;
            float maxX1 = node1.MinX + node1.Width;
            float minY1 = node1.MinY;
            float maxY1 = node1.MinY + node1.Height;

            float minX2 = node2.MinX;
            float maxX2 = node2.MinX + node2.Width;
            float minY2 = node2.MinY;
            float maxY2 = node2.MinY + node2.Height;

            if ( (maxX1 >= minX2) && (minX1 <= maxX2) )
            {
                collidingX = true;
            }
            if ((maxY1 >= minY2) && (minY1 <= maxY2))
            {
                collidingY = true;
            }

            return (collidingX == true && collidingY == true) ? true : false;
        }
    }
}
