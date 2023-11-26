using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Medication;

public class Medication : MonoBehaviour
{
    public TMP_InputField nameInput;
    public TMP_InputField dosageInput;
    public TMP_Dropdown frequencyDropdown;
    public Text datePickerInput;

    public SuccessMessage successMessage;
    public SuccessMessage successMessage2;
    private MedList medList = new MedList();
    [System.Serializable]
    public class MedList
    {
        public List<MedClass> Medications = new List<MedClass>();
    }



    // Start is called before the first frame update
    void Start()
    {
        LoadFromJson();
        ScheduleMedicationNotifications();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void saveToJson()
    {
        // Create a new MedicationData instance
        MedClass newMedication = new MedClass
        {
            Name = nameInput.text,
            Dosage = dosageInput.text,
            Frequency = frequencyDropdown.options[frequencyDropdown.value].text,
            Time = datePickerInput.text
        };

        // Load existing data from the JSON file
        if (File.Exists(Path.Combine(Application.persistentDataPath, "MedDataFile4.json")))
        {
            string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "MedDataFile4.json"));
            MedList medList = JsonUtility.FromJson<MedList>(json);

            // Add the new medication to the existing list
            medList.Medications.Add(newMedication);

            // Save the updated list to the JSON file
            string updatedJson = JsonUtility.ToJson(medList, true);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, "MedDataFile4.json"), updatedJson);
        }
        else
        {
            // If the file doesn't exist, create a new list with the current medication
            MedList medList = new MedList();
            medList.Medications.Add(newMedication);

            // Save the list to the JSON file
            string json = JsonUtility.ToJson(medList, true);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, "MedDataFile4.json"), json);
        }

        // Clear input fields after saving
        nameInput.text = "";
        dosageInput.text = "";
        frequencyDropdown.value = 0;
        datePickerInput.text = "";
    }
    public void LoadFromJson()
    {
        if (File.Exists(Application.persistentDataPath + "/MedDataFile4.json"))
        {
            string json = File.ReadAllText(Application.persistentDataPath + "/MedDataFile4.json");
            medList = JsonUtility.FromJson<MedList>(json);
        }
    }
    
    public void LoadScene()
    {
        SceneManager.LoadScene("MedListe");
    }
    public void SaveSuccessful()
    {
        // Your save logic

        // Show the success message
        successMessage.ShowSuccessMessage();
        successMessage2.ShowSuccessMessage();
    }
    public void SubmitMedication()
    {
        string medicationName = nameInput.text;
        string medicationDosage = dosageInput.text;
        string medicationFrequency = frequencyDropdown.options[frequencyDropdown.value].text;
        string medicationDate = datePickerInput.text;

        Debug.Log("Medication Name: " + medicationName);
        Debug.Log("Medication Dosage: " + medicationDosage);
        Debug.Log("Medication Frequency: " + medicationFrequency);
        Debug.Log("Medication Date: " + medicationDate);
    }


    public void ScheduleMedicationNotifications()
    {
        // Read the saved medication data from the JSON file
        if (File.Exists(Application.persistentDataPath + "/MedDataFile4.json"))
        {
            string json = File.ReadAllText(Application.persistentDataPath + "/MedDataFile4.json");
            MedList medList = JsonUtility.FromJson<MedList>(json);

            // Iterate through each medication entry in the list
            foreach (MedClass med in medList.Medications)
            {
                // Parse the date range from the medication data
                string dateRange = med.Time;
                string[] dateParts = dateRange.Split('-');

                if (dateParts.Length != 2)
                {
                    Debug.LogError("Invalid date range format.");
                    return;
                }

                string startDateStr = dateParts[0].Trim();
                string endDateStr = dateParts[1].Trim();

                // Parse the start and end dates.
                if (!DateTime.TryParse(startDateStr, out DateTime startDate) ||
                    !DateTime.TryParse(endDateStr, out DateTime endDate))
                {
                    Debug.LogError("Invalid date format.");
                    return;
                }

                // Check if the current date is within the medication date range
                DateTime currentDate = DateTime.Now;

                if (currentDate >= startDate && currentDate <= endDate)
                {
                    // Set the start time for notifications.
                    DateTime notificationTime = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 8, 0, 0);

                    // Schedule daily notifications from the start date to the end date.
                    while (notificationTime <= endDate)
                    {
                        // Create a notification for each day.
                        var notification = new AndroidNotification();
                        notification.Title = "Medication Reminder";
                        notification.Text = $"It's time to take your medication: {med.Name} - {med.Dosage} - {med.Frequency}";
                        notification.FireTime = notificationTime;
                        notification.SmallIcon = "med";
                        notification.LargeIcon = "med";
                        notification.ShowTimestamp = true;

                        AndroidNotificationCenter.SendNotification(notification, "channel_id");

                        // Schedule the next notification for the next day.
                        notificationTime = notificationTime.AddDays(1);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Medication data file not found.");
        }
    }


}
