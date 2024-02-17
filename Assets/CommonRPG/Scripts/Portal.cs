using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CommonRPG
{
    public class Portal : MonoBehaviour
    {
        [SerializeField]
        private BoxCollider portalCollider;

        [SerializeField]
        private string destinationSceneName;

        private AsyncOperation asyncSceneLoadOperation = null;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player") 
            {
                MoveToDestinationScene();
            }
        }

        private void MoveToDestinationScene()
        {
            asyncSceneLoadOperation = SceneManager.LoadSceneAsync(destinationSceneName);
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
