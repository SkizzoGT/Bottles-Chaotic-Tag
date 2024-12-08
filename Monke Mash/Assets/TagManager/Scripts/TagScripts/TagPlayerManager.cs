using Photon.Pun;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class TagPlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("Renderers To Change Material")]
    public Renderer[] PlayerParts;

    [Header("Materials")]
    [Tooltip("Your Tagged Materials, If You Have One Single Object For Your Player, Set All The Materials On Your Player Object Here, And Switch Out Your Fur Mat With A Lava Mat")]
    public Material[] TaggedMats;
    Material[] UnTaggedMats;

    [Header("The Tag Freeze CoolDown")]
    [Tooltip("When You Tag Someone, The Newly Tagged Player Will Be Frozen For This Amount Of Seconds")]
    public float FreezeTime = 3;

    [Header("If You Want To Have Particles Or Not, if Not Un-Check This Bool And Leave The Particle Stuff Blank")]
    public bool HasParticles = true;
    [Header("Tagged Particles")]
    [Tooltip("Your Tagged Particles, Preferibly Under Head")]
    public GameObject[] TaggedParticles;

    [Header("Tag Sound Stuff")]
    [Tooltip("The Sound That Plays When You Tag Somebody")]
    public AudioClip TagSound;
    [Tooltip("The Audio Source Of The Player, Make An Empty Under Hand, Add a AudioSource, and Put It In Here")]
    public AudioSource PlayerAudio;
    // Private Values
    [NonSerialized]
    public bool InTagFreeze;
    PhotonView MyView;
    [NonSerialized]
    public bool IsPlaying = true;
    int TaggedPlayers;
    [NonSerialized]
    public bool Tagged;
    int PlayersinRoom;
    int PlayerCheck;
    GorillaLocomotion.Player GorillaPlayer;

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (IsPlaying)
        {
            StartCoroutine(TagMaster());
        }
    }

    /// <summary>
    /// When Called It Tags ONLY This Player and untags everybody else.
    /// </summary>
    [PunRPC]
    public void TagThisPlayer()
    {
        foreach (TagPlayerManager Tag in FindObjectsOfType<TagPlayerManager>())
        {
            Tag.photonView.RPC("UnTaggedRPC", RpcTarget.All);

            photonView.RPC("TaggedRPC", RpcTarget.All, true);
        }
    }

    /*
    void OnPlayerLeft()
    {
      if (IsPlaying)
        {
            StartCoroutine(TagMaster());
        }
    }
    */

    /// <summary>
    /// When Called it Waits 1 Sec, Then Checks If Nobodys Tagged, If So: it Tags the Master Client.
    /// </summary>
    /// <returns></returns>
    public IEnumerator TagMaster()
    {
        yield return new WaitForSeconds(1);

        if (TaggedPlayers == 0)
        {
            foreach (TagPlayerManager Managers in FindObjectsOfType<TagPlayerManager>())
            {
                if (Managers.GetComponent<PhotonView>().Owner.IsMasterClient)
                {
                    Managers.TaggedRPC(true);
                }
            }
        }
    }


    #region Rpcs
    /// <summary>
    /// tags a player
    /// </summary>
    [PunRPC]
    public void TaggedRPC(bool HasTagFreeze)
    {
        if (IsPlaying)
        {
            Tagged = true;
            MyView.RPC("TagSoundRPC", RpcTarget.All);
            if (HasTagFreeze)
            {
                StartCoroutine(TagFreeze());
            }
        }
    }

    /// <summary>
    /// when called it makes the tag sound
    /// </summary>
    [PunRPC]
    public void TagSoundRPC()
    {
        PlayerAudio.clip = TagSound;
        PlayerAudio.Play();
    }

    /// <summary>
    /// UnTags A Player.
    /// </summary>
    [PunRPC]
    public void UnTaggedRPC()
    {
        if (IsPlaying)
        {
            Tagged = false;
        }
    }
    #endregion

    /// <summary>
    /// Called When You Tag Someone On The Newly Tagged Player, Makes Tag Freeze Work.
    /// </summary>
    /// <returns></returns>
    private IEnumerator TagFreeze()
    {
        InTagFreeze = true;
        yield return new WaitForSeconds(FreezeTime);
        InTagFreeze = false;
    }

    private void Start()
    {

        PlayerCheck = PhotonNetwork.CurrentRoom.PlayerCount;

        GorillaPlayer = FindObjectOfType<GorillaLocomotion.Player>();

        MyView = GetComponent<PhotonView>();

        foreach (Renderer Ren in PlayerParts)
        {
            UnTaggedMats = Ren.materials;
        }
    }


    private void Update()
    {
        PlayerHandler();
    }

    #region Handlers

    int GetTaggedPlayers()
    {
        var gameModeManagers = FindObjectsOfType<TagPlayerManager>();
        int taggedPlayersCount = gameModeManagers.Count(manager => manager.Tagged);

        return taggedPlayersCount;
    }


    void PlayerHandler()
    {
        if (IsPlaying)
        {
            foreach (GameObject particleObj in TaggedParticles)
            {
                if (HasParticles)
                {
                    particleObj.SetActive(Tagged);
                }
                else
                {
                    particleObj.SetActive(false);
                }
            }

            PlayersinRoom = PhotonNetwork.CurrentRoom.PlayerCount;
            if (PlayerCheck != PlayersinRoom)
            {
                if (PlayerCheck > PlayersinRoom)
                {
                    PlayerCheck = PlayersinRoom;
                    //OnPlayerLeft();
                }
                else
                {
                    PlayerCheck = PlayersinRoom;
                }
            }



            foreach (Renderer r in PlayerParts)
            {
                r.materials = Tagged ? TaggedMats : UnTaggedMats;

                foreach (Material Mat in TaggedMats)
                {
                    Mat.color = Color.white;
                }

            }

            if (PhotonNetwork.CurrentRoom.PlayerCount == 1 && !Tagged)
            {
                TaggedRPC(false);
            }

            TaggedPlayers = GetTaggedPlayers();

            GorillaPlayer.disableMovement = InTagFreeze;
        }
        else
        {
            Tagged = false;

            foreach (Renderer renderer in PlayerParts)
            {
                if (renderer.materials != UnTaggedMats)
                {
                    renderer.materials = UnTaggedMats;
                }
            }

            foreach (GameObject particleObj in TaggedParticles)
            {
                particleObj.SetActive(false);
            }
        }
    }
    #endregion


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Tagged);
        }
        else
        {
            bool receivedTagged = (bool)stream.ReceiveNext();
            Tagged = receivedTagged;
        }
    }
}

/*
NOTES FOR PERSONS:

the commented stuff, yeah thats for when you have the glitch where it has an error for the "OnPlayerLeftRoom" override, just remove the function and the make the commented stuff non commented, should fix it.

*/