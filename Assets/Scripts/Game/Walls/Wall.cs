using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] GameObject wallGlow;
    [SerializeField] int side;

    private float w, camW;
    private float h, camH;

    [SerializeField] private bool _active;
    [SerializeField] private GameInformation gameManager;
    public bool isActive
    {
        get { return _active; }
        set
        {
            _active = value;
            wallGlow.SetActive(value);
        }
    }

    void Awake()
    {
        camH = (Mathf.Abs(gameManager.cam.transform.position.z) * Mathf.Tan(gameManager.cam.GetComponent<Camera>().fieldOfView / 2 * (Mathf.PI / 180)) * 2);
        h = camH / gameManager.main.GetComponent<RectTransform>().rect.height;

        camW = (camH / (gameManager.main.GetComponent<RectTransform>().rect.height / gameManager.main.GetComponent<RectTransform>().rect.width));
        w = camW / gameManager.main.GetComponent<RectTransform>().rect.height;

        this.transform.localScale = new Vector3(0.5f, camH, 5);
        this.transform.position = new Vector3(side * camW / 2, 0);
    }
}
