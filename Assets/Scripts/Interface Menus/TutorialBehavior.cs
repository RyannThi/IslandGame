using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBehavior : MonoBehaviour
{
    public RectTransform mainGroup;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MoveOverSeconds(mainGroup, new Vector3(655, 0, 0), 1));
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

        yield return new WaitForSeconds(6);

        elapsedTime = 0;
        startingPos = objectToMove.anchoredPosition;
        while (elapsedTime < 4)
        {
            objectToMove.anchoredPosition = Vector3.Lerp(startingPos, new Vector3(1280, 0, 0), (elapsedTime / 4));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToMove.anchoredPosition = end;
        Destroy(objectToMove.gameObject);
    }
}
