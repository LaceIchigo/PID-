using Algorithms.Utilities;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;

namespace Algorithms.Sections
{
    public class Filters
    {
        public static Image<Gray, byte> Border(Image<Gray, byte> grayImage, int borderH, int borderW)
        {
            int halfBorderW = borderW / 2;
            int halfBorderH = borderH / 2;
            int newWidth = grayImage.Width + borderW;
            int newHeight = grayImage.Height + borderH;

            Image<Gray, byte> result = new Image<Gray, byte>(newWidth, newHeight, new Gray(0));

            for (int y = 0; y < grayImage.Height; y++)
                for (int x = 0; x < grayImage.Width; x++)
                {
                    result.Data[y + halfBorderH, x + halfBorderW, 0] = grayImage.Data[y, x, 0];
                }

            for (int y = 0; y < halfBorderH; y++)
                for (int x = halfBorderW; x < newWidth - halfBorderW; x++)
                {
                    result.Data[y, x, 0] = result.Data[halfBorderH, x, 0];
                }

            for (int y = newHeight - halfBorderH; y < newHeight; y++)
                for (int x = halfBorderW; x < newWidth - halfBorderW; x++)
                {
                    result.Data[y, x, 0] = result.Data[newHeight - halfBorderH - 1, x, 0];
                }

            for (int y = halfBorderH; y < newHeight - halfBorderH; y++)
                for (int x = 0; x < halfBorderW; x++)
                {
                    result.Data[y, x, 0] = result.Data[y, halfBorderW, 0];
                }

            for (int y = halfBorderH; y < newHeight - halfBorderH; y++)
                for (int x = newWidth - halfBorderW; x < newWidth; x++)
                {
                    result.Data[y, x, 0] = result.Data[y, newWidth - halfBorderW - 1, 0];
                }

            for (int y = 0; y < halfBorderH; y++)
                for (int x = 0; x < halfBorderW; x++)
                {
                    result.Data[y, x, 0] = result.Data[halfBorderH, halfBorderW, 0];
                }

            for (int y = newHeight - halfBorderH; y < newHeight; y++)
                for (int x = 0; x < halfBorderW; x++)
                {
                    result.Data[y, x, 0] = result.Data[newHeight - halfBorderH - 1, halfBorderW, 0];
                }

            for (int y = 0; y < halfBorderH; y++)
                for (int x = newWidth - halfBorderW; x < newWidth; x++)
                {
                    result.Data[y, x, 0] = result.Data[halfBorderH, newWidth - halfBorderW - 1, 0];
                }

            for (int y = newHeight - halfBorderH; y < newHeight; y++)
                for (int x = newWidth - halfBorderW; x < newWidth; x++)
                {
                    result.Data[y, x, 0] = result.Data[newHeight - halfBorderH - 1, newWidth - halfBorderW - 1, 0];
                }

            return result;
        }

        public static Image<Gray, byte> ApplyFilter(Image<Gray, byte> grayImage, float[,] kernel)
        {
            int kHeight = kernel.GetLength(0);
            int kWidth = kernel.GetLength(1);

            Image<Gray, byte> result = Border(grayImage, kHeight, kWidth);

            for (int y = 0 + kHeight / 2; y < grayImage.Height - kHeight / 2; y++)
                for (int x = 0 + kWidth / 2; x < grayImage.Width - kWidth / 2; x++)
                {
                    float sum = 0;
                    for (int ky = -kHeight / 2; ky <= kHeight / 2; ky++)
                    {
                        for (int kx = -kWidth / 2; kx <= kWidth / 2; kx++)
                        {
                            int pixelVal = grayImage.Data[y + ky, x + kx, 0];
                            float kernelVal = kernel[ky + kHeight / 2, kx + kWidth / 2];
                            sum += pixelVal * kernelVal;
                        }
                    }
                    result.Data[y, x, 0] = Tools.Tools.ClipPixel(sum);
                }


            return result;
        }

        public static int GetMedianFrHistogram(int[] histogram, int total)
        {
            int k = 0;
            int s = 0;
            int n = total / 2;

            while (k <= 255 && s + histogram[k] <= n)
            {

                s = s + histogram[k];
                k++;

            }

            return k;
        }

        public static Image<Gray, byte> SobelEdgeDetectionWithDirection(
         Image<Gray, byte> grayImage,
         float threshold,
         float targetAngleDegrees,
         float angleTolerance = 22.5f)
        {
           
            float[,] Sx = new float[,] {
                { -1, 0, 1 },
                { -2, 0, 2 },
                { -1, 0, 1 }
            };

            float[,] Sy = new float[,] {
                { -1, -2, -1 },
                {  0,  0,  0 },
                {  1,  2,  1 }
            };

         
            float targetAngleRad = targetAngleDegrees * (float)Math.PI / 180f;

           
            Image<Gray, byte> result = new Image<Gray, byte>(grayImage.Size);

          
            Image<Gray, byte> bordered = Border(grayImage, 3, 3);
            int offset = 1; 

            for (int y = 0; y < grayImage.Height; y++)
            {
                for (int x = 0; x < grayImage.Width; x++)
                {
               
                    float fx = 0;
                    for (int ky = -1; ky <= 1; ky++)
                    {
                        for (int kx = -1; kx <= 1; kx++)
                        {
                            int pixelVal = bordered.Data[y + offset + ky, x + offset + kx, 0];
                            fx += pixelVal * Sx[ky + 1, kx + 1];
                        }
                    }

                  
                    float fy = 0;
                    for (int ky = -1; ky <= 1; ky++)
                    {
                        for (int kx = -1; kx <= 1; kx++)
                        {
                            int pixelVal = bordered.Data[y + offset + ky, x + offset + kx, 0];
                            fy += pixelVal * Sy[ky + 1, kx + 1];
                        }
                    }

                    float gradientMagnitude = (float)Math.Sqrt(fx * fx + fy * fy);

                 
                    if (gradientMagnitude > threshold)
                    {
                        
                        float angle = (float)Math.Atan2(fy, fx);

                        
                        float normalizedAngle = angle;
                        if (normalizedAngle < 0)
                            normalizedAngle += 2 * (float)Math.PI;

                        float normalizedTarget = targetAngleRad;
                        if (normalizedTarget < 0)
                            normalizedTarget += 2 * (float)Math.PI;

                        float angleDiff = Math.Abs(normalizedAngle - normalizedTarget);

                        if (angleDiff > Math.PI)
                            angleDiff = 2 * (float)Math.PI - angleDiff;

                     
                        float toleranceRad = angleTolerance * (float)Math.PI / 180f;

                       
                        if (angleDiff <= toleranceRad)
                        {
                            result.Data[y, x, 0] = 255; 
                        }
                        else
                        {
                            result.Data[y, x, 0] = 0; 
                        }
                    }
                    else
                    {
                        result.Data[y, x, 0] = 0; 
                    }
                }
            }

            return result;
        }





        public static Image<Gray, byte> ApplyMedianFilter(Image<Gray, byte> grayImage, int kSize)
        {
            if (kSize % 2 == 0) kSize++;

            int half = kSize / 2;
            Image<Gray, byte> bordered = Border(grayImage, kSize, kSize);
            Image<Gray, byte> result = new Image<Gray, byte>(grayImage.Size);

            int xStart = half;
            int yStart = half;
            int xEndExclusive = half + grayImage.Width;
            int yEndExclusive = half + grayImage.Height;

            for (int y = yStart; y < yEndExclusive; y++)
            {
                int[] hist = new int[256];
                bool newline = true;

                for (int x = xStart; x < xEndExclusive; x++)
                {
                    if (newline)
                    {
                        for (int ky = -half; ky <= half; ky++)
                            for (int kx = -half; kx <= half; kx++)
                                hist[bordered.Data[y + ky, x + kx, 0]]++;
                        newline = false;
                    }
                    else
                    {
                        for (int ky = -half; ky <= half; ky++)
                            hist[bordered.Data[y + ky, x + half, 0]]++;
                    }

                    int median = GetMedianFrHistogram(hist, kSize * kSize);
                    result.Data[y - half, x - half, 0] = Tools.Tools.ClipPixel(median);

                    for (int ky = -half; ky <= half; ky++)
                        hist[bordered.Data[y + ky, x - half, 0]]--;
                }
            }

            return result;
        }
    }

}



