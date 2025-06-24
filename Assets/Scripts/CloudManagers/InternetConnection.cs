using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InternetConnection : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(CheckInternetConnection());
    }

    IEnumerator CheckInternetConnection()
    {
        UnityWebRequest request = new UnityWebRequest("https://just-for-kick-default-rtdb.firebaseio.com/");
        yield return request.SendWebRequest();

        if (request.error != null)
        {
            Debug.LogError("Connection Error");
        }
        else
        {
            Debug.LogAssertion("Connection Success");
        }
    }

    public void tryAgain()
    {
        StartCoroutine(CheckInternetConnection());
    }
}
