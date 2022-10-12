using UnityEngine;
using System.Collections;

public static class FalloffGenerator
{

    public static float[,] GenerateFalloffMap(int size)
    {
        float[,] map = new float[size, size];

        Vector2 center = new Vector2(size / 2f, size / 2f);

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float DistanceFromCenter = Vector2.Distance(center, new Vector2(i, j));
                float currentAlpha = 1;

                if ((1 - (DistanceFromCenter / size)) >= 0)
                {
                    currentAlpha = (1 - (DistanceFromCenter / size));
                }
                else
                {
                    currentAlpha = 0;
                }
                map[i, j] = Evaluate(currentAlpha, 3, 2.2f);
            }
        }
        return map;
    }
    static float Evaluate(float value, float av, float bv)
    {
        float a = av;
        float b = bv;

        return Mathf.Pow(value, -a) / (Mathf.Pow(value, -a) + Mathf.Pow(b - b * value, -a));
    }
}
