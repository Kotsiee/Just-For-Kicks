using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera _sceneCamera;

    [SerializeField]
    private LayerMask _placementLayermask;

    public event Action OnClicked, OnExit;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            OnClicked?.Invoke();
        if (Input.GetKeyDown(KeyCode.Escape))
            OnExit?.Invoke();
    }

    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = _sceneCamera.nearClipPlane;
        Ray ray = _sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        Vector3 lastPosition = new Vector3();
        if (Physics.Raycast(ray, out hit, 100, _placementLayermask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }

    public GameObject GetSelectedMapObject()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = _sceneCamera.nearClipPlane;
        Ray ray = _sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        GameObject hitObject = null;
        if (Physics.Raycast(ray, out hit, 100, _placementLayermask))
        {
            hitObject = hit.transform.gameObject;
        }
        return hitObject;
    }
}