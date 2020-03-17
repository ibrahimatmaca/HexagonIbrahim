using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchPanel : MonoBehaviour, IPointerDownHandler,IPointerUpHandler,IDragHandler
{

    private Vector2 downHand;
    private Vector2 upHand;

    public void OnPointerDown(PointerEventData eventData)
    {
        downHand = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        upHand = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        upHand = eventData.position;

        if(downHand == upHand)
        {
            if (!HexGrid2.instance.isSwap)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(upHand);

                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
                if (hit.collider != null)
                    HexGrid2.instance.SelectObject(hit.collider.gameObject, mousePos);
            }
        }
        else
        {
            if(upHand.x > downHand.x || upHand.y > downHand.y)
                StartCoroutine(HexGrid2.instance.UnClockWise());
            else
                StartCoroutine(HexGrid2.instance.ClockWise());
        }
    }
}
