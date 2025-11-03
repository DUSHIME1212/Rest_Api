using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Globalization;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] public Canvas mainCanvas;
    [SerializeField] public TextMeshPro playerNameText;
    [SerializeField] public TextMeshPro levelText;
    [SerializeField] public Slider healthSlider;
    [SerializeField] public TextMeshPro healthText;
    [SerializeField] public TextMeshPro positionText;
    [SerializeField] public Transform inventoryContent;
    [SerializeField] public GameObject inventoryItemPrefab;
    [SerializeField] public TextMeshPro metadataText;
    [SerializeField] public Button refreshButton;
    [SerializeField] public Dropdown sortDropdown;
    [SerializeField] public GameObject loadingPanel;
    [SerializeField] public TextMeshPro errorText;
    
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
        // Clear existing items
        foreach (Transform child in inventoryContent)
        {
            Destroy(child.gameObject);
        }
        
        // Create new items
        foreach (var item in inventory)
        {
            GameObject itemGO = Instantiate(inventoryItemPrefab, inventoryContent);
            InventoryItemUI itemUI = itemGO.GetComponent<InventoryItemUI>();
            itemUI.Setup(item);
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
                    Debug.LogError($"Error fetching player data: {error}");
                    ShowError($"Failed to load data: {error}");
                }
            );
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in LoadPlayerData: {ex}");
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