using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkSync : Photon.MonoBehaviour, IPunObservable
{
    public GameObject myAvatar;
    public PhotonView PV;
    public int myteam;
    

    //List of the scripts that should only be active for the local player (ex. PlayerController, MouseLook etc.)
    public MonoBehaviour[] localScripts;
    //List of the GameObjects that should only be active for the local player (ex. Camera, AudioListener etc.)
    public GameObject[] localObject;
    //Values that will be synced over network
    Vector3 latestPos;
    Quaternion latestRot;
    Vector3 latestVelocity;
    Rigidbody rb;


    // Use this for initialization
    void Start()
    {
        PV = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();

        //if (PV.isMine)
        //{
        //    //Player is local
        //    Debug.Log("PV is mine");
        //    PV.RPC("RPC_GetTeam", PhotonTargets.MasterClient);
        //    Debug.Log("RPC Done");
        //}

        if (!PV.isMine)
        {
            //Player is Remote
            Debug.Log("PV is NOT mine");
            for (int i = 0; i < localScripts.Length; i++)
            {
                localScripts[i].enabled = false;
            }
            for (int i = 0; i < localObject.Length; i++)
            {
                localObject[i].SetActive(false);
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(rb.position);
            stream.SendNext(rb.rotation);
            stream.SendNext(rb.velocity);
        }
        else
        {
            //Network player, receive data
            latestPos = (Vector3)stream.ReceiveNext();
            latestRot = (Quaternion)stream.ReceiveNext();
            latestVelocity = (Vector3)stream.ReceiveNext();

            float lag = Mathf.Abs((float)(PhotonNetwork.time - info.timestamp));
            latestPos += (latestVelocity * lag);

        }
    }

    private void Update()
    {

    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        if (!PV.isMine)
        {
            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            // transform.position = Vector3.Lerp(transform.position, latestPos, Time.deltaTime * 5);
            // transform.rotation = Quaternion.Lerp(transform.rotation, latestRot, Time.deltaTime * 5);
            rb.position = Vector3.MoveTowards(rb.position, latestPos, Time.fixedDeltaTime);
            rb.rotation = Quaternion.RotateTowards(rb.rotation, latestRot, Time.fixedDeltaTime * 100f);
            rb.velocity = Vector3.Lerp(rb.velocity, latestVelocity, Time.fixedDeltaTime);
        }
    }

    //[PunRPC]
    //void RPC_GetTeam()
    //{
    //    Debug.Log("hello");
    //    myteam = RoomController.RC.chooser;
    //    Debug.Log("h2");
    //    RoomController.RC.UpdateTeam();
    //    PV.RPC("RPC_SentTeam", PhotonTargets.OthersBuffered, myteam);
    //}

    //[PunRPC]
    //void RPC_SentTeam(int whichteam)
    //{
    //    Debug.Log("h3");
    //    myteam = whichteam;
    //}

}