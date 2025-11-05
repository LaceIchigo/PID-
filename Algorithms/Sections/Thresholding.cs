using Emgu.CV;
using Emgu.CV.Structure;

using Math = System.Math;

namespace Algorithms.Sections
{
    public class Thresholding
    {
        public static int MinErrTreshold(Image<Gray, byte> inputImage)
        {
            int T = 0;
            
            double minErr = double.MaxValue;
            
            int[] histogram = PointwiseOperations.Histogram(inputImage);
            
            double n = inputImage.Height * inputImage.Width;

            double[] p = new double[256];
            for (int k = 0; k <= 255; ++k)
                p[k] = histogram[k] / n;
            
            
            for (int t = 1; t <= 254; t++)
            {
                double P1 = 0, P2 = 0, u1 = 0, u2 = 0, s1 = 0, s2 = 0;
                
                for (int k = 0; k <= t; k++)
                    P1 += p[k];
                
                P2 = 1 - P1;
                
                if(P1 == 0 || P2 == 0)
                    continue;
                

                for (int k = 0; k <= t; k++)
                    u1 += (k * p[k]) / P1;
                
                for (int k = 0; k <= t; k++)
                    s1 += (k - u1) * (k - u1) * p[k] / P1;
                
                for(int j = t + 1; j <= 255; j++)
                    u2 += (j * p[j])  / P2;
                
                for(int j = t + 1; j <= 255; j++)
                    s2 += (j - u2) * (j - u2) * p[j] / P2;
                
                if (s1 <= 0 || s2 <= 0)
                    continue;
                
                double err = 1 + P1 * Math.Log(s1) + P2 * Math.Log(s2)
                             - 2 * P1 * Math.Log(P1) - 2 * P2 * Math.Log(P2);

                if (err < minErr)
                {
                    minErr = err;
                    T = t;
                }
            }
            
            return T;
        }
    }
}