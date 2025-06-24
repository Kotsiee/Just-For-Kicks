using System.Collections.Generic;
using UnityEngine;

public class User
{
    public string name;
    public int highscore;
    public int coins;
    public int gems;

    public string selectedBall;
    public string selectedShoe;
    public string gift;

    public Dictionary<string, string> shoes, boxes;
    public Dictionary<string, string> balls;

    public bool removeAds;

    public User(string user)
    {
        if (user == "")
            this.name = "user-" + Random.Range(1000, 1000000);
        else
            this.name = user;

        this.removeAds = false;

        this.highscore = 0;
        this.coins = 0;
        this.gems = 0;

        this.selectedBall = "Common:0";
        this.selectedShoe = "Foot/Human:1";
        this.gift = "01-01-2023 00;00;00";

        this.shoes = new Dictionary<string, string>
        {
            ["FooHum-40360"] = "Foot/Human:0",
            ["FooHum-49104"] = "Foot/Human:1",
            ["FooHum-58296"] = "Foot/Human:2",
            ["FooHum-61120"] = "Foot/Human:3"
        };

        this.balls = new Dictionary<string, string>
        {
            ["Bla-24288"] = "Common:0",
            ["Gre-30360"] = "Common:1",
            ["Whi-36432"] = "Common:2"
        };

        this.boxes = new Dictionary<string, string>
        {
            ["Dai-119304"] = "FreeBoxes:0:01-01-2023 00;00;00",
            ["AD-51800"] = "FreeBoxes:1:3:01-01-2023 00;00;00"
        };
    }
}
