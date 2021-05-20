using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public Text timeText;
    public float secPerMin = 0.5f;
    public int _hour = 0;
    public int _minute = 0;
    int _day = 1;

    public Light DirectionalLight;

    float timeFrame;

    
    //TO-DO Можно перенести из FixedUpdate в IEnumerator с yield и ожиданием secPerMin, дабы не обращаться к fixedUpdate. Но нужно учесть дальнейшую
    //архитектуру при учете того, что необходимо будет как то останавливать таймер пока идет "эвент" в игре.
    private void FixedUpdate()
    {
        CheckTime();
        timeFrame += Time.fixedDeltaTime;
        if(timeFrame >= secPerMin)
        {
            timeFrame -= secPerMin;
            _minute++;
            CheckTime();
        }
        UpdateLightingToTime();
    }

    public void SetTime(int hour, int minute)
    {
        _hour = hour;
        _minute = minute;
        CheckTime();
    }

    public void AddTime(int hour, int minute)
    {
        _hour = _hour + hour;
        _minute = _minute + minute;
        CheckTime();
    }

    private void CheckTime()
    {
        if (_minute >= 60)
        {
            _minute = _minute % 60;
            _hour++;
        }

        if (_hour >= 24)
        {
            _hour = _hour % 24;
            _day++;
        }

        timeText.text = string.Format("{0:00}:{1:00} \nDay: {2}", _hour, _minute,_day);
    }

    private void UpdateLightingToTime()
    {
        if (DirectionalLight != null)
        {
            //float timePercent = ((float)_hour + (float)_minute / 60) / 24f;
            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3(
                (((float)_hour + (float)_minute / 60) / 24f * 360f) - 90f,
                0f,
                0f));
        }
    }

}
