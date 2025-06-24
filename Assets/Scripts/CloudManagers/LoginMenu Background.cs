using UnityEngine;

public class LoginMenuBackground : MonoBehaviour
{
    [SerializeField] private GameObject spinner;


    private void Update()
    {
        //background.color = new Color(0, 1f - Mathf.Abs(Mathf.Sin(Time.fixedTime) * 0.3f), 1f - Mathf.Abs(Mathf.Sin(Time.fixedTime) * 0.3f));
        spinner.transform.rotation = Quaternion.Euler(Time.fixedTime * 160, -Time.fixedTime * 160, Time.fixedTime * 160);
    }
}
