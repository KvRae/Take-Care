using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WaterIntake : MonoBehaviour
{
    public Text WaterLevel;
    public Slider circularProgressIndicator;
    public int WaterLevelInt=0;
    public int Goal;
    public Text Goals;

    //public Text temperatureText;
    

    public GameObject mainPanel;
    public GameObject panel;
    public GameObject CustomIntakePanel;
    public TMP_InputField addWater;
    public InputField setNewGoal;


    public float weight;
    public TMP_Text goalS;

    void Start()
    {

        WaterLevelInt = 0;
        circularProgressIndicator.value = 0;
        panel.SetActive(false);
        weight = 70;
        goalS.text = DailyWaterIntakeEquation();
        Goals.text = DailyWaterIntakeEquation();
        //temperatureText.text = Tempeeature();

        savedsetGoal();
        savedWaterLevel();
    }

    private void Update()
    {
        resetWaterLevel();
    }

    public void Add250ml()
    {
        WaterLevelInt += 250;
        PlayerPrefs.SetInt("WaterLevelInt", WaterLevelInt);
        PlayerPrefs.Save();
        UpdateUI(); 
    }

   
    public void Add500ml()
    {
        WaterLevelInt += 500;
        PlayerPrefs.SetInt("WaterLevelInt", WaterLevelInt);
        PlayerPrefs.Save();
        UpdateUI(); 
    }

    public void AddCustomAmount()
    {
        if (addWater != null)
        {
            
            string inputText = addWater.text;

            int numberToAdd;

            numberToAdd = int.Parse(inputText);
            WaterLevelInt += numberToAdd;

            
            PlayerPrefs.SetInt("WaterLevelInt", WaterLevelInt);
            PlayerPrefs.Save();

            UpdateUI();
            hideCustomAmountPanal();


        }
    }

    //public string Tempeeature()
    //{
    //    // Access the Android battery manager
    //    AndroidJavaClass batteryClass = new AndroidJavaClass("android.os.BatteryManager");
    //    AndroidJavaObject batteryService = new AndroidJavaObject("android.os.BatteryManager");

    //    // Access the battery temperature method
    //    AndroidJavaObject batteryIntent = new AndroidJavaObject("android.content.Intent");
    //    int temperature = batteryService.Call<int>("getIntProperty", batteryIntent.Get<int>("EXTRA_TEMPERATURE"));

    //    // Print the battery temperature
    //    Debug.Log("Battery Temperature: " + temperature / 10.0f + " °C");
    //    return "Battery Temperature: " + temperature / 10.0f + " °C";
    //}

    public void savedsetGoal()
    {
        if (PlayerPrefs.HasKey("UserGoal"))
        {
            Goal = PlayerPrefs.GetInt("UserGoal");
            Goals.text = Goal.ToString() + " ml";
        }
        else
        {
            Goal = 1000; 
        }
    }

    public void savedWaterLevel()
    {
        if (PlayerPrefs.HasKey("WaterLevelInt"))
        {
            WaterLevelInt = PlayerPrefs.GetInt("WaterLevelInt");
        }
        else
        {
            WaterLevelInt = 0;
        }

        UpdateUI();
    }

    public void resetWaterLevel()
    {
        // Get the current time in hours and minutes.
        float currentHour = System.DateTime.Now.Hour;
        float currentMinute = System.DateTime.Now.Minute;
        float currentSecond = System.DateTime.Now.Second;

        
        if (currentHour == 8 && currentMinute == 00 && currentSecond ==00 )
        {
            // Reset WaterLevelInt to 0 ml.
            WaterLevelInt = 0;
            PlayerPrefs.SetInt("WaterLevelInt", WaterLevelInt);
            PlayerPrefs.Save();

            
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        WaterLevel.text = WaterLevelInt + " ml";
        circularProgressIndicator.value = (float)WaterLevelInt / Goal; 
    }

    public void submitGoal()
    {
        if (setNewGoal != null)
        {
            string inputText = setNewGoal.text;
            Goal = (int.Parse(inputText));
            Goals.text = Goal.ToString();

            // Save the goal value to PlayerPrefs.
            PlayerPrefs.SetInt("UserGoal", Goal);
            PlayerPrefs.Save();
        }
        DeactivateSecondaryPanel();
    }

    public void ChangeMainPanelColor()
    {
        if (panel.activeSelf)
        {
            mainPanel.GetComponent<Image>().color = Color.gray;
        }
        else
        {
            mainPanel.GetComponent<Image>().color = Color.white;
        }
    }

    public void ActivateSecondaryPanel()
    {
        panel.SetActive(true);
        ChangeMainPanelColor(); 
    }

    public void DeactivateSecondaryPanel()
    {
        panel.SetActive(false);
        ChangeMainPanelColor(); 
    }


    public void ShowCustomAmountPanal()
    {
        CustomIntakePanel.SetActive(true);
    }

    public void hideCustomAmountPanal()
    {
        CustomIntakePanel.SetActive(false );
    }

    public string DailyWaterIntakeEquation()
    {
        float water = (float)((weight * 0.033) * 1000);
        string goals = water.ToString() + " ml";
        return goals;
    }

    




}
