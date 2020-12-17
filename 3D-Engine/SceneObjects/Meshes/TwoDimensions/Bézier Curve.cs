﻿using _3D_Engine.Maths.Vectors;

namespace _3D_Engine.SceneObjects.Meshes.TwoDimensions
{
    public sealed class Bézier_Curve : Mesh
    {
        #region Fields and Properties

        #endregion

        #region Constructors
        
        public Bézier_Curve(Vector3D origin, Vector3D direction_forward, Vector3D direction_up) : base(origin, direction_forward, direction_up)
        {

        }

        #endregion
    }
}