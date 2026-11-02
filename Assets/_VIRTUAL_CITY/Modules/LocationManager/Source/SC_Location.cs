using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Numena.SC
{
    public class SC_Location : MonoBehaviour
    {
        [SerializeField] SC_LocationInfo _info;
        public SC_LocationInfo info => _info;

        /// <summary> event, when player teleported to this location</summary>
        public Action<SC_Location> onTeleport;

        private void OnEnable()
        {
            SC_LocationManager.AddLocation(this);
        }

        private void OnDisable()
        {
            SC_LocationManager.RemoveLocation(this);
        }
    }
}
