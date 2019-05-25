using UnityEngine;

public class QuitButton : MonoBehaviour
{
    void Start()
    {
#if UNITY_EDITOR

#elif UNITY_WEBGL
        gameObject.SetActive(false);
        Destroy(gameObject);
#endif
    }
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
