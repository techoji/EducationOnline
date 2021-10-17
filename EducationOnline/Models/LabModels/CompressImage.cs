using System;
using System.Drawing;
using System.Threading.Tasks;

namespace DistanceLearning.Models {
    public class CompressImage : IDisposable {

        public int CompressionRatio;

        public event Action<CompressingArgs> OnCompressing;
        public event Action OnCompressStart;
        public event Action OnCompressEnd;

        private int Width;
        private int Height;
        private Bitmap Matrix;
        public Bitmap OutputImage;
        private Bitmap InputImage;
        private CompressingArgs args;

        public CompressImage(Bitmap Image, int RootWidth, int OutputWidth) {
            InputImage = Image;
            Height = Image.Height;
            Width = Image.Width;
            CompressionRatio = (int)Math.Ceiling(double.Parse(((double)RootWidth / OutputWidth).ToString()));
            if (CompressionRatio <= 1)
                OutputImage = Image;
            else {
                OutputImage = new Bitmap((int)Math.Ceiling((double)Width / CompressionRatio), (int)Math.Ceiling((double)Height / CompressionRatio));
                Matrix = new Bitmap(CompressionRatio, CompressionRatio);
            }
        }

        public void Resize() {
            OnCompressStart?.Invoke();

            if (CompressionRatio <= 1) {
                OnCompressEnd?.Invoke();
                return;
            }

            int widthDif = Width - CompressionRatio;
            int heightDif = Height - CompressionRatio;

            int widthEdge = Width - widthDif;
            int heightEdge = Height - heightDif;

            Bitmap crop;
            CompressingArgs args = new CompressingArgs(Height * Width);
            Rectangle rect = new Rectangle(0, 0, CompressionRatio, CompressionRatio);
            int LastPercent = 0;

            for (int y = 0, outputY = 0; y < Height; y += CompressionRatio, outputY++) {
                for (int x = 0, outputX = 0; x < Width; x += CompressionRatio, outputX++) {
                    if (x > widthDif || y > heightDif) {
                        rect.X = widthDif;
                        rect.Y = heightDif;
                        rect.Width = widthEdge;
                        rect.Height = heightEdge;
                        crop = InputImage.Clone(rect, InputImage.PixelFormat);
                    }
                    else {
                        rect.X = x;
                        rect.Y = y;
                        crop = InputImage.Clone(rect, InputImage.PixelFormat);
                    }
                    OutputImage.SetPixel(outputX, outputY, getMiddlePixel(crop));
                }
                args.CompressingPixels = (y + 1) * (long)Width;
                if (args.CompressingPercent % 5 == 0 && LastPercent != args.CompressingPercent) {
                    OnCompressing?.Invoke(args);
                    LastPercent = args.CompressingPercent;
                }
            }

            OnCompressEnd?.Invoke();
        }

        private int GetСompressionRatio(Bitmap input, int dpi) => (int)input.VerticalResolution / dpi;

        private Color getMiddlePixel(Bitmap crop) {
            int midRed = 0;
            int midGreen = 0;
            int midBlue = 0;

            int countOfElements = CompressionRatio * CompressionRatio;

            for (int y = 0; y < CompressionRatio; y++) {
                for (int x = 0; x < CompressionRatio; x++) {
                    midBlue += crop.GetPixel(x, y).B;
                    midGreen += crop.GetPixel(x, y).G;
                    midRed += crop.GetPixel(x, y).R;
                }
            }

            midBlue /= countOfElements;
            midRed /= countOfElements;
            midGreen /= countOfElements;

            return Color.FromArgb(255, midRed, midGreen, midBlue);
        }

        public void Dispose() {
            OutputImage.Dispose();
            InputImage.Dispose();
            if (Matrix != null)
                Matrix.Dispose();
        }
    }

    public class CompressingArgs {
        public long Total { get; }
        public int CompressingPercent => (int)(CompressingPixels * 100 / Total);
        public long CompressingPixels { get; set; }

        public CompressingArgs(int Total) => this.Total = Total;
    }
}