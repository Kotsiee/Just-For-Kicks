using UnityEngine;

[CreateAssetMenu(fileName = "Prefabs/Shoe", menuName = "Ball data")]
public class BallScriptable : ScriptableObject, IScriptableObject
{

    public int _id;
    public string _displayName;
    public string _path;
    public float _cost;
    public GameObject _prefab;
    public Rarity _rarity;
    public string _colour;
    public string _GPID;
    public AudioClip[] bounceClips;

    public Color background, foreground;

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
        colour = prefab.name;
        id = int.Parse(this.name) - 1;

        switch (rarity)
        {
            case Rarity.Common:
                cost = 2;
                break;
            case Rarity.Uncommon:
                cost = 4;
                break;
            case Rarity.Rare:
                cost = 6;
                break;
            case Rarity.Golden:
                cost = 8;
                break;
            case Rarity.Legendary:
                cost = 10;
                break;
            case Rarity.Mythic:
                cost = 15;
                break;
        }

        char[] ch = displayName.ToCharArray();
        int count = 0;
        foreach (char ch2 in ch)
        {
            count += ch2;
        }
        int index = (count * (id + displayName.Length) * 16) + ((id + displayName.Length) * 8);
        uid = colour.Substring(0, 3) + "-" + index.ToString();

        GPID = rarity.ToString().ToLower();
    }
}
