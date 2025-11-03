# Player Data Display - Unity Application

A Unity application that fetches and displays player data from a JSON API, featuring an interactive UI with inventory management.

## Features

### 1. Data Fetching
- Retrieves player data from a JSON API endpoint
- Handles network requests asynchronously
- Implements error handling and loading states

### 2. Player Information Display
- Shows player name and level
- Displays health with a visual slider and percentage
- Shows player position in 3D space (X, Y, Z coordinates)

### 3. Inventory Management
- Displays a scrollable list of inventory items
- Each item shows:
  - Item name
  - Quantity
  - Weight with color-coded visual feedback
- Supports sorting by:
  - Item name (A-Z)
  - Quantity (high to low)
  - Weight (high to low)

### 4. User Interface
- Built with Unity's UI Toolkit (UGUI)
- Responsive layout that adapts to different screen sizes
- Loading indicator during data fetch
- Error messages for failed operations
- Refresh button to reload data

### 5. Technical Implementation
- **MVC Architecture**: Separates data, UI, and logic
- **Services Layer**: Handles API communication
- **Event-driven**: Uses UnityEvents for UI interactions
- **Error Handling**: Comprehensive error handling and logging

## Setup Instructions

 **Running the Application**
   - Press Play in the Unity Editor
   - The app will automatically fetch and display player data
   - Use the sort dropdown to reorder inventory items
   - Click the refresh button to reload data

## File Structure

- `Assets/`
  - `Scripts/`
    - [UIManager.cs](cci:7://file:///C:/Users/don360/Desktop/Nextjs/PlayerDataDisplay/Assets/Scripts/UIManager.cs:0:0-0:0) - Handles all UI updates and user interactions
    - `JSONBinService.cs` - Manages API communication
    - `GameManager.cs` - Central game controller
    - [InventoryItemUI.cs](cci:7://file:///C:/Users/don360/Desktop/Nextjs/PlayerDataDisplay/Assets/Scripts/InventoryItemUI.cs:0:0-0:0) - Controls individual inventory item display
    - `Data/` - Contains data models
      - `PlayerDataClasses.cs` - Data structure definitions

## Dependencies

- Unity Engine 2021.3+
- TextMeshPro (included in Unity)
- Newtonsoft.Json (for JSON parsing)

## Troubleshooting

- **Missing UI References**: Ensure all UI elements are assigned in the Unity Inspector
- **Network Issues**: Verify internet connection and API endpoint
- **Empty Inventory**: Check the JSON data structure matches the expected format
