using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public void ExitGame()
    {
        Application.Quit(); // For exiting the application on Android and iOS
        Debug.Log("Application exit");
    }
}
