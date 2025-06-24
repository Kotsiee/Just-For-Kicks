using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RandomItem : MonoBehaviour
{
    private void Awake()
    {
        GameObject selectedObject = null;

        float range = Random.Range(0f, 10f);
        switch (range)
        {
            case < 7f:
                this.AddComponent<Ball>();
                selectedObject = ItemSystem.ball;
                break;

            case (>= 7f and < 8.5f):
                this.AddComponent<Coin>();
                selectedObject = ItemSystem.coin;
                break;

            case (>= 8.5f and < 9.25f):
                this.AddComponent<Heart>();
                selectedObject = ItemSystem.heart;
                break;

            case (>= 9.25f and < 9.75f):
                this.AddComponent<Obsticle>();
                selectedObject = ItemSystem.obsticle;
                break;

            case (> 9.75f):
                this.AddComponent<Gem>();
                selectedObject = ItemSystem.gem;
                break;
        }

        this.GetComponent<MeshFilter>().sharedMesh = selectedObject.GetComponent<MeshFilter>().sharedMesh;
        this.GetComponent<MeshRenderer>().sharedMaterials = selectedObject.GetComponent<MeshRenderer>().sharedMaterials;
    }
}
