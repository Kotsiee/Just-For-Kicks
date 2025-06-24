using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Prefabs/Shoe", menuName = "Shoe data")]
public class ShoeScriptable : ScriptableObject, IScriptableObject
{
    public int _id;
    public string _displayName;
    public string _path;
    public float _cost;
    public GameObject _prefab;
    public Rarity _rarity;
    public string _colour;
    public string _GPID;
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


    internal object index;

    public string _uid;
    public string uid
    {
        get => _uid;
        set => _uid = value;
    }

    private void OnValidate()
    {
        switch (rarity)
        {
            case Rarity.Common:
                cost = 4;
                break;
            case Rarity.Uncommon:
                cost = 6;
                break;
            case Rarity.Rare:
                cost = 8;
                break;
            case Rarity.Golden:
                cost = 12;
                break;
            case Rarity.Legendary:
                cost = 16;
                break;
            case Rarity.Mythic:
                cost = 20;
                break;
        }
        char[] ch = colour.ToCharArray();
        int count = 0;
        foreach (char ch2 in ch)
        {
            count += ch2;
        }
        int index = (count * (id + colour.Length) * 16) + ((id + colour.Length) * 8);
        uid = path.Split("/")[0].Substring(0, 3) + path.Split("/")[1].Substring(0, 3) + "-" + index.ToString();

        colour = prefab.name;
        id = int.Parse(this.name) - 1;
        GPID = rarity.ToString().ToLower();
    }
}
