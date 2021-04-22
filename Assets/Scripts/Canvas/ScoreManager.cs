using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get
        {
            if(instance == null)
            {
                var gm = new GameObject("ScoreManager");
                instance = gm.AddComponent<ScoreManager>(); 

                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }

    private static ScoreManager instance;

    [HideInInspector]
    public int collectedCoin = 0;
    [HideInInspector]
    public int timeFromStart = 0;
    public Dictionary<string, int> highScores;

    

    public delegate void OnScoreUpdateDelegate(int coins);

    public static OnScoreUpdateDelegate OnScoreUpdate;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Debug.LogError("Score manager ha una doppia istanza nella scena");
            Destroy(this);
        }
        
        highScores = new Dictionary<string, int>();

        for (int i = 0; i < 10; i++)
        {
            highScores.Add("Host" + i, i);
        }

        ScoreTableUpdate();

        //LoadBestScore();
    }

    // aggiungere funzionalit�

    public void AddCoins(int value)
    {
        collectedCoin += value;
        OnScoreUpdate(collectedCoin);
    }

    public int GetTotalScore()
    {
        int lostPointsForTime = (int)(timeFromStart * 0.10);
        int totalScore = (collectedCoin - lostPointsForTime) * 120;
        return totalScore;
    }

    public void ResetCoinToZero()
    {
        collectedCoin = 0;
    }

    public void SaveBestScore()
    {
        SaveSystem.SaveBestScore(this);
    }

    public void LoadBestScore()
    {
        GameData data = SaveSystem.LoadBestScore();
        highScores = data.ScoreTable;
        ScoreTableUpdate();
    }

    public void SetScore(string name, int lastGameScore)
    {
        // fare giro su dizionario e cercare nome, poi controllo lo score e lo sostituisco
        // se non c'� key aggiungo
        highScores.Add(name, lastGameScore);
    }

    void ScoreTableUpdate()
    {

        if (highScores.Count == 0) return;

        int num = highScores.Count;
        if(highScores.Count >= 5)
        {
            num = 5;
        }
       
        highScores = highScores.OrderByDescending(x => x.Value).ToDictionary(X => X.Key, X => X.Value);
        //var a = from entry in highScores orderby entry.Value descending select entry;
        
        highScores = highScores.Take(num).ToDictionary(X => X.Key, X => X.Value);

    }
}
