using UnityEngine;

public class CaptureScreenshot : MonoBehaviour
	{
		public int superSize = 2;
		private int _shotIndex = 0;

		private void Update()
		{
			if(Input.GetKeyDown(KeyCode.P)) {
				ScreenCapture.CaptureScreenshot($"Screenshot{_shotIndex}.png", superSize);
				_shotIndex++;
			}
		}
	}