using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenTransition : MonoBehaviour
{
    public Animator animator;
    public static ScreenTransition instance;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator GoToScene(string sceneName, bool restartTime = false)
    {
        animator.SetTrigger("SwitchScene");
        yield return new WaitForSecondsRealtime(1f);
        if (restartTime == true)
        {
            Time.timeScale = 1f;
        }
        if (sceneName == "MainMenu" || sceneName == "Credits")
        {
            Destroy(GameObject.Find("Dont Destroy On Load"));
        }

        Debug.Log(sceneName);
        SceneManager.LoadScene(sceneName); 
        if (sceneName == "MainMenu")
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator ObjectGoToPosition(GameObject obj, Vector3 position)
    {
        animator.SetTrigger("SwitchScene");
        yield return new WaitForSecondsRealtime(1f);
        obj.transform.position = position;
    }
}
