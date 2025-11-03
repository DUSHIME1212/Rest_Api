using UnityEngine;

public class GameManager : MonoBehaviour
{
     public static GameManager Instance { get; private set; }
    
    public IDataService DataService { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeServices();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeServices()
    {
        DataService = new JSONBinService();
    }
}
