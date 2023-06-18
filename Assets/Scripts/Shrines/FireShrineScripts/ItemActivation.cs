using UnityEngine;

public class ItemActivation : MonoBehaviour
{
    private ControlKeys ck;

    public GameObject separateWall;

    private bool activated = false;
    private void Awake() { ck = new ControlKeys(); }
    private void OnEnable() { ck.Enable(); }
    private void OnDisable() { ck.Disable(); }
    private void Update()
    {
        if (Vector3.Distance(transform.position, PlayerCharControl.instance.transform.position) < 5)
        {
            if (ck.Player.Interact.WasPressedThisFrame())
            {
                ActivateItem();
                //SceneManager.LoadScene(sceneName);
            }
        }
       
        
    }

    private void ActivateItem()
    {
        activated = true;
        separateWall.SetActive(false);
        gameObject.SetActive(false);
    }
}
