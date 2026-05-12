using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class main : MonoBehaviour
{
    public class card
    {
        public string suit;
        public int rank;
    }
    
    private List<card> deck = new List<card>{
        new card { suit = "Diamonds", rank = 2},
        new card { suit = "Diamonds", rank = 3},
        new card { suit = "Diamonds", rank = 4},
        new card { suit = "Diamonds", rank = 5},
        new card { suit = "Diamonds", rank = 6},
        new card { suit = "Diamonds", rank = 7},
        new card { suit = "Diamonds", rank = 8},
        new card { suit = "Diamonds", rank = 9},
        new card { suit = "Diamonds", rank = 10},
        new card { suit = "Hearts", rank = 2},
        new card { suit = "Hearts", rank = 3},
        new card { suit = "Hearts", rank = 4},
        new card { suit = "Hearts", rank = 5},
        new card { suit = "Hearts", rank = 6},
        new card { suit = "Hearts", rank = 7},
        new card { suit = "Hearts", rank = 8},
        new card { suit = "Hearts", rank = 9},
        new card { suit = "Hearts", rank = 10},
        new card { suit = "Clubs", rank = 2},
        new card { suit = "Clubs", rank = 3},
        new card { suit = "Clubs", rank = 4},
        new card { suit = "Clubs", rank = 5},
        new card { suit = "Clubs", rank = 6},
        new card { suit = "Clubs", rank = 7},
        new card { suit = "Clubs", rank = 8},
        new card { suit = "Clubs", rank = 9},
        new card { suit = "Clubs", rank = 10},
        new card { suit = "Clubs", rank = 11},
        new card { suit = "Clubs", rank = 12},
        new card { suit = "Clubs", rank = 13},
        new card { suit = "Clubs", rank = 14},
        new card { suit = "Spades", rank = 2},
        new card { suit = "Spades", rank = 3},
        new card { suit = "Spades", rank = 4},
        new card { suit = "Spades", rank = 5},
        new card { suit = "Spades", rank = 6},
        new card { suit = "Spades", rank = 7},
        new card { suit = "Spades", rank = 8},
        new card { suit = "Spades", rank = 9},
        new card { suit = "Spades", rank = 10},
        new card { suit = "Spades", rank = 11},
        new card { suit = "Spades", rank = 12},
        new card { suit = "Spades", rank = 13},
        new card { suit = "Spades", rank = 14}
    };

    public List<card> currDeck = new List<card>();

    public List<card> currRound = new List<card>();
    public List<GameObject> currRoundCards = new List<GameObject>();

    public List<card> avoidDeck = new List<card>();

    public GameObject selectedCard;
    public RectTransform selectedCardTrans;
    public card selectedCardData;

    public RectTransform card1trans;
    public RectTransform card2trans;
    public RectTransform card3trans;
    public RectTransform card4trans;

    public List<Vector2> startPos = new List<Vector2>();

    public Vector2 pos1;
    public Vector2 pos2;
    public Vector2 pos3;
    public Vector2 pos4;

    public TMP_Text healthText;
    public TMP_Text slainText;
    
    public int health;
    public int slain;

    public GameObject tutorial;

    public Button avoidButton;

    public GameObject equipImage;
    public TMP_Text equipText;
    public GameObject duraObj;
    public Image duraImage;
    public TMP_Text duraText;

    public AudioSource faceSound;
    public AudioSource avoidSound;
    public AudioSource cardSound;
    public AudioSource cardSound2;
    public AudioSource winSound;
    public AudioSource loseSound;

    public int currEquip;
    public int currDura;

    public GameObject winScreen;
    public GameObject loseScreen;

    public bool gameOver;
    public bool healed;
    public bool avoided;
    public bool newRound;
    public bool isDragging;

    void Awake()
    {
        gameOver = false;
        loseScreen.SetActive(false);
        winScreen.SetActive(false);

        pos1 = card1trans.anchoredPosition;
        pos2 = card2trans.anchoredPosition;
        pos3 = card3trans.anchoredPosition;
        pos4 = card4trans.anchoredPosition;
        startPos.Add(pos1);
        startPos.Add(pos2);
        startPos.Add(pos3);
        startPos.Add(pos4);
    }

    void Start()
    {
        currDeck = deck;

        health = 20;

        slain = 0;

        currEquip = 0;
        currDura = 0;
        equipImage.SetActive(false);
        equipText.text = "";
        duraObj.SetActive(false);

        avoided = false;

        NewRound(4);
        DebugLog();
        Debug.Log("Cards Left: " + currDeck.Count + "   Avoided Cards: " + avoidDeck.Count);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Tab)) {
            tutorial.SetActive(true);
            tutorial.transform.SetAsLastSibling();
        } else {
            tutorial.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        if (gameOver) {
            return;
        }
        if (selectedCard != null) {
            int selectedCardIndex = currRoundCards.IndexOf(selectedCard);
            for (int i = 0; i < currRoundCards.Count; i++) {
                if (selectedCardTrans.anchoredPosition.x > startPos[i].x) {
                    if (selectedCardIndex < i) {
                        Nudge(i, selectedCardIndex);
                        SwapIndex(selectedCardIndex, i);
                        SwapCards(selectedCardIndex, i);
                        break;
                    }
                } 

                if (selectedCardTrans.anchoredPosition.x < startPos[i].x) {
                    if (selectedCardIndex > i) {
                        Nudge(i, selectedCardIndex);
                        SwapIndex(selectedCardIndex, i);
                        SwapCards(selectedCardIndex, i);
                        break;
                    }
                }
            }
        }
        if (avoided) {
            avoidButton.interactable = false;
        } else {
            avoidButton.interactable = true;
        }
    }

    public void NewRound(int newCards)
    {
        if (newCards < currDeck.Count) {
            for (int i = 0; i < newCards; i++) {
                int random = Random.Range(0, (currDeck.Count - 1));
                currRound.Add(currDeck[random]);
                currDeck.RemoveAt(random);
            }
        } else if (newCards < (currDeck.Count + avoidDeck.Count)) {
            currDeck.AddRange(avoidDeck);
            avoidDeck.Clear();
            for (int i = 0; i < newCards; i++) {
                int random = Random.Range(0, (currDeck.Count - 1));
                currRound.Add(currDeck[random]);
                currDeck.RemoveAt(random);
            }
        } else {
            currDeck.AddRange(avoidDeck);
            currRound.AddRange(currDeck);
            avoidDeck.Clear();
            currDeck.Clear();
        }

        currRoundCards.Clear();
        if (currRound.Count < 4) {
            for (int i = 0; i < (4 - currRound.Count); i++) {
                GameObject.Find("card" + (4 - i)).SetActive(false);
            }
        } else {
            for (int i = 0; i < currRound.Count; i++) {
                currRoundCards.Add(GameObject.Find("card" + (i + 1)));
            }
        }

        newRound = true;
        Invoke("RoundReset", 0.1f);

        healed = false;
    }

    public void Face()
    {
        for (int i = 0; i < 3; i++) {
            if (currRound[i].suit == "Clubs" || currRound[i].suit == "Spades") {
                if (currEquip != 0) {
                    if (currDura > currRound[i].rank) {
                        if (currEquip < currRound[i].rank) {
                            health += currEquip;
                            health -= currRound[i].rank;
                        }
                        currDura = currRound[i].rank;
                        duraObj.SetActive(true);
                        duraImage.sprite = Resources.Load<Sprite>(currRound[i].suit);
                        duraText.text = currDura.ToString();
                        if (currRound[i].rank < 11) {
                            duraText.text = currRound[i].rank.ToString();
                        } else {
                            if (currRound[i].rank == 11) {
                                duraText.text = "J";
                            } else if (currRound[i].rank == 12) {
                                duraText.text = "Q";
                            } else if (currRound[i].rank == 13) {
                                duraText.text = "K";
                            } else if (currRound[i].rank == 14) {
                                duraText.text = "A";
                            }
                        }
                    } else {
                        currEquip = 0;
                        currDura = 0;
                        equipImage.SetActive(false);
                        equipText.text = "";
                        duraObj.SetActive(false);
                        health -= currRound[i].rank;
                    }
                } else {
                    health -= currRound[i].rank;
                }
                if (health <= 0) {
                    loseSound.Play(0);
                    Debug.Log("Lose...");
                    loseScreen.SetActive(true);
                    loseScreen.transform.SetAsLastSibling();
                    gameOver = true;
                    return;
                }
                slain++;
                slainText.text = slain.ToString();
                if (slain >= 26) {
                    winSound.Play(0);
                    Debug.Log("Win!");
                    winScreen.SetActive(true);
                    winScreen.transform.SetAsLastSibling();
                    gameOver = true;
                    return;
                }
            } else if (currRound[i].suit == "Hearts") {
                if (!healed) {
                    health += currRound[i].rank;
                    if (health > 20) {
                        health = 20;
                    }
                    healed = true;
                } 
            } else if (currRound[i].suit == "Diamonds") {
                currEquip = currRound[i].rank;
                currDura = 15;
                equipImage.SetActive(true);
                equipText.text = currEquip.ToString();
                duraObj.SetActive(false);
            }
            healthText.text = health.ToString();
        }
        currRound.RemoveRange(0, 3);
        NewRound(3);
        avoided = false;
        faceSound.Play(0);
        DebugLog();
        Debug.Log("Cards Left: " + currDeck.Count + "   Avoided Cards: " + avoidDeck.Count);
    }

    public void Avoid()
    {
        if (!avoided) {
            avoidDeck.AddRange(currRound);
            currRound.Clear();
            NewRound(4);
            avoided = true;
        } 
        avoidSound.Play(0);
        DebugLog();
        Debug.Log("Cards Left: " + currDeck.Count + "   Avoided Cards: " + avoidDeck.Count);
    }

    public void SwapIndex(int swap1, int swap2)
    {
        card temp = currRound[swap1];
        currRound[swap1] = currRound[swap2];
        currRound[swap2] = temp;
    }

    public void SwapCards(int swap1, int swap2) 
    {
        GameObject temp = currRoundCards[swap1];
        Vector3 tempPos = currRoundCards[swap1].transform.position;
        currRoundCards[swap1] = currRoundCards[swap2];
        currRoundCards[swap1].transform.position = currRoundCards[swap2].transform.position;
        currRoundCards[swap2] = temp;
        currRoundCards[swap2].transform.position = tempPos;
    }

    public void Nudge(int index, int pos)
    {
        RectTransform temp = currRoundCards[index].GetComponent<RectTransform>();

        if (pos == 0) temp.anchoredPosition = pos1;
        if (pos == 1) temp.anchoredPosition = pos2;
        if (pos == 2) temp.anchoredPosition = pos3;
        if (pos == 3) temp.anchoredPosition = pos4;
    }

    public void RoundReset()
    {
        newRound = false;
    }

    public void DebugLog()
    {
        Debug.Log("Health: " + health);
        Debug.Log("Slain: " + slain);
        Debug.Log("Weapon: " + currEquip + ", " + currDura);
        foreach (var card in currRound)
        {
            Debug.Log("Current Round: " + card.suit + ", " + card.rank);
        }
    }
}
