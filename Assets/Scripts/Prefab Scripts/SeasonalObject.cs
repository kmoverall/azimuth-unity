using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class SeasonalObject : MonoBehaviour {
	protected Season currentSeason;

	protected void Start() {
		Transition(GameManager.GlobalSeason);
		currentSeason = GameManager.GlobalSeason;
	}

	//Boilerplate to make sure that the object-specific transitions only happen when needed
	//Also helps remember to actually set the current season
	public void Transition(Season toSeason) {
		if (currentSeason != toSeason) {
			DoTransition(toSeason);
			currentSeason = toSeason;
		}
	}

	protected abstract void DoTransition(Season toSeason);
}
