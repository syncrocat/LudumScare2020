using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButtonHandler : MonoBehaviour
{
    public GameObject GamerManager;
    // Start is called before the first frame update
    public void Pause()
    {
        GamerManager.GetComponent<GamerManager>().Pause();
    }

    public void Unpause()
    {
        GamerManager.GetComponent<GamerManager>().Unpause();
    }

    public void Quit()
    {
        SceneManager.LoadScene(0);
    }
}
