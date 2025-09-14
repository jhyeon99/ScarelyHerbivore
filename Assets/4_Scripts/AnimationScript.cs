using UnityEngine;
using UnityEngine.UI;

public class AnimationScript : MonoBehaviour
{
	[System.NonSerialized]
	public int order = 0;
	SpriteRenderer spriteRenderer = null;
	Image image = null;

	public bool isSpriteRenderer;
	
	[System.Serializable]
	public struct SAnim
	{
		[System.NonSerialized]
		public float timer;

		public float timeCycle;
		public Sprite[] sprite;
		
		public int loopTime;
	}
	
	public SAnim[] anim;

	void Start ()
	{
		if (isSpriteRenderer)
		{
			spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		}
		else
		{
			image = gameObject.GetComponent<Image>();
		}
	}

	void Update ()
	{
		if (isSpriteRenderer)
			anim[order].timer += Time.deltaTime * Singleton.GetInstance.gameManager.assignment;
		else
			anim[order].timer += Time.deltaTime;

		if (isSpriteRenderer)
		{
			if ((int)(anim[order].timer / (anim[order].timeCycle / anim[order].sprite.Length)) < anim[order].sprite.Length)
			{
				spriteRenderer.sprite = anim[order].sprite[(int)(anim[order].timer / (anim[order].timeCycle / anim[order].sprite.Length))];
			}
			else
			{
				anim[order].timer -= anim[order].timeCycle;                      // "loopTime = 0" means loopForever
				if (anim[order].loopTime > 0)
				{
					anim[order].loopTime--;
					if (anim[order].loopTime == 0)
					{
						++order;
						if (order >= anim.Length)
						{
							Destroy(gameObject);
						}
					}
				}
			}
		}
		else
		{
			if ((int)(anim[order].timer / (anim[order].timeCycle / anim[order].sprite.Length)) < anim[order].sprite.Length)
			{
				image.sprite = anim[order].sprite[(int)(anim[order].timer / (anim[order].timeCycle / anim[order].sprite.Length))];
			}
			else
			{
				anim[order].timer = 0;
				if (anim[order].loopTime > 0)
				{
					gameObject.SetActive(false);
				}
			}
		}
	}
}
