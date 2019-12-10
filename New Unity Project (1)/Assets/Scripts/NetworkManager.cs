using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    //Our player name
    string playerName = "Player 1";
    //This client's version number. Users are separated from each other by gameversion (which allows you to make breaking changes).
    string gameVersion = "0.9";
    //The list of created rooms
    RoomInfo[] createdRooms = new RoomInfo[0];
    //Use this name when creating a Room
    string roomName = "Room 1";
    Vector2 roomListScroll = Vector2.zero;
    bool joiningRoom = false;

    // Use this for initialization
    void Start()
    {
        //Automatically join Lobby after we connect to Photon Region
        PhotonNetwork.PhotonServerSettings.JoinLobby = true;
        //Enable Lobby Stats to receive the list of Created rooms
        PhotonNetwork.PhotonServerSettings.EnableLobbyStatistics = true;
        //This makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.automaticallySyncScene = true;

        if (!PhotonNetwork.connected)
        {
            // Connect to the photon master-server. We use the settings saved in PhotonServerSettings (a .asset file in this project)
            PhotonNetwork.ConnectUsingSettings(gameVersion);
        }
    }

    void OnFailedToConnectToPhoton(object parameters)
    {
        Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + parameters + " ServerAddress: " + PhotonNetwork.ServerAddress);
        //Try to connect again
        PhotonNetwork.ConnectUsingSettings(gameVersion);
    }

    void OnReceivedRoomListUpdate()
    {
        Debug.Log("We have received the Room list");
        //After this callback, PhotonNetwork.GetRoomList() becomes available
        createdRooms = PhotonNetwork.GetRoomList();
    }

    void OnGUI()
    {
        GUI.Window(0, new Rect(Screen.width / 2 - 450, Screen.height / 2 - 200, 900, 400), LobbyWindow, "Lobby");
    }

    void LobbyWindow(int index)
    {
        //Connection Status and Room creation Button
        GUILayout.BeginHorizontal();

        GUILayout.Label("Status: " + PhotonNetwork.connectionStateDetailed);

        if (joiningRoom || !PhotonNetwork.connected)
        {
            GUI.enabled = false;
        }

        GUILayout.FlexibleSpace();

        //Room name text field
        roomName = GUILayout.TextField(roomName, GUILayout.Width(250));

        if (GUILayout.Button("Create Room", GUILayout.Width(125)))
        {
            if (roomName != "")
            {
                joiningRoom = true;

                RoomOptions roomOptions = new RoomOptions();
                roomOptions.IsOpen = true;
                roomOptions.IsVisible = true;
                roomOptions.MaxPlayers = 3; //Set any number

                PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
            }
        }

        GUILayout.EndHorizontal();

        //Scroll through available rooms
        roomListScroll = GUILayout.BeginScrollView(roomListScroll, true, true);

        if (createdRooms.Length == 0)
        {
            GUILayout.Label("No Rooms were created yet...");
        }
        else
        {
            for (int i = 0; i < createdRooms.Length; i++)
            {
                GUILayout.BeginHorizontal("box");
                GUILayout.Label(createdRooms[i].Name, GUILayout.Width(400));
                GUILayout.Label(createdRooms[i].PlayerCount + "/" + createdRooms[i].MaxPlayers);

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Join Room"))
                {
                    joiningRoom = true;

                    //Set our Player name
                    PhotonNetwork.playerName = playerName;

                    //Join the Room
                    PhotonNetwork.JoinRoom(createdRooms[i].Name);
                }
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndScrollView();

        //Set player name and Refresh Room button
        GUILayout.BeginHorizontal();

        GUILayout.Label("Player Name: ", GUILayout.Width(85));
        //Player name text field
        playerName = GUILayout.TextField(playerName, GUILayout.Width(250));

        GUILayout.FlexibleSpace();

        GUI.enabled = PhotonNetwork.connectionState != ConnectionState.Connecting && !joiningRoom;
        if (GUILayout.Button("Refresh", GUILayout.Width(100)))
        {
            if (PhotonNetwork.connected)
            {
                //We are already connected, simply update the Room list
                createdRooms = PhotonNetwork.GetRoomList();
            }
            else
            {
                //We are not connected, estabilish a new connection
                PhotonNetwork.ConnectUsingSettings(gameVersion);
            }
        }

        GUILayout.EndHorizontal();

        if (joiningRoom)
        {
            GUI.enabled = true;
            GUI.Label(new Rect(900 / 2 - 50, 400 / 2 - 10, 100, 20), "Connecting...");
        }
    }

    void OnPhotonCreateRoomFailed()
    {
        Debug.Log("OnPhotonCreateRoomFailed got called. This can happen if the room exists (even if not visible). Try another room name.");
        joiningRoom = false;
    }

    void OnPhotonJoinRoomFailed(object[] cause)
    {
        Debug.Log("OnPhotonJoinRoomFailed got called. This can happen if the room is not existing or full or closed.");
        joiningRoom = false;
    }

    void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");
        //Set our player name
        PhotonNetwork.playerName = playerName;
        
        PhotonNetwork.LoadLevel("PlaygroundScene");
        PhotonNetwork.automaticallySyncScene = true;

    }

    void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
    }
}
