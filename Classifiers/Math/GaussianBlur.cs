using System;
using System.Collections.Generic;
using System.Text;

namespace Maths
{
    public class GaussianBlur
    {
        private int _radius;
        private float _sigma;
        private float[,] _kernel;

        private GaussianBlur(float sigma, int radius)
        {
            _sigma = sigma;
            _radius = radius;
            _kernel = new float[radius, radius];
        }

        public float[] BlurData(float[] data, int width, int height)
        {
            float[] blurred = new float[data.Length];

            return blurred;
        }

        public static GaussianBlur BuildBlur(float mean, int radius)
        {
            GaussianBlur blur = new GaussianBlur(mean, radius);

            double calculatedEuler = 1.0 /
                (2.0 * Math.PI * Math.Pow(blur._sigma, 2));

            float sumTotal = 0.0f;

            for (int y = -blur._radius; y <= blur._radius; y++)
            {
                for (int x = -blur._radius; x < blur._radius; x++)
                {
                    double distance = ((x * x) + (y * y)) /
                       (2 * (blur._sigma * blur._sigma));

                    float v = (float)(calculatedEuler * Math.Exp(-distance));
                    blur._kernel[y + blur._radius, x + blur._radius] = v;
                    sumTotal += v;
                }
            }

            int size = radius * 2;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    blur._kernel[y, x] = (float)(blur._kernel[y, x] * (1.0 / sumTotal));
                }
            }

            return blur;
        }
    }
}
