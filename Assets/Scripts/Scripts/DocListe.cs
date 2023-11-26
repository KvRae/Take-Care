using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static DoctorVisit;

public class DocListe : MonoBehaviour
{
    public Text dataDisplayText;

    void Start()
    {
        DisplayData();
    }

    void DisplayData()
    {
        if (File.Exists(Application.persistentDataPath + "/DocDataFile4.json"))
        {
            string json = File.ReadAllText(Application.persistentDataPath + "/DocDataFile4.json");
            DocList docList = JsonUtility.FromJson<DocList>(json);

            // Clear previous data
            dataDisplayText.text = "";

            // Display each doctor visit entry
            foreach (DocClass doctorVisit in docList.DoctorVisits)
            {
                dataDisplayText.text += $"Doctor Name: {doctorVisit.Name}\nAppointment Date: {doctorVisit.DateAppointment}\nTime: {doctorVisit.HH}:{doctorVisit.MM} {doctorVisit.DropDown}\nLocation: {doctorVisit.Location}\n\n";
            }
        }
        else
        {
            dataDisplayText.text = "No doctor visit data found.";
        }
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("DoctorVisit");
    }


}
