using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionFloater : MonoBehaviour
{
    public RectTransform floater;
    public float degreesPerSecond = 15.0f;
    public float amplitude = 0.5f;
    public float frequency = 1f;

    Vector3 posOffset = new Vector3();
    Vector3 tempPos = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        posOffset = floater.anchoredPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        floater.eulerAngles += new Vector3(0, 1, 0);

        // Float up/down with a Sin()
        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        floater.anchoredPosition = tempPos;
    }
}
