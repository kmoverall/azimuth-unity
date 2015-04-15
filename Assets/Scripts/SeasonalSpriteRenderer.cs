using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class SeasonalSpriteRenderer : MonoBehaviour {

	public Sprite maskSprite;
	public Texture2D springSprite;
	public Texture2D summerSprite;
	public Texture2D autumnSprite;
	public Texture2D winterSprite;

	public SpriteRenderer render;


	// Use this for initialization
	void Start () {
		render = GetComponent<SpriteRenderer>();
		render.sprite = maskSprite;
		//render.material = Resources.Load<Material>("SeasonSprite");

		UpdatePropertyBlock();
	}

	public void UpdatePropertyBlock() {
		MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
		render.GetPropertyBlock(propBlock);
		propBlock.SetTexture("_MainTex", maskSprite.texture);
		propBlock.SetTexture("_SprTex", springSprite);
		propBlock.SetTexture("_SumTex", summerSprite);
		propBlock.SetTexture("_AutTex", autumnSprite);
		propBlock.SetTexture("_WinTex", winterSprite);
		render.SetPropertyBlock(propBlock);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
