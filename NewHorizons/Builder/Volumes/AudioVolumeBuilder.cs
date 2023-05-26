using NewHorizons.Builder.Props;
using NewHorizons.External.Modules.Volumes.VolumeInfos;
using NewHorizons.Utility;
using NewHorizons.Utility.Files;
using NewHorizons.Utility.OuterWilds;
using NewHorizons.Utility.OWML;
using OWML.Common;
using UnityEngine;

namespace NewHorizons.Builder.Volumes
{
    public static class AudioVolumeBuilder
    {
        public static AudioVolume Make(GameObject planetGO, Sector sector, AudioVolumeInfo info, IModBehaviour mod)
        {
            var go = GeneralPropBuilder.MakeNew("AudioVolume", planetGO, sector, info);
            go.layer = Layer.AdvancedEffectVolume;

            var audioSource = go.AddComponent<AudioSource>();

            var owAudioSource = go.AddComponent<OWAudioSource>();
            owAudioSource._audioSource = audioSource;
            owAudioSource.loop = info.loop;
            owAudioSource.SetMaxVolume(info.volume);
            owAudioSource.SetClipSelectionType(info.clipSelection.ConvertToOW());
            owAudioSource.SetTrack(info.track.ConvertToOW());
            AudioUtilities.SetAudioClip(owAudioSource, info.audio, mod);
            Delay.FireOnNextUpdate(() =>
            {
                owAudioSource.spatialBlend = info.spatialBlend ? 1 : 0;
                owAudioSource.spread = info.spread;
            });

            var audioVolume = go.AddComponent<AudioVolume>();
            audioVolume._layer = info.layer;
            audioVolume.SetPriority(info.priority);
            audioVolume._fadeSeconds = info.fadeSeconds;
            audioVolume._noFadeFromBeginning = info.noFadeFromBeginning;
            audioVolume._randomizePlayhead = info.randomizePlayhead;
            audioVolume._pauseOnFadeOut = info.pauseOnFadeOut;

            var shape = go.AddComponent<SphereShape>();
            shape.radius = info.radius;

            var owTriggerVolume = go.AddComponent<OWTriggerVolume>();
            owTriggerVolume._shape = shape;
            audioVolume._triggerVolumeOverride = owTriggerVolume;

            go.SetActive(true);

            return audioVolume;
        }
    }
}
