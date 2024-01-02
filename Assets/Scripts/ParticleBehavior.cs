using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ParticleBehavior : MonoBehaviour
{
    [SerializeField]
    private int approximation;

    [SerializeField]
    private int ranInterval;

    [SerializeField]
    private float[] attractionForces;
    [SerializeField]
    private float[] maxDetectionDistances;

    [SerializeField]
    private double randMin;
    [SerializeField]
    private double randScale;

    [SerializeField]
    private float boarderRepelThreshold;

    private int approximationCounter;
    private int ranIntervalCounter;

    private int numOfParticleTypes;

    private int particleType;

    private Rigidbody2D body;
    private Randomizer randomizer;
    private Partitioner partitioner;
    private LinkedList<GameObject>[][][] partitionedParticles;
    private bool isGlobalReady = false;
    private int[] currPartition;

    public void SetDepences(Randomizer randomizer, Partitioner partitioner) 
    {
        this.randomizer = randomizer;
        this.partitioner = partitioner;
    }

    //Set the particles that this particle should be aware of
    public void SetPartitionedParticles(LinkedList<GameObject>[][][] partitionedParticles)
    {
        this.partitionedParticles = partitionedParticles;
    }
    //Call this function omce every other particle is ready
    public void setGlobalReady()
    {
        isGlobalReady = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        approximationCounter = randomizer.GetInt(0, approximation);
        ranIntervalCounter = randomizer.GetInt(0, ranInterval);

        body = this.GetComponent<Rigidbody2D>();

        currPartition = partitioner.VectorToPartition(this.transform.position);

        numOfParticleTypes = attractionForces.Length;

        particleType = int.Parse(this.tag.Replace("Particle", ""));


    }

    void FixedUpdate()
    {
        if (!isGlobalReady) { return; }

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
            for (int j = 0; j < numOfParticleTypes; j++)
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
        body.AddForce(new Vector2((float)randomizer.GetDouble(randMin, randScale), (float)randomizer.GetDouble(randMin, randScale)));
    }
}
