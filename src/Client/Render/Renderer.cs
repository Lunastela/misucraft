using System.Drawing;
using System.Numerics;
using Misucraft.Server;
using Misucraft.Util;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Misucraft.Client.Render {
    public class Renderer {
        public static GL ?_gl;

        // Handlers
        private static VertexArrayObject<uint, uint> chunkVertexObject;
        
        private static RenderTexture _texture;
        private static GeometryShader _blockshader;

        private Chunk testChunk;

        public unsafe Renderer(GL gl) {
            _gl = gl;
            GenBuffers();
            Program._window.Render += Render;
            Program._window.FramebufferResize += OnFramebufferResize;
        }

        public void GenBuffers() {
            chunkVertexObject = new VertexArrayObject<uint, uint>(_gl, 
                new BufferObject<uint>(_gl, new uint[]{}, BufferTargetARB.ArrayBuffer),
                new BufferObject<uint>(_gl, new uint[]{}, BufferTargetARB.ElementArrayBuffer)
            );
            chunkVertexObject.VertexAttributePointer(0, 1, VertexAttribPointerType.Float, 1, 0);

            testChunk = new Chunk(0, 0, 0);
            _texture = new RenderTexture(_gl, "rsrc/dirt.png");
            _blockshader = new GeometryShader(_gl, "block.vs", "block.fs", "block.gs");
        }

        public static void OnFramebufferResize(Vector2D<int> newSize) {
            _gl.Viewport(newSize);
        }

        public unsafe void Render(double deltaTime) {
            _gl.Enable(EnableCap.DepthTest);
            _gl.ClearColor(Color.CornflowerBlue);
            _gl.Clear((uint) (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));

            chunkVertexObject.UpdateBuffers(testChunk.FaceMesh.ToArray(), null);

            _texture.Bind();
            _blockshader.Use();
            // _blockshader.SetUniform("uTexture0", 0);

            var difference = 0f; // (float) Program._window.Time * 250f;
            var size = Program._window.FramebufferSize;
            var model = Matrix4x4.CreateRotationY(MathHelper.DegreesToRadians(difference));
            var view = Matrix4x4.CreateLookAt(Camera.Position, Camera.Position + Camera.Front, Camera.Up);
            var projection = Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Camera.Zoom), (float) size.X / size.Y, 0.1f, 100.0f);

            _blockshader.SetUniform("uModel", model);
            _blockshader.SetUniform("uView", view);
            _blockshader.SetUniform("uProjection", projection);

            _gl.DrawArrays(PrimitiveType.TriangleStrip, 0, (uint) testChunk.FaceMesh.Length);
        }
    }
}