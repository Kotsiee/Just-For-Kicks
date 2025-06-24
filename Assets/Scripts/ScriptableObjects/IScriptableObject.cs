using UnityEngine;

public interface IScriptableObject
{
    int id { get; set; }
    string displayName { get; set; }
    string path { get; set; }
    float cost { get; set; }
    GameObject prefab { get; set; }
    Rarity rarity { get; set; }
    string colour { get; set; }

    string uid { get; set; }
    string GPID { get; set; }
}
public enum Rarity
{
    Common = 0,
    Uncommon = 1,
    Rare = 2,
    Golden = 3,
    Legendary = 4,
    Mythic = 5
}