using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    public float curValue;
    public float startValue;
    public float maxValue;
    public float passiveValue;
    public Image uiBar;

    // Start is called before the first frame update
    private void Start()
    {
        curValue = startValue;
    }

    // Update is called once per frame
    private void Update()
    {
        // ui 업데이트
        uiBar.fillAmount = GetPercentage();
    }

    private float GetPercentage()
    {
        return curValue / maxValue;
    }

    public void Add(float value)
    {        
        curValue = Mathf.Min(curValue + value, maxValue);
    }

    public void Subtract(float value)
    {
        curValue = Mathf.Max(curValue - value, 0);
    }

    public void SetMaxValue(float newMaxValue)
    {   // 스탯포인트로 인해 증가한 최대값 적용
        maxValue = newMaxValue;
        curValue = Mathf.Min(curValue, maxValue); 
        uiBar.fillAmount = GetPercentage();        
    }
}
