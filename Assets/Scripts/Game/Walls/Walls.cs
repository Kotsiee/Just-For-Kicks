using UnityEngine;
using UnityEngine.UI;

public class Walls : Object
{
    public Walls(int numberOfSectors, int index, int side, int i)
    {
        Camera cam = Camera.main;
        GameObject eventSystem = GameObject.Find("EventSystem");

        int wallType = (i == numberOfSectors - 1 & WallScript.numberOfGoals == 0) ? 0 : Random.Range(0, WallScript.walls.Length);

        GameObject wall = Instantiate(WallScript.wall, WallScript.wallParent.transform);
        wall.GetComponent<Image>().sprite = WallScript.sprites[wallType];
        wall.GetComponent<RectTransform>().localScale = new Vector3(side, 1, 1);
        wall.GetComponent<RectTransform>().offsetMin = new Vector2(0, eventSystem.GetComponent<GameInformation>().main.GetComponent<RectTransform>().rect.height - (eventSystem.GetComponent<GameInformation>().main.GetComponent<RectTransform>().rect.height / numberOfSectors * (index + 1)));
        wall.GetComponent<RectTransform>().offsetMax = new Vector2(0, -(eventSystem.GetComponent<GameInformation>().main.GetComponent<RectTransform>().rect.height - (eventSystem.GetComponent<GameInformation>().main.GetComponent<RectTransform>().rect.height / numberOfSectors * (numberOfSectors - index))));
        if (side == 1)
        {
            wall.GetComponent<RectTransform>().anchorMin = new Vector2(0.85f, 0);
            wall.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        }
        else
        {
            wall.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            wall.GetComponent<RectTransform>().anchorMax = new Vector2(0.15f, 1);
        }

        GameObject wallGO = Instantiate(WallScript.walls[wallType], WallScript.wallGOParent.transform);
        wallGO.transform.localScale = new Vector3(0.5f, (float)10 / (float)numberOfSectors, 5);
        wallGO.transform.position = new Vector3(cam.ScreenToWorldPoint(new Vector3(0, 0)).x * -side, 5 - wallGO.transform.localScale.y / 2 - (wallGO.transform.localScale.y * index));

        if (wallType == 0)
        {
            WallScript.numberOfGoals++;
            wall.GetComponent<Image>().color = eventSystem.GetComponent<GameInformation>().ball.foreground;

            WallScript.goals[wallGO] = wall;
        }
    }
}
