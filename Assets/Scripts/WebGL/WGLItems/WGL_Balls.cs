using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class WGL_Balls : MonoBehaviour
{
    Camera cam;
    GameObject player, eventSystem;

    private void Awake()
    {
        cam = Camera.main;
        player = GameObject.Find("Player").transform.GetChild(0).gameObject;
        eventSystem = GameObject.Find("EventSystem");

        this.GetComponent<MeshFilter>().sharedMesh = eventSystem.GetComponent<WGL_GameInfo>().ball.prefab.GetComponent<MeshFilter>().sharedMesh;
    }

    private void Update()
    {
        if (this.transform.position.x <= cam.ScreenToWorldPoint(new Vector3(0, 0)).x | this.transform.position.x >= -cam.ScreenToWorldPoint(new Vector3(0, 0)).x)
        {
            Destroy(this.gameObject);
        }

        if (this.transform.position.y <= -WGL_GameInfo.camHeight / 2 - 1)
        {
            Destroy(this.gameObject);

            eventSystem.GetComponent<WGL_GAME>().error();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == player.transform.parent.name)
        {
            Vector3 dist = (transform.position - collision.collider.transform.position) * 8;
            this.GetComponent<Rigidbody>().AddForce(new Vector3(dist.x, dist.y * 2), ForceMode.Impulse);

            this.GetComponent<AddBounce>().isHit = true;
            eventSystem.GetComponent<WGL_GAME>().sfx.GetComponent<WGL_SFX>().playBounce();
        }
        else if (collision.collider.tag == "Wall")
        {
            Vector3 dist = (transform.position - collision.collider.transform.position) * 8;
            this.GetComponent<Rigidbody>().AddForce(new Vector3(dist.x, dist.y), ForceMode.Impulse);
            eventSystem.GetComponent<WGL_GAME>().sfx.GetComponent<WGL_SFX>().playBounce();
        }
        else
            Physics.IgnoreCollision(this.GetComponent<SphereCollider>(), collision.collider, true);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Goal")
        {
            if (this.GetComponent<AddBounce>().isHit)
                goal(collider.gameObject);
            else
                Destroy(this.gameObject);
        }
    }

    private async void goal(GameObject wall)
    {
        //Play sound
        eventSystem.GetComponent<WGL_GAME>().addScore(1);
        WGL_GameInfo.round = (WGL_GameInfo.round <= 10) ? Mathf.FloorToInt(WGL_GameInfo.points / 3) + 1 : 10;
        eventSystem.GetComponent<WGL_GAME>().sfx.GetComponent<WGL_SFX>().playGoal();
        Color wallColour = WGL_WallsScript.goals[wall].GetComponent<Image>().color;
        Destroy(this.gameObject);

        LeanTween.value(WGL_WallsScript.goals[wall], wallColour, new Color(1, 1, 1, 1), 0.15f)
            .setEaseLinear()
            .setOnUpdate((Color colour) =>
            {
                WGL_WallsScript.goals[wall].GetComponent<Image>().color = colour;
            });

        await Task.Delay(150);

        if (WGL_WallsScript.goals[wall] != null)
            LeanTween.value(WGL_WallsScript.goals[wall], new Color(1, 1, 1, 1), wallColour, 0.15f)
                     .setEaseLinear()
                     .setOnUpdate((Color colour) =>
                     {
                         WGL_WallsScript.goals[wall].GetComponent<Image>().color = colour;
                     });

        await Task.Delay(150);


        WGL_WallsScript.generateWalls(WGL_GameInfo.round);
    }
}
