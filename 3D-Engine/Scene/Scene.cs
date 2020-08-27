﻿using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace _3D_Engine
{
    public sealed partial class Scene
    {
        #region Fields and Properties

        private static readonly object locker = new object();
        private readonly static Clipping_Plane[] camera_screen_clipping_planes =
        new Clipping_Plane[]
        {
            new Clipping_Plane(-Vector3D.One, Vector3D.Unit_X), // Left
            new Clipping_Plane(-Vector3D.One, Vector3D.Unit_Y), // Bottom
            new Clipping_Plane(-Vector3D.One, Vector3D.Unit_Z), // Near
            new Clipping_Plane(Vector3D.One, Vector3D.Unit_Negative_X), // Right
            new Clipping_Plane(Vector3D.One, Vector3D.Unit_Negative_Y), // Top
            new Clipping_Plane(Vector3D.One, Vector3D.Unit_Negative_Z) // Far
        };

        private Rectangle entire_canvas_rectangle;

        // Buffers
        private double[][] z_buffer;
        private Color[][] colour_buffer;

        public Camera Render_Camera { get; set; }

        /// <summary>
        /// <see cref="PictureBox"/> where the <see cref="Scene"/> will be rendered.
        /// </summary>
        public PictureBox Canvas_Box { get; set; }
        /// <summary>
        /// The background <see cref="Color"/> of the <see cref="Scene"/>.
        /// </summary>
        public Color Background_Colour { get; set; } = Color.White;

        // Lists
        /// <include file="Help_3.xml" path="doc/members/member[@name='F:_3D_Engine.Scene.Cameras']/*"/>
        public readonly List<Camera> Cameras = new List<Camera>();
        /// <include file="Help_3.xml" path="doc/members/member[@name='F:_3D_Engine.Scene.Lights']/*"/>
        public readonly List<Light> Lights = new List<Light>();
        /// <include file="Help_3.xml" path="doc/members/member[@name='F:_3D_Engine.Scene.Meshes']/*"/>
        public readonly List<Mesh> Meshes = new List<Mesh>();

        #endregion

        #region Dimensions

        private Matrix4x4 screen_to_window;

        private int width, height;

        /// <include file="Help_3.xml" path="doc/members/member[@name='P:_3D_Engine.Scene.Width']/*"/>
        public int Width
        {
            get => width;
            set
            {
                lock (locker)
                {
                    width = value;
                    screen_to_window = Transform.Scale(0.5 * (width - 1), 0.5 * (height - 1), 1) * Transform.Translate(new Vector3D(1, 1, 0));
                    entire_canvas_rectangle = new Rectangle(0, 0, width, height);
                    Set_Buffers();
                }
            }
        }
        /// <include file="Help_3.xml" path="doc/members/member[@name='P:_3D_Engine.Scene.Height']/*"/>
        public int Height
        {
            get => height;
            set
            {
                lock (locker)
                {
                    height = value;
                    screen_to_window = Transform.Scale(0.5 * (width - 1), 0.5 * (height - 1), 1) * Transform.Translate(new Vector3D(1, 1, 0));
                    entire_canvas_rectangle = new Rectangle(0, 0, width, height);
                    Set_Buffers();
                }
            }
        }

        private void Set_Buffers()
        {
            z_buffer = new double[width][];
            colour_buffer = new Color[width][];
            for (int i = 0; i < width; i++)
            {
                z_buffer[i] = new double[height];
                colour_buffer[i] = new Color[height];
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="Scene"/>.
        /// </summary>
        /// <param name="canvas_box">The <see cref="PictureBox"/> where the <see cref="Scene"/> will be rendered.</param>
        /// <param name="width">The width of the <see cref="Scene"/>.</param>
        /// <param name="height">The height of the <see cref="Scene"/>.</param>
        public Scene(PictureBox canvas_box, int width, int height)
        {
            Canvas_Box = canvas_box;
            Width = width;
            Height = height;

            Debug.WriteLine("Scene created");
        }

        #endregion

        #region Add to scene methods

        /// <summary>
        /// Adds a <see cref="Scene_Object"/> to the <see cref="Scene"/>.
        /// </summary>
        /// <param name="scene_object"><see cref="Scene_Object"/> to add.</param>
        public void Add(Scene_Object scene_object)
        {
            switch(scene_object.GetType().BaseType.Name)
            {
                case "Camera":
                    Cameras.Add((Camera)scene_object);
                    break;
                case "Light":
                    Light light = (Light)scene_object;
                    Lights.Add(light);
                    break;
                case "Mesh":
                    Meshes.Add((Mesh)scene_object);
                    break;
            }
        }

        public void Add(Scene_Object[] scene_objects)
        {
            foreach (Scene_Object scene_object in scene_objects) Add(scene_object);
        }

        #endregion

        #region Remove from scene methods

        public void Remove(int ID)
        {
            lock (locker) Meshes.RemoveAll(x => x.ID == ID);
        }

        public void Remove(int start_ID, int finish_ID)
        {
            lock (locker)
            {
                for (int ID = start_ID; ID <= finish_ID; ID++)
                {
                    Meshes.RemoveAll(x => x.ID == ID);
                }
            }
        }

        #endregion

        /// <summary>
        /// Renders the <see cref="Scene"/>. A Render Camera must be set before this method is called.
        /// </summary>
        public void Render()
        {
            if (Render_Camera == null)
            {
                Debug.WriteLine("Failed to draw frame: No camera has been set yet!");
                return;
            }

            lock (locker)
            {
                // Create temporary canvas for this frame
                Bitmap temp_canvas = new Bitmap(width, height, PixelFormat.Format24bppRgb);

                // Reset scene buffers
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        z_buffer[i][j] = 2;
                        colour_buffer[i][j] = Background_Colour;
                    }
                }

                //
                foreach (Light light in Lights)
                {
                    light.Calculate_Light_View_Clipping_Planes(); // move somewhere else?
                    for (int i = 0; i < light.Shadow_Map_Width; i++) for (int j = 0; j < light.Shadow_Map_Height; j++)
                    {
                        light.Shadow_Map[i][j] = 1;
                    }
                }

                // Calculate render camera properties
                Render_Camera.Calculate_Model_to_World_Matrix();
                Render_Camera.World_Origin = new Vector3D(Render_Camera.Model_to_World * Render_Camera.Origin);
                Render_Camera.Calculate_World_to_Camera_View_Matrix();
                Matrix4x4 world_to_view = Render_Camera.World_to_Camera_View;
                Matrix4x4 view_to_screen = Render_Camera.Camera_View_to_Screen;

                // Calculate depth information for each light
                foreach (Light light in Lights)
                {
                    light.Calculate_Model_to_World_Matrix();
                    light.World_Origin = new Vector3D(light.Model_to_World * light.Origin);
                    light.Calculate_World_to_Light_View_Matrix();

                    foreach (Mesh mesh in Meshes)
                    {
                        mesh.Calculate_Model_to_World_Matrix();
                        if (mesh.Draw_Faces)
                        {
                            foreach (Face face in mesh.Faces)
                            {
                                if (face.Visible) Generate_Shadow_Map(light, face, mesh.Model_to_World, light.World_to_Light_View, light.Light_View_to_Light_Screen);
                            }
                        }
                    }
                }

                // Draw meshes
                foreach (Mesh mesh in Meshes)
                {
                    if (mesh.Visible)
                    {
                        // Draw directions
                        if (mesh.Has_Direction_Arrows && mesh.Display_Direction_Arrows)
                        {
                            Arrow direction_forward = (Arrow)mesh.Direction_Arrows.Scene_Objects[0];
                            Arrow direction_up = (Arrow)mesh.Direction_Arrows.Scene_Objects[1];
                            Arrow direction_right = (Arrow)mesh.Direction_Arrows.Scene_Objects[2];

                            foreach (Face face in direction_forward.Faces) Draw_Face(face, "Arrow", direction_forward.Model_to_World, world_to_view, view_to_screen);
                            foreach (Face face in direction_up.Faces) Draw_Face(face, "Arrow", direction_up.Model_to_World, world_to_view, view_to_screen);
                            foreach (Face face in direction_right.Faces) Draw_Face(face, "Arrow", direction_right.Model_to_World, world_to_view, view_to_screen);

                            foreach (Edge edge in direction_forward.Edges) Draw_Edge(edge, direction_forward.Model_to_World, world_to_view, view_to_screen);
                            foreach (Edge edge in direction_up.Edges) Draw_Edge(edge, direction_up.Model_to_World, world_to_view, view_to_screen);
                            foreach (Edge edge in direction_right.Edges) Draw_Edge(edge, direction_right.Model_to_World, world_to_view, view_to_screen);
                        }

                        mesh.Origin = screen_to_window * view_to_screen * world_to_view * mesh.Model_to_World * mesh.Origin;

                        // Draw faces
                        if (mesh.Draw_Faces)
                        {
                            string mesh_type = mesh.GetType().Name;

                            foreach (Face face in mesh.Faces)
                            {
                                if (face.Visible) Draw_Face(face, mesh_type, mesh.Model_to_World, world_to_view, view_to_screen);
                            }
                        }

                        // Draw edges
                        if (mesh.Draw_Edges)
                        {
                            foreach (Edge edge in mesh.Edges)
                            {
                                if (edge.Visible) Draw_Edge(edge, mesh.Model_to_World, world_to_view, view_to_screen);
                            }
                        }
                    }
                }

                // Draw camera views
                foreach (Camera camera_to_draw in Cameras)
                {
                    // Calculate camera matrix
                    camera_to_draw.Calculate_Model_to_World_Matrix();
                    Matrix4x4 model_to_world = camera_to_draw.Model_to_World;
                        
                    Draw_Camera(camera_to_draw, model_to_world, world_to_view, view_to_screen);
                }
                
                // Draw theh frame on the canvas and push it to the screen
                Draw_Colour_Buffer(temp_canvas, colour_buffer);
                Canvas_Box.Image = temp_canvas;
            }
        }

        private unsafe void Draw_Colour_Buffer(Bitmap canvas, Color[][] colour_buffer) // source of this method?!
        {
            BitmapData data = canvas.LockBits(entire_canvas_rectangle, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            for (int y = 0; y < height; y++)
            {
                byte* row_start = (byte*)data.Scan0 + y * data.Stride;
                for (int x = 0; x < width; x++)
                {
                    row_start[x * 3] = colour_buffer[x][y * -1 + height - 1].B; // Blue
                    row_start[x * 3 + 1] = colour_buffer[x][y * -1 + height - 1].G; // Green
                    row_start[x * 3 + 2] = colour_buffer[x][y * -1 + height - 1].R; // Red
                }
            }

            canvas.UnlockBits(data);
        }
    }
}