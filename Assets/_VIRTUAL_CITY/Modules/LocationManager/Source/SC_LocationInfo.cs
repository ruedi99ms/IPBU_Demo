using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Numena.SC
{
    /// <summary>
    /// Asset that stores information about a location. Use <see cref="SC_LocationManager"/> to move the player to a location.
    /// </summary>
    [CreateAssetMenu(fileName = "New Location Info", menuName = "SC/New Location Info")]
    public class SC_LocationInfo : ScriptableObject
    {
        [SerializeField] string _title = "Location";
        public string title => _title;

        [SerializeField] Sprite _thumbnail;
        public Sprite thumbnail => _thumbnail;

        [SerializeField] int _scene = 0;
        public int scene => _scene;
    }
}
