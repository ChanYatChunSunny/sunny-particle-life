using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float limit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0f) * speed * Time.deltaTime;
        transform.position += movement;
        //Ensure that the camera won't fly off the edge too much
        if (transform.position.x > limit)
        {
            transform.position = new Vector3(limit, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < -limit) 
        {
            transform.position = new Vector3(-limit, transform.position.y, transform.position.z);
        }
        if (transform.position.y > limit)
        {
            transform.position = new Vector3(transform.position.x, limit, transform.position.z);
        }
        else if (transform.position.y < -limit)
        {
            transform.position = new Vector3(transform.position.x, -limit, transform.position.z);
        }
    }
}
