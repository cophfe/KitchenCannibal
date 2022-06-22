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
        orderPositions = GetComponentsInChildren<Transform>();
        positionFull = new bool[orderPositions.Length];
        for(int i = 0; i < positionFull.Length; i++)
        {
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

        }
    }

    public void RemoveOrder()
    {

    }
}
