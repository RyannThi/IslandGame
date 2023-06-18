using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsBehavior1 : MonoBehaviour
{
    public RectTransform mainGroup;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MoveOverSeconds(mainGroup, new Vector3(0, 6895, 0), 23));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator MoveOverSeconds(RectTransform objectToMove, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.anchoredPosition;
        while (elapsedTime < seconds)
        {
            objectToMove.anchoredPosition = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToMove.anchoredPosition = end;

        yield return new WaitForSeconds(1);

        ScreenTransition.instance.GoToScene("MainMenu");
    }
}
