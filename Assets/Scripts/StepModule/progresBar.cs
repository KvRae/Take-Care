using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class progresBar : MonoBehaviour
{
    public Steps step;
    public Image _bar;
    public float _progressValue = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float _progressValue = 0;
        ProgressChange (_progressValue);
    }
    void ProgressChange (float progressValue){
        float amount = (_progressValue/100.0f) * 272.0f/360;
        _bar.fillAmount = amount;
    }
}
