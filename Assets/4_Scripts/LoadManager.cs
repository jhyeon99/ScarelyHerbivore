using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadManager : MonoBehaviour
{
	public Text loadingText;

	AsyncOperation async;
	float timer = 0;

	void Start ()
	{
		Loading();
	}

	void Loading ()
	{
		async = SceneManager.LoadSceneAsync("Main");
		async.allowSceneActivation = true;
	}

	void Update ()
	{
		timer += 4 * Time.deltaTime;

		if (timer >= 4)
		{
			timer -= 4;
		}
		if ((int)timer == 0)
			loadingText.text = "불러오는 중";
		else if ((int)timer == 1)
			loadingText.text = "불러오는 중 .";
		else if ((int)timer == 2)
			loadingText.text = "불러오는 중 . .";
		else if ((int)timer == 3)
			loadingText.text = "불러오는 중 . . .";
	}
}
