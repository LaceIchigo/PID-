using System;
using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Algorithms.Sections
{
    public class Segmentation
    {
        public static Image<Gray, byte> Hough3Quadrants(Image<Gray, byte> edges,
            out List<Tuple<Tuple<int, int>, Tuple<int, int>>> lineEndpoints)
        {
            int width = edges.Width;
            int height = edges.Height;

            int rhoMax = (int)Math.Sqrt(width * width + height * height);
            int rhoBins = rhoMax;
            int thetaBins = 271;

            int[,] accumulator = new int[rhoBins, thetaBins];

            double[] cosTable = new double[thetaBins];
            double[] sinTable = new double[thetaBins];

            for (int t = 0; t < thetaBins; t++)
            {
                double theta = t * Math.PI / 180.0;
                cosTable[t] = Math.Cos(theta);
                sinTable[t] = Math.Sin(theta);
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (edges.Data[y, x, 0] == 0) continue;

                    for (int t = 0; t < thetaBins; t++)
                    {
                        int rho = (int)Math.Round(x * cosTable[t] + y * sinTable[t]);
                        if (rho <= 0 || rho > rhoMax) continue;
                        accumulator[rho - 1, t]++;
                    }
                }
            }

            int maxVotes = FindMaxVotes(accumulator, rhoBins, thetaBins);
            int threshold = Math.Max(1, (int)(0.5 * maxVotes));

            bool[,] suppressed = new bool[rhoBins, thetaBins];
            lineEndpoints = new List<Tuple<Tuple<int, int>, Tuple<int, int>>>();

            for (int r = 0; r < rhoBins; r++)
            {
                for (int t = 0; t < thetaBins; t++)
                {
                    int v = accumulator[r, t];
                    if (v < threshold || suppressed[r, t]) continue;

                    bool isMax = IsLocalMaximum(accumulator, rhoBins, thetaBins, r, t, v);
                    if (!isMax) continue;

                    SuppressNeighbors(suppressed, rhoBins, thetaBins, r, t);

                    int rhoVal = r + 1;
                    double thetaVal = t * Math.PI / 180.0;
                    var pts = FindLineIntersections(width, height, rhoVal, thetaVal);
                    if (pts != null)
                        lineEndpoints.Add(pts);
                }
            }

            return CreateAccumulatorVisualization(accumulator, rhoBins, thetaBins, maxVotes);
        }

        private static int FindMaxVotes(int[,] accumulator, int rhoBins, int thetaBins)
        {
            int maxVotes = 0;
            for (int r = 0; r < rhoBins; r++)
            {
                for (int t = 0; t < thetaBins; t++)
                {
                    if (accumulator[r, t] > maxVotes)
                        maxVotes = accumulator[r, t];
                }
            }
            return maxVotes;
        }

        private static bool IsLocalMaximum(int[,] accumulator, int rhoBins, int thetaBins,
            int r, int t, int currentValue)
        {
            for (int dr = -1; dr <= 1; dr++)
            {
                int rr = r + dr;
                if (rr < 0 || rr >= rhoBins) continue;

                for (int dt = -1; dt <= 1; dt++)
                {
                    int tt = t + dt;
                    if (tt < 0 || tt >= thetaBins) continue;
                    if (dr == 0 && dt == 0) continue;

                    if (accumulator[rr, tt] > currentValue)
                        return false;
                }
            }
            return true;
        }

        private static void SuppressNeighbors(bool[,] suppressed, int rhoBins, int thetaBins, int r, int t)
        {
            for (int dr = -4; dr <= 4; dr++)
            {
                int rr = r + dr;
                if (rr < 0 || rr >= rhoBins) continue;

                for (int dt = -4; dt <= 4; dt++)
                {
                    int tt = t + dt;
                    if (tt < 0 || tt >= thetaBins) continue;
                    suppressed[rr, tt] = true;
                }
            }
        }

        private static Tuple<Tuple<int, int>, Tuple<int, int>> FindLineIntersections(int w, int h, int rho, double theta)
        {
            double cos = Math.Cos(theta);
            double sin = Math.Sin(theta);

            List<Tuple<int, int>> intersectionPoints = new List<Tuple<int, int>>();

            if (Math.Abs(sin) > 1e-6)
            {
                AddValidPoint(intersectionPoints, 0, (int)Math.Round(rho / sin), 0, h, w);
                AddValidPoint(intersectionPoints, w - 1, (int)Math.Round((rho - (w - 1) * cos) / sin), 0, h, w);
            }

            if (Math.Abs(cos) > 1e-6)
            {
                AddValidPoint(intersectionPoints, (int)Math.Round(rho / cos), 0, 0, w, h);
                AddValidPoint(intersectionPoints, (int)Math.Round((rho - (h - 1) * sin) / cos), h - 1, 0, w, h);
            }

            if (intersectionPoints.Count < 2) return null;

            return FindFarthestPoints(intersectionPoints);
        }

        private static void AddValidPoint(List<Tuple<int, int>> points, int x, int y, int min, int max, int otherLimit)
        {
            if (y >= min && y < max && x >= 0 && x < otherLimit)
                points.Add(Tuple.Create(x, y));
        }

        private static Tuple<Tuple<int, int>, Tuple<int, int>> FindFarthestPoints(List<Tuple<int, int>> points)
        {
            double maxDistance = -1;
            Tuple<int, int> pointA = null;
            Tuple<int, int> pointB = null;

            for (int i = 0; i < points.Count; i++)
            {
                for (int j = i + 1; j < points.Count; j++)
                {
                    double dx = points[i].Item1 - points[j].Item1;
                    double dy = points[i].Item2 - points[j].Item2;
                    double distance = dx * dx + dy * dy;

                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        pointA = points[i];
                        pointB = points[j];
                    }
                }
            }

            return Tuple.Create(pointA, pointB);
        }

        private static Image<Gray, byte> CreateAccumulatorVisualization(int[,] accumulator,
            int rhoBins, int thetaBins, int maxVotes)
        {
            Image<Gray, byte> accVis = new Image<Gray, byte>(thetaBins, rhoBins);
            double scale = maxVotes > 0 ? 255.0 / maxVotes : 0.0;

            for (int r = 0; r < rhoBins; r++)
            {
                for (int t = 0; t < thetaBins; t++)
                {
                    int val = (int)(accumulator[r, t] * scale);
                    if (val > 255) val = 255;
                    accVis.Data[r, t, 0] = (byte)val;
                }
            }

            return accVis;
        }
    }
}