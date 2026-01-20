using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrawingLibrary.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DrawingLibrary.Input
{
    public sealed class CustomKeyboard : ICustomKeyboard
    {
        private static CustomKeyboard _instance = null;
        private KeyboardState _currentState;
        private KeyboardState _previousState;

        public static CustomKeyboard Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CustomKeyboard();
                }
                return _instance;
            }
        }

        private CustomKeyboard() 
        {
            _currentState = Keyboard.GetState();
            _previousState = _currentState;
        }

        public bool IsKeyClicked(Keys key)
        {
            return _currentState.IsKeyDown(key) && !(_previousState.IsKeyDown(key));
        }

        public bool IsKeyDown(Keys key)
        {
            return _currentState.IsKeyDown(key);
        }

        public void Update()
        {
            _previousState = _currentState;
            _currentState = Keyboard.GetState();
        }
    }
}
