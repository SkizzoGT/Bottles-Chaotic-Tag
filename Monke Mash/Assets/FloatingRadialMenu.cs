using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class FloatingRadialMenu : MonoBehaviour
{
    [System.Serializable]
    public struct RadialMenuElement
    {
        public GameObject slot;
        public GameObject objectToChangeMaterial;
        public SlotPosition position;
    }

    public enum SlotPosition { Top, Left, Right }

    public Material selectedMaterial;
    public Material unselectedMaterial;
    public XRController controller;
    public List<GameObject> objectsToDisable = new List<GameObject>();
    public List<RadialMenuElement> menuElements = new List<RadialMenuElement>();

    private bool isMenuActive = false;
    private RadialMenuElement? currentSelection = null;
    private float neutralThumbstickTime = 0f;
    private float neutralTimeThreshold = 1f;

    private void Start()
    {
        SetMenuActive(false);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (isMenuActive)
            {
                CloseMenu();
            }
            else
            {
                OpenMenu();
            }
        }

        if (isMenuActive)
        {
            UpdateThumbstickSelection();
            CheckThumbstickNeutralPosition();
        }
    }

    private void OpenMenu()
    {
        SetMenuActive(true);
        isMenuActive = true;
        neutralThumbstickTime = 0f;
    }

    private void CloseMenu()
    {
        if (currentSelection.HasValue)
        {
            SetUnselected(currentSelection.Value.objectToChangeMaterial);
            currentSelection = null;
        }
        SetMenuActive(false);
        isMenuActive = false;

        foreach (GameObject obj in objectsToDisable)
        {
            obj.SetActive(false);
        }
    }

    private void UpdateThumbstickSelection()
    {
        Vector2 thumbstickValue;
        if (controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out thumbstickValue))
        {
            if (thumbstickValue.magnitude > 0.2f)
            {
                RadialMenuElement? newSelection = DetermineSelection(thumbstickValue);

                if (newSelection.HasValue && (!currentSelection.HasValue || newSelection.Value.slot != currentSelection.Value.slot))
                {
                    if (currentSelection.HasValue)
                        SetUnselected(currentSelection.Value.objectToChangeMaterial);

                    SetSelected(newSelection.Value.objectToChangeMaterial);
                    currentSelection = newSelection;
                }
                neutralThumbstickTime = 0f;
            }
        }
    }

    private void CheckThumbstickNeutralPosition()
    {
        Vector2 thumbstickValue;
        if (controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out thumbstickValue) && thumbstickValue.magnitude <= 0.2f)
        {
            neutralThumbstickTime += Time.deltaTime;

            if (neutralThumbstickTime >= neutralTimeThreshold)
            {
                CloseMenu();
            }
        }
        else
        {
            neutralThumbstickTime = 0f;
        }
    }

    private RadialMenuElement? DetermineSelection(Vector2 thumbstickValue)
    {
        float angle = Mathf.Atan2(thumbstickValue.y, thumbstickValue.x) * Mathf.Rad2Deg;
        angle = (angle + 360) % 360;

        if (angle > 45 && angle <= 135)
        {
            return menuElements.Find(element => element.position == SlotPosition.Top);
        }
        else if (angle > 135 && angle <= 225)
        {
            return menuElements.Find(element => element.position == SlotPosition.Left);
        }
        else
        {
            return menuElements.Find(element => element.position == SlotPosition.Right);
        }
    }

    private void SetMenuActive(bool isActive)
    {
        foreach (var element in menuElements)
        {
            element.slot.SetActive(isActive);
            SetUnselected(element.objectToChangeMaterial);
        }
    }

    private void SetSelected(GameObject item)
    {
        Renderer renderer = item.GetComponent<Renderer>();
        if (renderer != null)
            renderer.material = selectedMaterial;
    }

    private void SetUnselected(GameObject item)
    {
        Renderer renderer = item.GetComponent<Renderer>();
        if (renderer != null)
            renderer.material = unselectedMaterial;
    }
}
