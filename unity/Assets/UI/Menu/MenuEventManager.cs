using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public enum Menu
{
    None,
    Main,
    CreateNewTree,
    StartClam,
    Pause,
}

public class MenuEventManager : MonoBehaviour
{
    public GameObject m_MainMenuPrefab;
    public GameObject m_CreateNewTreeMenuPrefab;



    public ClamTreeData m_TreeData;
    private GameObject m_CurrentMenu;

    private Dictionary<Menu, UnityEvent> eventDictionary;

    private static MenuEventManager eventManager;

    public void Start()
    {
        SwitchToMainMenu();
    }

    public static MenuEventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(MenuEventManager)) as MenuEventManager;

                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init();
                }
            }
            return eventManager;
        }
    }

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<Menu, UnityEvent>();

            StartListening(Menu.Main, SwitchToMainMenu);
            StartListening(Menu.CreateNewTree, SwitchToCreateTree);
            StartListening(Menu.StartClam, StartClam);
        }
    }

    void SwitchToMainMenu()
    {
        m_CurrentMenu = Instantiate(m_MainMenuPrefab);
    }
    void SwitchToCreateTree()
    {
        m_CurrentMenu = Instantiate(m_CreateNewTreeMenuPrefab);
    }

    void StartClam()
    {
        var doc = m_CurrentMenu.GetComponent<UIDocument>();
        string dataName = doc.rootVisualElement.Q<TextField>("DatasetInputField").value;
        // this error handling should be taken care of by the textfield (i.e int parse)
        int cardinality = int.Parse(doc.rootVisualElement.Q<TextField>("CardinalityInputField").value);

        //var test = doc.rootVisualElement.Q<Button>("CreateTree");
        //var treeData = ScriptableObject.CreateInstance<ClamTreeData>();
        m_TreeData.cardinality = (uint)cardinality;
        m_TreeData.dataName = dataName;
        Debug.Log("swtiching scne?");
        SceneManager.LoadScene("Scenes/MainScene");

    }

    public static void StartListening(Menu eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(Menu eventName, UnityAction listener)
    {
        if (eventManager == null) return;
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void SwitchState(Menu eventName)
    {
        Debug.Log("switching state to " + eventName.ToString());
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }
}