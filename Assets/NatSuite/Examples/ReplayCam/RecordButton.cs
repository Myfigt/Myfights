/* 
*   NatCorder
*   Copyright (c) 2020 Yusuf Olokoba
*/

namespace NatSuite.Examples.Components {

	using System.Collections;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using TMPro;

	[RequireComponent(typeof(EventTrigger))]
	public class RecordButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,IPointerClickHandler {

		public Image button, countdown;
		public UnityEvent onTouchDown, onTouchUp;
		private bool pressed =false;
		private const float MaxRecordingTime = 5f; // seconds
		public TMP_Text countdownText;

		private void Start () {
			Reset();
		}

		private void Reset () {
			// Reset fill amounts
			if (button)
				button.fillAmount = 1.0f;
			if (countdown)
				countdown.fillAmount = 0.0f;
		}
		public void OnRecordButtonPress()
		{
			if (!pressed)
			{
				StartCoroutine(Countdown());
			}
			else
				pressed = false;
		}
		void IPointerDownHandler.OnPointerDown (PointerEventData eventData) {
			// Start counting
			//StartCoroutine(Countdown());
            //WebCam.instance.StartRecording();
		}

		void IPointerUpHandler.OnPointerUp (PointerEventData eventData) {
			// Reset pressed
			pressed = false;
           // WebCam.instance.StopRecording() ;
        }

		private IEnumerator Countdown () {
			pressed = true;
			// First wait a short time to make sure it's not a tap
			countdownText.gameObject.SetActive(true);
			float getREadyTime = 3f;
            while (getREadyTime>0)
            {
				getREadyTime -= Time.deltaTime;
				countdownText.text = ((int)getREadyTime+1).ToString();
				yield return null;
			}
			
			//yield return new WaitForSeconds(3f);
			if (!pressed)
				yield break;
			// Start recording
			onTouchDown?.Invoke();
			countdownText.gameObject.SetActive(false);
			// Animate the countdown
			float startTime = Time.time, ratio = 0f;
			while (pressed && (ratio = (Time.time - startTime) / MaxRecordingTime) < 1.0f) {
				countdown.fillAmount = ratio;
				button.fillAmount = 1f - ratio;
				yield return null;
			}
			// Reset
			Reset();
			// Stop recording
			onTouchUp?.Invoke();
		}

        public void OnPointerClick(PointerEventData eventData)
        {
			OnRecordButtonPress();

		}
    }
}