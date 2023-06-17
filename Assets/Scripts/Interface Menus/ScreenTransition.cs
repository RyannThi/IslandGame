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

    public IEnumerator GoToScene(string sceneName)
    {
        animator.SetTrigger("SwitchScene");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneName);
    }
}