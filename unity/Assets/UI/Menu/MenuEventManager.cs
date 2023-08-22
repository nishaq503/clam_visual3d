using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEditor;

public enum Menu
{
    None,
    Main,
    CreateNewTree,
    StartClam,
    Pause,
    Lock,
    Unlock,
    ResumePlay,
    IncludeHidden,
}

public class MenuEventManager : MonoBehaviour
{
    public GameObject m_MainMenuPrefab;
    public GameObject m_CreateNewTreeMenuPrefab;
    public GameObject m_PauseMenu;
    public GameObject m_InitalMenu;

    public ClamTreeData m_TreeData;
    private GameObject m_CurrentMenu;

    private Dictionary<Menu, UnityEvent> eventDictionary;
    private Dictionary<string, GameObject> m_Tree;

    private static MenuEventManager eventManager;

    public void Start()
    {
        //SwitchToMainMenu();
        //m_CurrentMenu = Instantiate(m_InitalMenu);
        m_CurrentMenu = Instantiate(m_InitalMenu);

    }

    public GameObject GetCurrentMenu()
    {
        return m_CurrentMenu;
    }

    public static MenuEventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(MenuEventManager)) as MenuEventManager;
                //m_CurrentMenu = Instantiate(m_InitalMenu);


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

    public Dictionary<string, GameObject> GetTree()
    {
        return m_Tree;
    }

    public void SetTree(Dictionary<string, GameObject> tree)
    {
        m_Tree = tree;
    }

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<Menu, UnityEvent>();

            StartListening(Menu.Main, SwitchToMainMenu);
            StartListening(Menu.CreateNewTree, SwitchToCreateTree);
            StartListening(Menu.StartClam, StartClam);

            StartListening(Menu.Lock, LockUserInput);
            StartListening(Menu.Unlock, UnLockUserInput);
            StartListening(Menu.Pause, Pause);
            StartListening(Menu.IncludeHidden, IncludeHiddenInSelection);
            //m_CurrentMenu = Instantiate(m_InitalMenu);

        }
        //if (m_CurrentMenu == null)
        {
            //m_CurrentMenu = Instantiate(m_InitalMenu);
        }

    }

    void Pause()
    {
        //var template = Resources.Load<VisualTreeAsset>("ui/PauseMenu");
        //var instance = template.Instantiate();

        var existingPauseMenu = FindObjectOfType(typeof(PauseMenu)) as PauseMenu;
        if(existingPauseMenu != null)
        {
            Debug.Log("already paused");
            return;
        }


        SwitchState(Menu.Unlock);
        var pauseMenu = Instantiate(m_PauseMenu);
        //UIHelpers.ShowPopup(pauseMenu.GetComponent<UIDocument>().rootVisualElement, pauseMenu.GetComponent<UIDocument>().rootVisualElement);

        var resumeButton = pauseMenu.GetComponent<UIDocument>().rootVisualElement.Q<Button>("Resume");

        resumeButton.clickable.clicked += () =>
        {
            //UIHelpers.PopupClose(uiDocument.rootVisualElement, pauseMenu.GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("PauseMainBackground"));
            //this.UnLockUserInput();
            Destroy(pauseMenu);
        };


    }

    void SwitchToMainMenu()
    {
        Clam.ClamFFI.ShutdownClam();
        SceneManager.LoadScene("Scenes/MainMenu");


        //m_CurrentMenu = Instantiate(m_MainMenuPrefab);
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

    private void IncludeHiddenInSelection()
    {
        m_CurrentMenu.GetComponent<ClusterUI_View>().IncludeHiddenInSelection();
    }

    private void LockUserInput()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        m_CurrentMenu.GetComponent<ClusterUI_View>().Lock();
        UnityEngine.Cursor.visible = false;

    }

    private void UnLockUserInput()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        m_CurrentMenu.GetComponent<ClusterUI_View>().UnLock();
        UnityEngine.Cursor.visible = true;

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

    //public static void LockUserInput()
    //{
    //    //UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    //    //.GetComponent<ClusterUI_View>().Lock();


    //}
}