using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneTransition.Load("Hub");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
