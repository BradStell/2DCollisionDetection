using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DCollisionDetection
{
    class BoundingVolumeHierarchy
    {
        private AABB[] BVH;
        private int size = 2;
        private int current = 0;
        public int Depth { get; set; }
        public int Count { get; set; }

        public BoundingVolumeHierarchy()
        {
            BVH = new AABB[size];
            Count = 0;
        }

        public void Add(AABB bv)
        {
            if (current == BVH.Length)
            {
                ExpandCapacity();
            }
                
            BVH[current++] = bv;
            Count++;
        }

        private void ExpandCapacity()
        {
            size *= 2;
            AABB[] temp = new AABB[size];
            for (int i = 0; i < BVH.Length; i++)
            {
                temp[i] = BVH[i];
            }
            BVH = temp;
        }

        public AABB[] GetTree()
        {
            AABB[] temp = new AABB[current];
            for (int i = 0; i < current; i++)
            {
                temp[i] = BVH[i];
            }
            return temp;
        }
    }
}
