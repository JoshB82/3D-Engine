﻿using _3D_Engine.Maths.Vectors;

using static _3D_Engine.Properties.Settings;

namespace _3D_Engine.SceneObjects.RenderingObjects.Cameras
{
    /// <summary>
    /// Encapsulates creation of a <see cref="StereoCamera"/>.
    /// </summary>
    public sealed class StereoCamera : Camera
    {
        #region Fields and Properties

        private float width = Default.CameraWidth;
        private float height = Default.CameraHeight;
        private float z_near = Default.CameraZNear;
        private float z_far = Default.CameraZFar;

        public override float ViewWidth { get; set; }
        public override float ViewHeight { get; set; }
        public override float ZNear { get; set; }
        public override float ZFar { get; set; }

        #endregion

        #region Constructors

        public StereoCamera(Vector3D origin, Vector3D directionForward, Vector3D directionUp, float viewWidth, float viewHeight, float zNear, float zFar) : base(origin, directionForward, directionUp, viewWidth, viewHeight, zNear, zFar) { }

        #endregion

        #region Methods

        internal override void ProcessLighting()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
