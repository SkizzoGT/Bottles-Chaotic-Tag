using Photon.Pun;
using System;
using UnityEngine;

public class TaggingHand : MonoBehaviour
{
    public float RayCastDistance = .05f;

    [NonSerialized] public GameObject LocalPlayer;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, RayCastDistance);
    }

    private void Update()
    {
        if (PhotonNetwork.InRoom)
        {
            foreach (TagPlayerManager Managers in FindObjectsOfType<TagPlayerManager>())
            {
                if (Managers.GetComponent<PhotonView>().IsMine)
                {
                    LocalPlayer = Managers.gameObject;
                }
            }

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, RayCastDistance);

            foreach (Collider collider in hitColliders)
            {
                TagCollider OtherCollider = collider.GetComponent<TagCollider>();
                if (OtherCollider != null)
                {
                    if (LocalPlayer.GetComponent<TagPlayerManager>().Tagged && !LocalPlayer.GetComponent<TagPlayerManager>().InTagFreeze)
                    {
                        PhotonView RPCView = OtherCollider.playerManager.GetComponent<PhotonView>();
                        LocalPlayer.GetComponent<PhotonView>().RPC("UnTaggedRPC", LocalPlayer.GetComponent<PhotonView>().Owner);
                        RPCView.RPC("TaggedRPC", RPCView.Owner, true);
                    }
                }
            }
        }
    }
}
