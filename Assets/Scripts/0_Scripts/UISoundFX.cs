using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundFX : MonoBehaviour
{
    public AudioSource m_AudioSource;
    [SerializeField] private AudioClip coin, gem, button, openBox, changeMenuSound, nextItem;

    public void pressButton()
    {
        m_AudioSource.clip = button;
        m_AudioSource.Play();
    }

    public void changeMenu()
    {
        m_AudioSource.clip = changeMenuSound;
        m_AudioSource.Play();
    }

    public void playOpenBox()
    {
        m_AudioSource.clip = openBox;
        m_AudioSource.Play();
    }

    public void playCoinChange()
    {
        m_AudioSource.clip = coin;
        m_AudioSource.Play();
    }

    public void playGemChange()
    {
        m_AudioSource.clip = gem;
        m_AudioSource.Play();
    }

    public void playNextItem()
    {
        m_AudioSource.clip = gem;
        m_AudioSource.Play();
    }
}
