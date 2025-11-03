using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    [SerializeField] private Text itemNameText;
    [SerializeField] private Text quantityText;
    [SerializeField] private Text weightText;
    [SerializeField] private Image backgroundImage;
    
    public void Setup(InventoryItem item)
    {
        itemNameText.text = item.itemName;
        quantityText.text = $"Qty: {item.quantity}";
        weightText.text = $"Weight: {item.weight}";
        
        // Visual feedback based on weight
        backgroundImage.color = GetWeightColor(item.weight);
    }
    
    private Color GetWeightColor(float weight)
    {
        return weight switch
        {
            > 5 => new Color(1f, 0.8f, 0.8f), // Light red for heavy items
            > 2 => new Color(1f, 1f, 0.8f),  // Light yellow for medium items
            _ => new Color(0.8f, 1f, 0.8f)   // Light green for light items
        };
    }
}
