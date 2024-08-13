using Silk.NET.OpenGL;
using System;

namespace Misucraft.Client.Render {
    public class BufferObject<TDataType> : IDisposable
        where TDataType : unmanaged
    {
        public uint _handle;
        private BufferTargetARB _bufferType;
        private GL _gl;
        private nuint _currentSize; 

        public unsafe BufferObject(GL gl, Span<TDataType> data, BufferTargetARB bufferType) {
            _gl = gl;
            _bufferType = bufferType;

            _handle = _gl.GenBuffer();
            Bind();
            fixed (void* d = data) {
                _currentSize = (nuint)(data.Length * sizeof(TDataType));
                _gl.BufferData(bufferType, _currentSize, d, BufferUsageARB.DynamicDraw);
            }
        }

        public void Bind()
        {
            _gl.BindBuffer(_bufferType, _handle);
        }

        public unsafe void UpdateData(Span<TDataType> data)
        {
            Bind(); // Ensure the buffer is bound
            nuint dataSize = (nuint)(data.Length * sizeof(TDataType));
            fixed (void* d = data)
            {
                if (dataSize > _currentSize)
                {
                    // Reallocate buffer if the new data size is larger than the current size
                    _currentSize = dataSize;
                    _gl.BufferData(_bufferType, _currentSize, d, BufferUsageARB.DynamicDraw);
                }
                else
                {
                    // Update the buffer with the new data
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