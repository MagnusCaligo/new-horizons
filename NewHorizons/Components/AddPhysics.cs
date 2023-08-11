using NewHorizons.Utility.OuterWilds;
using System;
using System.Collections;
using UnityEngine;

namespace NewHorizons.Components;

/// <summary>
/// properly add physics to a detail
/// </summary>
[DisallowMultipleComponent]
public class AddPhysics : MonoBehaviour
{
    [Tooltip("The sector that the rigidbody will be simulated in, or none for it to always be on.")]
    public Sector Sector;
    [Tooltip("The mass of the physics object.\n" +
        "Most pushable props use the default value, which matches the player mass.")]
    public float Mass = 0.001f;
    [Tooltip("The radius that the added sphere collider will use for physics collision.\n" +
        "If there's already good colliders on the detail, you can make this 0.")]
    public float Radius = 1f;
    [Tooltip("If true, this detail will stay still until it touches something.")]
    public bool SuspendUntilImpact;

    [NonSerialized]
    public bool KeepLoaded;

    private OWRigidbody _body;
    private ImpactSensor _impactSensor;

    private IEnumerator Start()
    {
        SuspendUntilImpact = true;
        
        // detectors dont detect unless we wait for some reason
        yield return new WaitForSeconds(.1f);

        var parentBody = GetComponentInParent<OWRigidbody>();

        // hack: make all mesh colliders convex
        // triggers are already convex
        // prints errors for non readable meshes but whatever
        foreach (var meshCollider in GetComponentsInChildren<MeshCollider>(true))
            meshCollider.convex = true;

        var bodyGo = new GameObject($"{name}_Body");
        bodyGo.SetActive(false);
        bodyGo.transform.parent = transform.parent;
        bodyGo.transform.position = transform.position;
        bodyGo.transform.rotation = transform.rotation;

        _body = bodyGo.AddComponent<OWRigidbody>();
        _body._simulateInSector = KeepLoaded ? null : Sector;

        bodyGo.layer = Layer.PhysicalDetector;
        bodyGo.tag = "DynamicPropDetector";
        // this collider is not included in groups. oh well
        bodyGo.AddComponent<SphereCollider>().radius = Radius;
        var shape = bodyGo.AddComponent<SphereShape>();
        shape._collisionMode = Shape.CollisionMode.Detector;
        shape._layerMask = (int)(Shape.Layer.Default | Shape.Layer.Gravity);
        shape._radius = Radius;
        bodyGo.AddComponent<DynamicForceDetector>();
        var fluidDetector = bodyGo.AddComponent<DynamicFluidDetector>();
        fluidDetector._buoyancy = Locator.GetProbe().GetOWRigidbody()._attachedFluidDetector._buoyancy;
        fluidDetector._splashEffects = Locator.GetProbe().GetOWRigidbody()._attachedFluidDetector._splashEffects;

        _impactSensor = bodyGo.AddComponent<ImpactSensor>();
        var audioSource = bodyGo.AddComponent<AudioSource>();
        audioSource.maxDistance = 30;
        audioSource.dopplerLevel = 0;
        audioSource.rolloffMode = AudioRolloffMode.Custom;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1;
        var owAudioSource = bodyGo.AddComponent<OWAudioSource>();
        owAudioSource._audioSource = audioSource;
        owAudioSource._track = OWAudioMixer.TrackName.Environment;
        var objectImpactAudio = bodyGo.AddComponent<ObjectImpactAudio>();
        objectImpactAudio._minPitch = 0.4f;
        objectImpactAudio._maxPitch = 0.6f;
        objectImpactAudio._impactSensor = _impactSensor;

        bodyGo.SetActive(true);

        transform.parent = bodyGo.transform;
        _body.SetMass(Mass);
        _body.SetVelocity(parentBody.GetPointVelocity(_body.GetWorldCenterOfMass()));
        _body.SetAngularVelocity(parentBody.GetAngularVelocity());

        // #536 - Physics objects in bramble dimensions not disabled on load
        // sectors wait 3 frames and then call OnSectorOccupantsUpdated
        // however we wait .1 real seconds which is longer
        // so we have to manually call this
        if (_body._simulateInSector != null)
            _body.OnSectorOccupantsUpdated();

        if (SuspendUntilImpact)
        {
            // copied from OWRigidbody.Suspend
            _body._suspensionBody = parentBody;
            _body.MakeKinematic();
            _body._transform.parent = parentBody.transform;
            _body._suspended = true;
            _body._unsuspendNextUpdate = false;
            
            _impactSensor.OnImpact += OnImpact;
        }
        else
        {
            Destroy(this);
        }
    }
    
    private void OnImpact(ImpactData impact)
    {
        _body.Unsuspend();
        _impactSensor.OnImpact -= OnImpact;
        Destroy(this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}
