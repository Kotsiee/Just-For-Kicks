using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WGL_RandomItem : MonoBehaviour
{
    private void Awake()
    {
        GameObject selectedObject = null;

        float range = Random.Range(0f, 10f);
        switch (range)
        {
            case < 7f:
                this.AddComponent<WGL_Balls>();
                selectedObject = ItemSystem.ball;
                break;

            case (>= 7f and < 8.5f):
                this.AddComponent<WGL_Coins>();
                selectedObject = ItemSystem.coin;
                break;

            case (>= 8.5f and < 9.25f):
                this.AddComponent<WGL_Heart>();
                selectedObject = ItemSystem.heart;
                break;

            case (>= 9.25f and < 9.75f):
                this.AddComponent<WGL_Obsticles>();
                selectedObject = ItemSystem.obsticle;
                break;

            case (> 9.75f):
                this.AddComponent<WGL_Gems>();
                selectedObject = ItemSystem.gem;
                break;
        }

        this.GetComponent<MeshFilter>().sharedMesh = selectedObject.GetComponent<MeshFilter>().sharedMesh;
        this.GetComponent<MeshRenderer>().sharedMaterials = selectedObject.GetComponent<MeshRenderer>().sharedMaterials;
    }
}
