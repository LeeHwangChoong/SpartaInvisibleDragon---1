using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    public Image damageImage;
    public Image hungerImage;
    public Image coldImage;
    public float flashSpeed;

    private PlayerCondition playerCondition;
        
    private Coroutine damageCoroutine;
        
    // Start is called before the first frame update
    void Start()
    {
        playerCondition = CharacterManager.Instance.Player.condition;
        playerCondition.onTakenDamage += DamageEffect;        
    }

    private void Update()
    {
        HungerImage();
        ColdImage();
    }

    public void DamageEffect()
    {
        if(damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
        }

        damageImage.enabled = true;
        damageImage.color = new Color(1f, 92f / 255f, 92f / 255f);
        damageCoroutine = StartCoroutine(FadeAway(damageImage));
    }

    private IEnumerator FadeAway(Image targetImage)
    {   
        float startAlpha = targetImage.color.a;
        float a = startAlpha;

        while (a > 0)
        {
            a -= (startAlpha / flashSpeed) * Time.deltaTime;
            targetImage.color = new Color(targetImage.color.r, targetImage.color.g, targetImage.color.b, a);
            yield return null;
        }

        targetImage.enabled = false;
    }

    private void HungerImage()
    {
        float hungerPercentage = playerCondition.hunger.curValue / playerCondition.hunger.maxValue;
        float targetAlpha = 0.35f * (1f - hungerPercentage);
        hungerImage.color = new Color(100f / 255f, 100f / 255f, 100f / 255f, targetAlpha);
    }

    private void ColdImage()
    {
        float temperaturePercentage = playerCondition.temperature.curValue / playerCondition.temperature.maxValue;
        float targetAlpha = 0.35f * (1f - temperaturePercentage);
        coldImage.color = new Color(97f / 255f, 97f / 255f, 224f / 255f, targetAlpha);
    }        
}
