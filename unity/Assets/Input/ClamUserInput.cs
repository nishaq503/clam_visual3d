using Clam;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClamUserInput : MonoBehaviour
{

    public PlayerInput playerInput;
    public GameObject clusterUI_Prefab;

    private GameObject m_ClusterUI;

    public void Awake()
    {
        m_ClusterUI = Instantiate(clusterUI_Prefab);
    }

    public void OnChangeMap(InputValue value)
    {
        Debug.Log("change map!");
        bool uiActive = UnityEngine.Cursor.visible;

        if (uiActive)
        {
            playerInput.SwitchCurrentActionMap("Player");
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            m_ClusterUI.GetComponent<ClusterUI_View>().Lock();

        }
        else
        {
            playerInput.SwitchCurrentActionMap("WorldUI");
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            m_ClusterUI.GetComponent<ClusterUI_View>().UnLock();
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

}
