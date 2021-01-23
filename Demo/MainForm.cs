﻿using _3D_Engine.Maths.Vectors;
using _3D_Engine.SceneObjects.Cameras;
using _3D_Engine.SceneObjects.Groups;
using _3D_Engine.SceneObjects.Meshes.OneDimension;
using _3D_Engine.SceneObjects.Meshes.ThreeDimensions;
using System.Windows.Forms;

namespace Simple_Demo
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            // Create a new scene
            Group scene = new();

            // Create some meshes
            WorldPoint origin = WorldPoint.ZeroOrigin;
            scene.Add(origin);
            Group axes = Arrow.Axes;
            scene.Add(axes);
            Cube cube = new(new Vector3D(10, 10, 10), Vector3D.UnitZ, Vector3D.UnitY, 30);
            scene.Add(cube);
            Cone cone = new(new Vector3D(70, 10, 10), Vector3D.UnitZ, Vector3D.UnitY, 30, 20, 50);
            scene.Add(cone);

            // Create a light
            //DistantLight light = new(new(0, 100, 0), scene.Meshes[0], Vector3D.UnitZ);
            //scene.Add(light);

            // Create a camera
            float cameraWidth = pictureBox.Width / 10f, cameraHeight = pictureBox.Height / 10f;
            PerspectiveCamera camera = new(new Vector3D(0, 0, -100), scene.SceneObjects[0], Vector3D.UnitY, cameraWidth, cameraHeight, 10, 750);
            scene.Add(camera);

            // Adjust render settings
            camera.MakeRenderSizeOfControl(pictureBox);

            // Render the scene and display the output in the picture box
            camera.Scene = scene;
            pictureBox.Image = camera.Render();
        }
    }
}