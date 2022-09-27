/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	public class UISafeAreaFixerTop : MonoBehaviour
	{
#region Fields
		RectTransform uiRectTransform;
#endregion

#region Unity API
		void Awake()
		{
			uiRectTransform = GetComponent< RectTransform >();

			var dangerArea = Screen.height - Screen.safeArea.height - Screen.safeArea.position.y;
			var offset = GameSettings.Instance.ui_safeArea_offset_top - dangerArea;

			var position = uiRectTransform.position;
			position.y += offset;

			uiRectTransform.position = position;
		}
#endregion
	}
}