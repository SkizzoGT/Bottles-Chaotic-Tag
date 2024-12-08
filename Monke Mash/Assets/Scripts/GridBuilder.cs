using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GridBuilder : MonoBehaviour
{
    [Header("Grid Settings")]
    public float gridSize = 1.0f; // Size of each grid cell
    public Vector3 gridDimensions = new Vector3(10, 10, 10); // Total grid dimensions in cells

    [Header("XR Controllers")]
    public XRController leftHandController;
    public XRController rightHandController;

    [System.Serializable]
    public class BlockElement
    {
        public string Name;
        public GameObject BlockPrefab;
        public GameObject PreviewModel;
        public GameObject cellTakenModel; // Model to display when a cell is occupied
        public int cellsX = 1; // Number of cells the block takes in X-axis
        public int cellsY = 1; // Number of cells the block takes in Y-axis
        public int cellsZ = 1; // Number of cells the block takes in Z-axis
    }

    [Header("Blocks List")]
    public List<BlockElement> blocks = new List<BlockElement>();

    private GameObject previewObject;
    private XRController activeController;
    private bool isTriggerHeld = false;
    private bool isPlacing = false;
    private float rotationAngle = 0f;

    private HashSet<Vector3> takenGridCells = new HashSet<Vector3>();
    private Vector3 lerpTargetPosition;

    private void Update()
    {
        HandleControllerInput(leftHandController);
        HandleControllerInput(rightHandController);

        if (previewObject != null && activeController != null)
        {
            UpdatePreviewPosition();
            ValidatePreviewPlacement();
        }
    }

    private void HandleControllerInput(XRController controller)
    {
        if (controller.inputDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.trigger, out float triggerValue))
        {
            if (triggerValue > 0.8f && !isTriggerHeld) // Trigger pressed
            {
                isTriggerHeld = true;
                activeController = controller;
                SpawnPreview();
            }
            else if (triggerValue < 0.2f && isTriggerHeld) // Trigger released
            {
                isTriggerHeld = false;
                PlaceBlock();
            }
        }

        if (controller.inputDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out Vector2 thumbstick))
        {
            if (thumbstick.x > 0.8f) // Rotate right
            {
                RotatePreview(90f);
            }
            else if (thumbstick.x < -0.8f) // Rotate left
            {
                RotatePreview(-90f);
            }
        }
    }

    private void SpawnPreview()
    {
        if (blocks.Count > 0 && blocks[0].PreviewModel != null && previewObject == null)
        {
            previewObject = Instantiate(blocks[0].PreviewModel);
            previewObject.SetActive(true); // Ensure it's visible
        }
    }

    private void UpdatePreviewPosition()
    {
        Vector3 controllerPosition = activeController.transform.position;
        Vector3 snappedPosition = GetSnappedPosition(controllerPosition);

        // Smoothly move the preview object to the target position
        lerpTargetPosition = snappedPosition;
        previewObject.transform.position = Vector3.Lerp(previewObject.transform.position, lerpTargetPosition, Time.deltaTime * 10f);
        previewObject.transform.rotation = Quaternion.Euler(0, rotationAngle, 0); // Maintain snapped rotation
    }

    private void ValidatePreviewPlacement()
    {
        Vector3 previewPosition = previewObject.transform.position;

        if (!IsValidPlacement(previewPosition, blocks[0]))
        {
            SetPreviewColor(Color.red); // Invalid position feedback
        }
        else
        {
            SetPreviewColor(Color.green); // Valid position feedback
        }
    }

    private void PlaceBlock()
    {
        if (previewObject == null || isPlacing) return;

        Vector3 targetPosition = previewObject.transform.position;

        if (IsValidPlacement(targetPosition, blocks[0]))
        {
            // Place the block
            isPlacing = true;
            GameObject placedBlock = Instantiate(blocks[0].BlockPrefab, targetPosition, Quaternion.Euler(0, rotationAngle, 0));
            MarkCellsAsTaken(targetPosition, blocks[0]);
        }

        Destroy(previewObject);
        previewObject = null;
        isPlacing = false;
    }

    private Vector3 GetSnappedPosition(Vector3 originalPosition)
    {
        return new Vector3(
            Mathf.Round(originalPosition.x / gridSize) * gridSize,
            Mathf.Round(originalPosition.y / gridSize) * gridSize,
            Mathf.Round(originalPosition.z / gridSize) * gridSize
        );
    }

    private bool IsValidPlacement(Vector3 position, BlockElement block)
    {
        return IsWithinGridBounds(position, block) &&
               !IsAnyCellTaken(position, block) &&
               IsOnValidBase(position, block);
    }

    private bool IsWithinGridBounds(Vector3 position, BlockElement block)
    {
        Vector3 localPosition = position - transform.position;

        return localPosition.x >= 0 && localPosition.x + (block.cellsX * gridSize) <= gridDimensions.x * gridSize &&
               localPosition.y >= 0 && localPosition.y + (block.cellsY * gridSize) <= gridDimensions.y * gridSize &&
               localPosition.z >= 0 && localPosition.z + (block.cellsZ * gridSize) <= gridDimensions.z * gridSize;
    }

    private bool IsAnyCellTaken(Vector3 position, BlockElement block)
    {
        Vector3[] occupiedCells = GetOccupiedCells(position, block);

        foreach (Vector3 cell in occupiedCells)
        {
            if (takenGridCells.Contains(cell))
            {
                return true; // At least one cell is already taken
            }
        }

        return false;
    }

    private bool IsOnValidBase(Vector3 position, BlockElement block)
    {
        Vector3 baseCell = GetSnappedPosition(position) - new Vector3(0, gridSize, 0);

        return baseCell.y >= 0 && takenGridCells.Contains(baseCell); // Must be on a base or adjacent block
    }

    private void MarkCellsAsTaken(Vector3 position, BlockElement block)
    {
        Vector3[] occupiedCells = GetOccupiedCells(position, block);

        foreach (Vector3 cell in occupiedCells)
        {
            takenGridCells.Add(cell);
        }
    }

    private Vector3[] GetOccupiedCells(Vector3 position, BlockElement block)
    {
        List<Vector3> occupiedCells = new List<Vector3>();

        Vector3 baseCell = GetSnappedPosition(position);

        for (int x = 0; x < block.cellsX; x++)
        {
            for (int y = 0; y < block.cellsY; y++)
            {
                for (int z = 0; z < block.cellsZ; z++)
                {
                    Vector3 cell = baseCell + new Vector3(x * gridSize, y * gridSize, z * gridSize);
                    occupiedCells.Add(cell);
                }
            }
        }

        return occupiedCells.ToArray();
    }

    private void RotatePreview(float angle)
    {
        rotationAngle = (rotationAngle + angle) % 360f;
    }

    private void SetPreviewColor(Color color)
    {
        Renderer previewRenderer = previewObject.GetComponent<Renderer>();
        if (previewRenderer != null)
        {
            previewRenderer.material.color = color;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 start = transform.position;

        for (int x = 0; x <= gridDimensions.x; x++)
        {
            for (int z = 0; z <= gridDimensions.z; z++)
            {
                Vector3 bottom = start + new Vector3(x * gridSize, 0, z * gridSize);
                Vector3 top = bottom + new Vector3(0, gridDimensions.y * gridSize, 0);
                Gizmos.DrawLine(bottom, top);
            }
        }
    }
}
