using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;



public class Ads : MonoBehaviour
{

    private Coroutine showAd;

    private string gameId = "3933475", type = "video";  
    private bool testMode = true, needToStop; 

    private static int countLoses; // shows our ad after each third game
    

    private void Start()
    {
        Advertisement.Initialize(gameId, testMode);
        
        countLoses++;
        if (countLoses % 3 == 0) 
        showAd = StartCoroutine(ShowAd());

    }


    private void Update ()
    {
        if (needToStop)
        {
            needToStop = false;
            StopCoroutine(showAd); 
        }
    }


        
    IEnumerator ShowAd()
    {
        while (true)
        {
            // check for loading ad
            if (Advertisement.IsReady(type))
            {
                Advertisement.Show(type); 
                Debug.Log("Ready");
                needToStop = true;
            }

            yield return new WaitForSeconds(10f); 

                
        }
    }



}
