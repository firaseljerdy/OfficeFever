/*
 * The MoneyPool script manages the money object pooling system to optimize performance,
 * similar to the PaperPool system. It uses a Queue to store available money objects and instantiates new ones if needed.
 * The GetMoney and ReturnMoney methods handle the retrieval and return of money objects in the pool.
 */


using System.Collections.Generic;
using UnityEngine;

public class MoneyPool : MonoBehaviour
{
    public static MoneyPool Instance;

    public GameObject moneyPrefab;
    public int initialPoolSize = 100;

    private Queue<GameObject> pooledMoney;
    private GameObject poolParent;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        pooledMoney = new Queue<GameObject>();
        poolParent = new GameObject("Money Pool");

        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject money = Instantiate(moneyPrefab);
            money.SetActive(false);
            money.transform.SetParent(poolParent.transform);
            pooledMoney.Enqueue(money);
        }
    }

    public GameObject GetMoney()
    {
        Debug.Log("GetMoney called");

        if (pooledMoney.Count > 0)
        {
            GameObject money = pooledMoney.Dequeue();
            money.SetActive(true);
            Debug.Log("Money retrieved from pool");
            return money;
        }
        else
        {
            GameObject newMoney = Instantiate(moneyPrefab);
            Debug.Log("New money instantiated");
            return newMoney;
        }
    }


    public void ReturnMoney(GameObject money)
    {
        money.SetActive(false);
        money.transform.SetParent(poolParent.transform);
        money.transform.localPosition = Vector3.zero;
        money.transform.localRotation = Quaternion.identity;
        pooledMoney.Enqueue(money);
    }
}
