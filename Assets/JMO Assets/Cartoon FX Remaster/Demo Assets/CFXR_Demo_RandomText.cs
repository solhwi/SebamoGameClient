using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CartoonFX
{
	public class CFXR_Demo_RandomText : MonoBehaviour
	{
		public ParticleSystem particles;
		public CFXR_ParticleText dynamicParticleText;

		void OnEnable()
		{
			InvokeRepeating("SetRandomText", 0f, 1.5f);
		}

		void OnDisable()
		{
			CancelInvoke("SetRandomText");
			particles.Clear(true);
		}

		void SetRandomText()
		{
			dynamicParticleText.UpdateText("GOAL IN!");
			particles.Play(true);
		}
	}
}