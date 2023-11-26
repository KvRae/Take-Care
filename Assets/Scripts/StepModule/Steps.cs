using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Android;
using UnityEngine.InputSystem;

using UnityEngine.InputSystem.Android;
using UnityEngine.UI;

public class Steps : MonoBehaviour
{
    public Image _bar;
    public Image _calbar;
    public Image _disbar;
    public Image _timerbar;

    public TextMeshProUGUI textMeshProComponent;
    public TextMeshProUGUI textMeshProComponent2;
    public TextMeshProUGUI caloriesText;
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI timeText; // Added: Text to display the time spent walking

    public Button btnClick;
    public Button btnClickCal;
    public Button btnClickDis;

    public InputField inputSteps;

    private bool isStepCounting = false;
    private int stepCount = 0;

    private float accelerationThreshold = 1.15f;
    private float lastAcceleration = 0.0f;
    private float stepsGoal = 0;
    private float percentage = 0;

    private float caloriesGoal = 0.0f;
    private float distanceGoal = 0.0f;
    private float TimerGoal = 0.0f;

    private float timeCount = 0.0f;
    private float walkingTime = 0.0f;
    private bool isWalking = false;
    private float caloriesCount = 0.0f;
    private float distanceCount = 0.0f;
    // Added: PlayerPrefs keys
    private string stepCountKey = "StepCount";
    private string timeCountKey = "TimeCount";
    private string stepsGoalKey = "StepsGoal";
    private string caloriesGoalKey = "CaloriesGoal";
    private string distanceGoalKey = "DistanceGoal";
    private string TimerGoalKey = "TimerGoal";
        private string walkingTimeKey = "walkingTime";


    void Start()
    {/*
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);


*/

        // Load saved values from PlayerPrefs
        stepCount = PlayerPrefs.GetInt(stepCountKey, 0);
        timeCount = PlayerPrefs.GetFloat(timeCountKey, 0.0f);
        stepsGoal = PlayerPrefs.GetFloat(stepsGoalKey, 8000);
        caloriesGoal = PlayerPrefs.GetFloat(caloriesGoalKey, stepsGoal * 0.04f);
        distanceGoal = PlayerPrefs.GetFloat(distanceGoalKey, stepsGoal * 0.0762f);
        TimerGoal = PlayerPrefs.GetFloat(TimerGoalKey, stepsGoal * 1.66f);
        walkingTime = PlayerPrefs.GetFloat(walkingTimeKey, 0f);

        btnClick.onClick.AddListener(GetInputOnClickHandler);
        btnClickCal.onClick.AddListener(GetInputOnClickHandlerCalories);
        btnClickDis.onClick.AddListener(GetInputOnClickHandlerDistance);
    }

    void Update()
    {
        Vector3 acceleration = Input.acceleration;
        float accelerationMagnitude = acceleration.magnitude;

        if (isStepCounting)
        {
            if (accelerationMagnitude < accelerationThreshold && lastAcceleration >= accelerationThreshold)
            {
                stepCount++;
                isStepCounting = false;
                StopCoroutine(UpdateWalkingTime());

            }
        }
        else
        {
            if (accelerationMagnitude >= accelerationThreshold && lastAcceleration < accelerationThreshold)
            {
                isStepCounting = true;
                StartCoroutine(UpdateWalkingTime());

                
            }
        }

        lastAcceleration = accelerationMagnitude;

        textMeshProComponent.text = stepCount.ToString();

         caloriesCount = stepCount * 0.04f;
         distanceCount = stepCount * 0.0762f;

        caloriesText.text = (int)Math.Floor(caloriesCount) + " Kcl";
        distanceText.text = (int)Math.Floor(distanceCount) + " metres";

        ProgressChange(stepCount, stepsGoal);
        ProgressChangeCal(caloriesCount, caloriesGoal);
        ProgressChangeDis(distanceCount, distanceGoal);
        ProgressChangeTimer(timeCount, TimerGoal);
        percentage = stepCount / stepsGoal * 100;

        textMeshProComponent2.text = "Vous avez parcouru " + (int)Math.Floor(percentage) + "% de votre objectif";
/*
        if ((int)Math.Floor(percentage) == 50) {
        var notification = new AndroidNotification();
        notification.Title = "TakeCare";
        notification.Text = "Vous êtes à la moitié de votre but.";
        notification.FireTime = System.DateTime.Now;
        AndroidNotificationCenter.SendNotification(notification, "channel_id");
        }
        if (stepCount == stepsGoal){
            isStepCounting = false;
        }*/
    }

    void ProgressChange(float stepCount, float stepsGoal)
    {
        float amount = (stepCount / stepsGoal) * 255.0f / 360;
        _bar.fillAmount = amount;
    }

    void ProgressChangeCal(float caloriesCount, float caloriesGoal)
    {
        float amount = (caloriesCount / caloriesGoal);
        _calbar.fillAmount = amount;
    }

    void ProgressChangeDis(float distanceCount, float distanceGoal)
    {
        float amount = (distanceCount / distanceGoal);
        _disbar.fillAmount = amount;
    }

    void ProgressChangeTimer(float walkingTime, float TimerGoal)
    {
        float amount = (walkingTime / TimerGoal);
        _timerbar.fillAmount = amount;
    }

    // Coroutine to update the walking time
    IEnumerator UpdateWalkingTime()
    {
        while (isStepCounting)
        {
            walkingTime += Time.deltaTime;
            TimeSpan timeSpan = TimeSpan.FromSeconds(walkingTime);
            timeCount = (float)timeSpan.TotalSeconds;


            if (timeCount >= 60)
            {
                timeText.text = (int)Math.Floor(timeCount)/60 + " min";
            }
            else
            {
                timeText.text = (int)Math.Floor(timeCount) + " sec";
            }

            yield return null;
        }
    }

    // Save values to PlayerPrefs when the application is closed
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt(stepCountKey, stepCount);
        PlayerPrefs.SetFloat(timeCountKey, timeCount);
        PlayerPrefs.SetFloat(stepsGoalKey, stepsGoal);
        PlayerPrefs.SetFloat(caloriesGoalKey, caloriesGoal);
        PlayerPrefs.SetFloat(distanceGoalKey, distanceGoal);
        PlayerPrefs.SetFloat(TimerGoalKey, TimerGoal);
        PlayerPrefs.SetFloat(walkingTimeKey,walkingTime);

        PlayerPrefs.Save();
    }

    public void GetInputOnClickHandler()
    {
        float floatValue;

        if (float.TryParse(inputSteps.text, out floatValue))
        {
            stepsGoal = floatValue;
            stepCount =0;
            caloriesCount = 0f;
            distanceCount = 0f;
            walkingTime = 0f;
            TimerGoal = stepsGoal * 1.66f;
            caloriesGoal = stepsGoal * 0.04f;
            distanceGoal = stepsGoal * 0.0762f;
        }
        else
        {
            stepsGoal = 8000;
            caloriesGoal = stepsGoal * 0.04f;
            distanceGoal = stepsGoal * 0.0762f;
        }
    }

    public void GetInputOnClickHandlerCalories()
    {
        float floatValue;

        if (float.TryParse(inputSteps.text, out floatValue))
        {
            caloriesGoal = floatValue;
            stepsGoal = caloriesGoal / 0.04f;

            stepCount =0;
            caloriesCount = 0f;
            distanceCount = 0f;
            walkingTime = 0f;

            TimerGoal = stepsGoal * 1.66f;
            stepsGoal = caloriesGoal / 0.04f;
            distanceGoal = stepsGoal * 0.0762f;
        }
        else
        {
            stepsGoal = 8000;
            caloriesGoal = stepsGoal * 0.04f;
            distanceGoal = stepsGoal * 0.0762f;
        }
    }

    public void GetInputOnClickHandlerDistance()
    {
        float floatValue;

        if (float.TryParse(inputSteps.text, out floatValue))
        {
            distanceGoal = floatValue;
            caloriesGoal = stepsGoal * 0.04f;
            stepsGoal = distanceGoal / 0.0762f;

            stepCount =0;
            caloriesCount = 0f;
            distanceCount = 0f;
            walkingTime = 0f;
            
            TimerGoal = stepsGoal * 1.66f;
        }
        else
        {
            stepsGoal = 8000;
            caloriesGoal = stepsGoal * 0.04f;
            distanceGoal = stepsGoal * 0.0762f;
        }
    }
 
}
