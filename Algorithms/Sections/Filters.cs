using Algorithms.Utilities;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Algorithms.Sections
{
    public class Filters
    {
        public static Image<Gray, byte> ApplyFilter(Image<Gray, byte> img, float[,] filter)
        {
            Image<Gray, byte> rez  = new Image<Gray, byte>(img.Size);

            int h = filter.GetLength(0);
            int w = filter.GetLength(1);
            
            for (int y = h/2; y < img.Height - h/2 - 1; y++)
                for (int x = w/2; x < img.Width - w/2 - 1; x++)
                {
                    float sum = 0;
                    for (int i = -h/2; i <= h/2; i++)
                        for (int j = -w / 2; j <= w / 2; j++)
                            sum += filter[i + h / 2, j + w / 2] * img.Data[y + i, x + j, 0];
                    rez.Data[y, x, 0] = Utils.ClipPixel(sum);
                }
            return rez;
        }
    }
}