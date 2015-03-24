using UnityEngine;
using System.Collections;

using System.Collections.Generic;

/**
 * A sample animation classes that works with the supplied
 * "Hero" 3d Model.
 */

public class HeroAnimator : MonoBehaviour {
	
	public RaycastCharacterController controller;

	/// <summary>
	///  Change this to false if you don't wont to see wlak style events while swimming.
	/// </summary>
	public bool playWalkAnimationsWhileSwimming = true;

	private Quaternion targetRotation;
	private CharacterState currentState;
	private CharacterState previousState;
	private Vector3 rootOffset;

	void Start(){
		// Set all animations to loop
   		GetComponent<Animation>().wrapMode = WrapMode.Loop;
   		// except a few
		if (GetComponent<Animation>()["jump"] != null) {
   			GetComponent<Animation>()["jump"].wrapMode = WrapMode.Once;
		}
		if (GetComponent<Animation>() ["slide3"] != null) {
			GetComponent<Animation>()["slide3"].wrapMode = WrapMode.Once;
		}
		if (GetComponent<Animation>() ["ledge_climb"] != null) {
			GetComponent<Animation>()["ledge_climb"].wrapMode = WrapMode.Clamp;
		}
		if (GetComponent<Animation>()["stun2"] != null) {
			GetComponent<Animation>()["stun2"].wrapMode = WrapMode.Once;
		}
		if (GetComponent<Animation>()["crouch"] != null) {
			GetComponent<Animation>()["crouch"].wrapMode = WrapMode.Clamp;
		}

		// Store root offset, we use this to reset position after animations this is only here to deal with a few rogue animations changing our root position
		// you probably wont need it!
		rootOffset = transform.localPosition;

		// Register listeners
		controller.CharacterAnimationEvent += new RaycastCharacterController.CharacterControllerEventDelegate (CharacterAnimationEvent);
	}

	void Update() {
		CheckDirection ();
		// When turning face towards the camera, unless climbing/rope climbing
		if (controller.State != CharacterState.CLIMBING && controller.State != CharacterState.ROPE_CLIMBING &&
		    controller.State != CharacterState.ROPE_SWING && controller.State != CharacterState.ROPE_HANGING) {
			transform.localRotation = Quaternion.RotateTowards (	transform.localRotation, 
				                                                    (Quaternion.Angle (transform.localRotation, targetRotation) >= 180) ? Quaternion.Euler(0, 180, 0) : targetRotation, 
				                                                    Time.deltaTime * 400.0f);
		} else {
			transform.localRotation = Quaternion.RotateTowards (	transform.localRotation, 
			                                                   		targetRotation, 
			                                                    	Time.deltaTime * 400.0f);
		}
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
		currentState = state;
		this.previousState = previousState;
		transform.localPosition = rootOffset;
		switch (state) {
			case CharacterState.IDLE: Idle(previousState); break;	
			case CharacterState.WALKING: Walk(); break;	
			case CharacterState.RUNNING: Run(); break;	
			case CharacterState.SLIDING: Slide(); break;	
			case CharacterState.JUMPING: Jump(); break;	
			case CharacterState.AIRBORNE: Airborne(); break;	
			case CharacterState.FALLING: Fall(); break;	
			case CharacterState.DOUBLE_JUMPING: Jump(); break;	
			case CharacterState.WALL_JUMPING: Jump(); break;	
			case CharacterState.HOLDING: Hold(previousState); break;	
			case CharacterState.CLIMBING: Climb(); break;	
			case CharacterState.CLIMB_TOP_OF_LADDER_UP: ClimbTopUp(); break;	
			case CharacterState.CLIMB_TOP_OF_LADDER_DOWN: ClimbTopDown(); break;	
			case CharacterState.LEDGE_HANGING: LedgeHang(); break;
			case CharacterState.LEDGE_CLIMBING: LedgeClimb(); break;
			case CharacterState.LEDGE_CLIMB_FINISHED: Idle (previousState); break;
			case CharacterState.ROPE_CLIMBING: RopeClimb(); break;
			case CharacterState.ROPE_SWING: RopeSwing();break;
			case CharacterState.ROPE_HANGING: RopeHang();break;
			case CharacterState.WALL_SLIDING: WallSlide(); break;
			case CharacterState.CROUCHING: Crouch(); break;
			case CharacterState.CROUCH_SLIDING: CrouchSlide(); break;
			case CharacterState.STUNNED: Stunned(previousState); break;
			case CharacterState.PUSHING: Push(); break;
			case CharacterState.PULLING: Pull(); break;
		}
	}
	
	protected void Idle (CharacterState previousState) {
		GetComponent<Animation>().CrossFade ("idle");
		CheckDirection();
	}
	
	protected void Walk ()
	{
		if (playWalkAnimationsWhileSwimming || !controller.IsSwimming) {
			GetComponent<Animation>().CrossFade ("run");
		} else {
			GetComponent<Animation>().CrossFade("fall");
		}
		CheckDirection();
	}

	protected void Run ()
	{
		if (playWalkAnimationsWhileSwimming || !controller.IsSwimming) {
			GetComponent<Animation>().CrossFade ("run");
		} else {
			GetComponent<Animation>().CrossFade("fall");
		}
		CheckDirection();
	}

	protected void Slide ()
	{
		if (playWalkAnimationsWhileSwimming || !controller.IsSwimming) {
			GetComponent<Animation>().CrossFade ("slide3");
		} else {
			GetComponent<Animation>().CrossFade("fall");
		}
		CheckDirection();
	}

	protected void Jump() {
		GetComponent<Animation>().CrossFade("jump");
		CheckDirection();
	}

	protected void Airborne() {
		GetComponent<Animation>().CrossFade("airborne");
		CheckDirection();
	}


	protected void Fall() {
		GetComponent<Animation>().CrossFade("fall");
		CheckDirection();
	}
	
	protected void Hold(CharacterState previousState) {
		if (previousState != CharacterState.CLIMBING) GetComponent<Animation>().CrossFade ("climb");
		GetComponent<Animation>()["climb"].speed = 0;
		GetComponent<Animation>()["ledge_climb"].speed = 0;
		if (!GetComponent<Animation>().IsPlaying("ledge_climb")) {
			transform.localPosition= new Vector3(0, -0.75f, -1);
		}
	}
	
	protected void Climb() {
		GetComponent<Animation>()["climb"].speed = 1;
		GetComponent<Animation>().CrossFade("climb");
		transform.localPosition = new Vector3(0, -0.75f,-1);
	}

	protected void ClimbTopUp() {
		GetComponent<Animation>()["ledge_climb"].speed = 1;
		if ( GetComponent<Animation>()["ledge_climb"].normalizedTime < 0.4f)  GetComponent<Animation>()["ledge_climb"].normalizedTime = 0.4f;
		GetComponent<Animation>().CrossFade("ledge_climb");
	}

	protected void ClimbTopDown() {
		transform.localRotation = Quaternion.Euler (0.0f, 0.0f, 0.0f);
		GetComponent<Animation>()["ledge_climb"].speed = -1;
		GetComponent<Animation>()["ledge_climb"].normalizedTime = 0.9f;
		// if ( animation["ledge_climb"].normalizedTime < 0.4f)  animation["ledge_climb"].normalizedTime = 0.4f;
		GetComponent<Animation>().Play("ledge_climb");
	}

	protected void LedgeHang() {
		GetComponent<Animation>().CrossFade("ledge_hang");
	}

	protected void LedgeClimb() {
		GetComponent<Animation>()["ledge_climb"].speed = 1;
		GetComponent<Animation>().Play("ledge_climb");
	}

	protected void RopeHang() {
		GetComponent<Animation>().CrossFade("rope_hang");
	}

	protected void RopeSwing() {
		// No animation here as yet
	}

	protected void RopeClimb() {
		GetComponent<Animation>().CrossFade("rope_climb");
	}
	
	protected void WallSlide() {
		GetComponent<Animation>().CrossFade("wallslide");
	}
	
	protected void Crouch() {
		GetComponent<Animation>().CrossFade("crouch3");
	}

	protected void CrouchSlide() {
		GetComponent<Animation>().CrossFade("groundslide");
	}

	protected void Push() {
		GetComponent<Animation>().CrossFade ("push");
	}
	
	protected void Pull() {
		GetComponent<Animation>().CrossFade ("pull");
	}

	protected void Stunned(CharacterState previousState) {
		// Don't play animation if it doesn't fit (for example while climbing)
		// in a finished game we might have animations for different kinds of hit
		if (previousState != CharacterState.ROPE_HANGING &&
		    previousState != CharacterState.ROPE_CLIMBING &&
		    previousState != CharacterState.ROPE_SWING &&
		    previousState != CharacterState.HOLDING &&
		    previousState != CharacterState.CLIMBING) {
			GetComponent<Animation>().CrossFade("stun2");
		}
	}

	protected void CheckDirection() {
		// Rope states
		if (currentState == CharacterState.ROPE_HANGING ||
		    currentState == CharacterState.ROPE_CLIMBING || 
		    (currentState == CharacterState.STUNNED  && (
			previousState == CharacterState.ROPE_HANGING ||
			previousState == CharacterState.ROPE_CLIMBING ))){
			targetRotation = Quaternion.Inverse(transform.parent.rotation) * Quaternion.Euler (0.0f, 90.0f, 0.0f);

		}
		else if (currentState ==  CharacterState.CLIMB_TOP_OF_LADDER_UP || currentState ==  CharacterState.CLIMB_TOP_OF_LADDER_DOWN ||
		         currentState ==  CharacterState.HOLDING && (previousState == CharacterState.CLIMB_TOP_OF_LADDER_UP || previousState == CharacterState.CLIMB_TOP_OF_LADDER_DOWN)) {
			targetRotation = Quaternion.Inverse(transform.parent.rotation) * Quaternion.Euler (0.0f, 0.0f, 0.0f);
		}
		// Climbing states
		else if (currentState ==  CharacterState.CLIMBING ||
		   		 currentState ==  CharacterState.HOLDING ) {
			targetRotation = Quaternion.Inverse(transform.parent.rotation) * Quaternion.Euler (0.0f, -90.0f, 0.0f);
		}

		// Directional states
		else {
			// You might need to switch 270 and 90 for other values depending on orientation of your model
			if (controller.CurrentDirection > 0 ) {
				targetRotation = Quaternion.Inverse(transform.parent.rotation) * Quaternion.Euler (0.0f, 90.0f, 0.0f);
			} else if (controller.CurrentDirection < 0) {
				targetRotation = Quaternion.Inverse(transform.parent.rotation) * Quaternion.Euler (0.0f, -90.0f, 0.0f);
			}
		}
	}
}