using Mirror;
using RTS.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace RTS.Cameras
{
    public class Minimap : MonoBehaviour,IPointerDownHandler, IDragHandler
    {
        [SerializeField] private RectTransform minimapRectTransform = null;
        [SerializeField] private float mapScale = 25f;
        [SerializeField] private float offset = -5f;

        private Transform playerCameraTransform;

        private void Start()
        {
           playerCameraTransform = NetworkClient.connection.identity.GetComponent<RTSPlayer>().GetCameraTransform();

        }

        private void Update()
        {
            
            if (NetworkClient.connection.identity == null) return;

        }

        private void MoveCamera()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            //checks if the mouse position is inside of the minimap rect transform
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapRectTransform, mousePosition, null, out Vector2 localPoint))
            {
                //this makes sure the scaling of clicking on a point of the minimap remains the same
                Vector2 lerp = new Vector2((localPoint.x - minimapRectTransform.rect.x) / minimapRectTransform.rect.width,
                    (localPoint.y - minimapRectTransform.rect.y) / minimapRectTransform.rect.height);

                Vector3 newCameraPosition = new Vector3(Mathf.Lerp(-mapScale, mapScale, lerp.x),
                                                        playerCameraTransform.position.y,
                                                        Mathf.Lerp(-mapScale, mapScale, lerp.y));

                playerCameraTransform.position = newCameraPosition + new Vector3(0f, 0f, offset);
            }

            return;
        }

        public void OnDrag(PointerEventData eventData)
        {
            MoveCamera();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            MoveCamera();
        }
    }


}



