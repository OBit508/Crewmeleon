using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Crewmeleon.Components
{
    public struct PaintStroke
    {
        public Color32 Color;
        public byte Radius;
        public PaintStroke(Color32 color, byte radius)
        {
            Color = color;
            Radius = radius;
        }
    }
}
