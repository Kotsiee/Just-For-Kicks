using UnityEngine;

[CreateAssetMenu(fileName = "Prefabs/Shoe", menuName = "Coin data")]
public class CoinScriptable : ScriptableObject, IScriptableObject, IShopItems
{
    public int _id;
    public string _displayName;
    public string _path;
    public float _cost;
    public GameObject _prefab;
    public Rarity _rarity;
    public string _colour;
    public Sprite _icon;
    public string _GPID;
    public int _noCoins;

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
    public Sprite icon
    {
        get => _icon;
        set => _icon = value;
    }
    public string GPID
    {
        get => _GPID;
        set => _GPID = value;
    }
    public int noCoins
    {
        get => _noCoins;
        set => _noCoins = value;
    }

    private string _uid;
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
            uid = displayName.Substring(0, 3) + "-" + index.ToString();
        }

        GPID = displayName.Replace(" ", "_").ToLower();
    }
}
