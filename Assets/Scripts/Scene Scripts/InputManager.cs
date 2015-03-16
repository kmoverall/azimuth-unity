using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class InputManager : MonoBehaviour {
	private static InputManager _instance;
	#region Singleton Boilerplate
	public static InputManager instance {
		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<InputManager>();
				if (_instance == null) {
					Debug.LogError("An instance of InputManager is needed in the scene, but there is none");
				}
			}
			return _instance;
		}
	}
	#endregion

	PlatformerCharacter2D player;
	public static void RegisterPlayer(PlatformerCharacter2D registerPlayer) {
		instance.player = registerPlayer;
	}

	bool jumpPressed;

	// Use this for initialization
	void Awake () {

	}
	
	// Update is called once per frame
	void Update () {
		switch (GameManager.State) {
			case GameState.Playing:

				if (!instance.jumpPressed) {
					// Read the jump input in Update so button presses aren't missed.
					instance.jumpPressed = CrossPlatformInputManager.GetButtonDown("Jump");
				}

				break;
		}
	}

	void FixedUpdate() {
		switch (GameManager.State) {
			case GameState.Playing:

				bool crouch = Input.GetKey(KeyCode.LeftControl);
				float h = CrossPlatformInputManager.GetAxis("Horizontal");
				// Pass all parameters to the character control script.
				instance.player.Move(h, crouch, instance.jumpPressed);
				instance.jumpPressed = false;

				break;
		}
	}
}
