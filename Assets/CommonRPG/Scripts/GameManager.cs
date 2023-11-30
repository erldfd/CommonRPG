using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    private void Awake()
    {
        instance = this;
        Debug.Assert(instance);
        DontDestroyOnLoad(gameObject);
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
