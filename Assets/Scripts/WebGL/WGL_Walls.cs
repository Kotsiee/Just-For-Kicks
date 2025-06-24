using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WGL_Walls : MonoBehaviour
{
    public WGL_Walls(int numberOfSectors, int index, int side, int i)
    {
        Camera cam = Camera.main;
        GameObject eventSystem = GameObject.Find("EventSystem");

        int wallType = (i == numberOfSectors - 1 & WGL_WallsScript.numberOfGoals == 0) ? 0 : Random.Range(0, WGL_WallsScript.walls.Length);

        GameObject wall = Instantiate(WGL_WallsScript.wall, WGL_WallsScript.wallParent.transform);
        wall.GetComponent<Image>().sprite = WGL_WallsScript.sprites[wallType];
        wall.GetComponent<RectTransform>().localScale = new Vector3(side, 1, 1);
        wall.GetComponent<RectTransform>().offsetMin = new Vector2(0, eventSystem.GetComponent<WGL_GameInfo>().main.GetComponent<RectTransform>().rect.height - (eventSystem.GetComponent<WGL_GameInfo>().main.GetComponent<RectTransform>().rect.height / numberOfSectors * (index + 1)));
        wall.GetComponent<RectTransform>().offsetMax = new Vector2(0, -(eventSystem.GetComponent<WGL_GameInfo>().main.GetComponent<RectTransform>().rect.height - (eventSystem.GetComponent<WGL_GameInfo>().main.GetComponent<RectTransform>().rect.height / numberOfSectors * (numberOfSectors - index))));
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

        GameObject wallGO = Instantiate(WGL_WallsScript.walls[wallType], WGL_WallsScript.wallGOParent.transform);
        wallGO.transform.localScale = new Vector3(0.5f, (float)10 / (float)numberOfSectors, 5);
        wallGO.transform.position = new Vector3(cam.ScreenToWorldPoint(new Vector3(0, 0)).x * -side, 5 - wallGO.transform.localScale.y / 2 - (wallGO.transform.localScale.y * index));

        if (wallType == 0)
        {
            WGL_WallsScript.numberOfGoals++;
            wall.GetComponent<Image>().color = eventSystem.GetComponent<WGL_GameInfo>().ball.foreground;

            WGL_WallsScript.goals[wallGO] = wall;
        }
    }
}
