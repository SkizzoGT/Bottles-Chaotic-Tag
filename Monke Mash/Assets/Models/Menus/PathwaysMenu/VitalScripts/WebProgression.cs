using System.Collections.Generic;
using UnityEngine;

public class WebProgression : MonoBehaviour
{
    [System.Serializable]
    public class WebItem
    {
        public GameObject item;      // The actual GameObject in the scene
        public int price;            // Price of the item
        public List<WebItem> leadsTo; // Other items this item unlocks
        [HideInInspector]
        public List<LineRenderer> paths = new List<LineRenderer>();  // The paths to this item (auto-generated)
        public bool isUnlocked;      // Has this item been unlocked yet?
    }

    public List<WebItem> webItems;                 // All items in the web
    public Material notUnlockedMaterial;           // Material for not unlocked paths
    public Material unlockedMaterial;              // Material for unlocked paths
    public float lineWidth = 0.1f;                 // Line width (adjust as needed)
    public float zOffset = -0.1f;                  // Z offset for visibility
    public string sortingLayerName = "Default";    // Sorting layer for the LineRenderer
    public int sortingOrder = 1;                   // Sorting order (higher = rendered in front)

    // Prefix for PlayerPrefs (to prevent key conflicts)
    private const string PlayerPrefsKey = "WebProgression_";

    void Start()
    {
        GeneratePaths();  // Create paths between items
        LoadProgress();   // Load saved progress

        // Initialize all paths
        foreach (var webItem in webItems)
        {
            UpdatePathColor(webItem);
        }
    }

    // Function to purchase an item
    public void PurchaseItem(WebItem webItem)
    {
        if (webItem.isUnlocked)
        {
            UnityEngine.Debug.Log("Item already unlocked.");
            return;
        }

        // Assuming you have a currency system
        int playerCurrency = PlayerPrefs.GetInt("PlayerCurrency", 0);

        if (playerCurrency >= webItem.price)
        {
            playerCurrency -= webItem.price;
            PlayerPrefs.SetInt("PlayerCurrency", playerCurrency);

            webItem.isUnlocked = true;
            PlayerPrefs.SetInt(PlayerPrefsKey + webItem.item.name, 1); // Save the item as unlocked

            // Update path colors for all items this item leads to
            foreach (var leadsToItem in webItem.leadsTo)
            {
                UpdatePathColor(leadsToItem);
            }

            UpdatePathColor(webItem); // Also update this item's path colors
        }
        else
        {
            UnityEngine.Debug.Log("Not enough currency.");
        }
    }

    // Generate paths dynamically using LineRenderer for multiple leadsTo items
    private void GeneratePaths()
    {
        foreach (var webItem in webItems)
        {
            foreach (var leadsToItem in webItem.leadsTo)
            {
                // Create a new LineRenderer component for each path
                LineRenderer lineRenderer = new GameObject("Path").AddComponent<LineRenderer>();

                // Set the line's material (initially to not unlocked material)
                lineRenderer.material = notUnlockedMaterial;
                lineRenderer.startWidth = lineWidth;
                lineRenderer.endWidth = lineWidth;
                lineRenderer.positionCount = 2;
                lineRenderer.useWorldSpace = true;  // Ensure line renders in world space

                // Set sorting layer and order (to ensure lines render properly in URP)
                lineRenderer.sortingLayerName = sortingLayerName;
                lineRenderer.sortingOrder = sortingOrder;

                // Set positions from the current item to the leadsToItem (with Z offset)
                Vector3 startPos = webItem.item.transform.position;
                Vector3 endPos = leadsToItem.item.transform.position;
                startPos.z += zOffset;
                endPos.z += zOffset;

                lineRenderer.SetPosition(0, startPos);
                lineRenderer.SetPosition(1, endPos);

                // Store the line renderer in both the web item and the leadsTo item
                webItem.paths.Add(lineRenderer);
            }
        }
    }

    // Update path color based on the unlock status, for each path between the web item and its leads
    private void UpdatePathColor(WebItem webItem)
    {
        foreach (var path in webItem.paths)
        {
            if (webItem.isUnlocked)
            {
                // Change path material to the unlocked material
                path.material = unlockedMaterial;
            }
            else
            {
                // Change path material to the not unlocked material
                path.material = notUnlockedMaterial;
            }
        }
    }

    // Load progress from PlayerPrefs
    private void LoadProgress()
    {
        foreach (var webItem in webItems)
        {
            // Check if item has been unlocked
            int unlocked = PlayerPrefs.GetInt(PlayerPrefsKey + webItem.item.name, 0);
            webItem.isUnlocked = unlocked == 1;

            // Update paths for each item
            UpdatePathColor(webItem);
        }
    }

    // Clear PlayerPrefs for testing purposes
    public void ClearProgress()
    {
        foreach (var webItem in webItems)
        {
            PlayerPrefs.DeleteKey(PlayerPrefsKey + webItem.item.name);
        }
        PlayerPrefs.DeleteKey("PlayerCurrency");
    }
}
