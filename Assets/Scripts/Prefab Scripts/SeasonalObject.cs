using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class SeasonalObject : MonoBehaviour {
	protected Season currentSeason;

	protected void Start() {
		Transition(GameManager.GlobalSeason);
		currentSeason = GameManager.GlobalSeason;
	}

	public abstract void Transition(Season toSeason);
}
