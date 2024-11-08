using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("main");
    }
}
