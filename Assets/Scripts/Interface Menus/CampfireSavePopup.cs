using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampfireSavePopup : MonoBehaviour
{
    public RectTransform image;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MoveOverSeconds(image, new Vector3(-905, 0, 0), 0.4f));
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

        yield return new WaitForSeconds(1.5f);

        elapsedTime = 0;
        startingPos = objectToMove.anchoredPosition;
        while (elapsedTime < 0.4f)
        {
            objectToMove.anchoredPosition = Vector3.Lerp(startingPos, new Vector3(-1090, 0, 0), (elapsedTime / 0.4f));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToMove.anchoredPosition = end;
        Destroy(objectToMove.gameObject);
    }
}
