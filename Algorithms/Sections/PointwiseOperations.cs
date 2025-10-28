using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Algorithms.Sections
{
    public class PointwiseOperations
    {
        #region Change Contrast and Brightness
        public static Image<Gray, byte> ApplyLUT(Image<Gray, byte> inputImage, byte[] LUT)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = LUT[inputImage.Data[y, x, 0]];
                }
            }
            return result;
        }

        public static byte[] ContrastBrightnessLinear(double a, double b)
        {
            byte[] LUT = new byte[256]; // look up table

            for (int r = 0; r < 256; ++r)
            {
                double result = a * r + b;

                LUT[r] = Clip(result);
            }

            return LUT;
        }

        public static byte[] ContrastBrightnessGamma(double gamma)
        {
            byte[] LUT = new byte[256]; // look up table
            double c = Math.Pow(255, 1 - gamma);

            for (int r = 0; r < 256; ++r)
            {
                double result = c * Math.Pow(r, gamma);
                LUT[r] = Clip(result);
            }

            return LUT;
        }
        #endregion

        #region Normalize histogram
        public static byte[] NormalizeHistogram(byte[] histogram)
        {
            double[] probability = CalculateProbabilities(histogram);
            double[] cumulativeProb = CumulativeProbabilities(probability);
            byte[] equalizedHistogram = EqualizeHistogram(cumulativeProb);

            return equalizedHistogram;
        }

        private static double[] CalculateProbabilities(byte[] histogram)
        {
            double[] probability = new double[histogram.Length];
            int numOfPixels = histogram.Sum(v => (int)v);

            for (int i=0; i < probability.Length; ++i)
            {
                probability[i] = (double)histogram[i] / numOfPixels;
            }

            return probability;
        }


        private static double[] CumulativeProbabilities(double[] probability)
        {
            double[] cumulProb = new double[probability.Length];

            if(probability.Length > 0)
                cumulProb[0] = probability[0];

            for (int i = 1; i < probability.Length; ++i)
            {
                cumulProb[i] = cumulProb[i - 1] + probability[i];
            }

            return cumulProb;
        }

        private static byte[] EqualizeHistogram(double[] cumulProbability)
        {
            byte[] transformedHistogram = new byte[256];

            for(int i = 0; i < 256; ++i)
            {
                transformedHistogram[i] = (byte)(255 * cumulProbability[i]);
            }

            return transformedHistogram;
        }
        #endregion

        #region Clipping values
        private static byte Clip(double value)
        {
            if (value < 0)
                return 0;
            if (value > 255)
                return 255;
            value += 0.5;
            return (byte)value;
        }
        #endregion
    }
}