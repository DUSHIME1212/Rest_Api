using UnityEngine;
using UnityEngine.UI;
using TMPro; // Add this for TextMeshPro
using System.Collections.Generic;
using System.Linq;
using System;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private TextMeshProUGUI playerNameText; // Changed from Text to TextMeshProUGUI
    [SerializeField] private TextMeshProUGUI levelText;      // Changed from Text to TextMeshProUGUI
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;     // Changed from Text to TextMeshProUGUI
    [SerializeField] private TextMeshProUGUI positionText;   // Changed from Text to TextMeshProUGUI
    [SerializeField] private Transform inventoryContent;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private TextMeshProUGUI metadataText;   // Changed from Text to TextMeshProUGUI
    [SerializeField] private Button refreshButton;
    [SerializeField] private TMP_Dropdown sortDropdown;      // Changed from Dropdown to TMP_Dropdown
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private TextMeshProUGUI errorText;      // Changed from Text to TextMeshProUGUI
    
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
    
        // Initial data load
        LoadPlayerData();
    }
    
    public void DisplayPlayerData(PlayerDataResponse data)
    {
        if (data == null) return;
        
        // Basic player info
        playerNameText.text = $"Player: {data.record.playerName}";
        levelText.text = $"Level: {data.record.level}";
        
        // Health with slider
        healthSlider.value = data.record.health / 100f;
        healthText.text = $"Health: {data.record.health}%";
        
        // Position
        positionText.text = $"Position: ({data.record.position.x}, {data.record.position.y}, {data.record.position.z})";
        
        // Inventory
        currentInventory = data.record.inventory;
        DisplayInventory(currentInventory);
        
        // Metadata
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

        // Clear existing items
        foreach (Transform child in inventoryContent)
        {
            if (child != null && child.gameObject != null)
            {
                Destroy(child.gameObject);
            }
        }
        
        // Create new items
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
                    // Fallback: Create basic UI manually
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

// Fallback method if InventoryItemUI component is missing
private void CreateBasicInventoryUI(GameObject itemGO, InventoryItem item)
{
    // Look for TextMeshPro components in children
    TextMeshProUGUI[] texts = itemGO.GetComponentsInChildren<TextMeshProUGUI>();
    
    if (texts.Length >= 3)
    {
        texts[0].text = item.itemName;
        texts[1].text = $"Qty: {item.quantity}";
        texts[2].text = $"Weight: {item.weight}";
    }
    else
    {
        // Last resort: log the item data
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