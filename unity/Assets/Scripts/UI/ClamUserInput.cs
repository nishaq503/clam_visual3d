using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Clam
{

    public class ClamUserInput : MonoBehaviour
    {

        public PlayerInput m_PlayerInput;

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

        public void OnChangeMapToPlayer(InputValue value)
        {
            //Debug.Log("cuserinput changemap to player");
            BlurFocus();

            m_PlayerInput.SwitchCurrentActionMap("Player");
            MenuEventManager.SwitchState(Menu.Lock);

            //MenuEventManager.SwitchInputActionMap("Player", playerInput);

            //Debug.Log("change map!");
            //bool uiActive = UnityEngine.Cursor.visible;

            //if (uiActive)
            //{
            //    //Debug.Log("locking567");
            //    //Debug.Log("cursor is visible");


            //    //UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            //    //m_ClusterUI.GetComponent<ClusterUI_View>().Lock();


            //}
            //else
            //{
            //    //Debug.Log("unlocking567");
            //    //playerInput.SwitchCurrentActionMap("WorldUI");
            //    ////UnityEngine.Cursor.lockState = CursorLockMode.None;
            //    ////m_ClusterUI.GetComponent<ClusterUI_View>().UnLock();
            //    //MenuEventManager.SwitchState(Menu.Unlock);

            //    //var focusedElement = GetFocusedElement();
            //    //if (focusedElement != null)
            //    //{

            //    //    //focusedElement.focusable = false;
            //    //    focusedElement.Blur();
            //    //}
            //}
            //UnityEngine.Cursor.visible = !UnityEngine.Cursor.visible;
        }

        public void BlurFocus()
        {
            var focusedElement = GetFocusedElement();
            if (focusedElement != null)
            {

                //focusedElement.focusable = false;
                focusedElement.Blur();
            }
        }

        public void OnLMC()
        {
            //Debug.Log("selecting onlmc");
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

                //Debug.Log("selexting");

                //if (!selectedNode.GetComponent<NodeScript>().Selected)
                {
                    //Clam.FFI.ClusterDataWrapper wrapper = new Clam.FFI.ClusterDataWrapper(selectedNode.GetComponent<Node>().ToNodeData());
                    //FFIError found = Clam.FFI.NativeMethods.GetClusterData(wrapper);
                    Clam.FFI.ClusterDataWrapper wrapper = Clam.FFI.NativeMethods.CreateClusterDataWrapper(selectedNode.GetComponent<Node>().GetId());
                    if (wrapper != null)
                    {
                        if (!selectedNode.GetComponent<Node>().Selected)
                        {
                            //m_ClusterUI.GetComponent<ClusterUI_View>().DisplayClusterInfo(wrapper.Data);
                            MenuEventManager.instance.GetCurrentMenu().GetComponent<SideMenu>().DisplayClusterInfo(wrapper.Data);
                            //Debug.Log(wrapper.Data.id.AsString);
                        }
                        else
                        {
                            //m_ClusterUI.GetComponent<ClusterUI_View>().ClearClusterInfo();
                            MenuEventManager.instance.GetCurrentMenu().GetComponent<SideMenu>().ClearClusterInfo();

                        }
                        selectedNode.GetComponent<Node>().ToggleSelect();
                        

                    }
                    else
                    {
                        Debug.LogError("wrapper was null in Create ClusterData");
                    }
                    
                }
            }
        }

        public void OnRMC()
        {
            //Debug.Log("selecting onlmc");
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

                if (!selectedNode.GetComponent<Node>().IsLeaf()) //redundant?...
                {
                    var lid = selectedNode.GetComponent<Node>().GetLeftChildID();
                    var rid = selectedNode.GetComponent<Node>().GetRightChildID();
                    GameObject leftChild = null;
                    GameObject rightChild = null;

                    //refactor all of this nonsense later ffs
                    if (!Cakes.Tree.Contains(lid))
                    {
                        leftChild = Cakes.Tree.Add(lid);
                        leftChild.SetActive(false);

                    }
                    if (!Cakes.Tree.Contains(rid))
                    {
                        rightChild = Cakes.Tree.Add(rid);
                        rightChild.SetActive(false);
                    }

                    //var leftChild = Cakes.Tree.GetOrAdd(selectedNode.GetComponent<Node>().GetLeftChildID());
                    //var rightChild = Cakes.Tree.GetOrAdd(selectedNode.GetComponent<Node>().GetRightChildID());

                    bool hasLeft = Cakes.Tree.GetTree().TryGetValue(lid, out leftChild);

                    bool hasRight = Cakes.Tree.GetTree().TryGetValue(rid, out rightChild);
                    //if (hasLeft && hasRight)
                    {
                        // should i handle case of only one being active?
                        if (leftChild.activeSelf && rightChild.activeSelf)
                        {
                            //leftChild.SetActive(!leftChild.activeSelf);
                            //rightChild.SetActive(!rightChild.activeSelf);
                            Clam.FFI.NativeMethods.ForEachDFT(SetInactiveCallBack, leftChild.GetComponent<Node>().GetId());
                            Clam.FFI.NativeMethods.ForEachDFT(SetInactiveCallBack, rightChild.GetComponent<Node>().GetId());

                            // if active that means setting inactive - set all subsequent children inactive as well

                        }
                        else
                        {
                            Debug.Log("else make children visible");
                            //if inactive - only set immediate two children as active
                            //if (Cakes.Tree.Contains)
                            leftChild.SetActive(true);
                            rightChild.SetActive(true);

                            // need to redraw parent child lines
                            //string rootName = "1";
                            var wrapper = Clam.FFI.NativeMethods.CreateClusterDataWrapper(selectedNode.GetComponent<Node>().GetId());
                            //Clam.FFI.ClusterDataWrapper wrapper = new Clam.FFI.ClusterDataWrapper();
                            //if (MenuEventManager.instance.GetTree().TryGetValue(rootName, out var root))
                            {
                                // tempoarary fix to prevent moving nodes around when already in reingold format
                                //if (!root.activeSelf)
                                {
                                    Clam.FFI.NativeMethods.DrawHierarchyOffsetFrom(wrapper, PositionUpdater);

                                }
                            }

                            //redrawing lines here after reingold call potentially
                            var springPrefab = Resources.Load("Spring") as GameObject;
                            var leftSpring = MenuEventManager.instance.MyInstantiate(springPrefab);
                            var rightSpring = MenuEventManager.instance.MyInstantiate(springPrefab);

                            leftSpring.GetComponent<Edge>().SetNodes(selectedNode, leftChild);
                            rightSpring.GetComponent<Edge>().SetNodes(selectedNode, rightChild);

                            //leftSpring.GetComponent<SpringScript>().SetColor(Color.white);
                            //rightSpring.GetComponent<SpringScript>().SetColor(Color.white);

                        }

                    }


                }
            }
        }

        unsafe void SetInactiveCallBack(ref FFI.ClusterData nodeData)
        {
            bool hasValue = Cakes.Tree.GetTree().TryGetValue(nodeData.id.AsString, out GameObject node);
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
        unsafe void PositionUpdater(ref Clam.FFI.ClusterData nodeData)
        {

            bool hasValue = Cakes.Tree.GetTree().TryGetValue(nodeData.id.AsString, out GameObject node);
            if (hasValue)
            {
                //node.GetComponent<NodeScript>().SetColor(nodeData.color.AsColor);
                node.GetComponent<Node>().SetPosition(nodeData.pos.AsVector3);
            }
            else
            {
                Debug.Log("reingoldify key not found - " + nodeData.id);
            }
        }
        void OnExit()
        {
            //Application.Quit();
            m_PlayerInput.SwitchCurrentActionMap("WorldUI");

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
}

