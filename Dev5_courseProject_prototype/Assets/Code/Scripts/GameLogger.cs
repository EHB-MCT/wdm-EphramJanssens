using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class GameLogger : MonoBehaviour
{
    public static GameLogger Instance {get; private set;}
    [Header("Backend Settings")]
    [SerializeField] private bool enableLogging = false;
    [SerializeField] private string serverUrl = "http://localhost:3000/api/log";
    [SerializeField] private string currentUserId = "Player_1";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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
