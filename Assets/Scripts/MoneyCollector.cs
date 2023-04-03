using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyCollector : MonoBehaviour
{
    public string moneyTag = "Money";
    public float collectionDistance = 1.5f;
    public float collectionSpeed = 0.5f;
    public float rotationSpeed = 5.0f;

    private List<GameObject> moneyObjects;

    private void Start()
    {
        moneyObjects = new List<GameObject>();
    }

    private void Update()
    {
        GameObject[] moneyArray = GameObject.FindGameObjectsWithTag(moneyTag);
        foreach (GameObject money in moneyArray)
        {
            if (!moneyObjects.Contains(money))
            {
                moneyObjects.Add(money);
            }
        }

        for (int i = 0; i < moneyObjects.Count; i++)
        {
            GameObject money = moneyObjects[i];
            if (money != null)
            {
                float distanceToMoney = Vector3.Distance(transform.position, money.transform.position);
                if (distanceToMoney <= collectionDistance && money != null)
                {
                    StartCoroutine(CollectMoney(money));
                    moneyObjects.RemoveAt(i);
                    i--;
                }
            }
            else
            {
                moneyObjects.RemoveAt(i);
                i--;
            }
            
        }
    }

    private IEnumerator CollectMoney(GameObject money)
    {
        float startTime = Time.time;
        Vector3 startPosition = money.transform.position;
        Quaternion startRotation = money.transform.rotation;

        while (money != null && Time.time - startTime < collectionSpeed)
        {
            float t = (Time.time - startTime) / collectionSpeed;
            money.transform.position = Vector3.Lerp(startPosition, transform.position, t);
            money.transform.rotation = Quaternion.Lerp(startRotation, transform.rotation, t);
            yield return null;
        }

        if (money != null)
        {
            Destroy(money);
        }
    }
}
