using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlankTransition : MonoBehaviour
{
    public bool descend = false;

    [SerializeField]
    public CanvasGroup cameraOverlay;

    // Start is called before the first frame update
    void Start()
    {
        cameraOverlay.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (descend == true)
        {
            if (transform.position.y > 45.5f)
            {
                cameraOverlay.alpha = Mathf.Lerp(cameraOverlay.alpha, 1f, 0.8f * Time.deltaTime);
            } else
            {
                cameraOverlay.alpha = Mathf.Lerp(cameraOverlay.alpha, 0f, 0.8f * Time.deltaTime);
            }
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, 45f, transform.position.z), 0.5f * Time.deltaTime);
        }
    }
}
