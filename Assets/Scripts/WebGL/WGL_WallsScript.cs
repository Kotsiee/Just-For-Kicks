using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WGL_WallsScript : MonoBehaviour
{
    [SerializeField] GameObject _wallParent, _wallGOParent, _wall, angledRoof;
    public static GameObject wallParent, wallGOParent, wall;

    [SerializeField]
    private GameObject[] _walls;
    public static GameObject[] walls;

    [SerializeField]
    private Sprite[] _sprites;
    public static Sprite[] sprites;

    public static int numberOfGoals;
    public static Dictionary<GameObject, GameObject> goals;

    private void Start()
    {
        wall = _wall;
        walls = _walls;
        sprites = _sprites;
        wallParent = _wallParent;
        wallGOParent = _wallGOParent;
        angledRoof.transform.GetChild(0).localPosition = new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(1, 0)).x + 1, 0);
        angledRoof.transform.GetChild(1).localPosition = new Vector3(-Camera.main.ScreenToWorldPoint(new Vector3(1, 0)).x - 1, 0);
    }

    public static void generateWalls(int numberOfSectors)
    {
        int index = 0;
        numberOfGoals = 0;
        goals = new Dictionary<GameObject, GameObject>();

        foreach (Transform child in wallGOParent.transform)
            Destroy(child.gameObject);

        foreach (Transform child in wallParent.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < numberOfSectors * 2; i++)
        {
            if (i < numberOfSectors)
            {
                new WGL_Walls(numberOfSectors, index, -1, i);
                index++;
            }
            else if (i == numberOfSectors)
            {
                index = 0;
                new WGL_Walls(numberOfSectors, index, 1, i);
                index++;
            }
            else
            {
                new WGL_Walls(numberOfSectors, index, 1, i);
                index++;
            }
        }
    }
}
