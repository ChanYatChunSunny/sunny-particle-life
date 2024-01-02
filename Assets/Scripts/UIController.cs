using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text wrongText;
    [SerializeField]
    private TMP_InputField seedField;
    [SerializeField]
    private TMP_InputField sizeField;

    [SerializeField]
    private GameObject randomizerObj;
    [SerializeField]
    private GameObject spawnerObj;

    // Start is called before the first frame update
    void Start()
    {
        wrongText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PressStart() 
    {
        wrongText.gameObject.SetActive(false);
        try
        {
            int seed = int.Parse(seedField.text);
            int size = int.Parse(sizeField.text);
            if (seed > 0 && size > 0) 
            {
                randomizerObj.GetComponent<Randomizer>().SetSeed(seed);
                Spawner spawner = spawnerObj.GetComponent<Spawner>();
                spawner.Clear();
                spawner.Spawn(size);
            }
            else
            {
                wrongText.gameObject.SetActive(true);
            }
        }
        catch (Exception) 
        {
            wrongText.gameObject.SetActive(true);
        }
    }
}
