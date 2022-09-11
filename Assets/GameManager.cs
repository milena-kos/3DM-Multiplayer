using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPun
{
    public GameObject PlayerPrefab;

    public void Start() {
        GameObject gb = PhotonNetwork.Instantiate(PlayerPrefab.name, new Vector3(51.1f, 5.71f, 81.24f), Quaternion.identity, 0);
    }
}
