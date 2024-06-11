using UnityEngine;

public class Status : MonoBehaviour
{
    public int level = 1;           
    public int curExp = 0;          
    public int maxExp = 100;        
    public float statusPoint = 0;   

    private int expPerLevel = 100;  // 레벨업할 때마다 증가하는 총 경험치 양

    public Stat maxHealth = new Stat(100, 20);      // 최대 체력, 스탯당 상승값
    public Stat attackPower = new Stat(10, 5);      // 공격력, 스탯당 상승값
    public Stat defence = new Stat(0, 2);           // 방어력, 스탯당 상승값
    public Stat handiCraft = new Stat(10, 5);       // 손재주, 스탯당 상승값
    public Stat weightLimit = new Stat(100, 50);    // 무게 한도, 스탯당 상승값

    private PlayerCondition playerCondition;
       
    private void Start()
    {
        playerCondition = GetComponent<PlayerCondition>();
        if (playerCondition != null)
        {
            playerCondition.health.SetMaxValue(maxHealth.Value);
        }
    }

    public void GetExperience(int amount)
    {
        curExp += amount;  // 총 경험치 증가
        while (curExp >= maxExp)
        {
            LevelUp();    
        }
    }

    private void LevelUp()
    {
        level++;          
        curExp -= maxExp; 
        maxExp += expPerLevel;  
        statusPoint += 1;       
    }

    private void IncreaseStat(Stat stat)
    {
        if (statusPoint > 0)
        {
            stat.Increase();   // 스탯 증가
            statusPoint -= 1;  
        }
    }

    public void IncreaseMaxHealth() => IncreaseStat(maxHealth);
    public void IncreaseAttackPower() => IncreaseStat(attackPower);
    public void IncreaseDefence() => IncreaseStat(defence);
    public void IncreaseHandiCraft() => IncreaseStat(handiCraft);
    public void IncreaseWeightLimit() => IncreaseStat(weightLimit);

    public void EquipItem(EquipTool item)
    {
        attackPower.Add(item.damage);
    }

    public void UnequipItem(EquipTool item)
    {
        attackPower.Add(-item.damage);
    }
}

public class Stat
{
    public float Value { get; private set; }  // 현재 스탯 값을 저장하는 프로퍼티
    private readonly float increaseAmount;    // 스탯 증가량을 저장하는 필드

    public Stat(float initialValue, float increaseAmount)
    {
        Value = initialValue;                 // 스탯의 초기값을 설정
        this.increaseAmount = increaseAmount; // 스탯 증가량을 설정
    }

    public void Increase()
    {
        Value += increaseAmount; // 스탯을 증가시킴
    }

    public void Add(float amount)
    {
        Value += amount; // 스탯을 특정 값만큼 증가 또는 감소시킴
    }
}