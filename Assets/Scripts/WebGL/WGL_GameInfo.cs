using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WGL_GameInfo : MonoBehaviour
{
    public TMP_Text coinsTxt, gemsTxt, scoreTxt, pointsTxt, highscoreTxt;
    public List<TMP_Text> colourText;
    [SerializeField] private List<Image> backgrounds;
    public List<GameObject> forgrounds;

    public GameObject ballPrefab, playerPrefab, xObj, xPanel, particles, main;
    public BallScriptable ball;
    public ShoeScriptable shoe;
    public Canvas canvas;
    public Camera cam;
    public GameObject gameOver, walls;
    public AudioClip[] bounceClips;

    public static int coins, gems, highscore, points, gameCoins, gameGems, games, round, errors;
    public static bool inPlay = false;
    public static float camHeight, camWidth, h, w;
    private void Awake()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        camHeight = cam.GetComponent<Camera>().orthographicSize * 2;
        camWidth = cam.GetComponent<Camera>().orthographicSize;
        h = camHeight / canvas.pixelRect.height;
        w = camWidth / canvas.pixelRect.width;

        playerPrefab.GetComponent<MeshFilter>().sharedMesh = shoe.prefab.GetComponent<MeshFilter>().sharedMesh;
        ballPrefab.GetComponent<MeshFilter>().sharedMesh = ball.prefab.GetComponent<MeshFilter>().sharedMesh;
        cam.backgroundColor = ball.background;
        colourText.ForEach(text => text.color = new Color(ball.background.r + 0.2f, ball.background.g + 0.2f, ball.background.b + 0.2f, ball.background.a));
        backgrounds.ForEach(image => image.color = ball.background);
        forgrounds.ForEach(image => { image.GetComponent<Image>().color = ball.foreground; });
        bounceClips = ball.bounceClips;

        WGL_WallsScript.generateWalls(1);
    }
}
