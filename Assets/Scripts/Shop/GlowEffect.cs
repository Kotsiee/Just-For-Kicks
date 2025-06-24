using System.Runtime.CompilerServices;
using UnityEngine;

public class GlowEffect : MonoBehaviour
{
    private float speed;
    public ShopNavigation shopNavigation;

    private void Awake()
    {
        speed = Random.Range(-1f, 1f);
    }

    void Update()
    {
        this.GetComponent<MeshRenderer>().material.color = shopNavigation.selectedColor;
        this.transform.rotation = Quaternion.Euler(0, 0, (Time.fixedTime + 2) * 160 * speed);
    }
}
