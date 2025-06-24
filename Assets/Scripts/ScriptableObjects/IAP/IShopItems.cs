using UnityEngine;

public interface IShopItems
{
    Sprite icon { get; set; }
    int noCoins { get; set; }
    float cost { get; set; }
    int id { get; set; }
    string displayName { get; set; }
    string colour { get; set; }
    string GPID { get; set; }
    GameObject prefab { get; set; }
}

public interface IAPItems
{
    float cost { get; set; }
    string displayName { get; set; }
    string GPID { get; set; }
}