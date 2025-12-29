using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;

public class GameLogger : MonoBehaviour
{
    public static GameLogger Instance {get; private set;}
    [Header("Backend Settings")]
    [SerializeField] private bool enableLogging = true;
    [SerializeField] private string serverUrl = "http://localhost:3000/api/log";
    private string currentUserId;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeUserId();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeUserId()
    {
        if (PlayerPrefs.HasKey("Player_UID"))
        {
            currentUserId = PlayerPrefs.GetString("Player_UID");
        }
        else
        {
            currentUserId = Guid.NewGuid().ToString();
            PlayerPrefs.SetString("Player_UID", currentUserId);
            PlayerPrefs.Save();
        }
        Debug.Log($"User ID loaded: {currentUserId}");
        LogAction("SessionStart", new {timestamp = System.DateTime.Now.ToString()});
    }

    public void LogAction (string actionType, object payload)
    {
        if (!enableLogging) return;
        StartCoroutine(PostLogRoutine(actionType, payload));
    }

    private IEnumerator PostLogRoutine(string actionType, object payloadData)
    {
        var logEntry = new
        {
            userId = currentUserId,
            actionType = actionType,
            payload = payloadData
        };

        string jsonBody = JsonConvert.SerializeObject(logEntry);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest request = new UnityWebRequest(serverUrl,"POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"[Backend] Log '{actionType}' saved succesfully.");
            } 
            else
            {
                Debug.LogError($"[Backend] Error with logging: '{request.error}'\nresponse: {request.downloadHandler.text} ");
            }
        }
    }
}
