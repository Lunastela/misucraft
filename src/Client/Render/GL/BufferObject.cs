using Silk.NET.OpenGL;
using System;

namespace Misucraft.Client.Render {
    public class BufferObject<TDataType> : IDisposable
        where TDataType : unmanaged
    {
        private uint _handle;
        private BufferTargetARB _bufferType;
        private GL _gl;
        private nuint _currentSize; 

        public unsafe BufferObject(GL gl, Span<TDataType> data, BufferTargetARB bufferType)
        {
            _gl = gl;
            _bufferType = bufferType;

            _handle = _gl.GenBuffer();
            Bind();
            fixed (void* d = data) {
                _currentSize = (nuint)(data.Length * sizeof(TDataType));
                _gl.BufferData(bufferType, _currentSize, d, BufferUsageARB.StaticDraw);
            }
        }

        public void Bind()
        {
            _gl.BindBuffer(_bufferType, _handle);
        }

        public unsafe void UpdateData(Span<TDataType> data)
        {
            Bind();
            nuint dataSize = (nuint)(data.Length * sizeof(TDataType));
            
            if (dataSize > _currentSize)
            {
                // Reallocate buffer if the new data size is larger than the current size
                fixed (void* d = data)
                {
                    _currentSize = dataSize;
                    _gl.BufferData(_bufferType, _currentSize, d, BufferUsageARB.StaticDraw);
                }
            }
            else
            {
                // Update the buffer with the new data
                fixed (void* d = data)
                {
                    _gl.BufferSubData(_bufferType, 0, dataSize, d);
                }
            }
        }

        public void Dispose()
        {
            _gl.DeleteBuffer(_handle);
        }
    }

}