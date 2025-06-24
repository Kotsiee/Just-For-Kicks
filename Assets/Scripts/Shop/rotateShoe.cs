using UnityEngine;

public class rotateShoe : MonoBehaviour
{

    private Vector3 tStartPos = Vector3.zero;
    private Vector3 tCurrPos = Vector3.zero;
    [SerializeField] private float sensitivity;

    public void OnDragEnter()
    {
        tStartPos = Input.mousePosition;
    }

    public void onDrags()
    {
        tCurrPos = Input.mousePosition - tStartPos;

        if (Vector3.Dot(transform.up, Vector3.up) >= 0)
        {
            this.transform.Rotate(transform.up, -Vector3.Dot(tCurrPos, Camera.main.transform.right) * sensitivity, Space.World);
        }
        else
        {
            this.transform.Rotate(transform.up, Vector3.Dot(tCurrPos, Camera.main.transform.right) * sensitivity, Space.World);
        }

        if (Vector3.Dot(Camera.main.transform.right, Vector3.forward) >= 0)
        {
            this.transform.Rotate(Camera.main.transform.right, Vector3.Dot(tCurrPos, Camera.main.transform.up) * sensitivity, Space.World);
        }
        else
        {
            this.transform.Rotate(Camera.main.transform.right, -Vector3.Dot(tCurrPos, Camera.main.transform.up) * sensitivity, Space.World);
        }

        tStartPos = Input.mousePosition;
    }
}
