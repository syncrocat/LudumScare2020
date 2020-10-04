﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TopScoreManager : MonoBehaviour
{
    public List<GameObject> TopScoreObjects;
    public List<Text> TopScoreTexts;
    public List<Score> TopScores;
    // Start is called before the first frame update
    void Awake()
    {
        TopScoreTexts = TopScoreObjects.Select(o => o.GetComponent<Text>()).ToList();
        TopScores = new List<Score>();
    }

    private void Start()
    {
        string[] presetScoreNames =
        {
            "ludumdoggy",
            "ludumdoggy",
            "JacketKeeper",
            "2edgy2handle",
            "Peppy",
            "ludumdoggy",
            "Peppy",
            "ludumdoggy",
            "THE_LEGEND",
            "DOGLIKE"
        };


        for (var i = 0; i < 10; i++)
        {
            string name;
            if (PlayerPrefs.HasKey($"TopScoreName{i}"))
            {
                name = PlayerPrefs.GetString($"TopScoreName{i}");
                //name = presetScoreNames[9 - i];
            }
            else
            {
                //name = $"Player {i + 1}";
                name = presetScoreNames[9 - i];
            }

            int score;
            if (PlayerPrefs.HasKey($"TopScore{i}"))
            {
                score = PlayerPrefs.GetInt($"TopScore{i}");
            }
            else
            {
                score = 20 * i;
            }
            RegisterTopScore(new Score() { name = name, score = score });
        }

        for (var i = 0; i < TopScoreTexts.Count; i++)
        {
            var name = TopScores[i].name;
            var score = TopScores[i].score;
            var minutes = (int)(score / 60);
            var seconds = score % 60;
            TopScoreTexts[i].text = $"{i + 1}. {name} - {minutes}:{seconds:00}";
        }
    }

    public void RegisterTopScore(Score score)
    {
        TopScores.Add(score);
        TopScores.Sort((a, b) => a.score > b.score ? -1 : 1); // TODO might be backwards
        TopScores = TopScores.Take(10).ToList();
    }

    public void SaveTopScores()
    {
        for (var i = 0; i < TopScores.Count; i++)
        {
            PlayerPrefs.SetString($"TopScoreName{i}", TopScores[i].name);
            PlayerPrefs.SetInt($"TopScore{i}", TopScores[i].score);
        }
    }
}

public class Score {
    public string name;
    public int score;
}
