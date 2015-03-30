using UnityEngine;
using System.Collections;

/// <summary>
/// A simple character input. Arrows to move, left SHIFT to run, SPACE to jump.
/// </summary>
public class SimpleCharacterInput : RaycastCharacterInput
{

	/// <summary>
	/// IF true always run.
	/// </summary>
	public bool alwaysRun;

	/// <summary>
	/// If true dropping from a passthrough platform requires user to press down and then jump.
	/// </summary>
	public bool jumpAndDownForDrop;

	private int movingDirection;

	private SeasonEmitter eHeldNode = null;
	[SerializeField]
	private float nodePickupRange = 0.5f;

	void Awake() {
		//InputManager.RegisterPlayerInput(this);
	}

	void Update ()
	{
		
		/*if (Input.GetKey(KeyCode.R)) {
			Application.LoadLevel(0);
		}*/

		if (Input.GetKeyDown(KeyCode.E)) {
			if (eHeldNode == null) {
				SeasonEmitter nodeToPickUp = GameManager.GetNearestNode(gameObject.transform.position);

				//If the node is within pickup range, parent it to the player character and disable it
				if (Vector3.Distance(nodeToPickUp.transform.position, gameObject.transform.position) < nodePickupRange) {
					nodeToPickUp.gameObject.transform.parent = gameObject.transform;
					nodeToPickUp.Deactivate();
					eHeldNode = nodeToPickUp;
				}
			}
			else {
				eHeldNode.transform.parent = null;
				eHeldNode.Activate();
				eHeldNode = null;
			}
		}
		
		jumpButtonHeld = false;
		jumpButtonDown = false;
		dropFromPlatform = false;
		x = 0;
		y = 0;
		
		if (Input.GetKey("right") && !Input.GetKey("left")) {
			x = 0.5f;
			movingDirection = 1;
		} else if (Input.GetKey("left") && !Input.GetKey("right")) {
			x = -0.5f;
			movingDirection = -1;
		} else if (Input.GetKey("right") && Input.GetKey("left")){
			x = movingDirection / 2.0f;
		}
	
		// Shift to run
		if (alwaysRun || Input.GetKey(KeyCode.LeftShift)) {
			x *= 2;
		}
		
		if (Input.GetKey("up") ) {
			y = 1;
		} else if (Input.GetKey("down") ) {
			y = -1;
			if (!jumpAndDownForDrop) dropFromPlatform = true;
		}
		
		if (Input.GetKey(KeyCode.Space) ) {
			jumpButtonHeld = true;
			if (Input.GetKeyDown(KeyCode.Space)) {
				if (jumpAndDownForDrop && Input.GetKey("down")) {
					dropFromPlatform = true;
				} else {
					jumpButtonDown = true;	
				}
				swimButtonDown = true;	
			} else {
				jumpButtonDown = false;		
				swimButtonDown = false;
			}
		} else {
			jumpButtonDown = false;
			swimButtonDown = false;
		}
	}
	
}

