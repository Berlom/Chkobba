using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour , IPointerDownHandler
{
    public int value;
    public string type;
    public GameObject holder = null;
    public Sprite faceUp = null;
    public Sprite faceDown = null;
    public static Card selectedCard = null;
    public static List<Card> picked = new List<Card>();
    public static int somme = 0;
    DeckMaster dm = null;
    void Start()
    {
        dm = FindObjectOfType<DeckMaster>();
    }

    private void Update()
    {
        if(holder == dm.cardHolders[0] || holder == dm.cardHolders[5])
        {
            GetComponent<Image>().sprite = faceUp;
        }
        else
        {
            GetComponent<Image>().sprite = faceDown;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Card selected = eventData.pointerCurrentRaycast.gameObject.GetComponent<Card>();
        //select a card if there's no card selected
        if (!selectedCard)
        {
            if (selected.holder == dm.cardHolders[0])
            {
                selectedCard = selectCard(selected);
            }
        }
        else
        {
            //select a card from the table
            if (selected.holder == dm.cardHolders[5])
            {
                //if the card in the table was not selected
                if (!picked.Contains(selected)) { 
                    picked.Add(selected);
                    somme += selected.value;
                }
                //unselect the card from the table if selected twice
                else
                {
                    picked.Remove(selected);
                    somme -= selected.value;
                }
                //if the sum of the cards selected from the table is equal to the card value 
                if (somme == selectedCard.value)
                {
                    int turn = dm.getTurn();
                    selectedCard.transform.SetParent(DeckMaster.collections[turn].transform, false);
                    //dm.setFaceUp(selectedCard);
                    foreach (Card carta in picked)
                    {
                        carta.transform.SetParent(DeckMaster.collections[turn].transform, false);
                        carta.holder = null;
                    }
                    //check for chkobba 
                    if (dm.cardHolders[5].transform.childCount == 0)
                    {
                        selectedCard.transform.rotation = Quaternion.Euler(1f, 1f, Random.Range(-20.0f, 20.0f));
                        DeckMaster.collections[turn].score++;
                    }
                    else
                    {
                        selectedCard.holder = null;
                    }
                    selectedCard = unselectCard(selectedCard);
                    dm.rotateTurn();
                    DeckMaster.lastEater = DeckMaster.collections[turn];
                    picked.Clear();
                }
                else if(somme > selectedCard.value)
                {
                    selectedCard = unselectCard(selectedCard);
                    picked.Clear();
                }
            }
            //unselect the card if you click twice
            else if (selected == selectedCard)
            {
                selectedCard = unselectCard(selectedCard);
                picked.Clear();
            }
            //select another card 
            else
            {
                selectedCard = unselectCard(selectedCard);
                selectedCard = selectCard(selected);
            }
        }
    }

    private Card selectCard(Card c=null)
    {
        if (c.holder == dm.cardHolders[0])
        {
            c.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
        }
        return c;
    }

    private Card unselectCard(Card c)
    {
        c.transform.localScale = new Vector3(1f, 1f, 1f);
        c = null;
        somme = 0;
        return c;
    }

}
