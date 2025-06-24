using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Prefabs/Shoe", menuName = "Consumable")]
public class Consumable : ScriptableObject, IAPItems
{
    public string _displayName;
    public float _cost;
    public string _GPID;
    public int _noCoins;
    public int _noGems;
    public string displayName
    {
        get => _displayName;
        set => _displayName = value;
    }
    public float cost
    {
        get => _cost;
        set => _cost = value;
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
    public int noGems
    {
        get => _noGems;
        set => _noGems = value;
    }
}
