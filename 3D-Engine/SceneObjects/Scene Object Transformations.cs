﻿using _3D_Engine.Maths.Vectors;
using _3D_Engine.Miscellaneous;
using _3D_Engine.SceneObjects.Meshes.ThreeDimensions;
using _3D_Engine.Transformations;
using System;

namespace _3D_Engine.SceneObjects
{
    public abstract partial class SceneObject
    {
        #region Rotations

        /// <summary>
        /// Sets the forward, up and right directions given the forward and up directions.
        /// </summary>
        /// <param name="newWorldDirectionForward">The forward direction.</param>
        /// <param name="newWorldDirectionUp">The up direction.</param>
        public virtual void SetDirection1(Vector3D newWorldDirectionForward, Vector3D newWorldDirectionUp)
        {
            VectorCheck(newWorldDirectionForward, newWorldDirectionUp);

            // if (new_world_direction_forward * new_world_direction_up != 0) throw new ArgumentException("Direction vectors are not orthogonal.");

            newWorldDirectionForward = newWorldDirectionForward.Normalise();
            newWorldDirectionUp = newWorldDirectionUp.Normalise();

            AdjustVectors(
                newWorldDirectionForward,
                newWorldDirectionUp,
                Transform.CalculateDirectionRight(newWorldDirectionForward, newWorldDirectionUp)
            );
        }
        /// <summary>
        /// Sets the forward, up and right directions given the up and right directions.
        /// </summary>
        /// <param name="newWorldDirectionUp">The up direction.</param>
        /// <param name="newWorldDirectionRight">The right direction.</param>
        public virtual void SetDirection2(Vector3D newWorldDirectionUp, Vector3D newWorldDirectionRight)
        {
            VectorCheck(newWorldDirectionUp, newWorldDirectionRight);

            // if (new_world_direction_up * new_world_direction_right != 0) throw new ArgumentException("Direction vectors are not orthogonal.");

            newWorldDirectionUp = newWorldDirectionUp.Normalise();
            newWorldDirectionRight = newWorldDirectionRight.Normalise();

            AdjustVectors(
                Transform.CalculateDirectionForward(newWorldDirectionUp, newWorldDirectionRight),
                newWorldDirectionUp,
                newWorldDirectionRight
            );
        }
        /// <summary>
        /// Sets the forward, up and right directions given the right and forward directions.
        /// </summary>
        /// <param name="newWorldDirectionRight">The right direction.</param>
        /// <param name="newWorldDirectionForward">The forward direction.</param>
        public virtual void SetDirection3(Vector3D newWorldDirectionRight, Vector3D newWorldDirectionForward)
        {
            VectorCheck(newWorldDirectionRight, newWorldDirectionForward);

            // if (new_world_direction_right * new_world_direction_forward != 0) throw new ArgumentException("Direction vectors are not orthogonal.");

            newWorldDirectionForward = newWorldDirectionForward.Normalise();
            newWorldDirectionRight = newWorldDirectionRight.Normalise();

            AdjustVectors(
                newWorldDirectionForward,
                Transform.CalculateDirectionUp(newWorldDirectionRight, newWorldDirectionForward),
                newWorldDirectionRight
            );
        }

        private void AdjustVectors(Vector3D directionForward, Vector3D directionUp, Vector3D directionRight)
        {
            WorldDirectionForward = directionForward;
            WorldDirectionUp = directionUp;
            WorldDirectionRight = directionRight;

            if (HasDirectionArrows)
            {
                ((Arrow)DirectionArrows.SceneObjects[0]).SetDirection1(directionForward, directionUp);
                ((Arrow)DirectionArrows.SceneObjects[1]).SetDirection1(directionForward, directionUp);
                ((Arrow)DirectionArrows.SceneObjects[2]).SetDirection1(directionForward, directionUp);
            }

            ConsoleOutput.DisplayOutputDirectionMessage(this, Properties.Settings.Default.Verbosity);

            if (RenderCamera is not null) RenderCamera.NewRenderNeeded = true;
        }

        private static void VectorCheck(Vector3D firstVector, Vector3D secondVector)
        {
            if (firstVector.ApproxEquals(Vector3D.Zero, 1E-6f) || secondVector.ApproxEquals(Vector3D.Zero, 1E-6f))
            {
                throw new ArgumentException("New direction vector(s) cannot be set to zero vector.");
            }
        }

        #endregion

        #region Translations

        /// <summary>
        /// Translates the <see cref="SceneObject"/> in the x-direction.
        /// </summary>
        /// <param name="distance">Amount to translate by.</param>
        public virtual void TranslateX(float distance) => WorldOrigin += new Vector3D(distance, 0, 0);
        /// <summary>
        /// Translates the <see cref="SceneObject"/> in the y-direction.
        /// </summary>
        /// <param name="distance">Amount to translate by.</param>
        public virtual void TranslateY(float distance) => WorldOrigin += new Vector3D(0, distance, 0);
        /// <summary>
        /// Translates the <see cref="SceneObject"/> in the z-direction.
        /// </summary>
        /// <param name="distance">Amount to translate by.</param>
        public virtual void TranslateZ(float distance) => WorldOrigin += new Vector3D(0, 0, distance);
        /// <summary>
        /// Translates the <see cref="SceneObject"/> by the given vector.
        /// </summary>
        /// <param name="displacement">Vector to translate by.</param>
        public virtual void Translate(Vector3D displacement) => WorldOrigin += displacement;

        #endregion
    }
}