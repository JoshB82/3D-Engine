﻿using System;
using System.Collections.Generic;

namespace _3D_Engine
{
    public sealed partial class Scene
    {
        // other clipping?
        public void Generate_Shadow_Map(Light light)
        {
            foreach (Mesh mesh in Meshes)
            {
                if (mesh.Visible && mesh.Draw_Faces)
                {
                    foreach (Face face in mesh.Faces)
                    {
                        if (face.Visible)
                        {
                            Calculate_Depth(face, mesh.Dimension, light, mesh.Model_to_World);
                        }
                    }
                }
            }
        }
            
        private void Calculate_Depth(Face face, int dimension, Light light,
            in Matrix4x4 model_to_world)
        {
            // Reset the vertices to model space values
            face.Reset_Vertices();

            // Move face from model space to world space
            face.Apply_Matrix(model_to_world);

            // Discard the face if it is not visible from the light's point of view
            if (dimension == 3)
            {
                Vector3D light_to_face = new Vector3D(face.P1) - light.World_Origin;
                Vector3D normal = Vector3D.Normal_From_Plane(new Vector3D(face.P1), new Vector3D(face.P2), new Vector3D(face.P3));

                if (light_to_face * normal >= 0) return;
            }

            // Move the face from world space to light-view space
            face.Apply_Matrix(light.World_to_Light_View);

            // Clip the face in light-view space
            Queue<Face> face_clip = new Queue<Face>();
            face_clip.Enqueue(face);

            if (!Clip_Faces_In_Queue(face_clip, light.Light_View_Clipping_Planes)) return;

            // Move the new triangles from light-view space to screen space, including a correction for perspective
            foreach (Face clipped_face in face_clip)
            {
                clipped_face.Apply_Matrix(light.Light_View_to_Light_Screen);

                if (light is Spotlight)
                {
                    clipped_face.P1 /= clipped_face.P1.w;
                    clipped_face.P2 /= clipped_face.P2.w;
                    clipped_face.P3 /= clipped_face.P3.w;
                }
            }

            // Clip the face in screen space
            if (Settings.Screen_Space_Clip && !Clip_Faces_In_Queue(face_clip, Camera.Camera_Screen_Clipping_Planes)) return;

            foreach (Face clipped_face in face_clip)
            {
                // Don't draw anything if the face is flat
                if ((clipped_face.P1.x == clipped_face.P2.x && clipped_face.P2.x == clipped_face.P3.x) ||
                    (clipped_face.P1.y == clipped_face.P2.y && clipped_face.P2.y == clipped_face.P3.y))
                {
                    continue;
                }

                // Move the new triangles from light-screen space to window space
                clipped_face.Apply_Matrix(screen_to_window);
                
                // Round the vertices
                int x1 = clipped_face.P1.x.Round_to_Int();
                int y1 = clipped_face.P1.y.Round_to_Int();
                float z1 = clipped_face.P1.z;
                int x2 = clipped_face.P2.x.Round_to_Int();
                int y2 = clipped_face.P2.y.Round_to_Int();
                float z2 = clipped_face.P2.z;
                int x3 = clipped_face.P3.x.Round_to_Int();
                int y3 = clipped_face.P3.y.Round_to_Int();
                float z3 = clipped_face.P3.z;

                // Sort the vertices by their y-co-ordinate
                Sort_By_Y(
                    ref x1, ref y1, ref z1,
                    ref x2, ref y2, ref z2,
                    ref x3, ref y3, ref z3);

                // Interpolate each point in the triangle
                Interpolate_Triangle(light, Mesh_Depth_From_Light,
                    x1, y1, z1,
                    x2, y2, z2,
                    x3, y3, z3);
            }
        }

        private static void Mesh_Depth_From_Light(object @object, int x, int y, float z)
        {
            Light light = @object as Light;

            try
            {
                if (z < light.Shadow_Map[x][y])
                {
                    light.Shadow_Map[x][y] = z;
                }
            }
            catch (IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException($"Attempted to check points outside the shadow map at ({x}, {y}, {z})", e);
            }
        }
    }
}