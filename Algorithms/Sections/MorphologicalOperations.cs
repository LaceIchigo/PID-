using Emgu.CV;
using Emgu.CV.Structure;

namespace Algorithms.Sections
{
    public class MorphologicalOperations
    {

        public static Image<Gray, byte> MorphologicalGradient(Image<Gray, byte> grayImage, int h, int w)
        {
            
            Image<Gray, byte> dilated = GrayDilate(grayImage, h, w);

          
            Image<Gray, byte> eroded = GrayErode(grayImage, h, w);

         
            Image<Gray, byte> gradient = new Image<Gray, byte>(grayImage.Size);

            
            for (int y = 0; y < gradient.Height; y++)
            {
                for (int x = 0; x < gradient.Width; x++)
                {
                    int dilatedValue = dilated.Data[y, x, 0];
                    int erodedValue = eroded.Data[y, x, 0];
                    int gradientValue = dilatedValue - erodedValue;

                 
                    if (gradientValue < 0) gradientValue = 0;
                    if (gradientValue > 255) gradientValue = 255;

                    gradient.Data[y, x, 0] = (byte)gradientValue;
                }
            }

            return gradient;
        }


        public static Image<Gray, byte> GrayDilate(Image<Gray, byte> grayImage, int h, int w)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(grayImage.Size);
            Image<Gray, byte> bordered = Filters.Border(grayImage, h, w);

            int halfH = h / 2;
            int halfW = w / 2;

            int yStart = halfH;
            int yEnd = grayImage.Height + halfH;

            int xStart = halfW;
            int xEnd = grayImage.Width + halfW;

            for (int y = yStart; y < yEnd; y++)
            {
                for (int x = xStart; x < xEnd; x++)
                {
                    byte maxValue = 0;

                  
                    for (int j = -halfH; j <= halfH; j++)
                    {
                        for (int i = -halfW; i <= halfW; i++)
                        {
                            byte pixelValue = bordered.Data[y + j, x + i, 0];
                            if (pixelValue > maxValue)
                            {
                                maxValue = pixelValue;
                            }
                        }
                    }

                    result.Data[y - halfH, x - halfW, 0] = maxValue;
                }
            }

            return result;
        }

        public static Image<Gray, byte> GrayErode(Image<Gray, byte> grayImage, int h, int w)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(grayImage.Size);
            Image<Gray, byte> bordered = Filters.Border(grayImage, h, w);

            int halfH = h / 2;
            int halfW = w / 2;

            int yStart = halfH;
            int yEnd = grayImage.Height + halfH;

            int xStart = halfW;
            int xEnd = grayImage.Width + halfW;

            for (int y = yStart; y < yEnd; y++)
            {
                for (int x = xStart; x < xEnd; x++)
                {
                    byte minValue = 255;


                    for (int j = -halfH; j <= halfH; j++)
                    {
                        for (int i = -halfW; i <= halfW; i++)
                        {
                            byte pixelValue = bordered.Data[y + j, x + i, 0];
                            if (pixelValue < minValue)
                            {
                                minValue = pixelValue;
                            }
                        }
                    }

                    result.Data[y - halfH, x - halfW, 0] = minValue;
                }
            }

            return result;
        }


        public static Image<Gray, byte> Dilate(Image<Gray, byte> binaryImage, bool option, int h, int w)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(binaryImage.Size);
            Image<Gray, byte> bordered = Filters.Border(binaryImage, h, w);

            int halfH = h / 2;
            int halfW = w / 2;

            int yStart = halfH;
            int yEnd = binaryImage.Height + halfH;

            int xStart = halfW;
            int xEnd = binaryImage.Width + halfW;

            int defaultColor = option ? 0 : 255;
            int dilateColor = option ? 255 : 0;

            for (int y = yStart; y < yEnd; y++)
                for (int x = xStart; x < xEnd; x++)
                {
                    result.Data[y - halfH, x - halfW, 0] = Tools.Tools.ClipPixel(defaultColor);

                    for (int j = -halfH; j <= halfH; j++)
                    {
                        for (int i = -halfW; i <= halfW; i++)
                        {
                            int pixel_color = bordered.Data[y + j, x + i, 0];
                            if (pixel_color == dilateColor)
                            {
                                result.Data[y - halfH, x - halfW, 0] = Tools.Tools.ClipPixel(dilateColor);
                                break;
                            }
                        }
                    }
                }

            return result;
        }

        public static Image<Gray, byte> Erode(Image<Gray, byte> binaryImage, bool option, int h, int w)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(binaryImage.Size);
            Image<Gray, byte> bordered = Filters.Border(binaryImage, h, w);

            int halfH = h / 2;
            int halfW = w / 2;
            int yStart = halfH;
            int yEnd = binaryImage.Height + halfH;

            int xStart = halfW;
            int xEnd = binaryImage.Width + halfW;

            int defaultColor = option ? 255 : 0;
            int erodeColor = option ? 0 : 255;

            for (int y = yStart; y < yEnd; y++)
                for (int x = xStart; x < xEnd; x++)
                {
                    result.Data[y - halfH, x - halfW, 0] = Tools.Tools.ClipPixel(defaultColor);
                    for (int j = -halfH; j <= halfH; j++)
                    {
                        for (int i = -halfW; i <= halfW; i++)
                        {
                            int pixel_color = bordered.Data[y + j, x + i, 0];
                            if (pixel_color == erodeColor)
                            {
                                result.Data[y - halfH, x - halfW, 0] = Tools.Tools.ClipPixel(erodeColor);
                                break;
                            }
                        }
                    }
                }
            return result;
        }

        public static Image<Gray, byte> Opening(Image<Gray, byte> binaryImage, bool option, int h, int w)
        {
            Image<Gray, byte> eroded = Erode(binaryImage, option, h, w);
            Image<Gray, byte> opened = Dilate(eroded, option, h, w);
            return opened;
        }

        public static Image<Gray, byte> Closing(Image<Gray, byte> binaryImage, bool option, int h, int w)
        {
            Image<Gray, byte> dilated = Dilate(binaryImage, option, h, w);
            Image<Gray, byte> closed = Erode(dilated, option, h, w);
            return closed;
        }


        public static Image<Gray, byte> ApplyMorph(Image<Gray, byte> grayImage, string morphType, int t, bool option, int h, int w)
        {
            Image<Gray, byte> binaryImage = Tools.Tools.Binary(grayImage, t);

            switch (morphType.ToLower())
            {
                case "dilate":
                    return Dilate(binaryImage, option, h, w);
                case "erode":
                    return Erode(binaryImage, option, h, w);
                case "opening":
                    return Opening(binaryImage, option, h, w);
                case "closing":
                    return Closing(binaryImage, option, h, w);
                case "gradient":
                    return MorphologicalGradient(grayImage, h, w);
            }

            return binaryImage;
        }


        public static double ComputeEuclidianColor(int b, int g, int r)
        {
            return System.Math.Sqrt(b * b + g * g + r * r);
        }


        public static Image<Bgr, byte> Dilate(Image<Bgr, byte> binaryImage, int h, int w)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(binaryImage.Size);
            Image<Bgr, byte> bordered = Filters.Border(binaryImage, h, w);

            int halfH = h / 2;
            int halfW = w / 2;

            int yStart = halfH;
            int yEnd = binaryImage.Height + halfH;

            int xStart = halfW;
            int xEnd = binaryImage.Width + halfW;


            for (int y = yStart; y < yEnd; y++)
                for (int x = xStart; x < xEnd; x++)
                {
                    double maxDist = -1;
                    int xMax = x, yMax = y;

                    for (int j = -halfH; j <= halfH; j++)
                    {
                        for (int i = -halfW; i <= halfW; i++)
                        {
                            int b = bordered.Data[y + j, x + i, 0];
                            int g = bordered.Data[y + j, x + i, 1];
                            int r = bordered.Data[y + j, x + i, 2];

                            double euclid_dist = ComputeEuclidianColor(b, g, r);

                            if (euclid_dist > maxDist)
                            {
                                maxDist = euclid_dist;
                                yMax = y + j;
                                xMax = x + i;
                                break;
                            }
                        }
                    }

                    result.Data[y - halfH, x - halfW, 0] = bordered.Data[yMax, xMax, 0];
                    result.Data[y - halfH, x - halfW, 1] = bordered.Data[yMax, xMax, 1];
                    result.Data[y - halfH, x - halfW, 2] = bordered.Data[yMax, xMax, 2];
                }

            return result;
        }

        public static Image<Bgr, byte> Erode(Image<Bgr, byte> binaryImage, int h, int w)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(binaryImage.Size);
            Image<Bgr, byte> bordered = Filters.Border(binaryImage, h, w);

            int halfH = h / 2;
            int halfW = w / 2;

            int yStart = halfH;
            int yEnd = binaryImage.Height + halfH;

            int xStart = halfW;
            int xEnd = binaryImage.Width + halfW;


            for (int y = yStart; y < yEnd; y++)
                for (int x = xStart; x < xEnd; x++)
                {
                    double minDist = 1000;
                    int xMax = x, yMax = y;

                    for (int j = -halfH; j <= halfH; j++)
                    {
                        for (int i = -halfW; i <= halfW; i++)
                        {
                            int b = bordered.Data[y + j, x + i, 0];
                            int g = bordered.Data[y + j, x + i, 1];
                            int r = bordered.Data[y + j, x + i, 2];

                            double euclid_dist = ComputeEuclidianColor(b, g, r);

                            if (euclid_dist < minDist)
                            {
                                minDist = euclid_dist;
                                yMax = y + j;
                                xMax = x + i;
                                break;
                            }
                        }
                    }

                    result.Data[y - halfH, x - halfW, 0] = bordered.Data[yMax, xMax, 0];
                    result.Data[y - halfH, x - halfW, 1] = bordered.Data[yMax, xMax, 1];
                    result.Data[y - halfH, x - halfW, 2] = bordered.Data[yMax, xMax, 2];
                }

            return result;
        }

        public static Image<Bgr, byte> Opening(Image<Bgr, byte> colorImage, int h, int w)
        {
            Image<Bgr, byte> eroded = Erode(colorImage, h, w);
            Image<Bgr, byte> opened = Dilate(eroded, h, w);
            return opened;
        }

        public static Image<Bgr, byte> Closing(Image<Bgr, byte> colorImage, int h, int w)
        {
            Image<Bgr, byte> dilated = Dilate(colorImage, h, w);
            Image<Bgr, byte> closed = Erode(dilated, h, w);
            return closed;
        }
    }
}