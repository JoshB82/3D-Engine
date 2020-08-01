﻿using System.Diagnostics;
using System.Drawing;

namespace _3D_Engine
{
    public sealed class Ambient_Light : Light
    {
        #region Constructors

        public Ambient_Light(Vector3D origin, Vector3D direction_forward, Vector3D direction_up) : base(origin, direction_forward, direction_up)
        {

        }

        #endregion
    }
}