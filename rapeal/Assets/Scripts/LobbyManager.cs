using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // Photon components for Unity
using Photon.Realtime; // Library for Photon services
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1"; // Game version
    public Text connectionInfoText; // Text for displaying network information
    public Button joinButton; // Button for accessing room

    // Try to access master server at the same time with game start
    private void Start()
    {
        // Set information(game version) for connection
        PhotonNetwork.GameVersion = gameVersion;
        // Try to access master server by setted information
        PhotonNetwork.ConnectUsingSettings();

        // Deactivate room access button
        joinButton.interactable = false;
        // Show the information of trying accessing in the text
        connectionInfoText.text = "Connecting to master server...";
    }

    /* 
     * If connection to master server is successful, a code block below
     * will automatically start
     */
    public override void OnConnectedToMaster()
    {
        // Activate room access button
        joinButton.interactable = true;
        // Show the connection information
        connectionInfoText.text = "Online: Connected to master server";
    }

    /*
     * If connection to master server is failed, a code block below
     * will automatically start
     */
    public override void OnDisconnected(DisconnectCause cause)
    {
        // Deactivate room access button
        joinButton.interactable = false;
        // Show the connection information
        connectionInfoText.text = "Offline: Doesn't connected to master server\nTrying reconnection...";

        // Try connection to master server
        PhotonNetwork.ConnectUsingSettings();
    }

    // Try access to room
    public void Connect()
    {
        // To avoid another room connection, deactivate join button
        joinButton.interactable = false;

        // If connected to master server,
        if (PhotonNetwork.IsConnected)
        {
            // Execute room join
            connectionInfoText.text = "Join to room...";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // If not connected to master server, try to access master server
            connectionInfoText.text = "Offline: Doesn't connected to master server\nTrying reconnection...";

            // Try connection to master server
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // If failed to enter room, a code block below will automatically run
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // Show connection inforamtion
        connectionInfoText.text = "There are no empty rooms, create a new one...";

        // Create room for maximum 4 people
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
    }

    // If entering room is successful, a code block below will automatically run
    public override void OnJoinedRoom()
    {
        // Show connection information
        connectionInfoText.text = "Joined to room!";
        // Make all room participants load the main scene
        PhotonNetwork.LoadLevel("Main");
    }
}
