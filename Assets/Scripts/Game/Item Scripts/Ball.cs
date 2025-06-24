using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Ball : MonoBehaviour
{
    Camera cam;
    GameObject player, eventSystem;

    private void Awake()
    {
        cam = Camera.main;
        player = GameObject.Find("Player").transform.GetChild(0).gameObject;
        eventSystem = GameObject.Find("EventSystem");

        this.GetComponent<MeshFilter>().sharedMesh = eventSystem.GetComponent<GameInformation>().ball.prefab.GetComponent<MeshFilter>().sharedMesh;
    }

    private void Update()
    {
        if (this.transform.position.x <= cam.ScreenToWorldPoint(new Vector3(0, 0)).x | this.transform.position.x >= -cam.ScreenToWorldPoint(new Vector3(0, 0)).x)
        {
            Destroy(this.gameObject);
        }

        if (this.transform.position.y <= -GameInformation.camHeight / 2 - 1)
        {
            Destroy(this.gameObject);

            eventSystem.GetComponent<Game>().error();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == player.transform.parent.name)
        {
            Vector3 dist = (transform.position - collision.collider.transform.position) * 8;
            this.GetComponent<Rigidbody>().AddForce(new Vector3(dist.x, dist.y * 2), ForceMode.Impulse);

            this.GetComponent<AddBounce>().isHit = true;
            eventSystem.GetComponent<Game>().sfx.GetComponent<SoundFX>().playBounce();
        }
        else if (collision.collider.tag == "Wall")
        {
            Vector3 dist = (transform.position - collision.collider.transform.position) * 8;
            this.GetComponent<Rigidbody>().AddForce(new Vector3(dist.x, dist.y), ForceMode.Impulse);
            eventSystem.GetComponent<Game>().sfx.GetComponent<SoundFX>().playBounce();
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
        eventSystem.GetComponent<Game>().addScore(1);
        GameInformation.round = (GameInformation.round <= 10) ? Mathf.FloorToInt(GameInformation.points / 3) + 1 : 10;
        eventSystem.GetComponent<Game>().sfx.GetComponent<SoundFX>().playGoal();
        Color wallColour = WallScript.goals[wall].GetComponent<Image>().color;
        Destroy(this.gameObject);

        LeanTween.value(WallScript.goals[wall], wallColour, new Color(1, 1, 1, 1), 0.15f)
            .setEaseLinear()
            .setOnUpdate((Color colour) =>
            {
                WallScript.goals[wall].GetComponent<Image>().color = colour;
            });

        await Task.Delay(150);

        if (WallScript.goals[wall] != null)
        LeanTween.value(WallScript.goals[wall], new Color(1, 1, 1, 1), wallColour, 0.15f)
                 .setEaseLinear()
                 .setOnUpdate((Color colour) =>
                 {
                     WallScript.goals[wall].GetComponent<Image>().color = colour;
                 });

        await Task.Delay(150);


        WallScript.generateWalls(GameInformation.round);
    }
}
