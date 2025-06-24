using UnityEngine;
using UnityEngine.UI;

public class WallSprite : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;

    private void Awake()
    {
        this.GetComponent<Image>().sprite = sprites[Random.Range(0, sprites.Length)];
    }
}
