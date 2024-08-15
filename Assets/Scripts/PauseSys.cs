using UnityEngine;

public class PauseSys : MonoBehaviour
{ 
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StaticData.SimRunning = !StaticData.SimRunning;
            Debug.Log(StaticData.SimRunning);
        }
    }
}
