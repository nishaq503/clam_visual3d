using Clam;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ClamUserInput : MonoBehaviour
{

    public PlayerInput playerInput;
    public GameObject clusterUI_Prefab;

    private GameObject m_ClusterUI;

    public void Awake()
    {
        m_ClusterUI = Instantiate(clusterUI_Prefab);
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    public void OnChangeMap(InputValue value)
    {
        Debug.Log("change map!");
        bool uiActive = UnityEngine.Cursor.visible;

        if (uiActive)
        {
            Debug.Log("locking");
            var focusedElement = GetFocusedElement();
            if (focusedElement != null)
            {
                
                //focusedElement.focusable = false;
                focusedElement.Blur();
            }
            playerInput.SwitchCurrentActionMap("Player");
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            m_ClusterUI.GetComponent<ClusterUI_View>().Lock();



        }
        else
        {
            Debug.Log("unlocking");
            playerInput.SwitchCurrentActionMap("WorldUI");
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            m_ClusterUI.GetComponent<ClusterUI_View>().UnLock();

            var focusedElement = GetFocusedElement();
            if (focusedElement != null)
            {

                //focusedElement.focusable = false;
                focusedElement.Blur();
            }
        }
        UnityEngine.Cursor.visible = !UnityEngine.Cursor.visible;
    }

    public void OnLMC()
    {
        Debug.Log("selecting onlmc");
        Camera.main.ScreenToViewportPoint(Mouse.current.position.ReadValue());
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hitInfo;

        if (PointerIsUIHit(mousePosition))
        {
            return;
        }

        if (Physics.Raycast(ray.origin, ray.direction * 10, out hitInfo, Mathf.Infinity))
        {
            var selectedNode = hitInfo.collider.gameObject;

            Debug.Log("selexting");

            //if (!selectedNode.GetComponent<NodeScript>().Selected)
            {
                Clam.NodeWrapper wrapper = new Clam.NodeWrapper(selectedNode.GetComponent<NodeScript>().ToNodeData());
                FFIError found = Clam.ClamFFI.GetClusterData(wrapper);
                if (found == FFIError.Ok)
                {
                    if (!selectedNode.GetComponent<NodeScript>().Selected)
                    {
                        m_ClusterUI.GetComponent<ClusterUI_View>().DisplayClusterInfo(wrapper.Data);
                    }
                    else
                    {
                        m_ClusterUI.GetComponent<ClusterUI_View>().ClearClusterInfo();

                    }
                    selectedNode.GetComponent<NodeScript>().Select();

                }
            }
        }
    }

    void OnExit()
    {
        Application.Quit();
    }

    public static Focusable GetFocusedElement()
    {
        EventSystem eventSystem = EventSystem.current;
        if (eventSystem == null)
        {
            return null;
        }

        GameObject selectedGameObject = eventSystem.currentSelectedGameObject;
        if (selectedGameObject == null)
        {
            return null;
        }

        PanelEventHandler panelEventHandler = selectedGameObject.GetComponent<PanelEventHandler>();
        if (panelEventHandler != null)
        {
            return panelEventHandler.panel.focusController.focusedElement;
        }

        return null;
    }

    private bool PointerIsUIHit(Vector2 position)
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = position;
        List<RaycastResult> raycastResults = new List<RaycastResult>();

        // UI Elements must have `picking mode` set to `position` to be hit
        EventSystem.current.RaycastAll(pointer, raycastResults);

        if (raycastResults.Count > 0)
        {
            foreach (RaycastResult result in raycastResults)
            {
                if (result.distance == 0 && result.isValid)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
