using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CommonRPG
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private GameManager gameManager = null;
        private AsyncOperation asyncSceneLoadOperation = null;

        private void Awake()
        {
            int gameManagerCount = FindObjectsOfType<GameManager>().Length;

            if (gameManagerCount == 0)
            {
                Instantiate(gameManager);
            }
        }

        public void OnTestGameButtonClicked()
        {
            Debug.Log("TestGameButton Clicked");

            asyncSceneLoadOperation = SceneManager.LoadSceneAsync(1);
            StartCoroutine(CheckSceneLoad());
        }

        private IEnumerator CheckSceneLoad()
        {
            while (asyncSceneLoadOperation.isDone == false)
            {
                Debug.Log($"SceneLoading : {asyncSceneLoadOperation.progress}");
                yield return null;
            }
        }
    }

}
