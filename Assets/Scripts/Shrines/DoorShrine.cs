using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorShrine : MonoBehaviour
{
    private ControlKeys ck;
    [SerializeField]
    private string sceneName;
    public bool checkForKeys;
    

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
                        ScreenTransition.instance.GoToScene(sceneName);
                    }
                }
            }
        }
        else if (Vector3.Distance(transform.position, PlayerCharControl.instance.transform.position) < 5)
        {
            if (ck.Player.Interact.WasPressedThisFrame())
            {
                ScreenTransition.instance.GoToScene(sceneName);
            }
        }
    }
}
