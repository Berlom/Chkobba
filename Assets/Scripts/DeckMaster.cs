using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DeckMaster : MonoBehaviour
{
    Deck deck;
    string[] allNumbers =  { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
    public List<string> allTypes = new List<string> { "heart", "club", "spade", "diamond" };
    public static Dictionary<string, int> cardValue = new Dictionary<string, int> { { "1", 1 }, { "2", 2 }, { "3", 3}, { "4", 4 }, { "5", 5 }, { "6", 6 }, { "7", 7 },  { "9", 9 }, { "8", 8 }, { "10", 10 } };
    public static List<string> allCards = new List<string>();
    public static List<string> thrown = new List<string>();
    [SerializeField] private CardImage[] images;
    public GameObject cardPrefab = null;
    public GameObject[] cardHolders = null;
    [SerializeField]
    Button throwCardButton = null;
    [SerializeField]
    Canvas scoreBoard = null;
    [SerializeField]
    Canvas nextTurn = null;
    [SerializeField]
    List<TextMeshProUGUI> scores = new List<TextMeshProUGUI>();
    [SerializeField]
    TextMeshProUGUI playerTurn = null;
    public static List<Player> collections = new List<Player>();
    public static Player lastEater = null;
    int turn = 0;
    // Start is called before the first frame update
    void Start()
    {
        deck = FindObjectOfType<Deck>();
        initialiseCards();
        initGame();
        throwCardButton = GameObject.Find("Throw").GetComponent<Button>();
        collections.Add(GameObject.Find("P1Collection").GetComponent<Player>());
        collections.Add(GameObject.Find("P2Collection").GetComponent<Player>());
        collections.Add(GameObject.Find("P3Collection").GetComponent<Player>());
        collections.Add(GameObject.Find("P4Collection").GetComponent<Player>());
        if (!Card.selectedCard)
        {
            throwCardButton.enabled = false;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (Card.selectedCard)
        {
            throwCardButton.enabled = true;
        }
        else
        {
            throwCardButton.enabled = false;
        }
    }

    public void initialiseCards()
    {
        foreach (var num in allNumbers)
        {
            foreach (var type in allTypes)
            {
                allCards.Add(type +' '+ num);
            }
        }
    }

    public void AssignImageToCard(Card c)
    {
        foreach (CardImage image in images)
        {
            if (c.name == image.nom)
            {
                c.faceUp = image.image;
            }
        }
    }

    public int getTurn()
    {
        return turn;
    }


    public void quit()
    {
        SceneManager.LoadScene(0);
    }

    public void restart()
    {
        SceneManager.LoadScene(1);
    }

    public void initGame()
    {
        for (int i = 0; i < 3; i++)
        {
            for(int j= 0; j < 4; j++)
            {
                deck.distributeCard(cardPrefab, cardHolders[j]);
            }
        }
        for(int i = 0; i < 4; i++)
        {
            deck.distributeCard(cardPrefab, cardHolders[5]);
        }
        int count = allCards.Count;
        for(int j = 0; j<count;j++)
        {
            deck.distributeCard(cardPrefab, cardHolders[4]);
        }
    }

    public void nextJarya()
    {
        if (checkEmpty())
        {
            if(cardHolders[4].transform.childCount == 0)
            {
                transferCards(cardHolders[5], lastEater.gameObject);
                winner7ayya();
                winnerBermila();
                winnerCarta();
                winnerDinery();
                //phase de calcule
                for (int i = 0; i < 4; i++)
                {
                   scores[i].SetText(collections[i].score.ToString());
                }
                scoreBoard.gameObject.SetActive(true);
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        Card c = null;
                        c = cardHolders[4].transform.GetChild(0).GetComponent<Card>();
                        c.transform.SetParent(cardHolders[j].transform, false);
                        c.holder = cardHolders[j];
                    }
                }
            }
        }
    }

    bool checkEmpty()
    {
        for(int i = 0; i < 4; i++)
        {
            if (cardHolders[i].transform.childCount != 0)
                return false;
        }
        return true;
    }

    void ClearChildren(GameObject parent)
    {
        GameObject[] allChildren = getAllChildren(parent.transform);

        foreach (GameObject child in allChildren)
        {
            Destroy(child.gameObject);
        }

    }

    GameObject[] getAllChildren(Transform parent)
    {
        int i = 0;

        GameObject[] allChildren = new GameObject[parent.childCount];

        foreach (Transform child in parent)
        {
            allChildren[i] = child.gameObject;
            i += 1;
        }

        return allChildren;
    }

    public void rotateTurn()
    {
        GameObject[] aux = new GameObject[4];
        for (int i = 0; i < 4; i++)
        {
            aux[i] = Instantiate(cardHolders[i]);
            ClearChildren(cardHolders[i]);
        }
        for (int i = 1; i < 4; i++)
        {
            transferCards(aux[i], cardHolders[i - 1]);
        }
        transferCards(aux[0], cardHolders[3]);
        foreach (GameObject elt in aux)
        {
            Destroy(elt);
        }
        if (turn == 3)
        {
            turn = 0;
        }
        else
        {
            turn++;
        }
        if (!(checkEmpty() && cardHolders[4].transform.childCount == 0))
        {
            playerTurn.SetText("It's " + cardHolders[turn].name + "turn to play");
            nextTurn.gameObject.SetActive(true);
        }
        nextJarya();
    }

    public void passTurn()
    {
        nextTurn.gameObject.SetActive(false);
    }

    void transferCards(GameObject giver, GameObject reveiver)
    {
        Transform children = giver.transform;
        Card c = null;
        for (int i = children.childCount - 1; i >= 0; --i)
        {
            c = children.GetChild(i).GetComponent<Card>();
            c.transform.SetParent(reveiver.transform, false);
            c.holder = reveiver;
        }
    }

    public void throwCard()
    {
        if (checkThrowable(Card.selectedCard)) { 
            Card.selectedCard.transform.SetParent(cardHolders[5].transform, false);
            Card.selectedCard.holder = cardHolders[5];
            Card.selectedCard.transform.localScale = new Vector3(1f, 1f, 1f);
            Card.selectedCard = null;
            rotateTurn();
        }
        else
        {
            //show a text popup saying it can't be thrown
        }
    }

    public bool checkThrowable(Card c)
    {
        GameObject[] allCard = getAllChildren(cardHolders[5].transform);
        List<int> cards = new List<int>();
        foreach(GameObject carta in allCard)
        {
            int cardVal = carta.GetComponent<Card>().value;
            if(cardVal == c.value)
            {
                return false;
            }else if(cardVal < c.value)
            {
                cards.Add(cardVal);
            }
        }

        return !existCombination(cards,cards.Count,c.value);
    }

    //check if there's a combination in cardVals that's equal to cardVal
    static bool existCombination(List<int> cardVals, int n, int cardVal)
    {
        if (cardVal == 0)
            return true;
        if (n == 0)
            return false;

        if (cardVals[n - 1] > cardVal)
            return existCombination(cardVals, n - 1, cardVal);

        return existCombination(cardVals, n - 1, cardVal)
          || existCombination(cardVals, n - 1, cardVal - cardVals[n - 1]);
    }

    //check barmila
    public void winnerBermila()
    {
        List<Player> winners = getWinnersByValue(7);
        if (winners.Count > 1) { 
            winners = getWinnersByValue(6);
            if (winners.Count == 1)
            {
                winners[0].score++;
                Debug.Log(winners[0].name + " has won the bermila");
            }
        }
        else if(winners.Count == 1)
        {
            winners[0].score++;
            Debug.Log(winners[0].name + " has won the bermila");
        }
    }
    //check dinery 
    public void winnerDinery()
    {
        List<Player> winners = new List<Player>();
        int compteur = 0;
        int max = 0;
        foreach(Player player in collections)
        {
            foreach(GameObject element in getAllChildren(player.transform))
            {
                if (element.GetComponent<Card>().type == "diamond")
                    compteur++;
            }
            if (compteur > max)
            {
                max = compteur;
                winners.Clear();
                winners.Add(player);
            }
            else if (compteur == max)
            {
                winners.Add(player);
            }
            compteur = 0;
        }
        if (winners.Count == 1)
        {
            winners[0].score++;
            Debug.Log(winners[0].name + " has won the Dinery");
        }
    }
    //check l carta  
    public void winnerCarta()
    {
        List<Player> winners = new List<Player>();
        int max = 0;
        foreach(Player player in collections)
        {
            if (player.transform.childCount > max)
            {
                winners.Clear();
                winners.Add(player);
                max = player.transform.childCount;
            }
            else if (max == player.transform.childCount)
            {
                winners.Add(player);
            }
        }
        if (winners.Count == 1)
        {
            winners[0].score++;
            Debug.Log(winners[0].name + " has won the Carta");
        }
    }
    //check l 7ayya
    public void winner7ayya()
    {
        foreach (Player player in collections)
        {
            foreach (GameObject element in getAllChildren(player.transform))
            {
                if (element.GetComponent<Card>().type == "diamond" && element.GetComponent<Card>().value == 7)
                {
                    player.score++;
                    Debug.Log(player.name + " has won the 7ayya");
                }
            }
        }
    }

    public List<Player> getWinnersByValue(int x)
    {
        List<Player> winners = new List<Player>();
        int compteur = 0;
        int max = 0;
        foreach (Player player in collections)
        {
            foreach (GameObject element in getAllChildren(player.transform))
            {
                if (element.GetComponent<Card>().value == x)
                    compteur++;
            }
            if (compteur > max)
            {
                max = compteur;
                winners.Clear();
                winners.Add(player);
            }
            else if (compteur == max)
            {
                winners.Add(player);
            }
            compteur = 0;
        }
        return winners;
    }
}
