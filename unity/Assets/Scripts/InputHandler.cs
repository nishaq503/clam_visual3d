using ClamFFI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InputHandler 
{
    private GameObject m_SelectedNode;
    private Color m_SelectedNodeActualColor;


    public void Update(ClusterUI_Script uiScript, GameObject tree)
    {
        HandleLMC(uiScript);
        HandleRMC(tree);
        MyQuit();
    }

    void MyQuit()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //print("quitting app");
            Application.Quit();
        }
    }

    void HandleLMC(ClusterUI_Script userInterface)
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray.origin, ray.direction * 10, out hitInfo, Mathf.Infinity))
            {
                if (m_SelectedNode != null)
                {
                    if (m_SelectedNode.GetComponent<NodeScript>().GetId() == hitInfo.collider.gameObject.GetComponent<NodeScript>().GetId())
                    {
                        m_SelectedNode.GetComponent<NodeScript>().SetColor(m_SelectedNodeActualColor);
                        m_SelectedNode = null;
                        userInterface.SetSelectedClusterInfo("");
                        
                        return;
                    }
                    else
                    {
                        m_SelectedNode.GetComponent<NodeScript>().SetColor(m_SelectedNodeActualColor);
                    }
                }

                m_SelectedNode = hitInfo.collider.gameObject;
                m_SelectedNodeActualColor = m_SelectedNode.GetComponent<NodeScript>().GetColor();
                m_SelectedNode.GetComponent<NodeScript>().SetColor(new Color(0.0f, 0.0f, 1.0f));

                Debug.Log(m_SelectedNode.GetComponent<NodeScript>().GetId() + " was clicked");
                Debug.Log("searching for node " + m_SelectedNode.GetComponent<NodeScript>().GetId());

                ClamFFI.NodeWrapper nodeWrapper = new ClamFFI.NodeWrapper(m_SelectedNode.GetComponent<NodeScript>().ToNodeData());
                FFIError found = ClamFFI.Clam.GetClusterData(nodeWrapper);
                if (found == FFIError.Ok)
                {
                    nodeWrapper.Data.LogInfo();
                    userInterface.SetSelectedClusterInfo(nodeWrapper.Data.GetInfo());
                }
                else
                {
                    Debug.LogError("node not found");
                }

            }
        }
    }

    void HandleRMC(GameObject tree)
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray.origin, ray.direction * 10, out hitInfo, Mathf.Infinity))
            {


                var selectedNode = hitInfo.collider.gameObject;

                //ClamFFI.NodeWrapper nodeWrapper = new ClamFFI.NodeWrapper(selectedNode.GetComponent<NodeScript>().ToNodeData());
                if (selectedNode != null)
                {
                    //ClamFFI.Clam.ForEachDFT(DeactivateChildren, selectedNode.GetComponent<NodeScript>().GetLeftChildID());
                    //ClamFFI.Clam.ForEachDFT(DeactivateChildren, selectedNode.GetComponent<NodeScript>().GetRightChildID());
                    var hasLC = tree.GetComponent<TreeScript>().GetTree().TryGetValue(selectedNode.GetComponent<NodeScript>().GetLeftChildID(), out var lc);
                    if (hasLC)
                    {
                        tree.GetComponent<TreeScript>().SetIsActiveToChildren(selectedNode, !lc.activeSelf);
                    }
                }
               
            }
        }
    }
}
