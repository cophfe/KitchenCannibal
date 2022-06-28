using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderRack : MonoBehaviour
{
    private Transform[] orderPositions = null;
    [SerializeField] private GameObject orderPrefab = null;
    private bool[] positionFull = null;

    // Start is called before the first frame update
    void Start()
    {
        orderPositions = new Transform[transform.childCount];
        positionFull = new bool[transform.childCount];
        for(int i = 0; i < transform.childCount; i++)
        {
            orderPositions[i] = transform.GetChild(i);
            positionFull[i] = false;
        }
    }

    public void AddOrder(Order order)
    {
        int openIndex = 0;
        bool indexFound = false;

        for(int i = 0;i < positionFull.Length; i++)
        {
            if(!positionFull[i])
            {
                openIndex = i;
                indexFound = true;
                break;
            }
        }
        
        if(indexFound)
        {
            positionFull[openIndex] = true;
            GameObject temp = Instantiate(orderPrefab);
            temp.transform.parent = orderPositions[openIndex];
            temp.transform.localPosition = Vector3.zero;
            order.display = temp.GetComponent<OrderDisplay>();
            order.display.UpdateDisplay(order);
            order.StartTime();
            order.orderRackIndex = openIndex;
            order.rack = this;
            GameManager.Instance.audioManager.PlayOneShot(SoundSources.Order, 0);
        }
    }

    public void RemoveOrder(int index)
    {
        positionFull[index] = false;
    }
}
