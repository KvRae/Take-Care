using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DoctorVisit : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject PanelMedication;
    public GameObject PanelDoctorVisit;
    public TMP_InputField appointmentHourInput;
    public TMP_InputField appointmentMinuteInput;
    public TMP_InputField doctorNameInput;
    public Text appointmentDateInput;
    public TMP_Dropdown amPmDropdown;
    public TMP_InputField appointmentLocationInput;

    public SuccessMessage successMessage;
    public SuccessMessage successMessage2;

    [System.Serializable]
    public class DocList
    {
        public List<DocClass> DoctorVisits = new List<DocClass>();
    }

    private DocList docList = new DocList();
    void Start()
    {
        appointmentHourInput.onValueChanged.AddListener(ValidateHourInput);
        appointmentMinuteInput.onValueChanged.AddListener(ValidateMinuteInput);
        LoadFromJson();
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SaveToJson()
    {
        DocClass doc = new DocClass
        {
            Name = doctorNameInput.text,
            DateAppointment = appointmentDateInput.text,
            HH = appointmentHourInput.text,
            MM = appointmentMinuteInput.text,
            DropDown = amPmDropdown.options[amPmDropdown.value].text,
            Location = appointmentLocationInput.text
        };

        // Load existing data from the JSON file
        if (File.Exists(Path.Combine(Application.persistentDataPath, "DocDataFile4.json")))
        {
            string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "DocDataFile4.json"));
            docList = JsonUtility.FromJson<DocList>(json);

            // Add the new doctor visit to the existing list
            docList.DoctorVisits.Add(doc);

            // Save the updated list to the JSON file
            string updatedJson = JsonUtility.ToJson(docList, true);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, "DocDataFile4.json"), updatedJson);
        }
        else
        {
            // If the file doesn't exist, create a new list with the current doctor visit
            docList = new DocList();
            docList.DoctorVisits.Add(doc);

            // Save the list to the JSON file
            string json = JsonUtility.ToJson(docList, true);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, "DocDataFile4.json"), json);
        }

        // Clear input fields after saving
        doctorNameInput.text = "";
        appointmentDateInput.text = "";
        appointmentHourInput.text = "";
        appointmentMinuteInput.text = "";
        appointmentLocationInput.text = "";
        amPmDropdown.value = 0;
    }

    public void LoadFromJson()
    {
        if (File.Exists(Path.Combine(Application.persistentDataPath, "DocDataFile4.json")))
        {
            string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "DocDataFile4.json"));
            docList = JsonUtility.FromJson<DocList>(json);
        }
    }
    public void SaveSuccessful()
    {
        // Your save logic

        // Show the success message
        successMessage.ShowSuccessMessage();
        successMessage2.ShowSuccessMessage();
    }

    public void SubmitDoctorVisit()
    {
        string doctorName = doctorNameInput.text;
        string appointmentDate = appointmentDateInput.text; // Use your date picker input
        string appointmentHour = appointmentHourInput.text;
        string appointmentMinute = appointmentMinuteInput.text;
        string amPm = amPmDropdown.options[amPmDropdown.value].text;
        string appointmentLocation = appointmentLocationInput.text;

        Debug.Log("Doctor's Name: " + doctorName);
        Debug.Log("Appointment Date: " + appointmentDate);
        Debug.Log("Appointment Time: " + appointmentHour + ":" + appointmentMinute + " " + amPm);
        Debug.Log("Appointment Location: " + appointmentLocation);
    }

    public void ScheduleDoctorVisitNotification()
    {
        // Convert the user-selected date and time to a valid DateTime object
        if (DateTime.TryParse(appointmentDateInput.text + " " + appointmentHourInput.text + ":" + appointmentMinuteInput.text + " " + amPmDropdown.options[amPmDropdown.value].text, out DateTime appointmentTime))
        {
            

            var notification = new AndroidNotification();
            notification.Title = "Doctor's Appointment";
            notification.Text = "Doctor: " + doctorNameInput.text + "\nLocation: " + appointmentLocationInput.text + "\nDate: " + appointmentDateInput.text + "\nTime: " + appointmentHourInput.text + ":" + appointmentMinuteInput.text + " " + amPmDropdown.options[amPmDropdown.value].text;
            notification.FireTime = appointmentTime;
            notification.SmallIcon = "doctor";
            notification.LargeIcon = "doctor";
            notification.ShowTimestamp = true;

            AndroidNotificationCenter.SendNotification(notification, "channel_id");
        }
        else
        {
            Debug.LogError("Invalid date or time format.");
        }
    }

    public void ShowFirstPanel()
    {
        PanelMedication.SetActive(true);
        PanelDoctorVisit.SetActive(false);
    }

    public void ShowSecondPanel()
    {
        PanelMedication.SetActive(false);
        PanelDoctorVisit.SetActive(true);
    }

    private void ValidateHourInput(string input)
    {
        // Remove any non-numeric characters from the input.
        string cleanedInput = new string(input.Where(char.IsDigit).ToArray());

        // Ensure the input doesn't exceed 2 characters.
        if (cleanedInput.Length > 2)
        {
            cleanedInput = cleanedInput.Substring(0, 2);
        }

        // Ensure the input doesn't exceed 12 (maximum hours).
        int hours;
        if (int.TryParse(cleanedInput, out hours))
        {
            if (hours > 12)
            {
                hours = 12;
                cleanedInput = hours.ToString();
            }
        }

        // Update the input field with the cleaned input.
        appointmentHourInput.text = cleanedInput;
    }

    private void ValidateMinuteInput(string input)
    {
        // Remove any non-numeric characters from the input.
        string cleanedInput = new string(input.Where(char.IsDigit).ToArray());

        // Ensure the input doesn't exceed 2 characters.
        if (cleanedInput.Length > 2)
        {
            cleanedInput = cleanedInput.Substring(0, 2);
        }

        // Ensure the input doesn't exceed 60 (maximum minutes).
        int minutes;
        if (int.TryParse(cleanedInput, out minutes))
        {
            if (minutes > 60)
            {
                minutes = 60;
                cleanedInput = minutes.ToString();
            }
        }

        // Update the input field with the cleaned input.
        appointmentMinuteInput.text = cleanedInput;
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("DocListe");
    }


}
