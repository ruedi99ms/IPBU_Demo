using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Numena.SC;

namespace Numena.SC.UI
{
    /// <summary>
    /// Item for location list UI. Uses <see cref="SC_LocationManager"/> to teleport the player to the target location.
    /// Displays location image and title.
    /// </summary>
    public class SC_LocationUIItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] float closedPos = 50, openPos = 0;//title positions for animation
        [SerializeField] RectTransform infoPanel;//title panel
        [SerializeField] Text titleText;
        [SerializeField] Image thumbnailImage;

        SC_LocationInfo _targetLocation;
        public SC_LocationInfo targetLocation { get => _targetLocation; set => SetTargetLocation(value); }

        public Action<SC_LocationUIItem> onSelect;

        bool targetState = false;

        public void OnPointerClick(PointerEventData eventData)
        {
            SC_LocationManager.TeleportToLocation(targetLocation);
            eventData.Use();
            onSelect?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            targetState = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            targetState = false;
        }

        void Update()
        {
            infoPanel.anchoredPosition = new Vector2(infoPanel.anchoredPosition.x, Mathf.MoveTowards(infoPanel.anchoredPosition.y, targetState ? openPos : closedPos, Time.deltaTime * 1200));
        }

        void SetTargetLocation(SC_LocationInfo loc)
        {
            _targetLocation = loc;
            titleText.text = loc.title;
            thumbnailImage.sprite = loc.thumbnail;
        }
    }
}
