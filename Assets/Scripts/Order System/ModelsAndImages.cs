using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelsAndImages : MonoBehaviour
{
    public GameObject burgerPrefab = null;



    private static ModelsAndImages m_Instance;                       // The current instance of MenuController
    public static ModelsAndImages Instance                           // The public current instance of MenuController
    {
        get { return m_Instance; }
    }

    void Awake()
    {
        // Initialize Singleton
        if (m_Instance != null && m_Instance != this)
            Destroy(this.gameObject);
        else
            m_Instance = this;
    }
}
