using System.Collections;
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
    }

    private void Start()
    {
        for (var i = 0; i < TopScoreTexts.Count; i++)
        {
            var name = TopScores[i].name;
            var score = TopScores[i].score;
            TopScoreTexts[i].text = $"{i}. {name} - {score}";
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

public class Score {
    public string name;
    public int score;
}
