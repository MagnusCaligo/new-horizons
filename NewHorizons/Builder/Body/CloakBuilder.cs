﻿#region

using NewHorizons.Components;
using NewHorizons.Utility;
using UnityEngine;

#endregion

namespace NewHorizons.Builder.Body
{
    public static class CloakBuilder
    {
        public static void Make(GameObject planetGO, Sector sector, OWRigidbody OWRB, float radius)
        {
            var cloak = SearchUtilities.Find("RingWorld_Body/CloakingField_IP");

            var newCloak = Object.Instantiate(cloak, sector?.transform ?? planetGO.transform);
            newCloak.transform.position = planetGO.transform.position;
            newCloak.transform.name = "CloakingField";
            newCloak.transform.localScale = Vector3.one * radius;

            Object.Destroy(newCloak.GetComponent<PlayerCloakEntryRedirector>());

            var cloakFieldController = newCloak.GetComponent<CloakFieldController>();
            cloakFieldController._cloakScaleDist = radius * 2000 / 3000f;
            cloakFieldController._farCloakRadius = radius * 500 / 3000f;
            cloakFieldController._innerCloakRadius = radius * 900 / 3000f;
            cloakFieldController._nearCloakRadius = radius * 800 / 3000f;

            cloakFieldController._referenceFrameVolume = OWRB._attachedRFVolume;
            cloakFieldController._exclusionSector = null;

            var cloakSectorController = newCloak.AddComponent<CloakSectorController>();
            cloakSectorController.Init(newCloak.GetComponent<CloakFieldController>(), planetGO);

            newCloak.SetActive(true);
            cloakFieldController.enabled = true;

            // To cloak from the start
            Main.Instance.ModHelper.Events.Unity.FireOnNextUpdate(cloakSectorController.OnPlayerExit);
        }
    }
}