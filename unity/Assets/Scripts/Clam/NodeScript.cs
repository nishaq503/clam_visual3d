using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public interface IClickable
{
    void OnClick();
}

public class NodeScript : MonoBehaviour
{

    private string _id;
    private string _leftChildID;
    private string _rightChildID;

    public int test = 5;
    private bool m_IsSelected = false;
    private Color m_ActualColor;
    private InputAction click;
    public float distanceToQuery = -1.0f;

    public bool Selected{ get { return m_IsSelected; } }


    //void Awake()
    //{
    //    click = new InputAction(binding: "<Mouse>/leftButton");
    //    click.performed += ctx => {
    //        RaycastHit hit;
    //        Vector3 coor = Mouse.current.position.ReadValue();
    //        if (Physics.Raycast(Camera.main.ScreenPointToRay(coor), out hit))
    //        {
    //            hit.collider.GetComponent<IClickable>()?.OnClick();
    //        }
    //    };
    //    click.Enable();
    //}
    // Start is called before the first frame update
    void Start()
    {
        m_ActualColor = new Color(153.0f / 255.0f, 50.0f / 255.0f, 204.0f / 255.0f);
    }
    //public void OnClick()
    //{
    //    Debug.Log("somebody clicked me" + " " + _id);
    //}
    // Update is called once per frame
    void Update()
    {
    }

    public void ToggleSelect()
    {
        if (m_IsSelected)
        {
            SetColor(m_ActualColor);
            m_IsSelected = false;
        }
        else
        {
            SetColor(new Color(0.0f, 0.0f, 1.0f));
            m_IsSelected = true;
        }
    }

    public void Select()
    {
        //if (m_IsSelected)
        //{
        //    SetColor(m_ActualColor);
        //    m_IsSelected = false;
        //}
        //else
        {
            SetColor(new Color(0.0f, 0.0f, 1.0f));
            m_IsSelected = true;
        }
    }

    public void Deselect()
    {
        SetColor(m_ActualColor);
        m_IsSelected = false;
    }

    public void ExpandSubtree()
    {
        //ClamFFI.Clam.ForEachDFT(m_ExpandSubtree, this._id);
    }


    public void SetPosition(Vector3 pos)
    {
        GetComponent<Transform>().position = new Vector3(pos.x, -pos.y, pos.z);
        //GetComponent<Transform>().position = pos;
    }

    public void SetColor(Color color)
    {
        //Debug.Log("setting node " + _id + " color to " + color);
        GetComponent<Renderer>().material.color = color;
    }

    public bool IsSelected()
    {
        return m_IsSelected;
    }

    public string GetId()
    {
        return _id;
    }

    public string GetLeftChildID()
    {
        return _leftChildID;
    }
    public string GetRightChildID()
    {
        return _rightChildID;
    }
    public Vector3 GetPosition()
    {
        return GetComponent<Transform>().position;
    }

    public Color GetColor()
    {
        return GetComponent<Renderer>().material.color;
    }
    public Color GetActualColor()
    {
        return m_ActualColor;
    }

    public void SetActualColor(Color color)
    {
        m_ActualColor = color;
    }

    public void SetID(string id)
    {
        _id = id;
    }

    public void SetLeft(string id)
    {
        _leftChildID = id;
    }

    public void SetRight(string id)
    {
        _rightChildID = id;
    }

    public bool IsLeaf()
    {
        print("-------------------------------------------------lid " + _leftChildID + ", rid " + _rightChildID);
        return _leftChildID == "None" && _rightChildID == "None";
    }


    public Clam.NodeDataFFI ToNodeData()
    {
        Clam.NodeDataFFI node = new Clam.NodeDataFFI(_id, _leftChildID, _rightChildID, GetPosition(), GetColor());

        return node;
    }

    public NodeDataUnity ToUnityData()
    {
        NodeDataUnity node = new NodeDataUnity(_id, _leftChildID, _rightChildID, GetPosition(), GetColor());

        return node;
    }
}
