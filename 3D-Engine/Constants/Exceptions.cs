﻿using System;

namespace _3D_Engine.Constants
{
    internal static class Exceptions
    {
        // Format:
        // <struct/class name that uses the exception><brief description of message content>

        #region Exceptions

        internal static readonly ArgumentException Angle = new("Cannot calculate angle with one or more zero vectors.");
        internal static readonly ArgumentException Normalise = new("Cannot normalise a zero vector.");
        internal static readonly ArgumentException QuaternionNormalise = new("Cannot normalise a zero quaternion.");
        internal static readonly NotSupportedException RenderingObjectTypeNotSupported = new("This type is not supported.");
        internal static readonly InvalidOperationException Matrix4x4NoInverse = new("Matrix4x4 does not have an inverse.");

        #endregion

        #region Messages

        internal const string Vector2DParameterLength = "Parameter \"elements\" must at least be of length 2.";
        internal const string Vector3DParameterLength = "Parameter \"elements\" must at least be of length 3.";
        internal const string FourParameterLength = "Parameter \"elements\" must at least be of length 4.";
        internal const string Matrix4x4ParameterSize = "Parameter \"elements\" must at least be of size 4x4.";
        
        #endregion
    }
}
