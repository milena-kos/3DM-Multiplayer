﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class Placing : MonoBehaviourPun
{
	public GameObject[] inventory;
	private GameObject cell;
	public int inventoryIndex = 0;
	public GameObject inventoryCell;
	
	public GameObject hover;
	public GameObject inventoryText;
	private GameObject temp;
	
	public GameObject player;
	
	public static Vector3[] rotations = new Vector3[6];
	public static int rotIndex = 0;
	
	public LayerMask ignore;
	
	Vector3 pos;
	bool inMenu;

	public float reach = 30f;
	
    // Start is called before the first frame update
    void Start()
    {
		cell = inventory[inventoryIndex];
		
		rotations[0] = new Vector3(0, 0, 0);
		rotations[1] = new Vector3(0, 90, 0);
		rotations[3] = new Vector3(0, 180, 0);
		rotations[4] = new Vector3(0, 270, 0);
		rotations[5] = new Vector3(-90, 0, 0);
		rotations[2] = new Vector3(90, 0, 0);
		
		temp = Instantiate(hover, pos, Quaternion.identity);
		temp.transform.localEulerAngles = rotations[rotIndex];
    }

	[PunRPC]
    void RemoveCell(Vector3 celluabad, PhotonMessageInfo info)
    {
        Debug.Log("got removecell");
        Vector3 start = celluabad;
        start.y = -1000; // the ray must start at a very low y coordinate...
        RaycastHit[] hits = Physics.RaycastAll(start, Vector3.up); // and go up
        foreach (RaycastHit hit in hits){ // check the few objects returned
            // find the position difference in all 3 axes
            Vector3 diff = celluabad - hit.transform.position;
            if (diff.magnitude < 1f){ // accept a small error margin
                hit.transform.GetComponent<Cell>().Delete(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
		if (rotIndex < 0) {rotIndex = rotations.Length-1;}
		if (rotIndex >= rotations.Length){rotIndex = 0;}
		if (inventoryIndex < 0) {inventoryIndex = inventory.Length-1;}
		if (inventoryIndex >= inventory.Length){inventoryIndex = 0;}
		inMenu = GameObject.FindGameObjectWithTag("Menu").GetComponent<Menu>().inMenu;
		if(!inMenu) {
			if(Physics.Raycast(this.transform.position, this.transform.TransformDirection(Vector3.forward), out RaycastHit hitInfo2, reach, ignore)) {
				Debug.DrawRay(this.transform.position, this.transform.TransformDirection(Vector3.forward) * hitInfo2.distance, Color.red);
				if(hitInfo2.transform.tag == "cell" || hitInfo2.transform.tag == "ground") {
					pos.x = Mathf.RoundToInt(hitInfo2.point.x+hitInfo2.normal.x/2);
					pos.y = Mathf.RoundToInt(hitInfo2.point.y+hitInfo2.normal.y/2);
					pos.z = Mathf.RoundToInt(hitInfo2.point.z+hitInfo2.normal.z/2);
					temp.transform.position = pos;
					StopAllCoroutines();
					StartCoroutine(Rotate(rotations[rotIndex]));
					//temp.transform.localEulerAngles = rotations[rotIndex];
				} else {
					pos.x = Mathf.RoundToInt(hitInfo2.point.x);
					pos.y = Mathf.RoundToInt(hitInfo2.point.y);
					pos.z = Mathf.RoundToInt(hitInfo2.point.z);
					temp.transform.position = pos;
					temp.transform.localEulerAngles = rotations[rotIndex];
				}
			} else {
				temp.transform.position = new Vector3(9999, 9999, 9999);
			}
			Vector3 t = inventoryCell.transform.position;
			Quaternion q = inventoryCell.transform.rotation;
			Destroy(inventoryCell);
			inventoryCell = inventory[inventoryIndex];
			inventoryCell = Instantiate(inventoryCell, t, q);
			inventoryText.GetComponent<TextMeshProUGUI>().text = inventoryCell.gameObject.name.Substring(0,inventoryCell.gameObject.name.Length-7);
			inventoryCell.gameObject.name = "inventoryCell";
			if(Input.GetMouseButtonDown(0)){
				Place(inventory[inventoryIndex]);
			}
			else if(Input.GetMouseButtonDown(1)){ 
				if(Physics.Raycast(this.transform.position, this.transform.TransformDirection(Vector3.forward), out RaycastHit hitInfo, reach, ignore)) {
					Debug.DrawRay(this.transform.position, this.transform.TransformDirection(Vector3.forward) * hitInfo.distance, Color.red);
					if(hitInfo.transform.tag == "cell" && hitInfo.transform.gameObject.name != "ground") {
						photonView.RPC("RemoveCell", RpcTarget.All, hitInfo.transform.position);
					}
				}
			}
			else if (Input.GetKeyDown("e")) {
				rotIndex += 1;
			}
			else if (Input.GetKeyDown("q")) {
				rotIndex -= 1;
			}
			else if (Input.GetKeyDown("z")) {
				inventoryIndex -= 1;
			}
			else if (Input.GetKeyDown("x")) {
				inventoryIndex += 1;
			}
		}
    }
	void Place(GameObject block) {
		if(Physics.Raycast(this.transform.position, this.transform.TransformDirection(Vector3.forward), out RaycastHit hitInfo, reach, ignore)) {
			Debug.DrawRay(this.transform.position, this.transform.TransformDirection(Vector3.forward) * hitInfo.distance, Color.red);
			if(hitInfo.transform.tag == "cell" || hitInfo.transform.tag == "ground") {
				pos.x = Mathf.RoundToInt(hitInfo.point.x+hitInfo.normal.x/2);
				pos.y = Mathf.RoundToInt(hitInfo.point.y+hitInfo.normal.y/2);
				pos.z = Mathf.RoundToInt(hitInfo.point.z+hitInfo.normal.z/2);
				Cell c = GridManager.instance.SpawnCell(block, pos, rotations[rotIndex], (Direction_e)rotIndex, false);
				c.setPosition(pos);
				//GameObject a = Instantiate(block, pos, Quaternion.Euler(0,0,0));
				//a.transform.localEulerAngles = rotations[rotIndex];
			} else {
				pos.x = Mathf.RoundToInt(hitInfo.point.x);
				pos.y = Mathf.RoundToInt(hitInfo.point.y);
				pos.z = Mathf.RoundToInt(hitInfo.point.z);
				Cell c = GridManager.instance.SpawnCell(block, pos, rotations[rotIndex], (Direction_e)rotIndex, false);
				c.setPosition(pos);
				//GameObject a = Instantiate(block, pos, Quaternion.Euler(0,0,0));
				//a.transform.localEulerAngles = rotations[rotIndex];
			}
		}
	}
	IEnumerator Rotate(Vector3 targetAngle) {
		while (temp.transform.eulerAngles != targetAngle) {
			temp.transform.rotation = Quaternion.Slerp(temp.transform.rotation, Quaternion.Euler(targetAngle), 9f * Time.deltaTime);
			yield return null;
		}
		temp.transform.rotation = Quaternion.Euler(targetAngle);
		yield return null;
	}  
}
