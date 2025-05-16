using CartoonFX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFXR_Text : MonoBehaviour
{
	[SerializeField] private CFXR_ParticleText textObj;
	[SerializeField] private ParticleSystem particle;
	[SerializeField] private string text = "Goal In!";

	void Start()
    {
		textObj.UpdateText(text);
		particle.Play();
	}
}
