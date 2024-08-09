using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private GameObject randomizerGameObject;
    [SerializeField]
    private GameObject partitionerGameObject;
    [SerializeField]
    private float radius;
    [SerializeField]
    private int numOfTypes;
    [SerializeField]
    private GameObject particlePrefab;

    private bool isGlobalReady;

    private Randomizer randomizer;
    private Partitioner partitioner;
    // The first two indices are the x and y of the partition, the third one is the type 
    private LinkedList<GameObject>[][][] partitionedParticles;

    // Inner array is to be assigned to each type
    private float[][] attractionForces;
    private float[][] maxDetectionDistances;
    private bool configChanged;

    // Side-effect of getting this property: Will set it to false
    public bool ConfigChanged
    {
        get { bool ret = configChanged;  configChanged = false; return ret; }
        private set { configChanged = value; }
    }

    public float GetAttractionForce(int i , int j)
    {
        return attractionForces[i][j];
    }
    public void SetAttractionForce(int i, int j, float force)
    {
        attractionForces[i][j] = force;
        configChanged = true;
    }

    public float GetMaxDetectionDistance(int i , int j)
    {
        return maxDetectionDistances[i][j];
    }
    public void SetMaxDetectionDistance(int i, int j, float distance)
    {
        maxDetectionDistances[i][j] = distance;
        configChanged = true;
    }


    private float[] randMins;
    private float[] randMaxs;

    // Start is called before the first frame update
    void Start()
    {
        isGlobalReady = false;
        randomizer = randomizerGameObject.GetComponent<Randomizer>();
        partitioner = partitionerGameObject.GetComponent<Partitioner>();
        // Init the types
        attractionForces = new float[numOfTypes][];
        maxDetectionDistances = new float[numOfTypes][];
        randMins = new float[numOfTypes];
        randMaxs = new float[numOfTypes];
        for (int i = 0; i < numOfTypes; i++)
        {
            attractionForces[i] = new float[numOfTypes];
            maxDetectionDistances[i] = new float[numOfTypes];
            for(int j = 0; j < numOfTypes; j++)
            {
                attractionForces[i][j] = 0f;
                maxDetectionDistances[i][j] = 0f;
            }
        }
        // Init the partitions
        int partitionCount = partitioner.GetPartitionCount();
        partitionedParticles = new LinkedList<GameObject>[partitionCount][][];
        for (int i = 0; i < partitionCount; i++) 
        {
            partitionedParticles[i] = new LinkedList<GameObject>[partitionCount][];
            for (int j = 0; j < partitionCount; j++) 
            {
                partitionedParticles[i][j] = new LinkedList<GameObject>[numOfTypes];
            }
        }
    }
    public void RandomizeTypes()
    {
        for (int i = 0; i < numOfTypes; i++)
        {
            if(randomizer.GetDouble() <= 0.2)
            {
                randMins[i] = (float)randomizer.GetDouble(0.1, 2);
                randMaxs[i] = randMins[i] + (float)randomizer.GetDouble(0.1, 2);
            }
            for (int j = 0; j < numOfTypes; j++)
            {
                attractionForces[i][j] = (float)randomizer.GetDouble(-128, 129);
                maxDetectionDistances[i][j] = (float)randomizer.GetDouble(0, 49);
            }
        }
        ConfigChanged = true;
    }

    public void Spawn(int amount, bool isRandConfig) 
    {
        if (isRandConfig)
        {
            RandomizeTypes();
        }

        int partitionCount = partitioner.GetPartitionCount();

        for (int i = 0; i < partitionCount; i++) 
        {
            for (int j = 0; j < partitionCount; j++)
            {
                for (int k = 0; k < numOfTypes; k++)
                {
                    partitionedParticles[i][j][k] = new LinkedList<GameObject>();
                }

            }
        }

        for (int i = 0; i < amount; i++)
        {
            int selectedType = randomizer.GetInt(0, numOfTypes);
            Vector2 pos = new Vector2((float)randomizer.GetDouble(-radius, radius), (float)randomizer.GetDouble(-radius, radius));
            GameObject newObj = Instantiate(particlePrefab, pos, Quaternion.identity);
            int[] partitionIndices = partitioner.VectorToPartition(pos);
            partitionedParticles[partitionIndices[0]][partitionIndices[1]][selectedType].AddLast(newObj);
        }
        isGlobalReady = true;

    }
    private void Update()
    {
        //This code can not be directly placed at the end of Spawn() as it will create a racing condition.
        if (isGlobalReady)
        {
            int partitionCount = partitioner.GetPartitionCount();
            for (int i = 0; i < partitionCount; i++)
            {
                for (int j = 0; j < partitionCount; j++)
                {
                    for (int k = 0; k < numOfTypes; k++)
                    {
                        foreach (GameObject particle in partitionedParticles[i][j][k])
                        {
                            ParticleBehavior particleBehavior = particle.GetComponent<ParticleBehavior>();
                            particleBehavior.SetDependences(randomizer, partitioner);
                            particleBehavior.SetParticleBehavior(k, attractionForces[k], maxDetectionDistances[k], randMins[k], randMaxs[k]);
                            particleBehavior.SetPartitionedParticles(partitionedParticles);
                        }
                    }
                    
                }
            }
            StaticData.SimRunning = true;
        }
        isGlobalReady = false;
    }

    public void Clear() 
    {
        int partitionCount = partitioner.GetPartitionCount();
        for (int i = 0; i < partitionCount; i++)
        {
            for (int j = 0; j < partitionCount; j++)
            {
                for (int k = 0; k < numOfTypes; k++) 
                {
                    if (partitionedParticles[i][j][k] != null)
                    {
                        while (partitionedParticles[i][j][k].Count > 0)
                        {
                            Destroy(partitionedParticles[i][j][k].First.Value);
                            partitionedParticles[i][j][k].RemoveFirst();
                        }
                    }
                }
            }
        }
    }

}
