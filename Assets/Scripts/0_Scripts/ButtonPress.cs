using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPress : MonoBehaviour
{
    public void buttonDown()
    {
        LeanTween.scale(this.gameObject, new Vector3(0.75f, 0.75f, 0.75f), 0.1f);
    }

    public void buttonUp()
    {
        LeanTween.scale(this.gameObject, Vector3.one, 0.15f).setEaseOutElastic();
    }
}
