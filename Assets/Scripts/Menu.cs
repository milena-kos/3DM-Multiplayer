﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
public class Menu : MonoBehaviour
{
	public bool inMenu = false;
	private List<GameObject> Objects = new List<GameObject>();
	public Slider slider;
	public GameObject cam;
	void Start() {
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("MenuStuff")) {
			Objects.Add(obj);
		}
		foreach (GameObject obj in Objects) {
			obj.SetActive(false);
		}
	}
    void Update()
    {
        if(Input.GetKeyDown("p") || Input.GetKeyDown("escape")) {
			inMenu = !inMenu;
		}
		if(inMenu) {
			foreach (GameObject obj in Objects) {
				obj.SetActive(true);
			}
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		} else {
			foreach (GameObject obj in Objects) {
				obj.SetActive(false);
			}
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
    }
	public void Quit() {
		PhotonNetwork.LeaveRoom();
		Application.Quit();
	}
	public void Sensitivity() {
		cam.GetComponent<Mouselook>().mouseSensitivity = slider.value;
	}	
	public void Reload() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
	public void Back() {
		PhotonNetwork.LeaveRoom();
		SceneManager.LoadScene(0);
	}
	public void noMenu() {
		inMenu = !inMenu;
	}
}
