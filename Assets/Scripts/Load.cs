using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Load : MonoBehaviour
{
    // Start is called before the first frame update
    private void LateUpdate()
    {
        SceneManager.LoadScene("MainScene");
    }
}
