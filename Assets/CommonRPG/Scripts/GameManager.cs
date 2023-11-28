using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;

    private void Awake()
    {
        _instance = this;
        if(_instance == null)
        {
            Debug.LogError("GameManager instance is null");
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager Initialize Succeeded");
        }
    }

    private void Start()
    {
        Debug.Log("GameManager Start");
    }

    private void OnEnable()
    {
        Debug.Log("GameManager OnEnable");
    }
}
