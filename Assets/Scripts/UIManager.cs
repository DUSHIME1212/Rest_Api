using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using System.Collections.Generic;
using System.Linq;
using System;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private TextMeshProUGUI playerNameText; 
    [SerializeField] private TextMeshProUGUI levelText;      
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;     
    [SerializeField] private TextMeshProUGUI positionText;   
    [SerializeField] private Transform inventoryContent;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private TextMeshProUGUI metadataText;   
    [SerializeField] private Button refreshButton;
    [SerializeField] private TMP_Dropdown sortDropdown;      
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private TextMeshProUGUI errorText;      
    
    private List<InventoryItem> currentInventory;
    
    private void Start()
    {
        if (refreshButton != null)
        {
            refreshButton.onClick.AddListener(OnRefreshClicked);
        }
        if (sortDropdown != null)
        {
            sortDropdown.onValueChanged.AddListener(OnSortChanged);
        }
    
        
        LoadPlayerData();
    }
    
    public void DisplayPlayerData(PlayerDataResponse data)
    {
        if (data == null) return;
        
        
        playerNameText.text = $"Player: {data.record.playerName}";
        levelText.text = $"Level: {data.record.level}";
        
        
        healthSlider.value = data.record.health / 100f;
        healthText.text = $"Health: {data.record.health}%";
        
        
        positionText.text = $"Position: ({data.record.position.x}, {data.record.position.y}, {data.record.position.z})";
        
        
        currentInventory = data.record.inventory;
        DisplayInventory(currentInventory);
        
        
        metadataText.text = $"Created: {System.DateTime.Parse(data.metadata.createdAt).ToString("MMM dd, yyyy HH:mm")}";
        
        HideLoading();
        HideError();
    }
    
    private void DisplayInventory(List<InventoryItem> inventory)
{
    try
    {
        if (inventory == null)
        {
            Debug.LogError("Inventory list is null!");
            return;
        }

        if (inventoryItemPrefab == null)
        {
            Debug.LogError("Inventory Item Prefab is not assigned!");
            return;
        }
        
        if (inventoryContent == null)
        {
            Debug.LogError("Inventory Content is not assigned!");
            return;
        }

        
        foreach (Transform child in inventoryContent)
        {
            if (child != null && child.gameObject != null)
            {
                Destroy(child.gameObject);
            }
        }
        
        
        foreach (var item in inventory)
        {
            if (item == null) continue;
            
            try
            {
                GameObject itemGO = Instantiate(inventoryItemPrefab, inventoryContent);
                if (itemGO == null)
                {
                    Debug.LogError("Failed to instantiate inventory item prefab");
                    continue;
                }
                
                var itemUI = itemGO.GetComponent<InventoryItemUI>();
                if (itemUI != null)
                {
                    itemUI.Setup(item);
                }
                else
                {
                    
                    Debug.LogWarning("InventoryItemUI component not found, creating basic UI");
                    CreateBasicInventoryUI(itemGO, item);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error creating inventory item: {ex.Message}");
            }
        }
    }
    catch (Exception ex)
    {
        Debug.LogError($"Error in DisplayInventory: {ex}");
        ShowError($"Error displaying inventory: {ex.Message}");
    }
}


private void CreateBasicInventoryUI(GameObject itemGO, InventoryItem item)
{
    
    TextMeshProUGUI[] texts = itemGO.GetComponentsInChildren<TextMeshProUGUI>();
    
    if (texts.Length >= 3)
    {
        texts[0].text = item.itemName;
        texts[1].text = $"Qty: {item.quantity}";
        texts[2].text = $"Weight: {item.weight}";
    }
    else
    {
        
        Debug.Log($"Item: {item.itemName}, Qty: {item.quantity}, Weight: {item.weight}");
    }
}
    
    private void OnSortChanged(int sortIndex)
    {
        if (currentInventory == null) return;
        
        List<InventoryItem> sortedInventory = sortIndex switch
        {
            1 => currentInventory.OrderBy(item => item.itemName).ToList(),
            2 => currentInventory.OrderByDescending(item => item.quantity).ToList(),
            3 => currentInventory.OrderByDescending(item => item.weight).ToList(),
            _ => currentInventory
        };
        
        DisplayInventory(sortedInventory);
    }
    
    private void OnRefreshClicked()
    {
        LoadPlayerData();
    }
    
    private void LoadPlayerData()
    {
        try
        {
            if (GameManager.Instance?.DataService == null)
            {
                ShowError("GameManager or DataService not initialized");
                return;
            }
            
            ShowLoading();
            GameManager.Instance.DataService.FetchPlayerData(
                DisplayPlayerData,
                error => 
                {
                    Debug.Log($"Error fetching player data: {error}");
                    ShowError($"Failed to load data: {error}");
                }
            );
        }
        catch (Exception ex)
        {
            ShowError($"Error: {ex.Message}");
            HideLoading();
        }
    }
    
    private void ShowLoading()
    {
        loadingPanel.SetActive(true);
        errorText.gameObject.SetActive(false);
    }
    
    private void HideLoading()
    {
        loadingPanel.SetActive(false);
    }
    
    private void ShowError(string message)
    {
        errorText.text = message;
        errorText.gameObject.SetActive(true);
        loadingPanel.SetActive(false);
    }
    
    private void HideError()
    {
        errorText.gameObject.SetActive(false);
    }
}