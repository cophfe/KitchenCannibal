using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public ModelsAndImages modelsAndimages = null;
    [HideInInspector] public OrderManager orderManager = null;
    [HideInInspector] public AudioMananger audioManager = null;

    #region Singleton/Initialize
    private static GameManager m_Instance;                       // The current instance of MenuController
    public static GameManager Instance                           // The public current instance of MenuController
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

        modelsAndimages = GetComponent<ModelsAndImages>();
        if (modelsAndimages == null)
            Debug.LogError("Missing a 'ModelsAndImages' component");

        orderManager = GetComponent<OrderManager>();
        if (orderManager == null)
            Debug.LogError("Missing a 'OrderManager' component");

        audioManager = GetComponent<AudioMananger>();
        if (audioManager == null)
            Debug.LogError("Missing a 'AudioManager' component");
    }
    #endregion



}
