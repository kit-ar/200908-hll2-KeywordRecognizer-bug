using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class DebugConsole : MonoBehaviour
{
    public enum DebugConsoleLevel
    {
        Debug,
        Warning,
        Error
    }

    private const int MAX_LENGTH_LOG_ENTRY = 300;

    public GameObject LogEntryPrefab;
    public GameObject LogPanelGameObject;
    public Color normal = Color.green;
    public Color warning = Color.yellow;
    public Color error = Color.red;
    public int maxMessages = 20;                   // The max number of messages displayed
    public ArrayList LogEntries = new ArrayList();
    public bool visible = true;                    // Does output show on screen by default or do we have to enable it with code? 
    public bool LogUnity = false;
    public static bool isVisible
    {
        get
        {
            return DebugConsole.instance.visible;
        }

        set
        {
            DebugConsole.instance.visible = value;
            if (value == true)
            {
                DebugConsole.instance.Display();
            }
            else if (value == false)
            {
                DebugConsole.instance.ClearScreen();
            }
        }
    }

    protected static System.Diagnostics.Stopwatch _timeStamper = new System.Diagnostics.Stopwatch();

    private static DebugConsole s_Instance = null;   // Our instance to allow this script to be called without a direct connection.
    public static DebugConsole instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(DebugConsole)) as DebugConsole;
                if (s_Instance == null)
                {
                    GameObject console = new GameObject();
                    console.AddComponent<DebugConsole>();
                    console.name = "DebugConsoleController";
                    s_Instance = FindObjectOfType(typeof(DebugConsole)) as DebugConsole;
                    DebugConsole.instance.InitGuis();
                }

            }

            return s_Instance;
        }
    }

    void Awake()
    {
        s_Instance = this;
        InitGuis();

    }

    protected bool guisCreated = false;
    protected float screenHeight = -1;
    public void InitGuis()
    {
        _timeStamper.Start();

        isVisible = visible;
    }

    void Update()
    {
        // If we are visible and the screenHeight has changed, reset linespacing
        if (visible == true && screenHeight != Screen.height)
        {
            InitGuis();
        }
    }
    //+++++++++ INTERFACE FUNCTIONS ++++++++++++++++++++++++++++++++
    public static void Log(string message, DebugConsoleLevel level)
    {
        DebugConsole.instance.AddMessage(message, level);
    }

    public static void LogWithTime(string message, DebugConsoleLevel level = DebugConsoleLevel.Debug)
    {
        double timeStampInSeconds = 1e-3 * _timeStamper.ElapsedMilliseconds;
        string timePrefix = string.Format("{0:00000.000}", timeStampInSeconds);
        DebugConsole.Log(timePrefix + ": " + message, level);
    }

    //++++ OVERLOAD ++++
    public static void Log(string message)
    {
        DebugConsole.instance.AddMessage(message);
    }

    public static void LogWithTime(string message)
    {
        double timeStampInSeconds = 1.1;//1e-3 * _timeStamper.ElapsedMilliseconds;
        string timePrefix = string.Format("{0:00000.000}", timeStampInSeconds);
        DebugConsole.Log(timePrefix + ": " + message);
    }

    public static void Clear()
    {
        DebugConsole.instance.ClearMessages();
    }
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public void AddMessage(string message, DebugConsoleLevel level)
    {
        Prune();

        Color actualColor = normal;
        if (level == DebugConsoleLevel.Error)
            actualColor = error;
        if (level == DebugConsoleLevel.Warning)
            actualColor = warning;

        GameObject go = Instantiate(LogEntryPrefab, LogPanelGameObject.transform);
        go.GetComponent<Text>().color = actualColor;
        if (message.Length > MAX_LENGTH_LOG_ENTRY)
        {
            message = message.Substring(0, MAX_LENGTH_LOG_ENTRY) + "... [" + (message.Length - MAX_LENGTH_LOG_ENTRY) + " characters surpressed]";
        }
        go.GetComponent<Text>().text = message;

        LogEntries.Add(go);
    }

    public void AddMessage(string message)
    {
        AddMessage(message, DebugConsoleLevel.Debug);
    }

    public void ClearMessages()
    {
        foreach (GameObject go in LogEntries)
        {
            Destroy(go);
        }
        LogEntries.Clear();
    }

    void ClearScreen()
    {
        this.gameObject.SetActive(false);
    }

    private void Display()
    {
        this.gameObject.SetActive(true);
    }

    void Prune()
    {
        int diff;
        if (LogEntries.Count > maxMessages)
        {
            if (LogEntries.Count <= 0)
            {
                diff = 0;
            }
            else
            {
                diff = LogEntries.Count - maxMessages;
            }

            for (int i = 0; i < diff; i++)
            {
                Destroy((GameObject)LogEntries[i]);
            }

            LogEntries.RemoveRange(0, diff);
        }

    }

    void OnEnable()
    {
        if(LogUnity)
            Application.logMessageReceived += HandleLog;
    }
    void OnDisable()
    {
        if(LogUnity)
            Application.logMessageReceived -= HandleLog;
    }
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        switch (type)
        {
            case LogType.Error:
                LogWithTime(logString, DebugConsoleLevel.Error);
                break;
            case LogType.Assert:
                LogWithTime(logString, DebugConsoleLevel.Error);
                break;
            case LogType.Warning:
                LogWithTime(logString, DebugConsoleLevel.Warning);
                break;
            case LogType.Log:
                LogWithTime(logString, DebugConsoleLevel.Debug);
                break;
            case LogType.Exception:
                LogWithTime(logString, DebugConsoleLevel.Error);
                break;
            default:
                break;
        }
    }
}
