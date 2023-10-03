using Clam;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ClamUserInput : MonoBehaviour
{

    public PlayerInput playerInput;

    //temporary
    //private GameObject m_NodeForHeirarchyOffset;
    //public GameObject clusterUI_Prefab;

    //private GameObject m_ClusterUI;

    public void Start()
    {
        //m_ClusterUI = Instantiate(clusterUI_Prefab);

        //m_ClusterUI = GameObject.Find("MenuManager").GetComponent<MenuEventManager>().GetCurrentMenu();
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
            //UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            //m_ClusterUI.GetComponent<ClusterUI_View>().Lock();

            MenuEventManager.SwitchState(Menu.Lock);

        }
        else
        {
            Debug.Log("unlocking");
            playerInput.SwitchCurrentActionMap("WorldUI");
            //UnityEngine.Cursor.lockState = CursorLockMode.None;
            //m_ClusterUI.GetComponent<ClusterUI_View>().UnLock();
            MenuEventManager.SwitchState(Menu.Unlock);

            var focusedElement = GetFocusedElement();
            if (focusedElement != null)
            {

                //focusedElement.focusable = false;
                focusedElement.Blur();
            }
        }
        //UnityEngine.Cursor.visible = !UnityEngine.Cursor.visible;
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
                        //m_ClusterUI.GetComponent<ClusterUI_View>().DisplayClusterInfo(wrapper.Data);
                        MenuEventManager.instance.GetCurrentMenu().GetComponent<ClusterUI_View>().DisplayClusterInfo(wrapper.Data);

                    }
                    else
                    {
                        //m_ClusterUI.GetComponent<ClusterUI_View>().ClearClusterInfo();
                        MenuEventManager.instance.GetCurrentMenu().GetComponent<ClusterUI_View>().ClearClusterInfo();

                    }
                    selectedNode.GetComponent<NodeScript>().ToggleSelect();

                }
            }
        }
    }

    public void OnRMC()
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

            if (!selectedNode.GetComponent<NodeScript>().IsLeaf()) //redundant?...
            {
                var lid = selectedNode.GetComponent<NodeScript>().GetLeftChildID();
                var rid = selectedNode.GetComponent<NodeScript>().GetRightChildID();

                bool hasLeft = MenuEventManager.instance.GetTree().TryGetValue(lid, out var leftChild);

                bool hasRight = MenuEventManager.instance.GetTree().TryGetValue(rid, out var rightChild);
                if (hasLeft && hasRight)
                {
                    // should i handle case of only one being active?
                    if (leftChild.activeSelf && rightChild.activeSelf)
                    {
                        //leftChild.SetActive(!leftChild.activeSelf);
                        //rightChild.SetActive(!rightChild.activeSelf);
                        ClamFFI.ForEachDFT(SetInactiveCallBack, leftChild.GetComponent<NodeScript>().GetId());
                        ClamFFI.ForEachDFT(SetInactiveCallBack, rightChild.GetComponent<NodeScript>().GetId());

                        // if active that means setting inactive - set all subsequent children inactive as well

                    }
                    else
                    {
                        //if inactive - only set immediate two children as active
                        leftChild.SetActive(true);
                        rightChild.SetActive(true);

                        // need to redraw parent child lines
                        string rootName = "1";
                        Clam.NodeWrapper wrapper = new Clam.NodeWrapper(selectedNode.GetComponent<NodeScript>().ToNodeData());
                        if (MenuEventManager.instance.GetTree().TryGetValue(rootName, out var root))
                        {
                            // tempoarary fix to prevent moving nodes around when already in reingold format
                            if (!root.activeSelf)
                            {
                                ClamFFI.DrawHeirarchyOffsetFrom(wrapper, PositionUpdater);

                            }
                        }

                        //redrawing lines here after reingold call potentially
                        var springPrefab = Resources.Load("Spring") as GameObject;
                        var leftSpring = MenuEventManager.instance.MyInstantiate(springPrefab);
                        var rightSpring = MenuEventManager.instance.MyInstantiate(springPrefab);

                        leftSpring.GetComponent<SpringScript>().SetNodes(selectedNode, leftChild);
                        rightSpring.GetComponent<SpringScript>().SetNodes(selectedNode, rightChild);

                        //leftSpring.GetComponent<SpringScript>().SetColor(Color.white);
                        //rightSpring.GetComponent<SpringScript>().SetColor(Color.white);

                    }

                }


            }
        }
    }

    unsafe void SetInactiveCallBack(ref Clam.NodeDataFFI nodeData)
    {
        bool hasValue = MenuEventManager.instance.GetTree().TryGetValue(nodeData.id.AsString, out GameObject node);
        if (hasValue)
        {
            //node.GetComponent<NodeScript>().SetColor(nodeData.color.AsColor);
            node.SetActive(false);
        }
        else
        {
            Debug.Log("set inactive key not found - " + nodeData.id);
        }
    }
    unsafe void PositionUpdater(ref Clam.NodeDataFFI nodeData)
    {

        bool hasValue = MenuEventManager.instance.GetTree().TryGetValue(nodeData.id.AsString, out GameObject node);
        if (hasValue)
        {
            //node.GetComponent<NodeScript>().SetColor(nodeData.color.AsColor);
            node.GetComponent<NodeScript>().SetPosition(nodeData.pos.AsVector3);
        }
        else
        {
            Debug.Log("reingoldify key not found - " + nodeData.id);
        }
    }
    void OnExit()
    {
        //Application.Quit();
        playerInput.SwitchCurrentActionMap("WorldUI");

        MenuEventManager.SwitchState(Menu.Pause);
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
