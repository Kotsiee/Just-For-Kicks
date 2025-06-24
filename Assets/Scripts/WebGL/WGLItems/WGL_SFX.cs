using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WGL_SFX : MonoBehaviour
{
    public AudioSource coinSource, gemSource, anvilSource, goSource, errorSource, goalSource, bounceSource;
    public AudioClip coin, gem, anvil, gameOver, error, goal;

    GameObject eventSystem;
    private void Awake()
    {
        eventSystem = GameObject.Find("EventSystem");
    }

    public void playBounce()
    {
        bounceSource.clip = eventSystem.GetComponent<WGL_GameInfo>().bounceClips[Random.Range(0, eventSystem.GetComponent<WGL_GameInfo>().bounceClips.Length)];
        bounceSource.Play();
    }

    public void playCoin()
    {
        coinSource.clip = coin;
        coinSource.Play();
    }

    public void playGem()
    {
        gemSource.clip = gem;
        gemSource.Play();
    }

    public void playAnvil()
    {
        anvilSource.clip = anvil;
        anvilSource.Play();
    }

    public void gameOverSound()
    {
        goSource.clip = gameOver;
        goSource.Play();
    }

    public void errorSound()
    {
        errorSource.clip = error;
        errorSource.Play();
    }

    public void playGoal()
    {
        goalSource.clip = goal;
        goalSource.Play();
    }
}
