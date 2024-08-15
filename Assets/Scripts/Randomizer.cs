using System;
using UnityEngine;

public class Randomizer : MonoBehaviour
{
    private int seed;
    private System.Random random;

    /// <summary>
    /// Side effect: Replace the current RNG
    /// </summary>
    /// <param name="seed"></param>
    public void SetSeed(int seed) 
    {
        random = new System.Random(seed);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns>an int >= min and < max</returns>
    public int GetInt(int min, int max) 
    {
        return random.Next(min, max);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns>a double >= min and < max</returns>
    public double GetDouble(double min, double max) 
    {
        return min + GetDouble() * (max - min);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="scale"></param>
    /// <returns>a double >= 0 and < scale</returns>
    public double GetDouble(Double scale) 
    {
        return GetDouble() * scale;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>a double >= 0 and < 1</returns>
    public double GetDouble(){
        return random.NextDouble();
    }
    
}
