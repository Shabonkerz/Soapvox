using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Media;
using System.Threading;
using System.IO;
using SandvoxConsole;

namespace Sandvox
{

    public struct Vertex : IVertexType
    {
        public Vector3 Position;

        public Vertex(Vector3 p)
        {
            this.Position = p;
        }
        public bool isEmpty()
        {
            return Position == null;
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement[] {
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0)
            }
        );

        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }
    };
    public struct VertexPositionNormalTexture : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TextureCoord;
        public Vector4 Tangent;

        public VertexPositionNormalTexture(Vector3 p, Vector3 n, Vector2 c)
        {
            this.Position = p;
            this.Normal = n;
            this.TextureCoord = c;
            this.Tangent = new Vector4(0, 0, 1, 1);
        }
        public bool isEmpty()
        {
            return Position == null;
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement[] {
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(36, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0)
            }
        );

        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }
    };

    public struct VertexPositionNormalColor : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector3 Color;
        //public Vector4 Tangent;

        public VertexPositionNormalColor(Vector3 p, Vector3 n, Vector3 c)
        {
            this.Position = p;
            this.Normal = n;
            this.Color = c;
            //this.Tangent = new Vector4(0, 0, 1, 1);
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement[] {
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                new VertexElement(24, VertexElementFormat.Vector3, VertexElementUsage.Color, 0),
                //new VertexElement(36, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0)
            }
        );
        public readonly static VertexPositionNormalColor Zero = new VertexPositionNormalColor(Vector3.Zero, Vector3.Zero, Vector3.Zero);
        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }
    };

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class BlockGame : Microsoft.Xna.Framework.Game
    {

        private struct Light
        {
            public enum LightType
            {
                DirectionalLight,
                PointLight,
                SpotLight
            }

            public LightType Type;
            public Vector3 Direction;
            public Vector3 Position;
            public Color Ambient;
            public Color Diffuse;
            public Color Specular;
            public float SpotInnerConeRadians;
            public float SpotOuterConeRadians;
            public float Radius;
        }

        /// <summary>
        /// A material. This material structure is the same as the one defined
        /// in the parallax_normal_mapping.fx file. We use the Color type here
        /// instead of a four element floating point array.
        /// </summary>
        private struct Material
        {
            public Color Ambient;
            public Color Diffuse;
            public Color Emissive;
            public Color Specular;
            public float Shininess;
        }
        public static bool Control = true;
        const int TOP_LEFT = 0;
        const int TOP_RIGHT = 1;
        const int BOTTOM_RIGHT = 2;
        const int BOTTOM_LEFT = 3;

        const float rotationSpeed = 0.3f;
        const float moveSpeed = 100.0f;

        public static GraphicsDeviceManager graphics;

        public static Matrix WorldViewProjection;
        public static GameWindow Window;
        public static Vector3 cameraPosition = new Vector3(190, 90, 64);

        public static float leftrightRot = MathHelper.PiOver2;
        public static float updownRot = -MathHelper.Pi / 10.0f;

        SpriteBatch spriteBatch;
        //Player player;
        GraphicsDevice device;
        MouseState originalMouseState;
        SpriteFont font;
        KeyboardState keyState;
        public static InstanceBrush brush;
        public static Octree<Volume> octree;
        Vector3 lightDirection = Vector3.One;
        Vector3 lightColor = new Vector3(0, 0, 0);

        public static CameraComponent camera;

        private const float CAMERA_FOV = 90.0f;
        private const float CAMERA_ZNEAR = 0.01f;
        private const float CAMERA_ZFAR = 1000.0f;
        private const float CAMERA_OFFSET = 100.0f;

        private Light light;
        private Light light2;
        private Material material;
        public static Effect effect;

        private Texture2D nullTexture;
        public static Texture2D voxelColors;
        private Texture2D floorColorMap;
        private Texture2D floorNormalMap;
        private Texture2D floorHeightMap;
        System.Drawing.Bitmap reddit;
        private Vector2 scaleBias;
        public static Color color;
        private bool disableParallax = true;

        public static World world;
        public static bool hasFocus = true;
        public static Vector3 brushSize = new Vector3(1, 1, 1);
        private SandvoxInterpreter interp;

        public SandvoxInterpreter Interpreter
        {
            get { return interp; }
            set {}
        }

        WorldVolume tmp;

        public BlockGame()
        {
            graphics = new GraphicsDeviceManager(this);
            device = graphics.GraphicsDevice;
            Content.RootDirectory = "Content";
            camera = new CameraComponent(this);
            Components.Add(camera);
            disableParallax = false;
            IsFixedTimeStep = false;
            IsMouseVisible = false;


        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Window = base.Window;
            Window.Title = "Sandvox";

            // Setup frame buffer.
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.Viewport.Width;
            graphics.PreferredBackBufferHeight = graphics.GraphicsDevice.Viewport.Height;
            graphics.PreferMultiSampling = true;
            graphics.GraphicsDevice.PresentationParameters.MultiSampleCount = 4;
            graphics.ApplyChanges();

            Mouse.SetPosition(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2);
            originalMouseState = Mouse.GetState();
            // Initialize light settings.
            light.Type = Light.LightType.DirectionalLight;
            light.Direction = new Vector3(2, 3, 0);
            light.Radius = Math.Max(35, 25);
            light.Position = new Vector3(6.0f, 1.0f, 1.0f);
            light.Ambient = new Color(0.3f, 0.3f, 0.3f);
            light.Diffuse = new Color(0.9f, 0.9f, 0.9f);
            light.Specular = Color.White;
            light.SpotInnerConeRadians = MathHelper.ToRadians(30.0f);
            light.SpotOuterConeRadians = MathHelper.ToRadians(100.0f);

            light2.Type = Light.LightType.DirectionalLight;
            light2.Direction = new Vector3(6, 1, 5);
            light2.Radius = Math.Max(25, 50);
            light2.Ambient = new Color(0.7f, 0.7f, 0.7f);
            light2.Diffuse = new Color(0.8f, 0.8f, 0.8f);
            light2.Specular = Color.White;
            light2.Position = new Vector3(0, 0, 0);
            light2.SpotInnerConeRadians = MathHelper.ToRadians(30.0f);
            light2.SpotOuterConeRadians = MathHelper.ToRadians(100.0f);

            // Initialize material settings for the floor.
            material.Ambient = new Color(new Vector4(0.6f, 0.6f, 0.6f, 1.0f));
            material.Diffuse = new Color(new Vector4(0.7f, 0.7f, 0.7f, 1.0f));
            material.Emissive = Color.Black;
            material.Specular = Color.White;
            material.Shininess = 0.0f;

            // Parallax mapping height scale and bias values.
            scaleBias = new Vector2(0.04f, -0.03f);
            //player = new Player();
            reddit = new System.Drawing.Bitmap(@"C:\gamephotos\awesomeface.jpg");
            InitCamera();
            
            world = new World(this, 10);
            this.Components.Add(world);

            color = Color.OrangeRed;

            interp = new SandvoxInterpreter(this, Content.Load<SpriteFont>("ConsoleFont"));
            interp.AddGlobal("game", this);

            brush = new InstanceBrush("brush.txt");
            //Face.InitializeFaces();
            base.Initialize();
        }
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            font = Content.Load<SpriteFont>("SpriteFont1");
            effect = Content.Load<Effect>("Default");

            //floorColorMap = Content.Load<Texture2D>("Textures\\floor_color_map");
            //floorNormalMap = Content.Load<Texture2D>("Textures\\floor_normal_map");
            //floorHeightMap = Content.Load<Texture2D>("Textures\\floor_height_map");
            //voxelColors = Content.Load<Texture2D>("Textures\\color_picker");

            nullTexture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);

            Color[] pixels = { Color.White };

            nullTexture.SetData(pixels);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summar>y
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        private void InitCamera()
        {
            GraphicsDevice device = graphics.GraphicsDevice;
            float aspectRatio = (float)graphics.GraphicsDevice.Viewport.Width / (float)graphics.GraphicsDevice.Viewport.Height;

            camera.Position = new Vector3(0.0f, 0.0f, 3.0f);
            camera.Acceleration = new Vector3(100.0f, 100.0f, 100.0f);
            camera.Velocity = new Vector3(300.0f, 300.0f, 300.0f);
            camera.OrbitMinZoom = 1.5f;
            camera.OrbitMaxZoom = 5.0f;
            camera.OrbitOffsetDistance = camera.OrbitMinZoom;
            camera.LookAt(new Vector3(0, 0, 0));
            ChangeCameraBehavior(Camera.Behavior.Spectator);
        }

        private void ChangeCameraBehavior(Camera.Behavior behavior)
        {
            if (camera.CurrentBehavior == behavior)
                return;


            camera.CurrentBehavior = behavior;

            // Position the camera behind and 30 degrees above the target.
            if (behavior == Camera.Behavior.Orbit)
                camera.Rotate(0.0f, -30.0f, 0.0f);
        }
        public static void setBrushColor(System.Drawing.Color c)
        {
            color = new Color(c.R, c.G, c.B, c.A);
        }
        public static void setBrushSize(int size_x, int size_y, int size_z)
        {
            brushSize = new Vector3(size_x, size_y, size_z);
        }
        private void ProcessInput()
        {

            // "Fuck get me out of here!" button.
            if (keyState.IsKeyDown(Keys.Escape))
            {
                world.Stop();
                this.Exit();
            }
            if (!Sandvox.BlockGame.hasFocus) return;

            if (keyState.IsKeyDown(Keys.D2) && Keyboard.GetState().IsKeyUp(Keys.D2))
            {
                Vector3 v = camera.ViewDirection * 15;
                v = v + camera.Position;
                List<Volume> list = brush.toList();
                foreach (Volume node in list)
                {
                    if (node.Visible) world.Add(new Volume(node.Position + v, node.Size, node.color));
                }
            }

            if (keyState.IsKeyDown(Keys.D5) && Keyboard.GetState().IsKeyUp(Keys.D5))
            {
                world.Add(new Volume(Vector3.Zero, new Vector3(100, 1, 1), Color.Blue));
                world.Add(new Volume(Vector3.Zero, new Vector3(1, 100, 1), Color.Purple));
                world.Add(new Volume(Vector3.Zero, new Vector3(1, 1, 100), Color.Red));

            }
            if (keyState.IsKeyDown(Keys.D6) && Keyboard.GetState().IsKeyUp(Keys.D6))
            {
                Vector3 pos = new Vector3(0, 0, 0);
                Vector3 size = new Vector3(1, 1, 1);
                Color c = new Color(0, 0, 0);
                world.faceBatch.AddFace(Face.Facing.Backward, ref pos, size, ref c);
                world.faceBatch.AddFace(Face.Facing.Forward, ref pos, size, ref c);
                world.faceBatch.AddFace(Face.Facing.Right, ref pos, size, ref c);
                world.faceBatch.AddFace(Face.Facing.Left, ref pos, size, ref c);
                world.faceBatch.AddFace(Face.Facing.Up, ref pos, size, ref c);
                world.faceBatch.AddFace(Face.Facing.Down, ref pos, size, ref c);
            }
            light.Position = camera.Position;
            light.Direction = camera.ViewDirection;
            #region Old commands
            /*
            if (keyState.IsKeyDown(Keys.J) && Keyboard.GetState().IsKeyUp(Keys.J))
            {
                world.Remove((int)light.Position.X, (int)light.Position.Y, (int)light.Position.Z);
                light.Position.X += 1.0f;
                //light.Direction += Vector3.Left;
            }
            if (keyState.IsKeyDown(Keys.L) && Keyboard.GetState().IsKeyUp(Keys.L))
            {
                world.Remove((int)light.Position.X, (int)light.Position.Y, (int)light.Position.Z);
                light.Position.X += 1.0f;
            }
            if (keyState.IsKeyDown(Keys.I) && Keyboard.GetState().IsKeyUp(Keys.I))
            {
                world.Remove((int)light.Position.X, (int)light.Position.Y, (int)light.Position.Z);
                light.Position.Y += 1.0f;
            }
            if (keyState.IsKeyDown(Keys.K) && Keyboard.GetState().IsKeyUp(Keys.K))
            {
                world.Remove((int)light.Position.X, (int)light.Position.Y, (int)light.Position.Z);
                light.Position.Y -= 1.0f;
            }
            if (keyState.IsKeyDown(Keys.U) && Keyboard.GetState().IsKeyUp(Keys.U))
            {
                world.Remove((int)light.Position.X, (int)light.Position.Y, (int)light.Position.Z);
                light.Position.Z += 1.0f;
            }
            if (keyState.IsKeyDown(Keys.M) && Keyboard.GetState().IsKeyUp(Keys.M))
            {
                world.Remove((int)light.Position.X, (int)light.Position.Y, (int)light.Position.Z);
                light.Position.Z -= 1.0f;
            }
            if (keyState.IsKeyDown(Keys.G) && Keyboard.GetState().IsKeyUp(Keys.G))
            {
                camera.Rotate(0f, -0.55f, 0f);
            }
            if (keyState.IsKeyDown(Keys.F))
            {
                camera.Rotate(0.55f, 0f, 0f );
            }
            if (keyState.IsKeyDown(Keys.H))
            {
                camera.Rotate(-0.55f, 0f, 0f );
            }
            if (keyState.IsKeyDown(Keys.T))
            {
                camera.Rotate(0f, 0.55f, 0f);
            }

            if (keyState.IsKeyDown(Keys.D2))
            {
                Vector3 v = camera.ViewDirection * 15;
                v = v + camera.Position;

                world.Add((int)v.X, (int)v.Y, (int)v.Z, brushSize, color);
            }
            /*
            if (keyState.IsKeyDown(Keys.D0) && Keyboard.GetState().IsKeyUp(Keys.D0))
            {
                int x_offset = 8; int y_offset = 8; int z_offset = 8;

                for (int i = 0; i < 80; i++)
                {
                    world.Add(i + x_offset, y_offset, z_offset, new Vector3(1, 1, 1), Color.Red);
                    world.Add(x_offset, i + y_offset, z_offset, new Vector3(1, 1, 1), Color.Red);
                    world.Add(x_offset, y_offset, i + z_offset, new Vector3(1, 1, 1), Color.Red);
                }

            }
            if (keyState.IsKeyDown(Keys.D5) && Keyboard.GetState().IsKeyUp(Keys.D5) )
            {
                int x_offset = 8; int y_offset = 8; int z_offset = 8;
                int j = (int)Math.Pow(2.0f, 9.0f);
                for (int i = 0; i < j; i+=j/8)
                {
                    world.Add(i + x_offset, y_offset, z_offset, new Vector3(1, j, 1), Color.Red);
                    world.Add(i + x_offset, y_offset, z_offset, new Vector3(1, 1, j), Color.Red);

                    world.Add(x_offset, y_offset, i + z_offset, new Vector3(1, j, 1), Color.Blue);
                    world.Add(x_offset, y_offset, i + z_offset, new Vector3(j, 1, 1), Color.Blue);

                    world.Add(x_offset, i + y_offset, z_offset, new Vector3(j, 1, 1), Color.Purple);
                    world.Add(x_offset, i + y_offset, z_offset, new Vector3(1, 1, j), Color.Purple);
                    
                }
                 
            }
            if (keyState.IsKeyDown(Keys.D1) && Keyboard.GetState().IsKeyUp(Keys.D1))
            {
                System.Drawing.Color c;
                for (int i = 0; i < reddit.Width; i++)
                {
                    for (int j = 0; j < reddit.Height; j++)
                    {
                        c = reddit.GetPixel(i, j);
                        world.Add(i+2, reddit.Height-j, 64, new Vector3(1, 1, 1), new Color(c.R, c.G, c.B, c.R));
                    }
                }
            }
            if (keyState.IsKeyDown(Keys.D2))
            {
                Vector3 v = camera.ViewDirection * 15;
                v = v + camera.Position;

                world.Add((int)v.X, (int)v.Y, (int)v.Z, new Vector3(1, 1, 1), color);
            }
            if (keyState.IsKeyDown(Keys.D3))
            {
                Vector3 v = camera.ViewDirection * 10;
                v = v + camera.Position;
                world.Remove((int)v.X, (int)v.Y, (int)v.Z);
            }
            if (keyState.IsKeyDown(Keys.D9) && Keyboard.GetState().IsKeyUp(Keys.D9))
            {
                color = Color.Chartreuse;
            }
            if (keyState.IsKeyDown(Keys.D6) && Keyboard.GetState().IsKeyUp(Keys.D6))
            {
                using (StreamWriter outfile = new StreamWriter("world.txt"))
                {
                    outfile.Write(world.toJSON());
                }
            }
            if (keyState.IsKeyDown(Keys.D8) && Keyboard.GetState().IsKeyUp(Keys.D8))
            {
                //int j = (int)Math.Pow(2.0f, 9.0f);
                for (int i = 0; i < 50; i++ )
                {
                    for (int j = 0; j < 50; j++)
                    {
                        for (int k = 0; k < 50; k++)
                        {

                            world.Add(i, j, k, new Vector3(1,1,1), Color.Red);
                        }
                    }
                }
            }
            if (keyState.IsKeyDown(Keys.D7) && Keyboard.GetState().IsKeyUp(Keys.D7))
            {
                world.loadFromFile("world.txt");
            }
            if (keyState.IsKeyDown(Keys.D4) && Keyboard.GetState().IsKeyUp(Keys.D4))
            {
                color = Color.Chocolate;
            }*/
            #endregion
            keyState = Keyboard.GetState();

        }
        private void UpdateEffect()
        {
            // Set shader matrix parameters.
            effect.Parameters["worldMatrix"].SetValue(Matrix.Identity);
            effect.Parameters["worldInverseTransposeMatrix"].SetValue(Matrix.Identity);
            effect.Parameters["worldViewProjectionMatrix"].SetValue(WorldViewProjection);

            // Set the shader camera position parameter.
            effect.Parameters["cameraPos"].SetValue(camera.Position);

            // Set the shader global ambiance parameters.
            effect.Parameters["globalAmbient"].SetValue(0.9f);

            // Set the shader parallax scale and bias parameter.
            effect.Parameters["scaleBias"].SetValue(scaleBias);

            // Set the shader lighting parameters.
            effect.Parameters["light"].StructureMembers["dir"].SetValue(light.Direction);
            effect.Parameters["light"].StructureMembers["pos"].SetValue(light.Position);
            effect.Parameters["light"].StructureMembers["ambient"].SetValue(light.Ambient.ToVector4());
            effect.Parameters["light"].StructureMembers["diffuse"].SetValue(light.Diffuse.ToVector4());
            effect.Parameters["light"].StructureMembers["specular"].SetValue(light.Specular.ToVector4());
            effect.Parameters["light"].StructureMembers["spotInnerCone"].SetValue(light.SpotInnerConeRadians);
            effect.Parameters["light"].StructureMembers["spotOuterCone"].SetValue(light.SpotOuterConeRadians);
            effect.Parameters["light"].StructureMembers["radius"].SetValue(light.Radius);
            effect.Parameters["light"].StructureMembers["power"].SetValue(0.9f);

            effect.Parameters["light2"].StructureMembers["dir"].SetValue(light2.Direction);
            effect.Parameters["light2"].StructureMembers["pos"].SetValue(light2.Position);
            effect.Parameters["light2"].StructureMembers["ambient"].SetValue(light2.Ambient.ToVector4());
            effect.Parameters["light2"].StructureMembers["diffuse"].SetValue(light2.Diffuse.ToVector4());
            effect.Parameters["light2"].StructureMembers["specular"].SetValue(light2.Specular.ToVector4());
            effect.Parameters["light2"].StructureMembers["spotInnerCone"].SetValue(light2.SpotInnerConeRadians);
            effect.Parameters["light2"].StructureMembers["spotOuterCone"].SetValue(light2.SpotOuterConeRadians);
            effect.Parameters["light2"].StructureMembers["radius"].SetValue(light2.Radius);
            effect.Parameters["light2"].StructureMembers["power"].SetValue(0.0f);

            // Set the shader material parameters.
            effect.Parameters["material"].StructureMembers["ambient"].SetValue(material.Ambient.ToVector4());
            effect.Parameters["material"].StructureMembers["diffuse"].SetValue(material.Diffuse.ToVector4());
            effect.Parameters["material"].StructureMembers["emissive"].SetValue(material.Emissive.ToVector4());
            effect.Parameters["material"].StructureMembers["specular"].SetValue(material.Specular.ToVector4());
            effect.Parameters["material"].StructureMembers["shininess"].SetValue(material.Shininess);

            // Select the shader based on light type.

            //effect.CurrentTechnique = effect.Techniques["PerPixelDirectionalLighting"];
            effect.CurrentTechnique = effect.Techniques["PerPixelLight"];
            //effect.CurrentTechnique = effect.Techniques["PerPixelNonInstanced"];
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            if (!this.IsActive)
                return;
            ProcessInput();
            WorldViewProjection = camera.ViewProjectionMatrix;
            UpdateEffect();

            base.Update(gameTime);
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;  // vertex order doesn't matter
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.Clear(Color.White);
            world.Draw();
            
            //player.Draw();
            base.Draw(gameTime);
        }
    }
}
