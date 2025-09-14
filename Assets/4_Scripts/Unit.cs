using UnityEngine;
using System.Collections;

[System.Serializable]
public class Unit
{
	[System.NonSerialized]
	public float attackTimer;
	[System.NonSerialized]
	public float productionTimer;

	[System.NonSerialized]
	public int overHeatCount;
	[System.NonSerialized]
	public float overHeatTimer;

	[System.NonSerialized]
	public int number;
	[System.NonSerialized]
	public int target;

	public GameObject character;
	public float price;
	public float hitPoint;
	public float attackDamage;
	public float attackSpeed;
	public float moveSpeed;
	public float range;
	public float delay;

	[System.NonSerialized]
	public float maxHitPoint;

	[System.NonSerialized]
	public SpriteRenderer componentSpriteRenderer;
	[System.NonSerialized]
	public float moveAnimOrder;

	[System.Serializable]
	public struct SAnim
	{
		public Sprite stand;
		public Sprite knock;
		public float moveSpeed;
		public Sprite[] move;
		public Sprite[] attack1;
		public Sprite[] attack2;
		public Sprite[] overHeat;
	};

	public SAnim anim;

	public struct SArc
	{
		public Vector2 startPos;
		public Vector2 velocity;

		public float knockTimer;

		public int knockStack;
		public int knockCount;
	};

	[System.NonSerialized]
	public SArc knockBack;

	public bool AttackChance
	{
		get
		{
			if (attackTimer >= attackSpeed)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}

	public bool ProductionChance
	{
		get
		{
			if (productionTimer <= 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}

	public void Copy (Unit unit)
	{
		character = unit.character;
		price = unit.price;
		hitPoint = unit.hitPoint;
		attackDamage = unit.attackDamage;
		attackSpeed = unit.attackSpeed;
		moveSpeed = unit.moveSpeed;
		range = unit.range;
		delay = unit.delay;
		anim = unit.anim;
		knockBack = unit.knockBack;
	}
}
