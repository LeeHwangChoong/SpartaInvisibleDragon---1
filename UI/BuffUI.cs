using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffUI : MonoBehaviour
{
    public Image boostImage;
    public Image antiColdImage;
    public Image invincibleImage;
    // Start is called before the first frame update
    void Start()
    {
        boostImage.gameObject.SetActive(false);
        antiColdImage.gameObject.SetActive(false);
        invincibleImage.gameObject.SetActive(false);
    }

    public void ShowBoost(bool isActive)
    {
        boostImage.gameObject.SetActive(isActive);
    }

    public void ShowInvincibility(bool isActive)
    {
        invincibleImage.gameObject.SetActive(isActive);
    }

    public void ShowAntiCold(bool isActive)
    {
        antiColdImage.gameObject.SetActive(isActive);
    }
}
