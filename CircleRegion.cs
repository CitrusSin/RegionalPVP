using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RegionalPVP
{
    public class CircleRegion
    {
        public Vector3 Center;
        public float Radius;

        public CircleRegion(Vector3 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public CircleRegion()
        {
            Center = Vector3.zero;
            Radius = 0;
        }

        public bool IsInRegion(Vector3 position)
        {
            Vector3 diffVec = position - Center;
            return diffVec.sqrMagnitude < Radius * Radius;
        }
    }
}
