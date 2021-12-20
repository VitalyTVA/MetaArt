using SkiaSharp;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MetaArt.Wpf {
    sealed class Painter : PainterBase {

        WriteableBitmap? bitmap;
        SKSurface? surface;
        public IntPtr ptr;
        public WriteableBitmap Bitmap {
            get {
                if(bitmap == null) {
                    bitmap = new WriteableBitmap(Width, Height, 96, 96, PixelFormats.Bgra32, BitmapPalettes.Halftone256Transparent);
                    bitmap.Lock();
                    ptr = bitmap.BackBuffer;
                }
                return bitmap;
            }
        }
        SKSurface SKSurface {
            get {
                var bmp = Bitmap;
                if(surface == null) {
                    var imageInfo = new SKImageInfo(
                        width: Width,
                        height: Height,
                        colorType: SKColorType.Bgra8888,
                        alphaType: SKAlphaType.Premul);
                    surface = SKSurface.Create(info: imageInfo, pixels: ptr, rowBytes: imageInfo.Width * 4);
                }
                return surface;
            }
        }

        public Painter(SketchBase sketch) : base(sketch) {
        }

        void ClearSurface() {
            if(surface != null) {
                surface.Dispose();
                surface = null;
            }
            ptr = IntPtr.Zero;
        }

        public void Setup() {
            SetupCore();
            var bmp = Bitmap;
            Bitmap.AddDirtyRect(new Int32Rect(0, 0, Width, Height));
            Bitmap.Unlock();
            ClearSurface();
        }

        public void Draw() {
            DrawCore();
            ClearSurface();
        }

        public override SKCanvas Canvas => SKSurface.Canvas;
        public override void SetSize(int width, int height) {
            if(bitmap != null)
                throw new InvalidOperationException();
            base.SetSize(width, height);
        }
    }
}
