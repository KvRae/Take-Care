using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Medication;

public class MedListe : MonoBehaviour
{
    public Text dataDisplayText;
    // Start is called before the first frame update
    void Start()
    {
        DisplayData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DisplayData()
    {
        if (File.Exists(Application.persistentDataPath + "/MedDataFile4.json"))
        {
            string json = File.ReadAllText(Application.persistentDataPath + "/MedDataFile4.json");
            MedList medList = JsonUtility.FromJson<MedList>(json);

            // Clear previous data
            dataDisplayText.text = "";

            // Display each medication entry
            foreach (MedClass med in medList.Medications)
            {
                dataDisplayText.text += $"Name: {med.Name}\nDosage: {med.Dosage}\nFrequency: {med.Frequency}\nTime: {med.Time}\n\n";
            }
        }
        else
        {
            dataDisplayText.text = "No data found.";
        }
    }
    public void LoadScene()
    {
        SceneManager.LoadScene("Medication");
    }

}
