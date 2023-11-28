using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameUI : MonoBehaviour
{
    public void OnReturnToMenuButtonClicked()
    {
        Debug.Log("Return to Menu Button Clicled.");
        SceneManager.LoadSceneAsync(0);
    }
}
