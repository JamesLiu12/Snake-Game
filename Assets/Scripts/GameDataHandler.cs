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
    public int uploadSuccess = 2;
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

        uploadSuccess = www.result == UnityWebRequest.Result.Success ? 1 : 0;
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

    public void RetrieveRanking(List<Tuple<int, string, int>> playerScore,
        Dictionary<int, List<Tuple<string, string>>> playerFoodTime)
    {
        StartCoroutine(GetPlayerData(playerScore, playerFoodTime));
    }

    IEnumerator GetPlayerData(List<Tuple<int, string, int>> playerScore,
        Dictionary<int,List<Tuple<string, string>>> playerFoodTime)
    {
        UnityWebRequest www = UnityWebRequest.Get("http://localhost/Snake/query.php");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            string jsonResult = www.downloadHandler.text;
            Wrapper<PlayerData> wrapper = JsonUtility.FromJson<Wrapper<PlayerData>>(jsonResult);

            List<PlayerData> playerDataList = wrapper.data;

            playerScore.Clear();
            playerFoodTime.Clear();

            HashSet<int> playerIDs = new();

            foreach (var playerData in playerDataList)
            {
                if (!playerIDs.Contains(playerData.PlayerID))
                {
                    playerScore.Add(new Tuple<int, string, int>(playerData.PlayerID, playerData.PlayerName,
                        playerData.Score));
                    playerIDs.Add(playerData.PlayerID);
                }
                if (!playerFoodTime.ContainsKey(playerData.PlayerID)) playerFoodTime.Add(playerData.PlayerID, new());
                playerFoodTime[playerData.PlayerID].Add(new Tuple<string, string>(playerData.FoodName, playerData.TakeTime));
            }
        }
    }

    [Serializable]
    public class PlayerData
    {
        public int PlayerID;
        public string PlayerName;
        public int Score;
        public string FoodName;
        public string TakeTime;
    }

    [Serializable]
    public class Wrapper<T>
    {
        public List<T> data;
    }
}