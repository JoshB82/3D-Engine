﻿using System.Collections.Generic;

namespace _3D_Engine
{
    public sealed partial class Scene
    {
        private void Draw_Face(Face face, string mesh_type,
            Matrix4x4 model_to_world,
            Matrix4x4 world_to_view,
            Matrix4x4 view_to_screen)
        {
            // Move the face from model space to world space
            face.World_P1 = new Vector3D(model_to_world * face.P1.Point);
            face.World_P2 = new Vector3D(model_to_world * face.P2.Point);
            face.World_P3 = new Vector3D(model_to_world * face.P3.Point);
            face.P1.Point = model_to_world * face.P1.Point;
            face.P2.Point = model_to_world * face.P2.Point;
            face.P3.Point = model_to_world * face.P3.Point;

            // Discard the face if it is not visible
            Vector3D camera_to_face = new Vector3D(face.P1.Point - new Vector4D(Render_Camera.World_Origin));
            Vector3D normal = Vector3D.Normal_From_Plane(face.World_P1, face.World_P2, face.World_P3);
            
            if (camera_to_face * normal >= 0
                && mesh_type != "Circle"
                && mesh_type != "Plane"
                && mesh_type != "Ring"
                && mesh_type != "Square"
                && mesh_type != "Text2D")
            return;
            
            // Draw outline if needed
            if (face.Draw_Outline)//
            {
                Draw_Edge(new Edge(face.P1, face.P2), model_to_world, world_to_view, view_to_screen);
                Draw_Edge(new Edge(face.P1, face.P3), model_to_world, world_to_view, view_to_screen);
                Draw_Edge(new Edge(face.P2, face.P3), model_to_world, world_to_view, view_to_screen);
            }

            // Move the face from world space to view space
            face.P1.Point = world_to_view * face.P1.Point;
            face.P2.Point = world_to_view * face.P2.Point;
            face.P3.Point = world_to_view * face.P3.Point;

            // Clip the face in view space
            Face[] new_view_triangles;
            
            if (Settings.View_Space_Clip)
            {
                Queue<Face> view_face_clip = new Queue<Face>();
                view_face_clip.Enqueue(face);
                if (Queue_Clip_Face(view_face_clip, Render_Camera.View_Clipping_Planes, out new_view_triangles) == 0) return;
            }
            else
            {
                new_view_triangles = new Face[1] { face };
            }

            // Move the new triangles from view space to screen space, including a correction for perspective
            for (int i = 0; i < new_view_triangles.Length; i++)
            {
                new_view_triangles[i].P1.Point = view_to_screen * new_view_triangles[i].P1.Point;
                new_view_triangles[i].P2.Point = view_to_screen * new_view_triangles[i].P2.Point;
                new_view_triangles[i].P3.Point = view_to_screen * new_view_triangles[i].P3.Point;

                if (render_camera_type == "Perspective_Camera")
                {
                    new_view_triangles[i].P1.Point /= new_view_triangles[i].P1.Point.W;
                    new_view_triangles[i].P2.Point /= new_view_triangles[i].P2.Point.W;
                    new_view_triangles[i].P3.Point /= new_view_triangles[i].P3.Point.W;
                    
                    if (face.Texture_Object != null)
                    {
                        new_view_triangles[i].T1 /= new_view_triangles[i].P1.Point.W;
                        new_view_triangles[i].T2 /= new_view_triangles[i].P2.Point.W;
                        new_view_triangles[i].T3 /= new_view_triangles[i].P3.Point.W;
                    }
                }
            }

            // Clip the face in screen space
            Queue<Face> screen_face_clip = new Queue<Face>(new_view_triangles);
            if (Queue_Clip_Face(screen_face_clip, screen_clipping_planes, out Face[] new_screen_triangles) == 0) return; // anything outside cube?

            for (int i = 0; i < new_screen_triangles.Length; i++)
            {
                // Mode the new triangles from screen space to window space
                new_screen_triangles[i].P1.Point = screen_to_window * new_screen_triangles[i].P1.Point;
                new_screen_triangles[i].P2.Point = screen_to_window * new_screen_triangles[i].P2.Point;
                new_screen_triangles[i].P3.Point = screen_to_window * new_screen_triangles[i].P3.Point;

                // Round the vertices
                int result_point_1_x = Round_To_Int(new_screen_triangles[i].P1.Point.X);
                int result_point_1_y = Round_To_Int(new_screen_triangles[i].P1.Point.Y);
                double result_point_1_z = new_screen_triangles[i].P1.Point.Z;
                int result_point_2_x = Round_To_Int(new_screen_triangles[i].P2.Point.X);
                int result_point_2_y = Round_To_Int(new_screen_triangles[i].P2.Point.Y);
                double result_point_2_z = new_screen_triangles[i].P2.Point.Z;
                int result_point_3_x = Round_To_Int(new_screen_triangles[i].P3.Point.X);
                int result_point_3_y = Round_To_Int(new_screen_triangles[i].P3.Point.Y);
                double result_point_3_z = new_screen_triangles[i].P3.Point.Z;
                
                // Finally draw the triangle
                if (face.Texture_Object == null)
                {
                    Solid_Triangle(Render_Camera, new_screen_triangles[i],
                        result_point_1_x, result_point_1_y, result_point_1_z,
                        result_point_2_x, result_point_2_y, result_point_2_z,
                        result_point_3_x, result_point_3_y, result_point_3_z);
                }
                else
                {
                    // Scale the texture co-ordinates
                    int width = face.Texture_Object.File.Width - 1;
                    int height = face.Texture_Object.File.Height - 1;

                    // AFTERWARDS?
                    int result_texture_point_1_x = Round_To_Int(face.T1.X * width);
                    int result_texture_point_1_y = Round_To_Int(face.T1.Y * height);
                    int result_texture_point_2_x = Round_To_Int(face.T2.X * width);
                    int result_texture_point_2_y = Round_To_Int(face.T2.Y * height);
                    int result_texture_point_3_x = Round_To_Int(face.T3.X * width);
                    int result_texture_point_3_y = Round_To_Int(face.T3.Y * height);

                    Textured_Triangle(face.Texture_Object.File,
                        result_point_1_x, result_point_1_y, result_point_1_z, result_texture_point_1_x, result_texture_point_1_y,
                        result_point_2_x, result_point_2_y, result_point_2_z, result_texture_point_2_x, result_texture_point_2_y,
                        result_point_3_x, result_point_3_y, result_point_3_z, result_texture_point_3_x, result_texture_point_3_y);
                }
            }
            // RANGE TO DRAW X: [0,WIDTH-1] Y: [0,HEIGHT-1]
        }

        private int Queue_Clip_Face(Queue<Face> face_clip, Clipping_Plane[] clipping_planes, out Face[] new_triangles)
        {
            // TODO Settings.View_Space_Clip
            int no_triangles = 1;

            foreach (Clipping_Plane clipping_plane in clipping_planes)
            {
                while (no_triangles > 0)
                {
                    Face triangle = face_clip.Dequeue();
                    Face[] triangles = new Face[2];
                    int num_intersection_points = Clip_Face(clipping_plane.Point, clipping_plane.Normal, triangle, out triangles[0], out triangles[1]); //OUT?
                    for (int i = 0; i < num_intersection_points; i++) face_clip.Enqueue(triangles[i]);
                    no_triangles--;
                }
                no_triangles = face_clip.Count;
            }

            new_triangles = face_clip.ToArray();
            return no_triangles;
        }
    }
}

/*
    
    Color face_colour = face.Colour;
    if (Light_List.Count > 0)
    {
        double max_intensity = 0, true_intensity = 0;
        foreach (Light light in Light_List) max_intensity += light.Intensity;
        foreach (Light light in Light_List)
        {
            switch (light.GetType().Name)
            {
                case "Distant_Light":
                    true_intensity = Math.Max(0, -light.World_Direction * normal) * light.Intensity;
                    break;
                case "Point_Light":
                    true_intensity = Math.Max(0, -new Vector3D(point_1 - light.World_Origin).Normalise() * normal) * light.Intensity;
                    break;
                case "Spot_Light":
                    Vector3D light_to_mesh = new Vector3D(point_1 - light.World_Origin);
                    if (light_to_mesh.Angle(light.World_Direction) > ((Spotlight)light).Angle || light_to_mesh * light.World_Direction > ((Spotlight)light).Distance) continue;
                    true_intensity = Math.Max(0, -light.World_Direction * normal) * light.Intensity;
                    break;
                case "Ambient_Light":
                    break;
            }
            double scaled_intensity = true_intensity / max_intensity;

            byte new_red = (byte)Round_To_Int((face.Colour.R + light.Colour.R) * 255 / 510 * scaled_intensity);
            byte new_green = (byte)Round_To_Int((face.Colour.G + light.Colour.G) * 255 / 510 * scaled_intensity);
            byte new_blue = (byte)Round_To_Int((face.Colour.B + light.Colour.B) * 255 / 510 * scaled_intensity);

            face_colour = Color.FromArgb(face.Colour.A, new_red, new_green, new_blue);
        }
    }
*/