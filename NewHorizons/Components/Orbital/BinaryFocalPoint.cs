﻿#region

using UnityEngine;

#endregion

namespace NewHorizons.Components.Orbital
{
    public class BinaryFocalPoint : MonoBehaviour
    {
        public string PrimaryName { get; set; }
        public string SecondaryName { get; set; }

        public AstroObject Primary { get; set; }
        public AstroObject Secondary { get; set; }

        public GameObject FakeMassBody { get; set; }

        private void Start()
        {
            // Make sure its active but maybe it hasn't been set yet
            if (FakeMassBody) FakeMassBody.SetActive(true);
        }

        private void Update()
        {
            if (Primary == null || Secondary == null)
            {
                CleanUp();
                gameObject.SetActive(false);
            }
            else
            {
                // Secondary and primary must have been engulfed by a star
                if (!Primary.isActiveAndEnabled && !Secondary.isActiveAndEnabled)
                {
                    CleanUp();
                    gameObject.SetActive(false);
                }
            }
        }

        private void CleanUp()
        {
            var component = Locator.GetPlayerBody()?.GetComponent<ReferenceFrameTracker>();
            if (component?.GetReferenceFrame()?.GetOWRigidBody() == gameObject) component.UntargetReferenceFrame();

            var component2 = gameObject.GetComponent<MapMarker>();
            if (component2 != null) component2.DisableMarker();

            FakeMassBody.SetActive(false);
        }
    }
}