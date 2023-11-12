using UnityEngine;

namespace IDosGames
{
	public class ScrollbarContentPositionResetter : MonoBehaviour
	{
		private void OnEnable()
		{
			transform.localPosition = Vector3.zero;
		}
	}
}