using UnityEngine;
using System.Collections;

public class BackButtonManager : MonoBehaviour
{
	AndroidJavaClass toastClass;
	AndroidJavaClass unityPlayerClass;
	AndroidJavaObject activity;
	AndroidJavaRunnable runnable;

	float timer = 0;

	void Awake()
	{
		Screen.SetResolution(360, 640, false);
#if UNITY_ANDROID && !UNITY_EDITOR
		toastClass = new AndroidJavaClass("android.widget.Toast");
		unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		activity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
		runnable = new AndroidJavaRunnable(
					() =>
					{
						toastClass.CallStatic<AndroidJavaObject>("makeText", activity, "뒤로버튼을 한번 더 누르면 종료됩니다.", 2).Call("show");
					}
					);
#endif
	}

	void Update()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		if (timer > 0)
		{
			timer -= Time.deltaTime;
			if (timer < 0)
			{
				timer = 0;
			}
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (timer <= 0)
			{
				activity.Call("runOnUiThread", runnable);
				timer = 2;
			}
			else
			{
				Application.Quit();
			}
		}
#endif
	}
}
