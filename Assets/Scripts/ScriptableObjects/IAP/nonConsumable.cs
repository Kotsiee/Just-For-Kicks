using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Prefabs/Shoe", menuName = "non consumable")]
public class nonConsumable : ScriptableObject, IAPItems
{
    public string _displayName;
    public float _cost;
    public string _GPID;
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
}
