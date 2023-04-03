using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PaperCollector : MonoBehaviour
{
    public Transform[] deskTargets;
    public Transform handTarget;
    public float collectionDistance = 1.5f;
    public float suctionForce = 5f;
    public Vector2 positionVariation = new Vector2(0.05f, 0.05f);
    public Vector2 rotationVariation = new Vector2(5f, 5f);
    public float collectionSpeed = 0.5f;
    public Vector3 handOffset;
    public float rotationSpeed = 5.0f;
    public Transform paperStackTarget;
    public PaperSetter paperSetter;

    private int paperCount;
    private bool[] coroutineStarted;
    public bool IsCollecting { get; private set; }
    private bool[] inRange;
    private void Start()
    {
        coroutineStarted = new bool[deskTargets.Length];
        inRange = new bool[deskTargets.Length];
    }

    private void Update()
    {
        for (int i = 0; i < deskTargets.Length; i++)
        {
            Transform deskTarget = deskTargets[i];
            float distanceToDesk = HorizontalDistance(transform.position, deskTarget.position);
            if (distanceToDesk <= collectionDistance)
            {
                inRange[i] = true;
                if (!coroutineStarted[i])
                {
                    coroutineStarted[i] = true;
                    StartCoroutine(CollectPapers(deskTarget, i));
                }
            }
            else
            {
                inRange[i] = false;
            }
        }

        PrinterManager.Instance.PapersCollected();

        if (paperSetter.CheckDistance(transform.position) && paperStackTarget.childCount > 0)
        {
            paperSetter.SetPapers(paperStackTarget);
        }

    }

    private float HorizontalDistance(Vector3 a, Vector3 b)
    {
        Vector2 aXZ = new Vector2(a.x, a.z);
        Vector2 bXZ = new Vector2(b.x, b.z);
        return Vector2.Distance(aXZ, bXZ);
    }

    private IEnumerator CollectPapers(Transform deskTarget, int deskIndex)
    {
        List<Transform> allPapers = new List<Transform>();

        // Get all the papers in the deskTarget
        foreach (Transform child in deskTarget)
        {
            if (child.gameObject.CompareTag("Paper"))
            {
                allPapers.Add(child);
                child.SetParent(null);
            }
        }

        bool[] paperArrived = new bool[allPapers.Count];
        int arrivedCount = 0;

        while (arrivedCount < allPapers.Count)
        {
            List<Vector3> targetPositions = allPapers.Select((paper, index) => {
                float yOffset = paperStackTarget.childCount * 0.01f;
                return paperStackTarget.position + new Vector3(0, yOffset + index * 0.01f, 0);
            }).ToList();

            if (inRange[deskIndex])
            {
                for (int i = 0; i < allPapers.Count; i++)
                {
                    if (!paperArrived[i])
                    {
                        allPapers[i].position = Vector3.MoveTowards(allPapers[i].position, targetPositions[i], collectionSpeed * Time.deltaTime);
                        allPapers[i].rotation = Quaternion.RotateTowards(allPapers[i].rotation, paperStackTarget.rotation, rotationSpeed * Time.deltaTime);

                        if (Vector3.Distance(allPapers[i].position, targetPositions[i]) < 0.01f)
                        {
                            allPapers[i].SetParent(paperStackTarget);
                            paperArrived[i] = true;
                            arrivedCount++;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < allPapers.Count; i++)
                {
                    if (!paperArrived[i])
                    {
                        Vector3 deskPaperPosition = deskTarget.TransformPoint(new Vector3(0, i * 0.01f, 0));
                        allPapers[i].position = Vector3.MoveTowards(allPapers[i].position, deskPaperPosition, collectionSpeed * Time.deltaTime);
                        allPapers[i].rotation = Quaternion.RotateTowards(allPapers[i].rotation, deskTarget.rotation, rotationSpeed * Time.deltaTime);
                    }
                }
            }

            yield return null;
        }

        paperCount += allPapers.Count;
        Debug.Log($"Collected {paperCount} paper(s)");
        coroutineStarted[deskIndex] = false;
    }
}

