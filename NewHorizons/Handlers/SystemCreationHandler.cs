using NewHorizons.Builder.StarSystem;
using NewHorizons.Components;
using NewHorizons.Utility;
using UnityEngine;
using Logger = NewHorizons.Utility.Logger;
using Object = UnityEngine.Object;
namespace NewHorizons.Handlers
{
    public static class SystemCreationHandler
    {
        public static void LoadSystem(NewHorizonsSystem system)
        {
            var skybox = SearchUtilities.Find("Skybox/Starfield");

            if (system.Config.skybox?.destroyStarField ?? false)
            {
                Object.Destroy(skybox);
            }

            if (system.Config.skybox?.assetBundle != null && system.Config.skybox?.path != null)
            {
                SkyboxBuilder.Make(system.Config.skybox, system.Mod);
            }

            if (system.Config.enableTimeLoop)
            {
                var timeLoopController = new GameObject("TimeLoopController");
                timeLoopController.AddComponent<TimeLoopController>();
            }

            if (!string.IsNullOrEmpty(system.Config.travelAudio))
            {
                Main.Instance.ModHelper.Events.Unity.FireOnNextUpdate(() => AudioUtilities.SetAudioClip(Locator.GetGlobalMusicController()._travelSource, system.Config.travelAudio, system.Mod));
            }
        }
    }
}
