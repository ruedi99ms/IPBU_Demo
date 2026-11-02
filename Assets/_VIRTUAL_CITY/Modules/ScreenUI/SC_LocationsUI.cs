using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Numena.SC;

namespace Numena.SC.UI
{
    /// <summary>
    /// Panel that allows selecting a location from a list.
    /// </summary>
    public class SC_LocationsUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] float closedPos = 300, openPos = 0;
        [SerializeField] RectTransform locationContainer;
        [SerializeField] SC_LocationUIItem itemPrefab;

        bool targetState = false;

        RectTransform rt;

        List<SC_LocationUIItem> items = new List<SC_LocationUIItem>();

        void Awake()
        {
            rt = transform as RectTransform;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            targetState = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            targetState = false;
        }

        void OnEnable()
        {
            /*
            List<SC_LocationInfo> locations = LocationManager.GetAllLocations();
            LocationManager.onLocationAdded += OnLocationAdded;
            LocationManager.onLocationRemoved += OnLocationRemoved;

            foreach(SC_LocationInfo loc in locations)
            {
                LocationUIItem item = Instantiate(itemPrefab, locationContainer);
                item.targetLocation = loc;
                items.Add(item);
            }
            */
        }

        void Start()
        {
            List<SC_LocationInfo> locations = SC_LocationManager.GetAllLocations();
            foreach (SC_LocationInfo loc in locations)
            {
                SC_LocationUIItem item = Instantiate(itemPrefab, locationContainer);
                item.targetLocation = loc;
                items.Add(item);
            }
        }

        void Update()
        {
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, Mathf.MoveTowards(rt.anchoredPosition.y, targetState ? openPos : closedPos, Time.deltaTime * 1200));
        }

        void OnDisable()
        {
            //LocationManager.onLocationAdded -= OnLocationAdded;
            //LocationManager.onLocationRemoved -= OnLocationRemoved;
        }

        /*void OnLocationAdded(SC_Location loc)
        {
            LocationUIItem item = Instantiate(itemPrefab, locationContainer);
            item.targetLocation = loc;
            items.Add(item);
        }

        void OnLocationRemoved(SC_Location loc)
        {
            LocationUIItem item = GetItem(loc);
            if (item == null)
                return;
            items.Remove(item);
            Destroy(item.gameObject);
        }*/

        SC_LocationUIItem GetItem(SC_LocationInfo loc)
        {
            foreach (SC_LocationUIItem item in items)
            {
                if (item.targetLocation == loc)
                    return item;
            }
            return null;
        }
    }
}
