using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHandiFillBar : MonoBehaviour
{
    public Image fillBar;
    public TextMeshProUGUI timeText;

    private float startTime;
    private float curTime;
    private bool isStart = false;

    private void OnEnable()
    {
        startTime = float.MaxValue;
    }

    public void InitUI(float duration)
    {
        startTime = duration; 
        curTime = startTime;
        fillBar.gameObject.SetActive(true);
        timeText.gameObject.SetActive(true);
        isStart = true;
    }

    private void Update()
    {
        if (!isStart) return;
        curTime -= Time.deltaTime;
        fillBar.fillAmount = curTime / startTime;
        timeText.text = curTime.ToString("F2");
        if (curTime <= 0)
        {
            fillBar.gameObject.SetActive(false);
            timeText.gameObject.SetActive(false);
            isStart = false;
            gameObject.SetActive(false);
        }
    }
}
