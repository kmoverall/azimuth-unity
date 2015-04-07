using UnityEngine;
using System.Collections;

//A simple moving platform, intended to be moved by other scripts
public class MovingPlatform2D : Platform2D {

	override public Transform ParentOnStand(RaycastCharacterController2D character) {
		return myTransform;
	}
}
