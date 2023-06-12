using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionFloater : MonoBehaviour
{
    public RectTransform floater;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        floater.eulerAngles += new Vector3(0, 1, 0);
    }
}
