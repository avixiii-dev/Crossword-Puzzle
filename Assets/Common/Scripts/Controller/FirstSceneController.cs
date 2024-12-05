using UnityEngine;
using System.Collections;

public class FirstSceneController : MonoBehaviour
{
	public static FirstSceneController instance;

	private void Awake()
	{
		instance = this;
		Application.targetFrameRate = 60;
	}

	private void Update()
    {
#if UNITY_ANDROID
        if (Input.GetKeyDown(KeyCode.Escape) && !DialogController.instance.IsDialogShowing())
		{
            Application.Quit();
        }
#endif
    }
}
