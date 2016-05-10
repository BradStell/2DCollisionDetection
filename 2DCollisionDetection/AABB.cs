using System.Windows;

namespace _2DCollisionDetection
{
    class AABB
    {
        public Point Location { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float MinX { get; set; }
        public float MinY { get; set; }

        public AABB()
        {
        }
    }
}
