using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviourPun
{
	public PhotonView photonView;
	public Text PlayerNameText;

	public CharacterController controller;
	public GameObject player;
	
	public float speed = 12f;
	public float gravity = -9.81f;
	public float groundDistance = 0.2f;
	public float jumpHeight = 3f;
	
	public LayerMask groundMask;

	public GameObject camera;
	
	public Transform groundCheck;
	public Transform ceilingCheck;

	public PlayerMovement pl;

	Vector3 velocity;
	Vector3 startingPos;
	bool isGrounded;
	bool isCeiling;
	bool inMenu;

	bool ismine;
    // Start is called before the first frame update
    void Start()
    {
		photonView = GetComponent<PhotonView>();
		if (photonView.IsMine) {
			startingPos = player.transform.position;
			pl.camera = GameObject.Find("Main Camera");
			camera.transform.parent = player.transform;
			camera.GetComponent<Mouselook>().playerBody = player.transform;
		} else {
			controller.enabled = false;
		}
    }

    // Update is called once per frame
    void Update()
    {
		inMenu = GameObject.FindGameObjectWithTag("Menu").GetComponent<Menu>().inMenu;
		if(!inMenu && photonView.IsMine) {
			isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
			isCeiling = Physics.CheckSphere(ceilingCheck.position, groundDistance, groundMask);
			if (isGrounded && velocity.y < 0) {
				velocity.y = -2f;
			}
			if (isCeiling) {
				velocity.y = -2f;
			}
			
			float x = Input.GetAxis("Horizontal");
			float z = Input.GetAxis("Vertical");
			
			Vector3 move = transform.right * x + transform.forward * z;
			
			controller.Move(move * speed * Time.deltaTime);
			
			if(Input.GetButtonDown("Jump") && isGrounded){
				velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
			}
			
			velocity.y += gravity * Time.deltaTime;
			
			controller.Move(velocity * Time.deltaTime);
			
			if (this.transform.position.y <= -1000) {
				print("die");
				velocity.y = -2f;
				this.transform.position = startingPos;
			}
		}
    }
}
