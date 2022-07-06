using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    public WinText winText;
    [HideInInspector] public ModelsAndImages modelsAndimages = null;
    [HideInInspector] public OrderManager orderManager = null;
    [HideInInspector] public AudioMananger audioManager = null;
    [HideInInspector] public HealthInspector healthInspector = null;
    [HideInInspector] public ScoreKeeper scoreKeeper = null;

    [HideInInspector] public List<Ingredient> activeIngredients = null;
    public float minimumCollisionVelocity = 0.5f;

	int currentOrderCount = 0;
    #region Singleton/Initialize
    private static GameManager m_Instance;                       // The current instance of MenuController
    public static GameManager Instance                           // The public current instance of MenuController
    {
        get { return m_Instance; }
    }

    private void OnEnable()
    {
        // Initialize Singleton
        if (m_Instance != null && m_Instance != this)
            Destroy(this.gameObject);
        else
            m_Instance = this;
    }

    void Awake()
    {
        modelsAndimages = GetComponent<ModelsAndImages>();
        if (modelsAndimages == null)
            Debug.LogError("Missing a 'ModelsAndImages' component");

        orderManager = GetComponent<OrderManager>();
        if (orderManager == null)
            Debug.LogError("Missing a 'OrderManager' component");

        audioManager = GetComponent<AudioMananger>();
        if (audioManager == null)
            Debug.LogError("Missing a 'AudioManager' component");
        
        healthInspector = GetComponent<HealthInspector>();
        if (healthInspector == null)
            Debug.LogError("Missing a 'HealthInspector' component");
        
        scoreKeeper = GetComponent<ScoreKeeper>();
        if (scoreKeeper == null)
            Debug.LogError("Missing a 'ScoreKeeper' component");

        activeIngredients = new List<Ingredient>();
    }
    #endregion

    private void Start()
    { 
        scoreKeeper.ResetScore();
    }

	public void RegisterOrder()
	{
		currentOrderCount++;
	}

	public void DeregisterOrder()
	{
		currentOrderCount--;

		if (winText && currentOrderCount <= 0)
		{
			winText.SetText(scoreKeeper);
			healthInspector.Disable();
		}
	}



	public void RegisterIngredient(Ingredient ingrdient)
    {
        activeIngredients.Add(ingrdient);
    }
    
    public void DeRegisterIngredient(Ingredient ingrdient)
    {
        activeIngredients.Remove(ingrdient);
    }
}
