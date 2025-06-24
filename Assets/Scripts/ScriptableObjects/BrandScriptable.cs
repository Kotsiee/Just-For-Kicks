using UnityEngine;


[CreateAssetMenu(fileName = "Prefabs/Shoe", menuName = "Brand data")]
public class BrandScriptable : ScriptableObject
{
    public int index;
    public string path;
    public Color colour = new Color(1, 1, 1, 1);
    public Sprite icon;
}
