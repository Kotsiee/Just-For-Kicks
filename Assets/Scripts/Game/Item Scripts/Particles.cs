using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Particles : MonoBehaviour
{
    public GameObject particle;
    public Color color;
    public float speed;
    public int duration;
    public int count;

    private async void Start()
    {
        Debug.Log("Started");
        for (int i = 0; i < count; i++)
        {
            GameObject particles = Instantiate(particle, this.transform);
            particles.transform.localScale = new Vector3(Random.Range(0.01f, 0.2f), Random.Range(0.01f, 0.2f), Random.Range(0.01f, 0.2f));
            particles.GetComponent<MeshRenderer>().material.color = color;
            particles.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0) * speed, ForceMode.Impulse);
            particles.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-100f, 100f), Random.Range(-100f, 100f), 0) * speed, ForceMode.Impulse);
        }

        await Task.Delay(duration);

        Destroy(this.gameObject);
    }
}
