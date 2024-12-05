using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MagicStar : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform selectIndicator;
    private Vector3 deltaPosition;
    private Vector3 beginPosition;
    private Cell nearestCell;

    private void Start()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        beginPosition = transform.position;
        deltaPosition = transform.position - Camera.main.ScreenToWorldPoint(eventData.position);
        iTween.MoveTo(gameObject, transform.position, 0.1f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
        transform.position = mousePosition + deltaPosition;

        nearestCell = WordRegion.instance.GetNearestCell(transform.position, 0.4f);
        if (nearestCell != null)
        {
            selectIndicator.gameObject.SetActive(true);
            selectIndicator.position = nearestCell.gameObject.transform.position;
        }
        else
        {
            selectIndicator.gameObject.SetActive(false);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (nearestCell == null)
        {
            iTween.MoveTo(gameObject, iTween.Hash("position", beginPosition, "time", 0.2f, "oncomplete", "OnReturnComplete"));
            selectIndicator.gameObject.SetActive(false);
        }
        else
        {
            if (CurrencyController.DebitBalance(ConfigController.Config.showLetterCost))
            {
                iTween.RotateBy(gameObject, iTween.Hash("z", 1, "time", 0.8f));
                iTween.ScaleTo(gameObject, Vector3.one * 1.3f, 0.4f);
                iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.one * 0.8f, "time", 0.4f, "delay", 0.4f));
                iTween.MoveTo(gameObject, iTween.Hash("position", beginPosition, "time", 0.2f, "delay", 0.8f));

                Timer.Schedule(this, 1f, () =>
                {
                    nearestCell.ShowAnswer();
                    WordRegion.instance.OnDoneMagicStar(nearestCell);
                    enabled = true;
                    selectIndicator.gameObject.SetActive(false);
                    WordRegion.instance.GetComponent<ScrollRect>().enabled = true;
                });
            }
            else
            {
                iTween.MoveTo(gameObject, iTween.Hash("position", beginPosition, "time", 0.2f, "oncomplete", "OnReturnComplete"));
                selectIndicator.gameObject.SetActive(false);
                Toast.instance.ShowMessage("You don't have enough rubies");

                DialogController.instance.ShowDialog(DialogType.Shop);
            }
        }
        enabled = false;
        WordRegion.instance.GetComponent<ScrollRect>().enabled = false;
    }

    private void OnReturnComplete()
    {
        enabled = true;
        WordRegion.instance.GetComponent<ScrollRect>().enabled = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Time.time - pointerDownTime < 0.2f)
        {
            DialogController.instance.ShowDialog(DialogType.MagicStar);
            Sound.instance.PlayButton();
        }
    }

    private float pointerDownTime;
    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDownTime = Time.time;
    }
}
