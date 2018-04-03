﻿using System;
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
            _kernel = new float[radius * 2 + 1, radius * 2 + 1];
        }

        public float[] SelectivelyBlurData(float[] data, int[] map, int width, int height)
        {
            float[] blurred = new float[data.Length];

            for (int y = _radius; y < height - _radius; y++)
            {
                for (int x = _radius; x < width - _radius; x++)
                {
                    int index = y * width + x; 

                    if(map[index] == 1)
                    {
                        float value = 0.0f;
                        for (int y1 = y - _radius, y2 = 0; y1 < y + _radius; y1++, y2++)
                        {
                            for (int x1 = x - _radius, x2 = 0; x1 < x + _radius; x1++, x2++)
                            {
                                float scaler = _kernel[x2, y2];
                                value += data[y1 * width + x1] * scaler;
                            }
                        }
                        blurred[index] = value;
                    }
                }
            }

            return blurred;
        }

        public float[] BlurData(float[] data, int width, int height)
        {
            float[] blurred = new float[data.Length];

            for (int y = _radius; y < height - _radius; y++)
            {
                for (int x = _radius; x < width - _radius; x++)
                {
                    float value = 0.0f;
                    for (int y1 = y - _radius, y2 = 0; y1 < y + _radius; y1++, y2++)
                    {
                        for (int x1 = x - _radius, x2 = 0; x1 < x + _radius; x1++, x2++)
                        {
                            float scaler = _kernel[x2, y2];
                            value += data[y1 * width + x1] * scaler;
                        }
                    }
                    blurred[y * width + x] = value;
                }
            }

            return blurred;
        }

        public static GaussianBlur BuildBlur(float sigma, int radius)
        {
            GaussianBlur blur = new GaussianBlur(sigma, radius);

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
