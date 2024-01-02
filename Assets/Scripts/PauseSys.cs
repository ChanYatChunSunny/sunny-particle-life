using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseSys : MonoBehaviour
{
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0f : 1f;
            if (isPaused){Physics.simulationMode = SimulationMode.Script;}
            else{Physics.simulationMode = SimulationMode.FixedUpdate;}
        }
    }
}
