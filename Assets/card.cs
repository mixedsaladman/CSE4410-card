using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using static main;

public class cardScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject mainObj;
    public main main;
    public Image suitImage;
    public TMP_Text rankText;

    public bool draggable;
    
    public int currIndex;
    public int initIndex;

    public card cardData;

    private Vector2 offset;
    private RectTransform rectTransform;

    void Awake() {
        rectTransform = GetComponent<RectTransform>();

        mainObj = GameObject.Find("game");
        main = mainObj.GetComponent<main>();

        draggable = true;
    } 

    void Start()
    {
        if (rectTransform.anchoredPosition == main.pos1) {
            currIndex = 0;
            initIndex = 0;
        } else if (rectTransform.anchoredPosition == main.pos2) {
            currIndex = 1;
            initIndex = 1;
        } else if (rectTransform.anchoredPosition == main.pos3) {
            currIndex = 2;
            initIndex = 2;
        } else if (rectTransform.anchoredPosition == main.pos4) {
            currIndex = 3;
            initIndex = 3;
        } 
    }

    void Update()
    {
        if (main.newRound) {
            if (initIndex == 0) {
                rectTransform.anchoredPosition = main.pos1;
            } else if (initIndex == 1) {
                rectTransform.anchoredPosition = main.pos2;
            } else if (initIndex == 2) {
                rectTransform.anchoredPosition = main.pos3;
            } else if (initIndex == 3) {
                rectTransform.anchoredPosition = main.pos4;
            }
            SetCard(initIndex);
            currIndex = initIndex;
        }
        
    }

    void LateUpdate() {
        currIndex = main.currRound.IndexOf(cardData);
        if (!main.isDragging) {
            if (currIndex == 0) {
                rectTransform.anchoredPosition = main.pos1;
            } else if (currIndex == 1) {
                rectTransform.anchoredPosition = main.pos2;
            } else if (currIndex == 2) {
                rectTransform.anchoredPosition = main.pos3;
            } else if (currIndex == 3) {
                rectTransform.anchoredPosition = main.pos4;
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (draggable) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform.parent as RectTransform, 
                eventData.position, 
                eventData.pressEventCamera, 
                out Vector2 localMousePos);

            offset = rectTransform.anchoredPosition - localMousePos;

            main.isDragging = true;

            main.selectedCard = this.gameObject;
            main.selectedCardTrans = rectTransform;
            main.selectedCardData = cardData;
        }
        main.cardSound.Play(0);
        // Debug.Log(cardData.rank + ", " + cardData.suit);
    }

    public void OnDrag(PointerEventData eventData) {
        if (draggable) {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform.parent as RectTransform, 
                eventData.position, 
                eventData.pressEventCamera, 
                out Vector2 localMousePos))
            {
                rectTransform.anchoredPosition = localMousePos + offset;
                transform.SetAsLastSibling();
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        main.isDragging = false;

        main.selectedCard = null;
        main.selectedCardTrans = null;
        main.selectedCardData = null;

        main.cardSound2.Play(0);
    }

    public void SetCard(int index) {
        if (index < (main.currRound.Count) || index >= 0) {
            cardData = main.currRound[index];
            suitImage.sprite = Resources.Load<Sprite>(cardData.suit);
            if (cardData.rank < 11) {
                rankText.text = cardData.rank.ToString();
            } else {
                if (cardData.rank == 11) {
                    rankText.text = "J";
                } else if (cardData.rank == 12) {
                    rankText.text = "Q";
                } else if (cardData.rank == 13) {
                    rankText.text = "K";
                } else if (cardData.rank == 14) {
                    rankText.text = "A";
                }
            }
        } else {
            this.gameObject.SetActive(false);
        }
    }
}
