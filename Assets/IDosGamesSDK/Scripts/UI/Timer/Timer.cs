using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace IDosGames
{
	public class Timer : MonoBehaviour
	{
		[SerializeField] private TMP_Text _timerText;

		public Action TimerStopped;

		private readonly TimeSpan TIME_SPAN_ONE_SECOND = TimeSpan.FromSeconds(1);

		private bool _isSetted;

		private DateTime _endDate;
		private TimeSpan _endDateTimeSpan => TimeSpan.FromTicks(_endDate.Ticks - DateTime.UtcNow.Ticks);

		protected virtual void OnEnable()
		{
			TryUpdate();
		}

		public void Set(DateTime endDate)
		{
			if (endDate <= DateTime.UtcNow)
			{
				return;
			}

			_isSetted = true;
			_endDate = endDate;

			if (gameObject.activeInHierarchy)
			{
				UpdateTimer();
			}
		}

		private void TryUpdate()
		{
			if (_isSetted)
			{
				UpdateTimer();
			}
		}

		private void UpdateTimer()
		{
			StopAllCoroutines();
			StartCoroutine(StartTimer(_endDateTimeSpan));
		}

		private IEnumerator StartTimer(TimeSpan time)
		{
			yield return new WaitForEndOfFrame();

			while (time.TotalSeconds > 0)
			{
				time = time.Subtract(TIME_SPAN_ONE_SECOND);

				UpdateUI(time);

				yield return new WaitForSecondsRealtime(1f);
			}

			TimerStopped?.Invoke();
		}

		private void UpdateUI(TimeSpan time)
		{
			_timerText.text = GetDisplayString(time);
		}

		private string GetDisplayString(TimeSpan time)
		{
			string displayString = $"{time:hh\\:mm\\:ss}";

			if (time.Days > 0)
			{
				displayString = $"{time.Days}d. {displayString}";
			}
			else if (time.Hours <= 0)
			{
				displayString = $"{time:mm\\:ss}";
			}

			return displayString;
		}
	}
}