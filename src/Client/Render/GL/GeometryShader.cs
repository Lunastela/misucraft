using Silk.NET.OpenGL;

namespace Misucraft.Client.Render {
    public class GeometryShader : RenderShader
    {
        public GeometryShader(GL gl, string vertexPath, string fragmentPath, string geometryPath) 
            : base(gl, vertexPath, fragmentPath) {
            
            _gl = gl;
            uint vertex = LoadShader(ShaderType.VertexShader, vertexPath);
            uint geometry = LoadShader(ShaderType.GeometryShader, geometryPath);
            uint fragment = LoadShader(ShaderType.FragmentShader, fragmentPath);

            _gl.DeleteProgram(_handle);
            _handle = _gl.CreateProgram();
            _gl.AttachShader(_handle, vertex);
            _gl.AttachShader(_handle, geometry);
            _gl.AttachShader(_handle, fragment);
            _gl.LinkProgram(_handle);
            
            _gl.GetProgram(_handle, GLEnum.LinkStatus, out var status);
            if (status == 0)
                throw new Exception($"Program failed to link with error: {_gl.GetProgramInfoLog(_handle)}");

            _gl.DetachShader(_handle, vertex);
            _gl.DetachShader(_handle, geometry);
            _gl.DetachShader(_handle, fragment);
            _gl.DeleteShader(vertex);
            _gl.DeleteShader(geometry);
            _gl.DeleteShader(fragment);
        }   
    }
}