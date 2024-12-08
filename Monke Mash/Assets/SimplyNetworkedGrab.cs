using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(PhotonView))]
public class SimplyNetworkedGrab : MonoBehaviour
{
    private PhotonView photonView;
    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Subscribe to grab events
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (PhotonNetwork.IsConnected && photonView != null)
        {
            // Transfer ownership to the player grabbing the object
            if (photonView.Owner != PhotonNetwork.LocalPlayer)
            {
                photonView.RequestOwnership();
            }
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        // Ownership remains with the last player who grabbed it
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to avoid memory leaks
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }
}
