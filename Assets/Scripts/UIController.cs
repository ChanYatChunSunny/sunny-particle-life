using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text wrongText;
    [SerializeField]
    private TMP_InputField seedField;
    [SerializeField]
    private TMP_InputField sizeField;
    [SerializeField]
    private TMP_Text[] forceLabels;
    [SerializeField]
    private TMP_Text[] distanceLabels;

    // The sliders IO here follow the IPO direction of user -> slider -> script, script -> slider -> user
    // So when user input, it is the responsibility of the slider to call the script, not the script to monitor
    [SerializeField]
    private Slider[] forceSliders;
    [SerializeField]
    private Slider[] distanceSliders;

    [SerializeField]
    private Toggle randConfigToggle;

    [SerializeField]
    private GameObject randomizerObj;

    [SerializeField]
    private GameObject spawnerObj;
    private Spawner spawner;

    private int selectedType;

    // Start is called before the first frame update
    void Start()
    {
        spawner = spawnerObj.GetComponent<Spawner>();
        wrongText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (spawner.ConfigChanged)
        {
           for(int i = 0; i < 8; i++)
            {
                float force = spawner.GetAttractionForce(selectedType, i);
                forceSliders[i].value = force;
                forceLabels[i].text = force.ToString("0.00");
                float distance = spawner.GetMaxDetectionDistance(selectedType, i);
                distanceSliders[i].value = distance;
                distanceLabels[i].text = distance.ToString("0.00");
            }
        }
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
                spawner.Clear();
                spawner.Spawn(size, randConfigToggle.isOn);
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
    public void SelectParticleType(int type)
    {
        selectedType = type;
        for (int i = 0; i < 8; i++)
        {
            forceSliders[i].value = spawner.GetAttractionForce(type, i);
            distanceSliders[i].value = spawner.GetMaxDetectionDistance(type, i);
        }
    }
    public void SetForce(Slider slider)
    {
        spawner.SetAttractionForce(selectedType, int.Parse(slider.gameObject.name.Replace("Force", "").Replace("Slider", "")), slider.value);
    }

    public void SetDistance(Slider slider)
    {
        spawner.SetMaxDetectionDistance(selectedType, int.Parse(slider.gameObject.name.Replace("Distance", "").Replace("Slider", "")), slider.value);
    }
}
