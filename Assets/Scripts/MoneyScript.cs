using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyScript : MonoBehaviour
{
    public int earnedMoney;
    public Button adsButton;
    public Ads ads;
    public Text moneyOutText;
    public int moneyAmount, moneyNow, moneyRes;
    // Start is called before the first frame update
    void Awake()
    {
       earnedMoney = PlayerPrefs.GetInt("Gold");
       moneyAmount = earnedMoney;
    }

    public void EarnGold()
    {
        
        earnedMoney += 20;
        PlayerPrefs.SetInt("Gold", earnedMoney);
        
    }

    public void EarnedMoneyFromAds()
    {
        adsButton.gameObject.SetActive(false);
        earnedMoney += 100;
        PlayerPrefs.SetInt("Gold", earnedMoney);
        ads.ShowAds();
        
    }
    void FixedUpdate()
    {
        moneyNow = PlayerPrefs.GetInt("Gold");
        moneyRes = moneyNow - moneyAmount;
        moneyOutText.text = "+" + moneyRes.ToString();
    }
}
