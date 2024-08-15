using System.Collections.Generic;
using UnityEngine;

public class ParticleBehavior : MonoBehaviour
{
    [SerializeField]
    private int approximation;

    [SerializeField]
    private int ranInterval;

    [SerializeField]
    private float boarderRepelThreshold;

    private float[] attractionForces;
    private float[] maxDetectionDistances;

    private float randMin;
    private float randMax;

    private int approximationCounter;
    private int ranIntervalCounter;


    private int particleType;
    public int ParticleType
    {
        get { return particleType; }
    }


    private Rigidbody2D body;
    private Randomizer randomizer;
    private Partitioner partitioner;
    private LinkedList<GameObject>[][][] partitionedParticles;
    private int[] currPartition;

    public void SetDependences(Randomizer randomizer, Partitioner partitioner) 
    {
        this.randomizer = randomizer;
        this.partitioner = partitioner;
    }

    // Set the actual behavior 
    public void SetParticleBehavior(int particleType, float[] attractionForces, float[] maxDetectionDistances, float randMin, float randMax)
    {
        this.particleType = particleType;
        SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
        switch (particleType)
        {
            case 0:
                spriteRenderer.color = Color.black;
                break;
            case 1:
                spriteRenderer.color = Color.white;
                break;
            case 2:
                spriteRenderer.color = Color.red;
                break;
            case 3:
                spriteRenderer.color = Color.green;
                break;
            case 4:
                spriteRenderer.color = Color.blue;
                break;
            case 5:
                spriteRenderer.color = Color.yellow;
                break;
            case 6:
                spriteRenderer.color = Color.cyan;
                break;
            default:
                spriteRenderer.color = Color.magenta;
                break;
              
        }
        this.attractionForces = attractionForces;
        this.maxDetectionDistances = maxDetectionDistances;
        this.randMin = randMin;
        this.randMax = randMax;
    }

    //Set the particles that this particle should be aware of, do NOT change ref to the LinkedList
    public void SetPartitionedParticles(LinkedList<GameObject>[][][] partitionedParticles)
    {
        this.partitionedParticles = partitionedParticles;
    }

    // Start is called before the first frame update
    void Start()
    {
        approximationCounter = randomizer.GetInt(0, approximation);
        ranIntervalCounter = randomizer.GetInt(0, ranInterval);

        body = this.GetComponent<Rigidbody2D>();

        currPartition = partitioner.VectorToPartition(this.transform.position);
    }

    void FixedUpdate()
    {
        body.simulated = StaticData.SimRunning;
        if (!StaticData.SimRunning) { return; }

        //Only do the calculation once per "approximation" amount of fixed updates for performance
        //This may also greatly influence the characteristics of the particles
        approximationCounter = (approximationCounter + 1) % approximation;
        if (approximationCounter != 0) { return; }

        //Calculate and update to the latest partition
        CalcPartition();

        //Border repel to help avoid particles cramming at the edge
        BorderRepel();

        //Interact with other particles
        Interact();

        //Only random once per "ranInterval"*"approxmiation" amount of fixed updates so that it is more likely to move straight
        if (ranInterval == 0) { return; }
        ranIntervalCounter = (ranIntervalCounter + 1) % ranInterval;
        if (ranIntervalCounter != 0) { return; }

        //Random motion
        RandomWalk();
    }

    private void CalcPartition() 
    {
        int[] newPartition = partitioner.VectorToPartition(this.transform.position);
        if (!partitioner.IsSamePartition(currPartition, newPartition))
        {
            partitionedParticles[currPartition[0]][currPartition[1]][particleType].Remove(this.gameObject);
            partitionedParticles[newPartition[0]][newPartition[1]][particleType].AddFirst(this.gameObject);
            currPartition = newPartition;
        }
    }

    private void BorderRepel() 
    {
        Vector2 thisPos = this.transform.position;
        float thisMagnitude = thisPos.magnitude;
        if (thisMagnitude > boarderRepelThreshold)
        {
            body.AddForce((-thisPos.normalized) * (thisMagnitude * 0.032f));
        }
    }

    private void Interact() 
    {
        int[][] neighbours = partitioner.GetNeighbours(currPartition);
        //Only interact with particles within the same partition or are within the neighbour partitions
        for (int i = 0; i < 9; i++)
        {
            if (!partitioner.IsPartitionValid(neighbours[i])) { continue; }
            LinkedList<GameObject>[] particlesInPartiton = partitionedParticles[neighbours[i][0]][neighbours[i][1]];
            for (int j = 0; j < StaticData.NumOfTypes; j++)
            {
                foreach (GameObject particle in particlesInPartiton[j])
                {
                    Vector2 direction = particle.transform.position - this.transform.position;
                    float distance = direction.magnitude;
                    if (distance > 0 && distance < maxDetectionDistances[j])
                    {
                        float forceMagnitude = attractionForces[j] * (1 / distance);
                        Vector2 force = direction.normalized * forceMagnitude;
                        body.AddForce(force);
                    }
                }
            }

        }
    }

    private void RandomWalk() 
    {
        body.AddForce(new Vector2((float)randomizer.GetDouble(randMin, randMax), (float)randomizer.GetDouble(randMin, randMax)));
    }
}
