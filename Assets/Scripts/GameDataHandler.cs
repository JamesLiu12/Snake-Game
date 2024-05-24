using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameDataHandler : MonoBehaviour
{
    public TMP_InputField playerNameInput;
    public Snake snakeScript;
    void Start()
    {
        
    }

    public void UploadData()
    {
        string playerName = playerNameInput.text;

        List<FoodTime> foodTimes = new List<FoodTime>();
        foreach (var foodTime in snakeScript.foodTimeList)
        {
            foodTimes.Add(new FoodTime { foodName = foodTime.Item1, takeTime = foodTime.Item2.ToString() });
        }

        FoodTimeListWrapper foodTimesWrapper = new FoodTimeListWrapper { foodTimes = foodTimes };
        string foodTimesJson = JsonUtility.ToJson(foodTimesWrapper);
        
        Debug.Log(foodTimes.Count);
        Debug.Log(foodTimesJson);

        StartCoroutine(PostData(playerName, foodTimesJson));
    }

    IEnumerator PostData(string playerName, string foodTimesJson)
    {
        WWWForm form = new WWWForm();
        form.AddField("playerName", playerName);
        form.AddField("foodTimes", foodTimesJson);

        using UnityWebRequest www = UnityWebRequest.Post("http://localhost/Snake/uploadData.php", form);
        yield return www.SendWebRequest();

        Debug.Log(www.result != UnityWebRequest.Result.Success ? www.error : www.downloadHandler.text);
    }

    [Serializable]
    private class FoodTime
    {
        public string foodName;
        public string takeTime;
    }

    [Serializable]
    private class FoodTimeListWrapper
    {
        public List<FoodTime> foodTimes;
    }
}
