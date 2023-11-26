using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class DocCell : MonoBehaviour
{

    public Text nameLabel;
    public Text dateLabel;
    public Text timeLabel;
    public Text locationLabel;
    public void SetDoctorVisitData(DocClass doctorVisit)
    {
        nameLabel.text = "Doctor: " + doctorVisit.Name;
        dateLabel.text = "Date: " + doctorVisit.DateAppointment;
        timeLabel.text = "Time: " + doctorVisit.HH + ":" + doctorVisit.MM + " " + doctorVisit.DropDown;
        locationLabel.text = "Location: " + doctorVisit.Location;
    }

}
