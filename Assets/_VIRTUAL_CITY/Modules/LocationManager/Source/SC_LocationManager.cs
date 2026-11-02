using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Numena.Utilities;

namespace Numena.SC
{
    /// <summary>
    /// Allows moving the player to predefined locations (also in different scenes).
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class SC_LocationManager : MonoBehaviour
    {
        static SC_LocationManager main;

        [SerializeField] SC_LocationTeleportHelper teleportHelper;
        [SerializeField] SC_LocationSceneHelper sceneHelper;

        [SerializeField] List<SC_LocationInfo> allLocations = new List<SC_LocationInfo>();

        List<SC_Location> sceneLocations = new List<SC_Location>();
        public static Action<SC_Location> onLocationAdded, onLocationRemoved;

        private void Awake()
        {
            if (main != null)
                Debug.LogError(nameof(SC_LocationManager) + " is a singleton. Please make sure there is only one instance in the scene.");
            main = this;
        }

        private void OnDestroy()
        {
            main = null;
        }

        public static void AddLocation(SC_Location loc)
        {
            if (main.sceneLocations.Contains(loc))
                return;
            main.sceneLocations.Add(loc);
            onLocationAdded?.Invoke(loc);
        }

        public static void RemoveLocation(SC_Location loc)
        {
            if (main.sceneLocations.Contains(loc))
            {
                main.sceneLocations.Remove(loc);
                onLocationRemoved?.Invoke(loc);
            }
        }

        public static List<SC_Location> GetActiveSceneLocations()
        {
            return new List<SC_Location>(main.sceneLocations);
        }

        public static List<SC_LocationInfo> GetAllLocations()
        {
            return new List<SC_LocationInfo>(main.allLocations);
        }

        public static SC_Location FindLocationInCurrentScene(SC_LocationInfo info)
        {
            foreach (SC_Location loc in main.sceneLocations)
            {
                if (loc.info == info)
                    return loc;
            }
            return null;
        }

        public static void TeleportToLocation(SC_LocationInfo location)
        {
            SC_Location loc = FindLocationInCurrentScene(location);

            if (loc != null)
            {
                main.teleportHelper.TeleportToLocation(loc);
            }
            else
            {
                main.sceneHelper.LoadLocation(location);
            }
        }
    }
}
