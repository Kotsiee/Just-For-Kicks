using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] Transform spawner;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

#if UNITY_EDITOR
    void Update()
    {
        float time = Time.deltaTime * 10;
        if (Input.GetMouseButton(0) & WGL_GameInfo.inPlay)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(cam.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y)).x, cam.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y)).y, 0), time);
        }

        if (spawner.childCount > 1)
        {

            GameObject nearest = spawner.GetChild(0).gameObject;

            for (int i = 0; i < spawner.childCount - 1; i++)
            {
                if (Vector3.Distance(spawner.GetChild(i).transform.position, this.transform.GetChild(0).position) < Vector3.Distance(spawner.GetChild(i + 1).transform.position, this.transform.GetChild(0).position))
                {
                    nearest = spawner.GetChild(i).gameObject;
                }
                else
                {
                    nearest = spawner.GetChild(i + 1).gameObject;
                }
            }

            Quaternion toRotation = Quaternion.LookRotation(new Vector3(nearest.transform.position.x * 8, -nearest.transform.position.y, cam.transform.position.z));
            toRotation.eulerAngles = toRotation.eulerAngles + new Vector3(-toRotation.eulerAngles.x, -90, toRotation.eulerAngles.x);
            this.transform.GetChild(0).rotation = Quaternion.Lerp(this.transform.GetChild(0).rotation, toRotation, time);
        }
        else if (spawner.childCount > 0)
        {
            GameObject nearest = spawner.GetChild(0).gameObject;
            Quaternion toRotation = Quaternion.LookRotation(new Vector3(nearest.transform.position.x * 8, -nearest.transform.position.y, cam.transform.position.z));
            toRotation.eulerAngles = toRotation.eulerAngles + new Vector3(-toRotation.eulerAngles.x, -90, toRotation.eulerAngles.x);
            this.transform.GetChild(0).rotation = Quaternion.Lerp(this.transform.GetChild(0).rotation, toRotation, time);
        }
        else
        {
            Quaternion toRotation = Quaternion.LookRotation(new Vector3(-this.transform.position.x * 8, -this.transform.position.y * 4, cam.transform.position.z));
            toRotation.eulerAngles = (!WGL_GameInfo.inPlay) ? toRotation.eulerAngles + new Vector3(-toRotation.eulerAngles.x, -180, toRotation.eulerAngles.x) : toRotation.eulerAngles + new Vector3(-toRotation.eulerAngles.x, -90, toRotation.eulerAngles.x);
            this.transform.GetChild(0).rotation = Quaternion.Lerp(this.transform.GetChild(0).rotation, toRotation, time);
        }
    }

#else
void Update()
    {
        float time = Time.deltaTime * 10;
        if (Input.touchCount > 0 & GameInformation.inPlay)
        {
            Touch touch = Input.GetTouch(0);
            this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(cam.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y)).x, cam.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y)).y, 0), time);
        }

        if (spawner.childCount > 1)
        {

            GameObject nearest = spawner.GetChild(0).gameObject;

            for (int i = 0; i < spawner.childCount - 1; i++)
            {
                if (Vector3.Distance(spawner.GetChild(i).transform.position, this.transform.GetChild(0).position) < Vector3.Distance(spawner.GetChild(i + 1).transform.position, this.transform.GetChild(0).position))
                {
                    nearest = spawner.GetChild(i).gameObject;
                }
                else
                {
                    nearest = spawner.GetChild(i + 1).gameObject;
                }
            }

            Quaternion toRotation = Quaternion.LookRotation(new Vector3(nearest.transform.position.x * 8, -nearest.transform.position.y, cam.transform.position.z));
            toRotation.eulerAngles = toRotation.eulerAngles + new Vector3(-toRotation.eulerAngles.x, -90, toRotation.eulerAngles.x);
            this.transform.GetChild(0).rotation = Quaternion.Lerp(this.transform.GetChild(0).rotation, toRotation, time);
        }
        else if (spawner.childCount > 0)
        {
            GameObject nearest = spawner.GetChild(0).gameObject;
            Quaternion toRotation = Quaternion.LookRotation(new Vector3(nearest.transform.position.x * 8, -nearest.transform.position.y, cam.transform.position.z));
            toRotation.eulerAngles = toRotation.eulerAngles + new Vector3(-toRotation.eulerAngles.x, -90, toRotation.eulerAngles.x);
            this.transform.GetChild(0).rotation = Quaternion.Lerp(this.transform.GetChild(0).rotation, toRotation, time);
        }
        else
        {
            Quaternion toRotation = Quaternion.LookRotation(new Vector3(-this.transform.position.x * 8, -this.transform.position.y * 4, cam.transform.position.z));
            toRotation.eulerAngles = (!GameInformation.inPlay) ? toRotation.eulerAngles + new Vector3(-toRotation.eulerAngles.x, -180, toRotation.eulerAngles.x) : toRotation.eulerAngles + new Vector3(-toRotation.eulerAngles.x, -90, toRotation.eulerAngles.x);
            this.transform.GetChild(0).rotation = Quaternion.Lerp(this.transform.GetChild(0).rotation, toRotation, time);
        }
    }
#endif
}
