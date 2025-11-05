using Emgu.CV;
using Emgu.CV.Structure;

namespace Algorithms.Tools
{
    public class Tools
    {
        #region Copy
        public static Image<Gray, byte> Copy(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = inputImage.Clone();
            return result;
        }

        public static Image<Bgr, byte> Copy(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = inputImage.Clone();
            return result;
        }
        #endregion

        #region Invert
        public static Image<Gray, byte> Invert(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                }
            }
            return result;
        }

        public static Image<Bgr, byte> Invert(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                    result.Data[y, x, 1] = (byte)(255 - inputImage.Data[y, x, 1]);
                    result.Data[y, x, 2] = (byte)(255 - inputImage.Data[y, x, 2]);
                }
            }
            return result;
        }
        #endregion

        #region Convert color image to grayscale image
        public static Image<Gray, byte> Convert(Image<Bgr, byte> inputImage)
        {
            Image<Gray, byte> result = inputImage.Convert<Gray, byte>();
            return result;
        }
        #endregion
        
        #region Binary

        public static Image<Gray, byte> Binary(Image<Gray, byte> inputImage, int treshold)
        {
            Image<Gray, byte> result = inputImage.Convert<Gray, byte>();
            
            for (int x = 0; x < inputImage.Height; x++)
            {
                for (int y = 0; y < inputImage.Width; y++)
                {
                    Gray pixel = inputImage[x, y];

                    if (pixel.Intensity <= treshold)
                        result[x, y] = new Gray(255);
                    else
                    {
                        result[x, y] = new Gray(0);
                    }
                }
            }
                    
            
            return result;
        }
        
        #endregion
        
        #region MirrorImage

        public static Image<Gray, byte> Mirror(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = inputImage.Convert<Gray, byte>();

            for (int x = 0; x < inputImage.Height; x++)
            {
                for (int y = 0; y < inputImage.Width; y++)
                {
                    result.Data[x, y, 0] = inputImage.Data[x, inputImage.Width - 1 - y, 0];
                }
            }

            return result;
        }
        
        public static Image<Bgr, byte> Mirror(Image<Bgr, byte> inputImage)
        {
               Image<Bgr, byte> result = inputImage.Convert<Bgr, byte>();
               for (int x = 0; x < inputImage.Height; x++)
               {
                   for (int y = 0; y < inputImage.Width; y++)
                   {
                       for (int z = 0; z < inputImage.NumberOfChannels; z++)
                       {
                           result.Data[x, y, z] = inputImage.Data[x, inputImage.Width - 1 - y, z];
                       }
                   }
               }
               return result;
        }
        #endregion
        
        #region RotateImage

        public static Image<Bgr, byte> RotateClockwise(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Height, inputImage.Width);

            for (int x = 0; x < inputImage.Height; x++)
            {
                for (int y = 0; y < inputImage.Width; y++)
                {
                    result[y, inputImage.Height - 1 - x] = inputImage[x, y];
                }
            }
            
            return result;
        }

        public static Image<Gray, byte> RotateClockwise(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Height, inputImage.Width);
            
            for (int x = 0; x < inputImage.Height; x++)
            {
                for (int y = 0; y < inputImage.Width; y++)
                {
                    result[y, inputImage.Height - 1 - x] = inputImage[x, y];
                }
            }
            
            return result;
        }

        public static Image<Bgr, byte> RotateCounterClockwise(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Height, inputImage.Width);

            for (int x = 0; x < inputImage.Height; x++)
            {
                for (int y = 0; y < inputImage.Width; y++)
                {
                    result[inputImage.Width - 1 - y, x] = inputImage[x, y];
                }
            }
            
            return result;
        }
        
        public static Image<Gray, byte> RotateCounterClockwise(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Height, inputImage.Width);
            
            for (int x = 0; x < inputImage.Height; x++)
            {
                for (int y = 0; y < inputImage.Width; y++)
                {
                    result[inputImage.Width - 1 - y, x] = inputImage[x, y];
                }
            }
            
            return result;
        }

        #endregion

        public static byte ClipPixel(float value)
        {
            if (value > 255)
                return 255;
            if (value < 0)
                return 0;
            return (byte)(value + .5);
        }
    }
}