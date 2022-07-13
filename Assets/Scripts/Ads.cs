using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class Ads : MonoBehaviour
{
    public string myGameIdAndroid = "4817202";
    public string myVideoPlacement = "Rewarded_Android";
    // Start is called before the first frame update
    void Start()
    {
        Advertisement.Initialize(myGameIdAndroid, false);
        

    }

    public void ShowAds()
    {
        if (Advertisement.IsReady())
        {
            Advertisement.Show();
        }
        else
        {
            Debug.Log("Ads Error");
        }
    }
}
