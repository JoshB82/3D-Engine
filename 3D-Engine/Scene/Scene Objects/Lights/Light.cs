﻿/*
 *       -3D-Engine-
 *     (c) Josh Bryant
 * https://joshdbryant.com
 *
 * Full license is available in the GitHub repository:
 * https://github.com/JoshB82/3D-Engine/blob/master/LICENSE
 *
 * Code description for this file:
 * Handles creation of a light.
 */

using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace _3D_Engine
{
    /// <summary>
    /// Handles creation of a <see cref="Light"/>.
    /// </summary>
    public abstract partial class Light : Scene_Object
    {
        #region Fields and Properties

        // Appearance
        public Color Colour { get; set; } = Color.White;
        public Mesh Icon { get; protected set; }
        public float Strength { get; set; }

        /// <summary>
        /// Determines if the <see cref="Light"/> is drawn in the <see cref="Scene"/>.
        /// </summary>
        public bool Draw_Icon { get; set; } = false;

        // View Volume
        private Volume_Outline volume_style = Volume_Outline.None;

        public Volume_Outline Volume_Style
        {
            get => volume_style;
            set
            {
                volume_style = value;

                List<Edge> volume_edges = new List<Edge>();

                float semi_width = Shadow_Map_Width / 2f, semi_height = Shadow_Map_Height / 2f;

                Vertex zero_point = new Vertex(Vector4D.Zero);
                Vertex near_top_left_point = new Vertex(new Vector4D(-semi_width, semi_height, Shadow_Map_Z_Near));
                Vertex near_top_right_point = new Vertex(new Vector4D(semi_width, semi_height, Shadow_Map_Z_Near));
                Vertex near_bottom_left_point = new Vertex(new Vector4D(-semi_width, -semi_height, Shadow_Map_Z_Near));
                Vertex near_bottom_right_point = new Vertex(new Vector4D(semi_width, -semi_height, Shadow_Map_Z_Near));

                if ((volume_style & Volume_Outline.Near) == Volume_Outline.Near)
                {
                    volume_edges.AddRange(new[]
                    {
                        new Edge(zero_point, near_top_left_point), // Near top left
                        new Edge(zero_point, near_top_right_point), // Near top right
                        new Edge(zero_point, near_bottom_left_point), // Near bottom left
                        new Edge(zero_point, near_bottom_right_point), // Near bottom right
                        new Edge(near_top_left_point, near_top_right_point), // Near top
                        new Edge(near_bottom_left_point, near_bottom_right_point), // Near bottom
                        new Edge(near_top_left_point, near_bottom_left_point), // Near left
                        new Edge(near_top_right_point, near_bottom_right_point) // Near right
                    });
                }

                if ((volume_style & Volume_Outline.Far) == Volume_Outline.Far)
                {
                    float ratio = (this is Distant_Light) ? 1 : Shadow_Map_Z_Far / Shadow_Map_Z_Near;
                    float semi_width_ratio = semi_width * ratio, semi_height_ratio = semi_height * ratio;

                    Vertex far_top_left_point = new Vertex(new Vector4D(-semi_width_ratio, semi_height_ratio, Shadow_Map_Z_Far));
                    Vertex far_top_right_point = new Vertex(new Vector4D(semi_width_ratio, semi_height_ratio, Shadow_Map_Z_Far));
                    Vertex far_bottom_left_point =
                        new Vertex(new Vector4D(-semi_width_ratio, -semi_height_ratio, Shadow_Map_Z_Far));
                    Vertex far_bottom_right_point =
                        new Vertex(new Vector4D(semi_width_ratio, -semi_height_ratio, Shadow_Map_Z_Far));

                    volume_edges.AddRange(new[]
                    {
                        new Edge(near_top_left_point, far_top_left_point), // Far top left
                        new Edge(near_top_right_point, far_top_right_point), // Far top right
                        new Edge(near_bottom_left_point, far_bottom_left_point), // Far bottom left
                        new Edge(near_bottom_right_point, far_bottom_right_point), // Far bottom right
                        new Edge(far_top_left_point, far_top_right_point), // Far top
                        new Edge(far_bottom_left_point, far_bottom_right_point), // Far bottom
                        new Edge(far_top_left_point, far_bottom_left_point), // Far left
                        new Edge(far_top_right_point, far_bottom_right_point) // Far right
                    });
                }

                Volume_Edges = volume_edges.ToArray();
            }
        }

        internal Edge[] Volume_Edges = new Edge[0];

        // Matrices
        internal Matrix4x4 World_to_Light_View, Light_View_to_Light_Screen, Light_Screen_to_Light_Window;

        internal override void Calculate_Matrices()
        {
            base.Calculate_Matrices();
            World_to_Light_View = Model_to_World.Inverse();
        }

        // Clipping planes
        internal Clipping_Plane[] Light_View_Clipping_Planes;

        // Shadow map volume
        internal float[][] Shadow_Map;
        public abstract int Shadow_Map_Width { get; set; }
        public abstract int Shadow_Map_Height { get; set; }
        public abstract float Shadow_Map_Z_Near { get; set; }
        public abstract float Shadow_Map_Z_Far { get; set; }

        private static readonly Matrix4x4 window_translate = Transform.Translate(new Vector3D(1, 1, 0));
        protected void Set_Shadow_Map()
        {
            // Set shadow map
            Shadow_Map = new float[Shadow_Map_Width][];
            for (int i = 0; i < Shadow_Map_Width; i++) Shadow_Map[i] = new float[Shadow_Map_Height];
            
            // Set light-screen-to-light-window matrix
            Light_Screen_to_Light_Window = Transform.Scale(0.5f * (Shadow_Map_Width - 1), 0.5f * (Shadow_Map_Height - 1), 1) * window_translate;
        }

        /// <include file="Help_8.xml" path="doc/members/member[@name='']/*"/>

        #endregion

        #region Constructors

        internal Light(Vector3D origin, Vector3D direction_forward, Vector3D direction_up) : base(origin, direction_forward, direction_up) { }

        #endregion

        #region Methods

        // Export
        /// <include file="Help_7.xml" path="doc/members/member[@name='M:_3D_Engine.Light.Export_Shadow_Map']/*"/>
        public void Export_Shadow_Map() => Export_Shadow_Map($"{Directory.GetCurrentDirectory()}\\Export\\{GetType().Name}_{ID}_Export_Map.bmp");

        /// <include file="Help_7.xml" path="doc/members/member[@name='M:_3D_Engine.Light.Export_Shadow_Map(System.String)']/*"/>
        public void Export_Shadow_Map(string file_path)
        {
            Trace.WriteLine($"Generating shadow map for {GetType().Name}...");

            string file_directory = Path.GetDirectoryName(file_path);
            if (!Directory.Exists(file_directory)) Directory.CreateDirectory(file_directory);

            using (Bitmap shadow_map_bitmap = new Bitmap(Shadow_Map_Width, Shadow_Map_Height))
            {
                for (int x = 0; x < Shadow_Map_Width; x++)
                {
                    for (int y = 0; y < Shadow_Map_Height; y++)
                    {
                        int value = (255 * ((Shadow_Map[x][y] + 1) / 2)).Round_to_Int();

                        Color greyscale_colour = Color.FromArgb(255, value, value, value);
                        shadow_map_bitmap.SetPixel(x, y, greyscale_colour);
                    }
                }

                shadow_map_bitmap.Save(file_path, ImageFormat.Bmp);
            }

            Trace.WriteLine($"Successfully saved shadow map for {GetType().Name}");
        }

        #endregion
    }
}