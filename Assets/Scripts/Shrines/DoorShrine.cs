using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorShrine : MonoBehaviour
{
    private ControlKeys ck;
    [SerializeField]
    private string sceneName;
    public bool checkForKeys = false;
    

    private void Awake() { ck = new ControlKeys(); }
    private void OnEnable() { ck.Enable(); }
    private void OnDisable() { ck.Disable(); }

    private void Update()
    {
        if(checkForKeys)
        {
            PlayerCharControl player = PlayerCharControl.instance;
            if(player.GetFireKey() && player.GetIceKey() )
            {
                if (Vector3.Distance(transform.position, PlayerCharControl.instance.transform.position) < 5)
                {
                    if (ck.Player.Interact.WasPressedThisFrame())
                    {
                        ScreenTransition.instance.StartCoroutine(ScreenTransition.instance.GoToScene(sceneName));
                        //SceneManager.LoadScene(sceneName);
                    }
                }
            }
        }
        else if (Vector3.Distance(transform.position, PlayerCharControl.instance.transform.position) < 5)
        {
            if (ck.Player.Interact.WasPressedThisFrame())
            {
                Debug.Log("Pressionou");
                ScreenTransition.instance.StartCoroutine(ScreenTransition.instance.GoToScene(sceneName));
                //SceneManager.LoadScene(sceneName);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 5);
    }
}
