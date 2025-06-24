using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Prefabs/Shoe", menuName = "Box data")]
public class BoxScriptable : ScriptableObject, IScriptableObject
{
    public int _id;
    public string _displayName;
    public string _path;
    public float _cost;
    public GameObject _prefab;
    public Rarity _rarity;
    public List<float> rarities;
    public string _colour;
    public Sprite icon;
    public string _GPID;

    public Texture lidTexture, boxTexture;
    public int rangeUpper;
    public BoxType type;
    public string brand;

    public int id
    {
        get => _id;
        set => _id = value;
    }
    public string displayName
    {
        get => _displayName;
        set => _displayName = value;
    }
    public string path
    {
        get => _path;
        set => _path = value;
    }
    public float cost
    {
        get => _cost;
        set => _cost = value;
    }
    public GameObject prefab
    {
        get => _prefab;
        set => _prefab = value;
    }
    public Rarity rarity
    {
        get => _rarity;
        set => _rarity = value;
    }
    public string colour
    {
        get => _colour;
        set => _colour = value;
    }
    public string GPID
    {
        get => _GPID;
        set => _GPID = value;
    }

    public string _uid;
    public string uid
    {
        get => _uid;
        set => _uid = value;
    }

    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(uid))
        {
            char[] ch = displayName.ToCharArray();
            int count = 0;
            foreach (char ch2 in ch)
            {
                count += ch2;
            }
            int index = (count * (id + displayName.Length) * 16) + ((id + displayName.Length) * 8);
            uid = displayName.Substring(0, 3).Trim() + "-" + index.ToString();
        }
    }
}

public enum BoxType
{
    Ad,
    Free,
    Rarity,
    Branded
}
