using UnityEngine;

public class RoomController : MonoBehaviour
{
    public int chooser;
    //Player instance prefab, must be located in the Resources folder
    public GameObject playerPrefab;
    public GameObject hunterPrefab;
    //Player spawn point
    public Transform playerSpawnPoint;
    public Transform hunterSpawnPoint;

    // Use this for initialization
    void Start()
    {
        //In case we started this demo with the wrong scene being active, simply load the menu scene
        if (!PhotonNetwork.connected)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            return;
        }

        chooser = Random.Range(1,2); //(1,2);

        if (chooser == 1)
        {
            //We're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            PhotonNetwork.Instantiate(playerPrefab.name, playerSpawnPoint.position, Quaternion.identity, 0);
            PhotonNetwork.automaticallySyncScene = true;
        }

        if (chooser == 2)
        {
            PhotonNetwork.Instantiate(hunterPrefab.name, hunterSpawnPoint.position, Quaternion.identity, 0);
            PhotonNetwork.automaticallySyncScene = true;
        }
        
    }

    void OnGUI()
    {
        if (PhotonNetwork.room == null)
            return;

        //Leave this Room
        if (GUI.Button(new Rect(5, 5, 125, 25), "Leave Room"))
        {
            PhotonNetwork.LeaveRoom();
        }

        //Show the Room name
        GUI.Label(new Rect(135, 5, 200, 25), PhotonNetwork.room.Name);

        //Show the list of the players connected to this Room
        for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
        {
            //Show if this player is a Master Client. There can only be one Master Client per Room so use this to define the authoritative logic etc.)
            string isMasterClient = (PhotonNetwork.playerList[i].IsMasterClient ? ": MasterClient" : "");
            GUI.Label(new Rect(5, 35 + 30 * i, 200, 25), PhotonNetwork.playerList[i].NickName + isMasterClient);
        }
    }

    void OnLeftRoom()
    {
        //We have left the Room, return to the MainMenu
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}