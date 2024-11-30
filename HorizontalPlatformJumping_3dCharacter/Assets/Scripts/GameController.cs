using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField, Range(0f,3f)] private float timeScale = 1f;
    void Start()
    {
        timeScale = 1f;
    }

    void Update()
    {
        Time.timeScale = timeScale;
    }
}
