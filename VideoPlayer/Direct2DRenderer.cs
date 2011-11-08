
//
// A simple abstraction for a D2D rendering framework.
//


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using SlimDX;
using SlimDX.Direct2D;

namespace VideoPlayer
{
    class Direct2DRenderer
    {
        public Color4 clearColor;
        public bool isInitialized;

        private Factory factory;
        private WindowRenderTarget renderTarget;

        public Direct2DRenderer(Color4 clearColor)
        {
            isInitialized = false;
            this.clearColor = clearColor;
        }

        public bool OnInitialize(IntPtr handle, int width, int height) 
        { 
            bool result = true;

            if( isInitialized == true )
            {
                OnShutdown();
            }

            if( result == true )
            {
                if (handle == IntPtr.Zero)
                {
                    // Not a valid handle.
                    result = false;
                }
            }

            if( result == true )
            {
                if (width < 1 || height < 1)
                {
                    // Not valid back buffer dimensions.
                    result = false;
                }
            }

            if( result == true )
            {
                // Create the factory.
                factory = new Factory();
            }

            if( result == true )
            {
                // Create the render target.
                WindowRenderTargetProperties properties = new WindowRenderTargetProperties();
                properties.Handle = handle;
                properties.PixelSize = new Size(width, height);
                properties.PresentOptions = PresentOptions.Immediately;

                renderTarget = new WindowRenderTarget(factory, properties);
            }

            if (result == true)
            {
                isInitialized = true;
            }

            return result;
        }

        public void OnUpdate() 
        {
            if (isInitialized == true)
            {
                // TODO: Figure out how to stream the movie
                // into things we can render into D2D.
            }
        }

        public void OnRender() 
        {
            if (isInitialized == true)
            {
                renderTarget.BeginDraw();

                // Clear the backbuffer.
                renderTarget.Clear(clearColor);

                // TODO: Draw the current frame of the movie.

                renderTarget.EndDraw();
            }
        }

        public void OnResize(int width, int height)
        {
            if (width > 0 && height > 0)
            {
                renderTarget.Resize(new Size(width, height));
            }
        }

        public void OnShutdown()
        {
            factory.Dispose();
            renderTarget.Dispose();
        }
    }
}
