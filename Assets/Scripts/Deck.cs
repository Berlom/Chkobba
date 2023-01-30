using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public GameObject cardPrefab=null;
    DeckMaster dm;
    GameObject deck;
    public void Start()
    {
        dm = FindObjectOfType<DeckMaster>();
        deck = GameObject.Find("Deck");
        
    }

    private void Update()
    {
        
    }

    

    public void distributeCard(GameObject card, GameObject holder)
    {
        GameObject ob= null;
        ob = Instantiate(card, new Vector3(0,0,0), Quaternion.identity);
        
        ob.transform.SetParent(holder.transform, false);

        Card c = ob.GetComponent<Card>();
        int rand = (int)Random.Range(0, DeckMaster.allCards.Count);
        string s = DeckMaster.allCards[rand];
        string[] word = s.Split(' ');
        ob.name = s;
        c.value = DeckMaster.cardValue[word[1]];
        c.type = word[0];
        c.holder = holder;
        dm.AssignImageToCard(c);
        DeckMaster.allCards.RemoveAt(rand);
    }
}
