using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class OscilatingGlow : MonoBehaviour
{
    public ShopNavigation shopNavigation;

    void Update()
    {
        this.GetComponent<MeshRenderer>().material.color = new Color(shopNavigation.selectedColor.r - Mathf.Abs(Mathf.Sin(Time.fixedTime) * 0.5f), shopNavigation.selectedColor.g - Mathf.Abs(Mathf.Sin(Time.fixedTime) * 0.5f), shopNavigation.selectedColor.b - Mathf.Abs(Mathf.Sin(Time.fixedTime) * 0.5f));
    }
}
