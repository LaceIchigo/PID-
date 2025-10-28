using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;

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
        public static Image<Gray, byte> BinaryImage(Image<Gray, byte> inputImage, int T = 128)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    if (inputImage.Data[y, x, 0] < T)
                        result.Data[y, x, 0] = (byte)0;
                    else
                        result.Data[y, x, 0] = (byte)255;
                }
            }
            return result;
        }

        #endregion

        #region Mirror
        public static Image<Gray, byte> MirrorImage(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = inputImage.Data[y, inputImage.Width - x - 1, 0];
                }
            }
            return result;
        }

        public static Image<Bgr, byte> MirrorImage(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = inputImage.Data[y, inputImage.Width - x - 1, 0];
                    result.Data[y, x, 1] = inputImage.Data[y, inputImage.Width - x - 1, 1];
                    result.Data[y, x, 2] = inputImage.Data[y, inputImage.Width - x - 1, 2];
                }
            }
            return result;
        }

        #endregion

        #region Rotate right
        public static Image<Gray, byte> RotateRight(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Height, inputImage.Width);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[x, inputImage.Height - y - 1, 0] = inputImage.Data[y, x, 0];
                }
            }
            return result;
        }

        public static Image<Bgr, byte> RotateRight(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Height, inputImage.Width);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[x, inputImage.Height - y - 1, 0] = inputImage.Data[y, x, 0];
                    result.Data[x, inputImage.Height - y - 1, 1] = inputImage.Data[y, x, 1];
                    result.Data[x, inputImage.Height - y - 1, 2] = inputImage.Data[y, x, 2];
                }
            }
            return result;
        }

        #endregion

        #region Rotate left
        public static Image<Gray, byte> RotateLeft(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Height, inputImage.Width);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[inputImage.Width - x - 1, y, 0] = inputImage.Data[y, x, 0];
                }
            }
            return result;
        }

        public static Image<Bgr, byte> RotateLeft(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Height, inputImage.Width);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[inputImage.Width - x - 1, y, 0] = inputImage.Data[y, x, 0];
                    result.Data[inputImage.Width - x - 1, y, 1] = inputImage.Data[y, x, 1];
                    result.Data[inputImage.Width - x - 1, y, 2] = inputImage.Data[y, x, 2];
                }
            }
            return result;
        }

        #endregion

        #region Histogam

        public static List<double> Histogram(Image<Gray, byte> inputImage)
        {

           
            List<double> histogram = new List<double>(new double[256]);

            if (inputImage == null)
                return histogram;

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    byte pixelValue = inputImage.Data[y, x, 0];
                    histogram[pixelValue]++;
                }
            }

            double totalPixels = inputImage.Width * inputImage.Height;
            if (totalPixels > 0)
            {
                for (int i = 0; i < histogram.Count; i++)
                {
                    histogram[i] /= totalPixels;
                }
            }

            return histogram;
        }

        #endregion


       

        public static int MinError(Image<Gray, byte> inputImage)
        {
            List<double> H = Histogram(inputImage);
            int n = inputImage.Width * inputImage.Height;

          
            List<double> p = new List<double>(256);
            for (int k = 0; k <= 255; k++)
            {
                p.Add(H[k]);
            }

            int T = 0;
            double minErr = double.MaxValue;

           
            for (int t = 1; t <= 254; t++)
            {
               
                double P1 = 0.0;
                for (int k = 0; k <= t; k++)
                {
                    P1 += p[k];
                }

              
                double P2 = 1.0 - P1;

              
                if (P1 <= 0.0 || P2 <= 0.0)
                    continue;

             
                double mu1 = 0.0;
                for (int k = 0; k <= t; k++)
                {
                    mu1 += k * p[k];
                }
                mu1 /= P1;

              
                double mu2 = 0.0;
                for (int k = t + 1; k <= 255; k++)
                {
                    mu2 += k * p[k];
                }
                mu2 /= P2;

              
                double s1 = 0.0;
                for (int k = 0; k <= t; k++)
                {
                    double diff = k - mu1;
                    s1 += diff * diff * p[k];
                }
                s1 /= P1;

              
                double s2 = 0.0;
                for (int k = t + 1; k <= 255; k++)
                {
                    double diff = k - mu2;
                    s2 += diff * diff * p[k];
                }
                s2 /= P2;

               
                if (s1 <= 0.0) s1 = 1e-10;
                if (s2 <= 0.0) s2 = 1e-10;

               
                double Err = 1.0 + P1 * Math.Log(s1) + P2 * Math.Log(s2)
                           - 2.0 * P1 * Math.Log(P1) - 2.0 * P2 * Math.Log(P2);

                
                if (Err < minErr)
                {
                    minErr = Err;
                    T = t;
                }
            }

            return T;
        }
    }
}