﻿
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
                    currentFrameBytes = new byte[frame.bytesPerFrame];
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
                    renderTarget.DrawBitmap(currentFrame);
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
            SlimDX.Direct2D.PixelFormat format =
                new SlimDX.Direct2D.PixelFormat(SlimDX.DXGI.Format.B8G8R8A8_UNorm, AlphaMode.Ignore);
            BitmapProperties props = new BitmapProperties();
            props.PixelFormat = format;

            try
            {
                currentFrame = new SlimDX.Direct2D.Bitmap(renderTarget, new Size(frameWidth, frameHeight),
                    stream, bytesPerStride, props);

            }
            catch
            {
                result = false;
            }

            return result;
        }
    }
}