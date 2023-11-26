﻿using UnityEngine;

using System;
using System.Linq;

public class Dual_DateRangePicker : DateRangePicker
{
    // FW == First Window Calender
    // SW == Second Window Calender
    [SerializeField] DayOfWeek m_FirstDayOfWeek = DayOfWeek.Monday;
    [SerializeField] bool m_ShowDaysInOtherMonths = false;
    [SerializeField] bool m_CloseOnLastSelection = false;
    [SerializeField] Calender FW_Calender;
    [SerializeField] Calender SW_Calender;
    [SerializeField] UITweenManager UITweenManager;

    private DateTime? m_StartDate;
    private CalenderButton m_StartDate_SelectedBTN;
    private DateTime? m_EndDate;

    private void Start()
    {
        Setup();
    }

    public override void Setup()
    {
        FW_Calender.Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        SW_Calender.Date = FW_Calender.Date.AddMonths(1);

        FW_Calender.PointerEnter += OnPointerEnter;
        FW_Calender.PointerDown += OnPointerDown;
        FW_Calender.PointerExit += OnPointerExit;

        SW_Calender.PointerEnter += OnPointerEnter;
        SW_Calender.PointerDown += OnPointerDown;
        SW_Calender.PointerExit += OnPointerExit;

        FW_Calender.CalenderRefreshed += OnCalenderRefreshed;
        SW_Calender.CalenderRefreshed += OnCalenderRefreshed;

        FW_Calender.Setup(DateTime.Now.Year, DateTime.Now.Month, m_FirstDayOfWeek, m_ShowDaysInOtherMonths, m_StartDate, m_EndDate, UITweenManager);
        SW_Calender.Setup(DateTime.Now.Year, DateTime.Now.AddMonths(1).Month, m_FirstDayOfWeek, m_ShowDaysInOtherMonths, m_StartDate, m_EndDate, UITweenManager);
    }

    public void OnPointerEnter(CalenderButton chosenCalenderButton, Calender calender)
    {
        if (chosenCalenderButton.CurrentState == CalenderButton.State.Normal && m_EndDate == null && DateIsInCalenderMonth(chosenCalenderButton.Date, calender.Date))
        {
            chosenCalenderButton.UpdateState(CalenderButton.State.Hover, calender.Date, m_StartDate, m_EndDate);
        }
    }

    public void OnPointerDown(CalenderButton chosenCalenderButton, DateTime chosenDate, Calender calender)
    {
        // clears selection
        if (m_StartDate != null && m_EndDate != null)
        {
            for (int i = 0; i < 42; i++)
            {
                FW_Calender.CalenderButtons[i].ResetToOriginal();
                SW_Calender.CalenderButtons[i].ResetToOriginal();
            }
            
            m_StartDate = null;
            m_EndDate = null;

            // don't return on this one
        }

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


        // revert, if second date is selected is less that the first chosen ddate
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

        if (m_StartDate != null && m_EndDate == null)
        {
            m_EndDate = chosenDate;

            // select end button
            chosenCalenderButton.UpdateState(CalenderButton.State.Selected, chosenDate, m_StartDate, m_EndDate);

            DateTime date = m_StartDate.Value;

            for (int i = 0; i < (m_EndDate - m_StartDate).Value.TotalDays + 1; i++)
            {

                CalenderButton fw_CalenderBTN = null;
                CalenderButton sw_CalenderBTN = null;

                fw_CalenderBTN = FW_Calender.CalenderButtons.Where(x => x.Date == date && x.CurrentState != CalenderButton.State.Disabled).FirstOrDefault();
                sw_CalenderBTN = SW_Calender.CalenderButtons.Where(x => x.Date == date && x.CurrentState != CalenderButton.State.Disabled).FirstOrDefault();

                if (fw_CalenderBTN != null && DateIsInCalenderMonth(date, FW_Calender.Date))
                {
                    fw_CalenderBTN.UpdateState(CalenderButton.State.Highlighted, date, m_StartDate, m_EndDate);
                }

                if (sw_CalenderBTN != null && DateIsInCalenderMonth(date, SW_Calender.Date))
                {
                    sw_CalenderBTN.UpdateState(CalenderButton.State.Highlighted, date, m_StartDate, m_EndDate);
                }


                date = date.AddDays(1);
            }

            CalendersUpdated?.Invoke(m_StartDate, m_EndDate);

            if(m_CloseOnLastSelection)
            {
                FW_Calender.gameObject.SetActive(false);
                SW_Calender.gameObject.SetActive(false);
            }

            return;
        }
    }

    public void OnCalenderRefreshed(DateTime calenderDate, CalenderButton calenderButton, DateTime buttonDate)
    {
        if (DateIsInCalenderMonth(calenderDate, buttonDate))
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
                calenderButton.UpdateState(CalenderButton.State.Highlighted, buttonDate, m_StartDate, m_EndDate);
            }
            // single 'end' selection
            else if (m_EndDate != null && m_EndDate == buttonDate && m_StartDate != null)
            {
                calenderButton.UpdateState(CalenderButton.State.Selected, buttonDate, m_StartDate, m_EndDate);
                calenderButton.UpdateState(CalenderButton.State.Highlighted, buttonDate, m_StartDate, m_EndDate);
            }
            else if (m_EndDate != null && m_EndDate == buttonDate)
            {
                calenderButton.UpdateState(CalenderButton.State.Selected, buttonDate, m_StartDate, m_EndDate);
            }
            else if (m_StartDate != null && m_EndDate != null && buttonDate >= m_StartDate && buttonDate <= m_EndDate)
            {
                calenderButton.UpdateState(CalenderButton.State.Highlighted, buttonDate, m_StartDate, m_EndDate);
            }
        }
    }

    private bool DateIsInCalenderMonth(DateTime chosenDate, DateTime calenderDate)
    {
        if(calenderDate.Month == chosenDate.Month)
        {
            return true;
        }

        return false;
    }


    public void OnPointerExit(CalenderButton chosenCalenderButton, Calender calender)
    {
        if (chosenCalenderButton.CurrentState == CalenderButton.State.Hover)
        {
            if (chosenCalenderButton.CurrentState != CalenderButton.State.Disabled)
                chosenCalenderButton.UpdateState(CalenderButton.State.Normal, calender.Date, m_StartDate, m_EndDate);
        }
    }

    public void OnClick_NextCalenderMonth()
    { 
        FW_Calender.Date = FW_Calender.Date.AddMonths(1);
        SW_Calender.Date = SW_Calender.Date.AddMonths(1);

        FW_Calender.Setup(FW_Calender.Date.Year, FW_Calender.Date.Month, m_FirstDayOfWeek, m_ShowDaysInOtherMonths, m_StartDate, m_EndDate, UITweenManager);
        SW_Calender.Setup(SW_Calender.Date.Year, SW_Calender.Date.Month, m_FirstDayOfWeek, m_ShowDaysInOtherMonths, m_StartDate, m_EndDate, UITweenManager);
    }

    public void OnClick_NextCalenderYear()
    {
        FW_Calender.Date = FW_Calender.Date.AddYears(1);
        SW_Calender.Date = SW_Calender.Date.AddYears(1);

        FW_Calender.Setup(FW_Calender.Date.Year, FW_Calender.Date.Month, m_FirstDayOfWeek, m_ShowDaysInOtherMonths, m_StartDate, m_EndDate, UITweenManager);
        SW_Calender.Setup(SW_Calender.Date.Year, SW_Calender.Date.Month, m_FirstDayOfWeek, m_ShowDaysInOtherMonths, m_StartDate, m_EndDate, UITweenManager);
    }

    public void OnClick_PreviousCalenderMonth()
    {
        FW_Calender.Date = FW_Calender.Date.AddMonths(-1);
        SW_Calender.Date = SW_Calender.Date.AddMonths(-1);

        FW_Calender.Setup(FW_Calender.Date.Year, FW_Calender.Date.Month, m_FirstDayOfWeek, m_ShowDaysInOtherMonths, m_StartDate, m_EndDate, UITweenManager);
        SW_Calender.Setup(SW_Calender.Date.Year, SW_Calender.Date.Month, m_FirstDayOfWeek, m_ShowDaysInOtherMonths, m_StartDate, m_EndDate, UITweenManager);
    }

    public void OnClick_PreviousCalenderYear()
    {
        FW_Calender.Date = FW_Calender.Date.AddYears(-1);
        SW_Calender.Date = SW_Calender.Date.AddYears(-1);

        FW_Calender.Setup(FW_Calender.Date.Year, FW_Calender.Date.Month, m_FirstDayOfWeek, m_ShowDaysInOtherMonths, m_StartDate, m_EndDate, UITweenManager);
        SW_Calender.Setup(SW_Calender.Date.Year, SW_Calender.Date.Month, m_FirstDayOfWeek, m_ShowDaysInOtherMonths, m_StartDate, m_EndDate, UITweenManager);
    }

    public void OnClick_ToggleCalenders()
    {
        FW_Calender.gameObject.SetActive(!FW_Calender.gameObject.activeInHierarchy);
        SW_Calender.gameObject.SetActive(!SW_Calender.gameObject.activeInHierarchy);
    }
}
