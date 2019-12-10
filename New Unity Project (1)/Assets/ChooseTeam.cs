using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseTeam : MonoBehaviour
{
    public GameObject myAvatar;
    public int myteam;
    //Player instance prefab, must be located in the Resources folder
    public GameObject playerPrefab;
    public GameObject hunterPrefab;
    public PhotonView PV;
    // Update is called once per frame

    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (PV.isMine)
        {
            //Player is local
            Debug.Log("PV is mine");
            PV.RPC("RPC_GetTeam", PhotonTargets.MasterClient);
            Debug.Log("RPC Done");
        }
    }
    void Update()
    {
        if (myAvatar == null && myteam != 0)
        {
            if (myteam == 1)
            {
                if (PV.isMine)
                {
                    Debug.Log("PSpawn");
                    myAvatar = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0, 0, 0), Quaternion.identity, 0);
                    PhotonNetwork.automaticallySyncScene = true;
                }
            }

            else if (myteam == 2)
            {
                if (PV.isMine)
                {
                    Debug.Log("HSpawn");
                    myAvatar = PhotonNetwork.Instantiate(hunterPrefab.name, new Vector3(0, 0, 0), Quaternion.identity, 0);
                    PhotonNetwork.automaticallySyncScene = true;
                }

            }

        }
    }


    [PunRPC]
    void RPC_GetTeam()
    {
        Debug.Log("hello");
        myteam = RoomController.RC.chooser;
        Debug.Log("h2");
        RoomController.RC.UpdateTeam();
        PV.RPC("RPC_SentTeam", PhotonTargets.OthersBuffered, myteam);
    }

    [PunRPC]
    void RPC_SentTeam(int whichteam)
    {
        Debug.Log("h3");
        myteam = whichteam;
    }

}
