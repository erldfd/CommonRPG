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

        [SerializeField]
        private AudioClip portalAudioClip;

        private AsyncOperation asyncSceneLoadOperation = null;

        private void Awake()
        {
            Debug.Assert(portalAudioClip);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player") 
            {
                MoveToDestinationScene();
            }
        }

        private void MoveToDestinationScene()
        {
            GameManager.AudioManager.PlayAudio2D(portalAudioClip, 1);

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
