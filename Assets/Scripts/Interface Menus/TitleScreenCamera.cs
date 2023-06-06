using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenCamera : MonoBehaviour
{
    public Transform camera;
    private Vector3 cameraPosRef = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CameraMovement());
    }

    // Update is called once per frame
    void Update()
    {
        camera.eulerAngles += new Vector3(0, 0.9f, 0) * Time.deltaTime;
    }

    private IEnumerator CameraMovement()
    {
        Vector3 tempPos;
        while (camera.position.y < 240)
        {
            //camera.position = Vector3.SmoothDamp(camera.position, new Vector3(66, 240, 30), ref cameraPosRef, 0.002f * Time.deltaTime);

            tempPos = camera.position;
            tempPos.y = Mathf.Lerp(tempPos.y, 240, 0.225f * Time.deltaTime);
            camera.position = tempPos;

            yield return null;
        }
        
    }
}
