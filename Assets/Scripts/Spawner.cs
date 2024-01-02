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
    private GameObject[] particlePrefabsList;



    private int amount;
    private bool isGlobalReady;

    private Randomizer randomizer;
    private Partitioner partitioner;
    //The first two indices are the x and y of the partition, the third one is the type 
    private LinkedList<GameObject>[][][] partitionedParticles;
    
    // Start is called before the first frame update
    void Start()
    {
        isGlobalReady = false;
        randomizer = randomizerGameObject.GetComponent<Randomizer>();
        partitioner = partitionerGameObject.GetComponent<Partitioner>();

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
    public void Spawn(int amount) 
    {
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
            int selectedType = randomizer.GetInt(0, particlePrefabsList.Length);
            GameObject selectedParticle = particlePrefabsList[selectedType];
            Vector2 pos = new Vector2((float)randomizer.GetDouble(-radius, radius * 2), (float)randomizer.GetDouble(-radius, radius * 2));
            GameObject newObj = Instantiate(selectedParticle, pos, Quaternion.identity);
            int[] partitionIndices = partitioner.VectorToPartition(pos);
            partitionedParticles[partitionIndices[0]][partitionIndices[1]][int.Parse(newObj.tag.Replace("Particle", ""))].AddLast(newObj);
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
                            particleBehavior.SetDepences(randomizer, partitioner);
                            particleBehavior.SetPartitionedParticles(partitionedParticles);
                            particleBehavior.setGlobalReady();
                        }
                    }
                    
                }
            }
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
