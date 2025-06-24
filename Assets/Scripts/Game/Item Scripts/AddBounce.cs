using UnityEngine;

public class AddBounce : MonoBehaviour
{
    public bool isHit;

    private void Awake()
    {
        int direction = (this.transform.position.x < 0) ? 1 : -1;
        this.GetComponent<Rigidbody>().AddForce(new Vector3(direction * (float)(Random.Range(0.025f, 0.25f)), 1) * (Random.Range(7.5f, 15f)), ForceMode.Impulse);
        this.GetComponent<Rigidbody>().AddTorque(new Vector3(direction * (float)(Random.Range(0, 20)), direction * (float)(Random.Range(0, 20)), direction * (float)(Random.Range(0, 20))) * (Random.Range(7.5f, 15)), ForceMode.Impulse);

        isHit = false;
    }
}