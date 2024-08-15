using System;
using System.Collections;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text infoText;
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
        infoText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (spawner.ConfigChanged)
        {
           for(int i = 0; i < StaticData.NumOfTypes; i++)
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

    private void DisplayInfoText(String str)
    {
        infoText.text = str;
        infoText.gameObject.SetActive(true);
        StartCoroutine(WaitAndHideInfoText());
    }
    private IEnumerator WaitAndHideInfoText()
    {
        yield return new WaitForSeconds(2);
        infoText.gameObject.SetActive(false);
    }

    public void PressStart() 
    {
        infoText.gameObject.SetActive(false);
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
                DisplayInfoText("Seed and size must be positive integer.");
            }
        }
        catch (Exception) 
        {
            DisplayInfoText("Seed and size must be positive integer.");
        }
    }
    public void SelectParticleType(int type)
    {
        selectedType = type;
        for (int i = 0; i < StaticData.NumOfTypes; i++)
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
    /*
     * Config file format:
     * 8 rows of force i for 8 j
     * 8 rows of distance i for 8 j
     */

    public void SaveConfig()
    {
        
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < StaticData.NumOfTypes; i++)
        {
            for (int j = 0; j < StaticData.NumOfTypes; j++)
            {
                sb.Append(spawner.GetAttractionForce(i, j).ToString("0.00"));
                sb.Append("\t");
            }
            sb.Append("\n");
        }
        for (int i = 0; i < StaticData.NumOfTypes; i++)
        {
            for (int j = 0; j < StaticData.NumOfTypes; j++)
            {
                sb.Append(spawner.GetMaxDetectionDistance(i, j).ToString("0.00"));
                sb.Append("\t");
            }
            sb.Append("\n");
        }
        
        File.WriteAllText(StaticData.ConfigPath, sb.ToString());
        DisplayInfoText("File saved at " + StaticData.ConfigPath + ".");
    }

    public void LoadConfig()
    {

        if (!File.Exists(StaticData.ConfigPath))
        {
            DisplayInfoText("Config could not be found at " + StaticData.ConfigPath + ".");
        }
        try
        {
            String[] lines = File.ReadAllLines(StaticData.ConfigPath);
            for(int i = 0; i < StaticData.NumOfTypes; i++)
            {
                String[] valStrs = lines[i].Split("\t");
                for(int j = 0; j < StaticData.NumOfTypes; j++)
                {
                    spawner.SetAttractionForce(i, j, float.Parse(valStrs[j]));
                }
            }
            for (int i = 0; i < StaticData.NumOfTypes; i++)
            {
                String[] valStrs = lines[i + StaticData.NumOfTypes].Split("\t");
                for (int j = 0; j < StaticData.NumOfTypes; j++)
                {
                    spawner.SetMaxDetectionDistance(i, j, float.Parse(valStrs[j]));
                }
            }
        }
        catch (Exception)
        {
            DisplayInfoText("Config at " + StaticData.ConfigPath + " is corrupted.");
        }
    }
}
