using UnityEngine;

public class ItemActivation : MonoBehaviour
{
    public GameObject separateWall;

    private bool activated = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !activated)
        {
            ActivateItem();
        }
    }

    private void ActivateItem()
    {
        activated = true;
        separateWall.SetActive(true);
        gameObject.SetActive(false);
    }
}
