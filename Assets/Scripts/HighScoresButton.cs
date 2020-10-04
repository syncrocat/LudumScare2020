using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HighScoresButton : MonoBehaviour
{
    public void HighScores()
    {
        SceneManager.LoadScene(2);
    }
}
