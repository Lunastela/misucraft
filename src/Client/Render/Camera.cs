using System.Numerics;
using Misucraft.Util;
using Silk.NET.Input;

namespace Misucraft.Client.Render {
    public static class Camera
    {
        public static Vector3 Position = new Vector3(0.0f, 1.5f, 0.0f);
        public static Vector3 Front = new Vector3(0.0f, 0.0f, -1.0f);
        public static Vector3 Up = Vector3.UnitY;
        public static Vector3 Direction = Vector3.Zero;
        public static float Zoom = 75f;

        private static float Yaw = -90f;
        private static float Pitch = 0f;

        private static Vector2 LastMousePosition;
        public static IKeyboard primaryKeyboard;
        
        public static unsafe void OnUpdate(double deltaTime) {
            var moveSpeed = 2.5f * (float) deltaTime;
            if (primaryKeyboard.IsKeyPressed(Key.ShiftLeft))
                moveSpeed = moveSpeed * 2f;

            if (primaryKeyboard.IsKeyPressed(Key.W))
                Position += moveSpeed * Front;
            if (primaryKeyboard.IsKeyPressed(Key.S))
                Position -= moveSpeed * Front;
            if (primaryKeyboard.IsKeyPressed(Key.A))
                Position -= Vector3.Normalize(Vector3.Cross(Front, Up)) * moveSpeed;
            if (primaryKeyboard.IsKeyPressed(Key.D))
                Position += Vector3.Normalize(Vector3.Cross(Front, Up)) * moveSpeed;
        }

        public static unsafe void OnMouseMove(IMouse mouse, Vector2 position) {
            var lookSensitivity = 0.1f;
            if (LastMousePosition == default) 
                LastMousePosition = position;
            else {
                var xOffset = (position.X - LastMousePosition.X) * lookSensitivity;
                var yOffset = (position.Y - LastMousePosition.Y) * lookSensitivity;
                LastMousePosition = position;

                Yaw += xOffset;
                Pitch -= yOffset;

                Pitch = Math.Clamp(Pitch, -89.0f, 89.0f);

                Direction.X = MathF.Cos(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
                Direction.Y = MathF.Sin(MathHelper.DegreesToRadians(Pitch));
                Direction.Z = MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
                Front = Vector3.Normalize(Direction);
            }
        }

        public static unsafe void OnMouseWheel(IMouse mouse, ScrollWheel scrollWheel) {
            Zoom = Math.Clamp(Zoom - scrollWheel.Y, 1.0f, 90f);
        }
    }
}