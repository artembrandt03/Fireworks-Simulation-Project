using DrawingLibrary.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DrawingLibrary.Input
{
    public sealed class CustomMouse : ICustomMouse
    {
        private static CustomMouse _instance = null;
        private MouseState _currentState;
        private MouseState _previousState;

        public static CustomMouse Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CustomMouse();
                }
                return _instance;
            }
        }

        private CustomMouse()
        {
            _currentState = Mouse.GetState();
            _previousState = _currentState;
        }
        public Point WindowPosition => _currentState.Position;

        public Vector2? GetScreenPosition(IScreen screen)
        {
            if (screen == null) 
            { 
                throw new ArgumentNullException(nameof(screen));
            }


            //Rectangle where the virtual screen is drawn inside the window
            Rectangle destination = screen.CalculateDestinationRectangle();

            Point point = _currentState.Position;

            //If outside the drawn area, we want to return null
            if (point.X < destination.X || point.Y < destination.Y || point.X >= destination.Right || point.Y >= destination.Bottom)
            {
                return null;
            }

            //Convert to screen coordinates
            //Calculations: substract to put the mouse to top left of destination rectangle, then divide by width/height to get relative coordinates
            float u = (point.X - destination.X) / (float)destination.Width;
            float v = (point.Y - destination.Y) / (float)destination.Height;
            //Scaling to virtual screen coordinates
            float sx = u * screen.Width;
            float sy = v * screen.Height;
            //If everything goes well should work like this: (640, 430) -> (320, 240) for a 640x480 screen in an 800x600 window

            return new Vector2(sx, sy);
        }

        public bool IsLeftButtonClicked()
        {
            return _currentState.LeftButton == ButtonState.Pressed && 
                _previousState.LeftButton == ButtonState.Released;
        }

        public bool IsLeftButtonDown()
        {
            return _currentState.LeftButton == ButtonState.Pressed;
        }

        public bool IsLeftButtonUp()
        {
            return _currentState.LeftButton == ButtonState.Released;
        }

        public bool IsMiddleButtonClicked()
        {
            return _currentState.MiddleButton == ButtonState.Pressed &&
                _previousState.MiddleButton == ButtonState.Released;
        }

        public bool IsMiddleButtonDown()
        {
            return _currentState.MiddleButton == ButtonState.Pressed;
        }

        public bool IsRightButtonClicked()
        {
            return _currentState.RightButton == ButtonState.Pressed &&
                _previousState.RightButton == ButtonState.Released;
        }

        public bool IsRightButtonDown()
        {
            return _currentState.RightButton == ButtonState.Pressed;
        }

        public void Update()
        {
            _previousState = _currentState;
            _currentState = Mouse.GetState();
        }
    }
}
