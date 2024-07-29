using Silk.NET.OpenGL;
using System;

namespace Misucraft.Client.Render {
    public class VertexArrayObject<TVertexType, TIndexType> : IDisposable
        where TVertexType : unmanaged
        where TIndexType : unmanaged
    {
        private uint _handle;
        private GL _gl;
        private BufferObject<TVertexType> _vbo;
        private BufferObject<TIndexType> _ebo;

        public VertexArrayObject(GL gl, BufferObject<TVertexType> vbo, BufferObject<TIndexType> ebo)
        {
            _gl = gl;
            _vbo = vbo;
            _ebo = ebo;

            _handle = _gl.GenVertexArray();
            Bind();
            _vbo.Bind();
            _ebo.Bind();
        }

        public unsafe void VertexAttributePointer(uint index, int count, VertexAttribPointerType type, uint vertexSize, int offSet)
        {
            _gl.VertexAttribPointer(index, count, type, false, vertexSize * (uint) sizeof(TVertexType), (void*) (offSet * sizeof(TVertexType)));
            _gl.EnableVertexAttribArray(index);
        }

        public void Bind()
        {
            _gl.BindVertexArray(_handle);
        }

        public void UpdateBuffers(Span<TVertexType> vertexData, Span<TIndexType> indexData)
        {
            Bind();
            _vbo.UpdateData(vertexData);
            _ebo.UpdateData(indexData);
        }

        public void Dispose()
        {
            _gl.DeleteVertexArray(_handle);
            _vbo?.Dispose();
            _ebo?.Dispose();
        }
    }
}