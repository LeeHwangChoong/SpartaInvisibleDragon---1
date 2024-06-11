using UnityEngine;
using UnityEngine.UI;

public class UIConditions : MonoBehaviour
{
    public Condition health;
    public Condition hunger;
    public Condition temperature;    

    // Start is called before the first frame update
    void Start()
    {
        CharacterManager.Instance.Player.condition.uiCondition = this;
    }
}
