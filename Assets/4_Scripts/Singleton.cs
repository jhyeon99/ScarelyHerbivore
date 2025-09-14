using UnityEngine;
using System.Collections;

public class Singleton : MonoBehaviour
{
	static Singleton instance;

	public static Singleton GetInstance
	{
		get
		{
			if (instance == null)
				instance = FindObjectOfType(typeof(Singleton)) as Singleton;
			return instance;
		}
	}

	public GameManager gameManager;
}
