using DrawingLibrary.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace DrawingLibrary.Graphics
{
    public class Screen : IScreen
    {
        private readonly RenderTarget2D _renderTarg;
        public int Width => _renderTarg.Width;
        public int Height => _renderTarg.Height;
        private readonly GraphicsDevice _graphicsDevice;
        private bool _isSet; 

        public Screen(RenderTarget2D renderTarget)
        {
            if (renderTarget == null)
            {
                throw new ArgumentNullException(nameof(renderTarget));
            }
            _renderTarg = renderTarget;

            if (_renderTarg.GraphicsDevice == null)
            {
                throw new ArgumentException("RenderTarget2D needs to have a valid GraphicsDevice!!", nameof(renderTarget));
            }
            _graphicsDevice = _renderTarg.GraphicsDevice;
        }

        public void Present(ISpritesRenderer spritesRenderer, bool textureFiltering = true)
        {
            if (spritesRenderer == null)
            {
                throw new ArgumentNullException(nameof(spritesRenderer)); 
            }
            if (_isSet)
            {
                throw new Exception("call UnSet() before Present()");
            }
                
            var dest = CalculateDestinationRectangle();

            spritesRenderer.Begin(textureFiltering);

            spritesRenderer.Draw(_renderTarg, dest, Color.White);

            spritesRenderer.End();
        }

        public void Set()
        {
            if (_isSet)
            {
                throw new Exception("Screen is already set!");
            }
                

            _graphicsDevice.SetRenderTarget(_renderTarg);
            _isSet = true;
        }

        public void UnSet()
        {
            if (!_isSet)
                throw new Exception("Screen is not set yet!");

            _graphicsDevice.SetRenderTarget(null);
            _isSet = false;
        }

        public Rectangle CalculateDestinationRectangle()
        {
            int windowWidth = _graphicsDevice.Viewport.Width;
            int windowHeight = _graphicsDevice.Viewport.Height;
            int destWidth;
            int destHeight;
            int positionX;
            int positionY;

            if (windowWidth <= 0 || windowHeight <= 0 || Width <= 0 || Height <= 0)
            {
                return new Rectangle(0, 0, windowWidth, windowHeight);
            }

            float screenAspect = (float)Width / Height;
            float windowAspect = (float)windowWidth / windowHeight;

            if (windowAspect > screenAspect)
            {
                destHeight = windowHeight;
                destWidth = (int)(destHeight * screenAspect);
                positionX = (windowWidth - destWidth) / 2;
                positionY = 0;
            }
            else
            {
                destWidth = windowWidth;
                destHeight = (int)(destWidth / screenAspect);
                positionX = 0;
                positionY = (windowHeight - destHeight) / 2;
            }

            return new Rectangle(positionX, positionY, destWidth, destHeight);
        }

        public void Dispose()
        {
            _renderTarg?.Dispose();
        }
    }
}
