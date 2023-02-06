using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemController : MonoBehaviour
{
    [SerializeField] private ParticleSystem pS;
    private bool emissionEnabled = false;

    private void Update()
    {
        ParticleSystem.EmissionModule emissionModule = pS.emission;
        emissionModule.enabled = emissionEnabled;
    }

    public void ToggleParticles (bool enabled) {
        emissionEnabled = enabled;
    }
}
