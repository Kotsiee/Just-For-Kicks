using GooglePlayGames.BasicApi;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Heart : MonoBehaviour
{
    GameObject player, eventSystem, particles;
    private void Awake()
    {
        player = GameObject.Find("Player").transform.GetChild(0).gameObject;
        eventSystem = GameObject.Find("EventSystem");
        this.transform.localScale = new Vector3(20,20,20);

        particles = eventSystem.GetComponent<GameInformation>().particles;
        particles.GetComponent<Particles>().color = new Color32(255, 0, 155, 255);
        particles.GetComponent<Particles>().speed = 5;
        particles.GetComponent<Particles>().count = 15;
        particles.GetComponent<Particles>().duration = 250;
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
            eventSystem.GetComponent<Game>().addLife();
            eventSystem.GetComponent<Game>().sfx.GetComponent<SoundFX>().playGem();
            this.GetComponent<Renderer>().enabled = false;
            this.GetComponent<SphereCollider>().enabled = false;

            Instantiate(particles, this.transform.position, Quaternion.identity);
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
