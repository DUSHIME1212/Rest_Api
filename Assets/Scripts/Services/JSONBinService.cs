using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;

public interface IDataService
{
    void FetchPlayerData(Action<PlayerDataResponse> onSuccess, Action<string> onError);
}

public class JSONBinService : IDataService
{
    private const string API_URL = "https://api.jsonbin.io/v3/b/6686a992e41b4d34e40d06fa";
    
    public void FetchPlayerData(Action<PlayerDataResponse> onSuccess, Action<string> onError)
    {
        GameManager.Instance.StartCoroutine(FetchDataCoroutine(onSuccess, onError));
    }
    
    private IEnumerator FetchDataCoroutine(Action<PlayerDataResponse> onSuccess, Action<string> onError)
    {
        Debug.Log($"[JSONBinService] Starting to fetch data from: {API_URL}");
        
        using (UnityWebRequest request = UnityWebRequest.Get(API_URL))
        {
            Debug.Log("[JSONBinService] Sending API request...");
            yield return request.SendWebRequest();
            
            Debug.Log($"[JSONBinService] Request completed with status: {request.result}");
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string jsonText = request.downloadHandler.text;
                    Debug.Log($"[JSONBinService] Raw JSON response: {jsonText}");
                    
                    PlayerDataResponse data = JsonUtility.FromJson<PlayerDataResponse>(jsonText);
                    
                    if (data != null)
                    {
                        Debug.Log($"[JSONBinService] Successfully parsed PlayerDataResponse. Record ID: {data.metadata.id}");
                        Debug.Log($"[JSONBinService] Player Name: {data.record.playerName}, Level: {data.record.level}");
                        onSuccess?.Invoke(data);
                    }
                    else
                    {
                        string errorMsg = "[JSONBinService] Failed to parse response data - data is null";
                        Debug.LogError(errorMsg);
                        onError?.Invoke(errorMsg);
                    }
                }
                catch (Exception e)
                {
                    string errorMsg = $"[JSONBinService] JSON Parse Error: {e.Message}\n{e.StackTrace}";
                    Debug.LogError(errorMsg);
                    onError?.Invoke(errorMsg);
                }
            }
            else
            {
                string errorMsg = $"[JSONBinService] HTTP Error: {request.error}\nResponse Code: {request.responseCode}";
                Debug.LogError(errorMsg);
                onError?.Invoke(errorMsg);
            }
        }
    }
}