﻿using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// nh - no highlight date picker
/// </summary>
public class nh_Single_DateRangePicker : DateRangePicker
{
    [SerializeField] DayOfWeek m_FirstDayOfWeek = DayOfWeek.Monday;

    [SerializeField] UITweenManager UITweenManager;
    [SerializeField] bool m_ShowDaysInOtherMonths = false;
    [SerializeField] bool m_CloseOnLastSelection = false;

    [SerializeField] Calender m_Calender;

    private DateTime? m_StartDate;
    private CalenderButton m_StartDate_SelectedBTN;
    private DateTime? m_EndDate;
    private CalenderButton m_EndDate_SelectedBTN;

    private void Start()
    {
        Setup();
    }

    public override void Setup()
    {
        m_Calender.PointerEnter = OnPointerEnter;
        m_Calender.PointerDown = OnPointerDown;
        m_Calender.PointerExit = OnPointerExit;
        m_Calender.CalenderRefreshed = OnCalenderRefreshed;
        m_Calender.Setup(DateTime.Now.Year, DateTime.Now.Month, m_FirstDayOfWeek, m_ShowDaysInOtherMonths, m_StartDate, m_EndDate, UITweenManager);
    }

    public void OnPointerEnter(CalenderButton chosenCalenderButton, Calender calender)
    {
        if (chosenCalenderButton.CurrentState == CalenderButton.State.Normal && m_EndDate == null)
        {
            chosenCalenderButton.UpdateState(CalenderButton.State.Hover, calender.Date, m_StartDate, m_EndDate);
        }
    }

    public void OnPointerDown(CalenderButton chosenCalenderButton, DateTime chosenDate, Calender calender)
    {
        // clears selection
        if (m_StartDate != null && m_EndDate != null)
        {
            m_StartDate_SelectedBTN.ResetToOriginal();
            m_EndDate_SelectedBTN.ResetToOriginal();

            m_StartDate = null;
            m_EndDate = null;

            // don't return on this one
        }

        // intiate first click
        if (m_StartDate == null && m_EndDate == null)
        {
            if (chosenCalenderButton.CurrentState != CalenderButton.State.Disabled)
            {
                m_StartDate = chosenDate;
                m_StartDate_SelectedBTN = chosenCalenderButton;

                CalendersUpdated?.Invoke(m_StartDate, m_EndDate);
                chosenCalenderButton.UpdateState(CalenderButton.State.Selected, chosenDate, m_StartDate, m_EndDate);
            }
            return;
        }

        if (m_StartDate != null && chosenDate <= m_StartDate && m_EndDate == null)
        {
            if (chosenCalenderButton.CurrentState != CalenderButton.State.Disabled)
            {
                // revert previous selected start date
                m_StartDate_SelectedBTN.ResetToOriginal();

                m_StartDate = chosenDate;
                m_StartDate_SelectedBTN = chosenCalenderButton;

                CalendersUpdated?.Invoke(m_StartDate, m_EndDate);
                chosenCalenderButton.UpdateState(CalenderButton.State.Selected, chosenDate, m_StartDate, m_EndDate);
            }

            return;
        }

        // initiate second click
        if (m_StartDate != null && m_EndDate == null)
        {
            m_EndDate = chosenDate;
            m_EndDate_SelectedBTN = chosenCalenderButton;
            // select end button
            chosenCalenderButton.UpdateState(CalenderButton.State.Selected, chosenDate, m_StartDate, m_EndDate);

            CalendersUpdated?.Invoke(m_StartDate, m_EndDate);

            if (m_CloseOnLastSelection)
            {
                m_Calender.gameObject.SetActive(false);
            }

            return;
        }
    }

    public void OnPointerExit(CalenderButton chosenCalenderButton, Calender calender)
    {
        if (chosenCalenderButton.CurrentState == CalenderButton.State.Hover)
        {
            if (chosenCalenderButton.CurrentState != CalenderButton.State.Disabled)
                chosenCalenderButton.UpdateState(CalenderButton.State.Normal, calender.Date, m_StartDate, m_EndDate);
        }
    }

    public void OnCalenderRefreshed(DateTime calenderDate, CalenderButton calenderButton, DateTime buttonDate)
    {
        // single selection
        if (m_StartDate != null && m_StartDate == buttonDate && m_EndDate == null)
        {
            calenderButton.UpdateState(CalenderButton.State.Selected, buttonDate, m_StartDate, m_EndDate);
        }
        // single selection but we also need to show highlight
        else if (m_StartDate != null && m_StartDate == buttonDate)
        {
            calenderButton.UpdateState(CalenderButton.State.Selected, buttonDate, m_StartDate, m_EndDate);
        }
        // single 'end' selection
        else if (m_EndDate != null && m_EndDate == buttonDate && m_StartDate != null)
        {
            calenderButton.UpdateState(CalenderButton.State.Selected, buttonDate, m_StartDate, m_EndDate);
        }
        else if (m_EndDate != null && m_EndDate == buttonDate)
        {
            calenderButton.UpdateState(CalenderButton.State.Selected, buttonDate, m_StartDate, m_EndDate);
        }
    }

    public void OnClick_NextCalenderMonth()
    {
        m_Calender.Date = m_Calender.Date.AddMonths(1);

        m_Calender.Setup(m_Calender.Date.Year, m_Calender.Date.Month, m_FirstDayOfWeek, m_ShowDaysInOtherMonths, m_StartDate, m_EndDate, UITweenManager);
    }

    public void OnClick_NextCalenderYear()
    {
        m_Calender.Date = m_Calender.Date.AddYears(1);

        m_Calender.Setup(m_Calender.Date.Year, m_Calender.Date.Month, m_FirstDayOfWeek, m_ShowDaysInOtherMonths, m_StartDate, m_EndDate, UITweenManager);
        m_Calender.Setup(m_Calender.Date.Year, m_Calender.Date.Month, m_FirstDayOfWeek, m_ShowDaysInOtherMonths, m_StartDate, m_EndDate, UITweenManager);
    }

    public void OnClick_PreviousCalenderMonth()
    {
        m_Calender.Date = m_Calender.Date.AddMonths(-1);
        m_Calender.Setup(m_Calender.Date.Year, m_Calender.Date.Month, m_FirstDayOfWeek, m_ShowDaysInOtherMonths, m_StartDate, m_EndDate, UITweenManager);
    }

    public void OnClick_PreviousCalenderYear()
    {
        m_Calender.Date = m_Calender.Date.AddYears(-1);
        m_Calender.Setup(m_Calender.Date.Year, m_Calender.Date.Month, m_FirstDayOfWeek, m_ShowDaysInOtherMonths, m_StartDate, m_EndDate, UITweenManager);
    }

    public void OnClick_ToggleCalenders()
    {
        m_Calender.gameObject.SetActive(!m_Calender.gameObject.activeInHierarchy);
    }
}
