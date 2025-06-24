using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.EventSystems;

public class Obsticle : MonoBehaviour
{
    GameObject player, eventSystem;
    private void Awake()
    {
        this.transform.localScale = new Vector3(80, 80, 80);
        player = GameObject.Find("Player").transform.GetChild(0).gameObject;
        eventSystem = GameObject.Find("EventSystem");
    }

    private void Update()
    {
        if (this.transform.position.y <= -GameInformation.camHeight / 2 - 1)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == player.transform.parent.name)
        {
            eventSystem.GetComponent<Game>().sfx.GetComponent<SoundFX>().playAnvil();
            Vector3 dist = -(transform.position - collision.collider.transform.position);
            LeanTween.move(player.transform.parent.gameObject, new Vector3(player.transform.parent.transform.position.x + dist.x, player.transform.parent.transform.position.y + dist.y, this.transform.position.z), 0.1f);
        }
        else
            Physics.IgnoreCollision(this.GetComponent<SphereCollider>(), collision.collider, true);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Wall")
        {
            Destroy(this.gameObject);
        }
    }
}
