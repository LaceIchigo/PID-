using Emgu.CV;
using Emgu.CV.Structure;
using System;

namespace Algorithms.Sections
{
    public class PointwiseOperations
    {
        private static byte[] CreateLut(double alpha, double beta)
        {
            byte[] lut = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                double value = alpha * i + beta;
                if (value > 255)
                {
                    value = 255;
                }
                else if (value < 0)
                {
                    value = 0;
                }

                lut[i] = (byte)value;
            }

            return lut;
        }

        public static Image<Gray, byte> Brightness(Image<Gray, byte> inputImage, double alpha, double beta)
        {
            byte[] lut = CreateLut(alpha, beta);
            Image<Gray, byte> result = inputImage.Clone();

            for (int y = 0; y < result.Height; y++)
            {
                for (int x = 0; x < result.Width; x++)
                {
                    byte originalValue = result.Data[y, x, 0];
                    result.Data[y, x, 0] = lut[originalValue];
                }
            }

            return result;
        }

        public static Image<Bgr, byte> Brightness(Image<Bgr, byte> inputImage, double alpha, double beta)
        {
            byte[] lut = CreateLut(alpha, beta);
            Image<Bgr, byte> result = inputImage.Clone();

            for (int y = 0; y < result.Height; y++)
            {
                for (int x = 0; x < result.Width; x++)
                {
                    // Blue channel
                    byte originalBlue = result.Data[y, x, 0];
                    result.Data[y, x, 0] = lut[originalBlue];

                    // Green channel
                    byte originalGreen = result.Data[y, x, 1];
                    result.Data[y, x, 1] = lut[originalGreen];

                    // Red channel
                    byte originalRed = result.Data[y, x, 2];
                    result.Data[y, x, 2] = lut[originalRed];
                }
            }

            return result;
        }

        public static Image<Gray, byte> GammaCorrection(Image<Gray, byte> inputImage, double gamma)
        {
            byte[] lut = CreateGammaLut(gamma);
            Image<Gray, byte> result = inputImage.Clone();

            for (int y = 0; y < result.Height; y++)
            {
                for (int x = 0; x < result.Width; x++)
                {
                    byte originalValue = result.Data[y, x, 0];
                    result.Data[y, x, 0] = lut[originalValue];
                }
            }

            return result;
        }

        public static Image<Bgr, byte> GammaCorrection(Image<Bgr, byte> inputImage, double gamma)
        {
            byte[] lut = CreateGammaLut(gamma);
            Image<Bgr, byte> result = inputImage.Clone();

            for (int y = 0; y < result.Height; y++)
            {
                for (int x = 0; x < result.Width; x++)
                {
                    result.Data[y, x, 0] = lut[result.Data[y, x, 0]]; // Blue
                    result.Data[y, x, 1] = lut[result.Data[y, x, 1]]; // Green
                    result.Data[y, x, 2] = lut[result.Data[y, x, 2]]; // Red
                }
            }

            return result;
        }

        private static byte[] CreateGammaLut(double gamma)
        {
            byte[] lut = new byte[256];
            double c = Math.Pow(255, 1 - gamma);

            for (int i = 0; i < 256; i++)
            {
                double value = c * Math.Pow(i, gamma);

                if (value > 255)
                {
                    value = 255;
                }
                else if (value < 0)
                {
                    value = 0;
                }

                lut[i] = (byte)value;
            }

            return lut;
        }

        public static int[] Histogram(Image<Gray, byte> inputImage)
        {
            int[] histogram = new int[256];
            
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    int i = inputImage.Data[y, x, 0];
                    histogram[i]++;
                }
            }
            
            return histogram;
        }
    }
}