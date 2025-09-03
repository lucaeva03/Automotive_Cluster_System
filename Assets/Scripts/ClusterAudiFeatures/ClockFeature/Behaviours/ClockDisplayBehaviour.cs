using ClusterAudi;
using System.Collections;
using TMPro;
using UnityEngine;

namespace ClusterAudiFeatures
{
	public class ClockDisplayBehaviour : BaseMonoBehaviour<IClockFeatureInternal>
	{
		[Header("Clock Display UI")]
		[SerializeField] private TextMeshProUGUI _clockText;

		// Coroutine per aggiornamento continuo orologio
		private Coroutine _clockUpdateCoroutine;

		protected override void ManagedStart()
		{
			// Avvia aggiornamento continuo con coroutine
			_clockUpdateCoroutine = StartCoroutine(ClockUpdateCoroutine());
		}

		protected override void ManagedOnDestroy()
		{
			// Ferma coroutine
			if (_clockUpdateCoroutine != null)
			{
				StopCoroutine(_clockUpdateCoroutine);
			}
		}

		// Coroutine per aggiornamento continuo orologio
		private IEnumerator ClockUpdateCoroutine()
		{
			while (true)
			{
				if (_clockText != null)
				{
					_clockText.text = System.DateTime.Now.ToString("HH:mm");
				}

				// Attendi 1 frame prima del prossimo aggiornamento
				yield return new WaitForSeconds(1f);
			}
		}
	}
}