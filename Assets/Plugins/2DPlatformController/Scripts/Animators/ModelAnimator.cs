using UnityEngine;
using System.Collections;

using System.Collections.Generic;

/**
 * A sample animation classes that works with 3d models.
 */

public class ModelAnimator : MonoBehaviour {
	
	public RaycastCharacterController controller;
	
	void Start(){
		// Set all animations to loop
   		GetComponent<Animation>().wrapMode = WrapMode.Loop;
   		// except jumping
		if (GetComponent<Animation>()["jump"] != null) {
   			GetComponent<Animation>()["jump"].wrapMode = WrapMode.Once;
   			GetComponent<Animation>()["jump"].layer = 1;
		}

		// Register listeners
		controller.CharacterAnimationEvent += new RaycastCharacterController.CharacterControllerEventDelegate (CharacterAnimationEvent);
	}
	
	/// <summary>
	/// Respond to an animation event.
	/// </summary>
	/// <param name='state'>
	/// State.
	/// </param>
	/// <param name='previousState'>
	/// Previous state.
	/// </param>
	public void CharacterAnimationEvent (CharacterState state, CharacterState previousState) {
		switch (state) {
			case CharacterState.IDLE: Idle(); break;	
			case CharacterState.WALKING: Walk(); break;	
			case CharacterState.RUNNING: Run(); break;	
			case CharacterState.JUMPING: Jump(); break;	
			case CharacterState.FALLING: Fall(); break;	
			case CharacterState.DOUBLE_JUMPING: Jump(); break;	
			case CharacterState.WALL_JUMPING: Jump(); break;	
			case CharacterState.HOLDING: Hold(); break;	
			case CharacterState.CLIMBING: Climb(); break;	
		}
	}
	
	protected void Idle () {
		GetComponent<Animation>().CrossFade("idle");
		CheckDirection();
	}
	
	protected void Walk () {
		GetComponent<Animation>().CrossFade("walk");
		CheckDirection();
	}

	protected void Run () {
		GetComponent<Animation>().CrossFade("run");
		CheckDirection();
	}

	protected void Jump() {
		GetComponent<Animation>().CrossFade("jump");
		CheckDirection();
	}
	
	protected void Fall() {
		GetComponent<Animation>().CrossFade("fall");
		CheckDirection();
	}
	
	protected void Hold() {
		GetComponent<Animation>().CrossFade("hold");
		transform.localRotation = Quaternion.Euler (0.0f, 180.0f, 0.0f);
	}
	
	protected void Climb() {
		GetComponent<Animation>().CrossFade("walk");
		transform.localRotation = Quaternion.Euler (0.0f, 180.0f, 0.0f);
	}
		
	protected void CheckDirection(){
		// You might need to switch 270 and 90 for other values depending on orientation of your model
		if (controller.Velocity.x > 0 ) {
			transform.localRotation = Quaternion.Euler (0.0f, 270.0f, 0.0f);
		} else if (controller.Velocity.x < 0) {
			transform.localRotation = Quaternion.Euler (0.0f, 90.0f, 0.0f);
		}	
	}
}