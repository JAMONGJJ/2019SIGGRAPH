using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.IO;

public class Blur : MonoBehaviour
{
    public static double[,] Calculate1DSampleKernel(double deviation, int size)
    {
        double[,] ret = new double[size, 1];
        double sum = 0;
        int half = size / 2;
        for (int i = 0; i < size; i++)
        {
            ret[i, 0] = 1 / (Math.Sqrt(2 * Math.PI) * deviation) * Math.Exp(-(i - half) * (i - half) / (2 * deviation * deviation));
            sum += ret[i, 0];
        }
        return ret;
    }
    public static double[,] Calculate1DSampleKernel(double deviation)
    {
        int size = (int)Math.Ceiling(deviation * 2) * 2 + 1;
        return Calculate1DSampleKernel(deviation, size);
    }
    public static double[,] CalculateNormalized1DSampleKernel(double deviation)
    {
        return NormalizeMatrix(Calculate1DSampleKernel(deviation));
    }
    public static double[,] NormalizeMatrix(double[,] matrix)
    {
        double[,] ret = new double[matrix.GetLength(0), matrix.GetLength(1)];
        double sum = 0;
        for (int i = 0; i < ret.GetLength(0); i++)
        {
            for (int j = 0; j < ret.GetLength(1); j++)
                sum += matrix[i, j];
        }
        if (sum != 0)
        {
            for (int i = 0; i < ret.GetLength(0); i++)
            {
                for (int j = 0; j < ret.GetLength(1); j++)
                    ret[i, j] = matrix[i, j] / sum;
            }
        }
        return ret;
    }
}
