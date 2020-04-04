using Microsoft.Xna.Framework.Input;

namespace Sudoku_Solver_CS
{
    class MouseInput
    {
        private static MouseState currentMouseState;
        private static MouseState previousMouseState;

        public MouseInput(MouseState mouseState)
        {
            currentMouseState = mouseState;
            previousMouseState = mouseState;
        }

        public bool RightClick()
        {
            return currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Released;
        }

        public bool LeftClick()
        {
            return currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released;
        }

        public bool MiddleClick()
        {
            return currentMouseState.MiddleButton == ButtonState.Pressed && previousMouseState.MiddleButton == ButtonState.Released;
        }

        public void Update(MouseState mouseState)
        {
            previousMouseState = currentMouseState;
            currentMouseState = mouseState;
        }

        public int getMouseX()
        {
            return Mouse.GetState().X;
        }

        public int getMouseY()
        {
            return Mouse.GetState().Y;
        }
    }
}