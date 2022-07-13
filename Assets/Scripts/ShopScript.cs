using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour
{
    public Text firstButton;
    public Text secondButton;
    public Text thirdButton;
    public GameObject priceSecond;
    public GameObject priceThird;
    public Text moneyText;
    public int currentMoney;



    private bool firstCheck, secondCheck, thirdCheck, secondBuy, thirdBuy;

    

    // Start is called before the first frame update
    void Start()
    {

        
        GetSaves();
        if(secondBuy == false && thirdBuy == false)
        {
            firstCheck = true;
            PlayerPrefs.SetInt("firstSkin", Convert.ToInt32(firstCheck));
        }
    }
    void Update()
    {
        GetSaves();
        
        if (firstCheck == false)
        {
            firstButton.text = "Use";
        }
        
       

        if (secondCheck == false && secondBuy == true)
        {
            secondButton.text = "Use";
            priceSecond.SetActive(false);
        }
        else if(secondCheck == true && secondBuy == true)
        {
            secondButton.text = "Current";
            priceSecond.SetActive(false);
        }
     

        if(thirdCheck == false && thirdBuy == true)
        {
            thirdButton.text = "Use";
            priceThird.SetActive(false);
        }
        else if(thirdCheck == true && thirdBuy == true)
        {
            thirdButton.text = "Current";
            priceThird.SetActive(false);
        }
        moneyText.text = currentMoney.ToString();
    }
    public void FirstButtonClick()
    {
        firstButton.text = "Current";
        firstCheck = true;
        secondCheck = false;
        thirdCheck = false;



        Saves();
    }

    public void SecondButtonClick()
    {

        if(secondBuy == false && secondCheck == false)
        {
            if (currentMoney >= 7000)
            {
                currentMoney -= 7000;

                

                secondButton.text = "Current";
                firstCheck = false;
                secondCheck = true;
                thirdCheck = false;
                secondBuy = true;
                priceSecond.SetActive(false);
            }
            
        }
        else if(secondBuy == true && secondCheck == false)
        {
            secondButton.text = "Current";
            firstCheck = false;
            secondCheck = true;
            thirdCheck = false;
        }


        

        

        Saves();
    }

    public void ThirdButtonClick()
    {
        if (thirdBuy == false && thirdCheck == false)
        {
            if(currentMoney >= 14000)
            {
                currentMoney -= 14000;

                
                thirdButton.text = "Current";
                firstCheck = false;
                secondCheck = false;
                thirdCheck = true;
                thirdBuy = true;
                priceThird.SetActive(false);
            }
            
        }
        else if (thirdBuy == true && thirdCheck == false)
        {

            thirdButton.text = "Current";
            firstCheck = false;
            secondCheck = false;
            thirdCheck = true;

        }


        
        Saves();
    }
    private void Saves()
    {
        PlayerPrefs.SetInt("firstSkin", Convert.ToInt32(firstCheck));
        PlayerPrefs.SetInt("secondSkin", Convert.ToInt32(secondCheck));
        PlayerPrefs.SetInt("secondBuySkin", Convert.ToInt32(secondBuy));
        PlayerPrefs.SetInt("thirdSkin", Convert.ToInt32(thirdCheck));
        PlayerPrefs.SetInt("thirdBuySkin", Convert.ToInt32(thirdBuy));
        PlayerPrefs.SetInt("Gold", currentMoney);

        moneyText.text = currentMoney.ToString();
    }
    private void GetSaves()
    {
        firstCheck = Convert.ToBoolean(PlayerPrefs.GetInt("firstSkin"));
        secondCheck = Convert.ToBoolean(PlayerPrefs.GetInt("secondSkin"));
        thirdCheck = Convert.ToBoolean(PlayerPrefs.GetInt("thirdSkin"));
        secondBuy = Convert.ToBoolean(PlayerPrefs.GetInt("secondBuySkin"));
        thirdBuy = Convert.ToBoolean(PlayerPrefs.GetInt("thirdBuySkin"));
        currentMoney = PlayerPrefs.GetInt("Gold");
    }
}
