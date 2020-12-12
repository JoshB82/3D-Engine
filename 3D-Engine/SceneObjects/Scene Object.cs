﻿/*
 *       -3D-Engine-
 *     (c) Josh Bryant
 * https://joshdbryant.com
 *
 * Full license is available in the GitHub repository:
 * https://github.com/JoshB82/3D-Engine/blob/master/LICENSE
 *
 * Code description for this file:
 * Encapsulates creation of a scene object.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace _3D_Engine
{
    /// <include file="Help_8.xml" path="doc/members/member[@name='T:_3D_Engine.SceneObject']/*"/>
    public abstract partial class SceneObject
    {
        #region Fields and Properties

        // ID
        /// <include file="Help_8.xml" path="doc/members/member[@name='P:_3D_Engine.Scene_Object.ID']/*"/>
        public int ID { get; private set; }
        private static int nextId = -1;

        // Matrices
        internal Matrix4x4 Model_to_World;

        internal virtual void Calculate_Matrices()
        {
            Matrix4x4 directionForwardRotation = Transform.Rotate_Between_Vectors(Model_Direction_Forward, World_Direction_Forward);
            Matrix4x4 directionUpRotation = Transform.Rotate_Between_Vectors((Vector3D)(directionForwardRotation * Model_Direction_Up), World_Direction_Up);
            Matrix4x4 translation = Transform.Translate(World_Origin);

            // String the transformations together in the following order:
            // 1) Rotation around direction forward vector
            // 2) Rotation around direction up vector
            // 3) Translation to final position in world space
            Model_to_World = translation * directionUpRotation * directionForwardRotation;
        }

        // Origins
        internal static readonly Vector4D Model_Origin = Vector4D.Unit_W;

        /// <include file="Help_8.xml" path="doc/members/member[@name='P:_3D_Engine.Scene_Object.World_Origin']/*"/>
        public virtual Vector3D World_Origin { get; set; }

        internal void Calculate_World_Origin() => World_Origin = (Vector3D)(Model_to_World * Model_Origin);

        // Directions
        internal static readonly Vector3D Model_Direction_Forward = Vector3D.Unit_Z;
        internal static readonly Vector3D Model_Direction_Up = Vector3D.Unit_Y;
        internal static readonly Vector3D Model_Direction_Right = Vector3D.Unit_X;

        /// <include file="Help_8.xml" path="doc/members/member[@name='P:_3D_Engine.Scene_Object.World_Direction_Forward']/*"/>
        public Vector3D World_Direction_Forward { get; private set; }
        /// <include file="Help_8.xml" path="doc/members/member[@name='P:_3D_Engine.Scene_Object.World_Direction_Up']/*"/>
        public Vector3D World_Direction_Up { get; private set; }
        /// <include file="Help_8.xml" path="doc/members/member[@name='P:_3D_Engine.Scene_Object.World_Direction_Right']/*"/>
        public Vector3D World_Direction_Right { get; private set; }

        // Appearance
        /// <include file="Help_8.xml" path="doc/members/member[@name='P:_3D_Engine.Scene_Object.Visible']/*"/>
        public bool Visible { get; set; } = true;

        // Direction Arrows
        internal Group DirectionArrows { get; }
        internal bool Has_Direction_Arrows;

        /// <include file="Help_8.xml" path="doc/members/member[@name='P:_3D_Engine.Scene_Object.Display_Direction_Arrows']/*"/>
        public bool Display_Direction_Arrows { get; set; } = false;

        #endregion

        #region Constructors

        internal SceneObject(Vector3D origin, Vector3D direction_forward, Vector3D direction_up, bool has_direction_arrows = true)
        {
            ID = ++nextId;

            World_Origin = origin;
            Set_Direction_1(direction_forward, direction_up);

            if (Has_Direction_Arrows = has_direction_arrows)
            {
                const int resolution = 30, body_radius = 10, tip_radius = 20, body_length = 10, tip_length = 5;

                List<SceneObject> direction_arrows = new List<SceneObject>
                {
                    new Arrow(origin, World_Direction_Forward, World_Direction_Up, body_length, body_radius, tip_length, tip_radius, resolution, false) { Face_Colour = Color.Blue }, // Z-axis
                    new Arrow(origin, World_Direction_Up, -World_Direction_Forward, body_length, body_radius, tip_length, tip_radius, resolution, false) { Face_Colour = Color.Green }, // Y-axis
                    new Arrow(origin, World_Direction_Right, -World_Direction_Up, body_length, body_radius, tip_length, tip_radius, resolution, false) { Face_Colour = Color.Red } // X-axis
                };
                DirectionArrows = new Group(origin, direction_forward, direction_up, direction_arrows, false);
            }

            Trace.WriteLine($"{GetType().Name} created at {origin}");
        }

        #endregion
    }

    /// <include file="Help_8.xml" path="doc/members/member[@name='T:_3D_Engine.Volume_Outline']/*"/>
    [Flags]
    public enum Volume_Outline : byte
    {
        /// <include file="Help_8.xml" path="doc/members/member[@name='F:_3D_Engine.Volume_Outline.None']/*"/>
        None = 0,
        /// <include file="Help_8.xml" path="doc/members/member[@name='F:_3D_Engine.Volume_Outline.Near']/*"/>
        Near = 1,
        /// <include file="Help_8.xml" path="doc/members/member[@name='F:_3D_Engine.Volume_Outline.Far']/*"/>
        Far = 2
    }
}