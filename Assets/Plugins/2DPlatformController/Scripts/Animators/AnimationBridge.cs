using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Effective2DAnimationWithMecanim
{
	/// <summary>
	/// An animator that plays animations directly on a mecanim controller. Typically used for 2D sprites.
	/// </summary>
	public class AnimationBridge : MonoBehaviour
	{
		
		/// <summary>
		/// Cached reference to the character.
		/// </summary>
		public RaycastCharacterController myCharacter;
		
		/// <summary>
		/// Cached reference to the character.
		/// </summary>
		public RaycastCharacterController2D myCharacter2D;

		/// <summary>
		/// The controller will only handle states you add here.
		/// </summary>
		public List<CharacterState> handledStates;

		/// <summary>
		/// States in this list will take priority over other states.
		/// </summary>
		public List<CharacterState> priorityStates;

		/// <summary>
		/// Delay IDLE? This can smooth some animations but may mean you skip some animations.
		/// </summary>
		public bool delayIdle = false;

		/// <summary>
		/// Use this to offset the sprite position when it is on an angle.
		/// </summary>
		public float slopeOffset = 0.0f;

		#region members
		
		/// <summary>
		/// Cached reference to the animator.
		/// </summary>
		protected Animator myAnimator;
		
		/// <summary>
		/// The state currently playing.
		/// </summary>
		protected CharacterState state;

		/// <summary>
		/// The current states priority.
		/// </summary>
		protected int priority;

		/// <summary>
		/// The animation state that should play next.
		/// </summary>
		protected Queue<CharacterState> queuedStates;

		/// <summary>
		/// The queued states priority.
		/// </summary>
		protected Queue<int> queuedPriorities;

		/// <summary>
		/// Cached copy of local starting position.
		/// </summary>
		protected Vector3 startingLocalPosition;

		#endregion
		
		#region unity hooks
		
		/// <summary>
		/// Unity start hook.
		/// </summary>
		void Start()
		{
			// Get character reference
			if (myCharacter == null && myCharacter2D == null) Debug.LogError ("AnimationBridge unable to find character");
			myAnimator = GetComponentInChildren<Animator>();
			if (myAnimator == null) Debug.LogError ("AnimationBridge unable to find Unity Animator reference");
			queuedStates = new Queue<CharacterState> ();
			queuedPriorities = new Queue<int> ();
			state = CharacterState.NONE;
			priority = -1;

			startingLocalPosition = transform.localPosition;

			if (myCharacter != null) myCharacter.CharacterAnimationEvent += new RaycastCharacterController.CharacterControllerEventDelegate (CharacterAnimationEvent);
			if (myCharacter2D != null) myCharacter2D.CharacterAnimationEvent += new RaycastCharacterController2D.CharacterControllerEventDelegate (CharacterAnimationEvent);

		}
		
		/// <summary>
		/// Unity Update hook.
		/// </summary>
		void Update()
		{
			// If we have a new animation to play
			if (queuedStates.Count > 0)
			{
				CharacterState nextState = queuedStates.Peek ();
				int nextPriority = queuedPriorities.Peek ();
				AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo(0);
				// Ensure we played the current state for at least one frame, this is to work around for Mecanim issue where calling Play isn't always playing the animation
				if (state == CharacterState.NONE || info.IsName(state.ToString()))
				{
					// Next animation has higher priority, play it now
					if (nextPriority >= priority || info.normalizedTime >= 1.0f || info.loop)
					{
						myAnimator.Play(nextState.ToString());
						state = nextState;
						priority = nextPriority;
						queuedStates.Dequeue ();
						queuedPriorities.Dequeue();
					}
				}
			}
			// Facing direction
			if (myCharacter != null) {
				if (myCharacter.CurrentDirection > 0 && transform.localScale.x != 1) transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z); 
				else if (myCharacter.CurrentDirection < 0 && transform.localScale.x != -1)  transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z); 
			} else if (myCharacter2D != null) {
				if (myCharacter2D.CurrentDirection > 0 && transform.localScale.x != 1) transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z); 
				else if (myCharacter2D.CurrentDirection < 0 && transform.localScale.x != -1)  transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z); 
			}
			// Offset for slopes
			if ((myCharacter != null && Mathf.Approximately(myCharacter.targetSlope, 0.0f)) ||
			    (myCharacter2D != null && Mathf.Approximately(myCharacter2D.targetSlope, 0.0f))) {
				transform.localPosition = startingLocalPosition;
			} else {
				transform.localPosition = startingLocalPosition - Vector3.right * slopeOffset;
			}
		}
		
		/// <summary>
		/// Unity OnDestory hook.
		/// </summary>
		void OnDestroy()
		{
			if (myCharacter != null) myCharacter.CharacterAnimationEvent -= new RaycastCharacterController.CharacterControllerEventDelegate (CharacterAnimationEvent);
			if (myCharacter2D != null) myCharacter2D.CharacterAnimationEvent -= new RaycastCharacterController2D.CharacterControllerEventDelegate (CharacterAnimationEvent);
		}
		
		#endregion
		
		#region protected methods

		/// <summary>
		/// Handles animation state changed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
		protected void CharacterAnimationEvent(CharacterState state, CharacterState previousState)
		{
			if (handledStates.Contains(state)) {
				if (state == CharacterState.IDLE && delayIdle) {
					StartCoroutine(DelayedIdle());
				} else {
					queuedStates.Enqueue (state);
					if (priorityStates.Contains(state)) {
						queuedPriorities.Enqueue (10);
					} else {
						queuedPriorities.Enqueue (5);
					}
			}
			}
		}

		/// <summary>
		/// Delayed IDLE can help to smooth some rapid transitions.
		/// </summary>
		/// <returns>The idle.</returns>
		protected IEnumerator DelayedIdle() {
			yield return true;
			yield return true;
			queuedStates.Enqueue (CharacterState.IDLE);
			queuedPriorities.Enqueue (0);
		}

		#endregion
	}
}