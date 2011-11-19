
//
// A simple abstraction for a D2D rendering framework.
//


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Drawing.Imaging;
using SlimDX;
using SlimDX.Direct2D;

namespace VideoPlayer
{
    class Direct2DRenderer
    {
        public Color4 clearColor;
        public bool isInitialized;
        public bool isImageLoaded;

        private Factory factory;
        private WindowRenderTarget renderTarget;

        private byte[] currentFrameBytes;
        private SlimDX.Direct2D.Bitmap currentFrame;

        public Direct2DRenderer(Color4 clearColor)
        {
            isInitialized = false;
            isImageLoaded = false;
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
                properties.PresentOptions = PresentOptions.RetainContents | PresentOptions.None;

                renderTarget = new WindowRenderTarget(factory, properties);
            }

            if (result == true)
            {
                isInitialized = true;
            }

            return result;
        }

        public bool OnUpdate(Frame frame) 
        {
            bool result = true;

            if (isInitialized == true)
            {
                // First time calling this. Allocate our byte buffer.
                if (currentFrameBytes == null)
                {
                    currentFrameBytes = new byte[frame.bytesPerFrameInMemory];
                }

                // Convert the frame to bytes
                frame.GetBytes(ref currentFrameBytes);

                result = LoadImage(currentFrameBytes, frame.width, frame.height, frame.bytesPerStride);


            }

            if (result == true)
            {
                isImageLoaded = true;
            }

            return result;
        }

        public void OnRender() 
        {
            if (isInitialized == true)
            {
                renderTarget.BeginDraw();

                // Clear the backbuffer.
                renderTarget.Clear(clearColor);

                if (isImageLoaded == true)
                {
                    // Center image on the render target.
                    SizeF bBufferSize = renderTarget.Size;
                    SizeF frameSize = currentFrame.Size;

                    PointF offset = new PointF((bBufferSize.Width - frameSize.Width) / 2.0f,
                        (bBufferSize.Height - frameSize.Height) / 2.0f);

                    RectangleF rect = new RectangleF(new PointF(0.0f, 0.0f), new SizeF(50, 50));
                    renderTarget.DrawBitmap(currentFrame, rect);
                }

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

        public void OnReset()
        {
            isImageLoaded = false;

            if (currentFrame != null)
            {
                currentFrame.Dispose();
            }
            currentFrame = null;
        }

        public void OnShutdown()
        {
            factory.Dispose();
            renderTarget.Dispose();

            if (currentFrame != null)
            {
                currentFrame.Dispose();
            }
        }

        //
        // Helper Function
        //

        private bool LoadImage( byte[] data, int frameWidth, int frameHeight, int bytesPerStride)
        {
            bool result = true;

            if (currentFrame != null)
            {
                currentFrame.Dispose();
                currentFrame = null;
            }

            // Load the image into D2D
            DataStream stream = new DataStream(data, true, false);
            SlimDX.Direct2D.BitmapProperties properties = new SlimDX.Direct2D.BitmapProperties();
            properties.PixelFormat = new SlimDX.Direct2D.PixelFormat(SlimDX.DXGI.Format.B8G8R8A8_UNorm, SlimDX.Direct2D.AlphaMode.Ignore);
       
            try
            {
                currentFrame = new SlimDX.Direct2D.Bitmap(renderTarget, new Size(frameWidth, frameHeight),
                    stream, bytesPerStride, properties);
            }
            catch
            {
                result = false;
            }

            return result;
        }
    }
}
