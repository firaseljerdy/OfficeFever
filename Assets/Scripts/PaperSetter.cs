using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperSetter : MonoBehaviour
{
    public Transform destinationDesk;
    public Transform paperStackTarget;
    public float settingDistance = 1.5f;
    public float lerpSpeed = 1.0f;

    public bool IsSetting { get; private set; }

    private List<Transform> paperStack;
    private bool isSetting;

    private bool coroutineStarted;


    private void Start()
    {
        paperStack = new List<Transform>();
        isSetting = false;
    }

    public void SetPapers(Transform paperStack)
    {
        if (!coroutineStarted)
        {
            coroutineStarted = true;
            StartCoroutine(SetPapersOnDesk(paperStack));
        }
    }


    private void Update()
    {
        if (paperStackTarget.childCount > 0 && !isSetting)
        {
            float distanceToDesk = Vector3.Distance(transform.position, destinationDesk.position);
            if (distanceToDesk <= settingDistance)
            {
                isSetting = true;
                StartCoroutine(SetPapersOnDesk(paperStackTarget));
            }
        }
    }


    public bool CheckDistance(Vector3 playerPosition)
    {
        float distance = Vector3.Distance(playerPosition, destinationDesk.position);
        return distance <= settingDistance;
    }

    private IEnumerator SetPapersOnDesk(Transform paperStackTransform)
    {
        while (paperStackTransform.childCount > 0)
        {
            Transform paper = paperStackTransform.GetChild(0);
            paper.SetParent(null);

            Vector3 targetPosition = destinationDesk.position;
            Quaternion targetRotation = destinationDesk.rotation;
            targetPosition += new Vector3(0, destinationDesk.childCount * 0.01f, 0);

            float startTime = Time.time;
            Vector3 startPosition = paper.position;
            Quaternion startRotation = paper.rotation;

            while (Time.time - startTime < lerpSpeed)
            {
                float t = (Time.time - startTime) / lerpSpeed;
                paper.position = Vector3.Lerp(startPosition, targetPosition, t);
                paper.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
                yield return null;
            }

            paper.SetParent(destinationDesk);
        }

        isSetting = false;
    }


}
