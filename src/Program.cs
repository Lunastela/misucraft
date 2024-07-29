using Misucraft.Client.Render;
using Misucraft.Server;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Misucraft {
    class Program {
        public static IWindow ?_window;
        public static Renderer ?renderer;
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

            // Create the Renderer
            renderer = new Renderer(_window.CreateOpenGL());
        }

        private static void OnUpdate(double deltaTime) {

        }

        private static void KeyDown(IKeyboard keyboard, Key key, int keyCode) {
            if (key == Key.Escape)
                _window?.Close();
        }
    }
}