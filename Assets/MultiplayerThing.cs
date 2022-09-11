using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class MultiplayerThing : MonoBehaviourPunCallbacks
{
    public InputField JoinInput;
    
    private void Awake() {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void JoinGame() {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 10;
        PhotonNetwork.JoinOrCreateRoom(JoinInput.text, roomOptions, TypedLobby.Default);
    }
    public override void OnJoinedRoom(){
        PlayerPrefs.SetInt("Multiplayer", 1);
        PhotonNetwork.LoadLevel("game");
    }
}
