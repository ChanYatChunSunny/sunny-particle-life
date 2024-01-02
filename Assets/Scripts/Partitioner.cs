using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Space partitioning to reduce the amount of computation of interactions per particle in avg case
/// Worst case should still be O(n^2) if all particles are in the same partition
/// </summary>
public class Partitioner : MonoBehaviour
{
    [SerializeField]
    private int width;

    [SerializeField]
    //Must be even number
    private int partitionsLen;

    [SerializeField]
    private int partitionSize;

    public int GetPartitionCount() { return partitionsLen; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public int[] VectorToPartition(Vector2 coordinate) 
    {
        int widthDivTwo = width / 2;
        int xIndex = (((int)coordinate.x)+ widthDivTwo) / partitionSize;
        int yIndex = (((int)coordinate.y) + widthDivTwo) / partitionSize;
        return new int[] {xIndex, yIndex};

    }
    /// <summary>
    /// This function will not check for negative invalid neigbours
    /// </summary>
    /// <param name="partition">A 2-elements array with the x and y of the partition</param>
    /// <returns>an 9-elements array of neigbouring partitions (including itself)</returns>
    public int[][] GetNeighbours(int[] partition) 
    {
        int x = partition[0];
        int y = partition[1];
        int[][] ret = new int[][]
        {
            new int[] {x, y}, //self
            new int[] { x, y - 1 }, // Up
            new int[] { x, y + 1 }, // Down
            new int[] { x - 1, y }, // Left
            new int[] { x + 1, y }, // Right
            new int[]{ x - 1, y - 1 }, // Top-left
            new int[] { x + 1, y + 1 }, // Bottom-right
            new int[] { x - 1, y + 1 }, // Bottom-left
            new int[] { x + 1, y - 1 } // Top-right
        };

        return ret;
    }

    /// <summary>
    /// Will return false if one of the two given arrays is not 2 elements array 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public bool IsSamePartition(int[] a, int[] b) 
    {
        if(a == null || b == null) { return false; }
        if(a.Length != 2) { return false; }
        if(b.Length != a.Length) { return false; }
        return a[0] == b[0] && a[1] == b[1];
    }

    /// <summary>
    /// Check whether a given partition is invalid (if x or y is < 0 or >= number of partitions)
    /// </summary>
    /// <param name="partition"></param>
    /// <returns></returns>
    public bool IsPartitionValid(int[] partition)
    {
        if (partition[0] < 0 || partition[0] >= partitionsLen) { return false; }
        if (partition[1] < 0 || partition[1] >= partitionsLen) { return false; }
        return true;
    }

}
