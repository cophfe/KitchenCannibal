using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelsAndImages : MonoBehaviour
{
    public GameObject burgerPrefab = null;

    public Sprite BLT = null;
    public Sprite buns = null;
    public Sprite cookedMeat = null;
    public Sprite tomatoe = null;
    public Sprite lettuce = null;

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
