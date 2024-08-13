using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using Misucraft.Client.Render;
using Misucraft.Server;
using Misucraft.Util;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Misucraft {
    class Program {
        public static IWindow _window;
        public static GL gl;
        static void Main(string[] args) {
            WindowOptions options = WindowOptions.Default with {
                Size = new Vector2D<int>(1280, 720),
                Title = "Misucraft (Not Minecraft)",
                IsVisible = false
            };

            // Create the Window
            _window = Window.Create(options);
            _window.Load += OnLoad;
            _window.Update += OnUpdate;
            _window.Run();
            _window.Dispose();
        }

        private static void OnUpdate(double deltaTime) {}

        private static VertexArrayObject<uint, int> chunkVertexObject;
        private static BufferObject<DrawCommand> indirectBuffer;
        private static List<Chunk> renderedChunks = new List<Chunk>();
        private static void OnLoad() {
            if (_window == null)
                return;

            // Create the Input Context
            IInputContext input = _window.CreateInput();
            if (input != null) {
                Camera.primaryKeyboard = input.Keyboards.FirstOrDefault();
                for (int i = 0; i < input.Mice.Count; i++) {
                    input.Mice[i].Cursor.CursorMode = CursorMode.Raw;
                    input.Mice[i].MouseMove += Camera.OnMouseMove;
                    input.Mice[i].Scroll += Camera.OnMouseWheel;
                }
                _window.Update += Camera.OnUpdate;
            }
            _window.Center();
            _window.IsVisible = true;

            // Set Up Rendering
            gl = _window.CreateOpenGL();
            _window.FramebufferResize += gl.Viewport;
            _window.Update += OnUpdate;
            _window.Render += OnRender;

            chunkVertexObject = new VertexArrayObject<uint, int>(gl, 
                new BufferObject<uint>(gl, new uint[]{}, BufferTargetARB.ArrayBuffer),
                new BufferObject<int>(gl, new int[]{}, BufferTargetARB.ShaderStorageBuffer)
            );
            chunkVertexObject.VertexAttributePointer(0, 1, VertexAttribPointerType.Float, 1, 0);
            gl.VertexAttribDivisor(0, 1);
            gl.BindBufferBase(BufferTargetARB.ShaderStorageBuffer, 1, chunkVertexObject._ebo._handle); // not actually an EBO 
            indirectBuffer = new BufferObject<DrawCommand>(gl, new DrawCommand[]{}, BufferTargetARB.DrawIndirectBuffer);

            // gl.PolygonMode(GLEnum.FrontAndBack, GLEnum.Line);
            // for (int x = 0; x < 4; x++) {
            //     for (int y = 0; y < 4; y++) {
            //         for (int z = 0; z < 4; z++) {
            //             renderedChunks.Add(new Chunk(x, y, z));
            //         }
            //     }
            // }
            renderedChunks.Add(new Chunk(0, 0, 0));

            _texture = new RenderTexture(gl, "rsrc/textures/grass_top.png");
            _blockshader = new RenderShader(gl, "block.vs", "block.fs");

            // gl.PolygonMode(GLEnum.FrontAndBack, GLEnum.Line);

            UpdateChunks();
        }

        private static unsafe void UpdateChunks() {
            combinedMeshArray.Clear();
            chunkPositionArray.Clear();
            commandList.Clear();

            int blockIndex = 0;
            foreach (var chunk in renderedChunks) {
                chunkPositionArray.AddRange([chunk.Position.X, chunk.Position.Y, chunk.Position.Z]);
                for (int i = 0; i < 6; i++) {
                    if (chunk.FaceMesh[i] != null) {
                        combinedMeshArray.AddRange(chunk.FaceMesh[i]);
                        commandList.Add(new DrawCommand {
                            Count = 4,
                            InstanceCount = (uint) chunk.FaceMesh[i].Count,
                            First = 0,
                            BaseInstance = (uint) blockIndex
                        });
                        blockIndex += chunk.FaceMesh[i].Count;
                    }

                }
            }
            
            chunkVertexObject.UpdateBuffers(
                combinedMeshArray.ToArray(),
                chunkPositionArray.ToArray()
            );
            indirectBuffer.UpdateData(commandList.ToArray());
        }

        private static RenderTexture _texture;
        private static RenderShader _blockshader;
        
        private static List<uint> combinedMeshArray = new List<uint>();
        private static List<int> chunkPositionArray = new List<int>();
        private static List<DrawCommand> commandList = new List<DrawCommand>();
        
        private unsafe static void OnRender(double deltaTime) {
            gl.Enable(EnableCap.DepthTest);
            gl.ClearColor(Color.CornflowerBlue);
            gl.Clear((uint) (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));         

            chunkVertexObject.Bind();
            indirectBuffer.Bind();
            _texture.Bind();
            _blockshader.Use();
            
            var difference = 0f; // (float) _window.Time * 250f;
            var size = _window.FramebufferSize;
            var model = Matrix4x4.CreateRotationY(MathHelper.DegreesToRadians(difference));
            var view = Matrix4x4.CreateLookAt(Camera.Position, Camera.Position + Camera.Front, Camera.Up);
            var projection = Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Camera.Zoom), (float) size.X / size.Y, 0.1f, 1000.0f);

            _blockshader.SetUniform("uModel", model);
            _blockshader.SetUniform("uView", view);
            _blockshader.SetUniform("uProjection", projection);

            gl.MultiDrawArraysIndirect(PrimitiveType.TriangleStrip, (void*) 0, (uint) commandList.Count, (uint) 0);

            GLEnum error = gl.GetError();
            if (error != GLEnum.NoError) {
                Console.WriteLine(error);
            }
        }
    }
}