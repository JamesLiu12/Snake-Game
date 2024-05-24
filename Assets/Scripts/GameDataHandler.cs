using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameDataHandler : MonoBehaviour
{
    public InputField playerNameInput;
    public Snake snakeScript;
    void Start()
    {
        
    }

    void UploadData()
    {
        string playerName = playerNameInput.text;

        List<Dictionary<string, string>> foodTimes = new();
        foreach (var foodTime in snakeScript.foodTimeList)
        {
            Dictionary<string, string> entry = new Dictionary<string, string>
            {
                { "foodName", foodTime.Item1 },
                { "takeTime", foodTime.Item2.ToString() }
            };
            foodTimes.Add(entry);
        }

        string foodTimesJson = JsonUtility.ToJson(new SerializationWrapper(foodTimes));

        StartCoroutine(PostData(playerName, foodTimesJson));
    }

    IEnumerator PostData(string playerName, string foodTimesJson)
    {
        WWWForm form = new WWWForm();
        form.AddField("playerName", playerName);
        form.AddField("foodTimes", foodTimesJson);

        using UnityWebRequest www = UnityWebRequest.Post("http://localhost/uploadData.php", form);
        yield return www.SendWebRequest();

        Debug.Log(www.result != UnityWebRequest.Result.Success ? www.error : www.downloadHandler.text);
    }

    [Serializable]
    private class SerializationWrapper
    {
        public List<Dictionary<string, string>> items;

        public SerializationWrapper(List<Dictionary<string, string>> items)
        {
            this.items = items;
        }
    }
}
