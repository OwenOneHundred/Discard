using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardRewardObj : MonoBehaviour
{
    [SerializeField] GameObject textPrefab;
    GameObject player;
    [SerializeField] float interactionDistance = 5;
    GameObject associatedText;
    [SerializeField] float offset = 3;
    public static Pair<GameObject, float> closestPair;
    bool setUp = false;

    public List<Card> offeredCards;

    void Start()
    {
        associatedText = Instantiate(textPrefab, transform.position, Quaternion.identity, GameObject.Find("WorldSpaceCanvas").transform);
        player = GameObject.FindGameObjectWithTag("Player");
        if (!setUp)
        {
            Debug.LogWarning("CardRewardObj was not immediately set up after being spawned. Call CardRewardObj.SetUp().");
        }
    }

    public void SetUp(List<Card> newOfferedCards)
    {
        offeredCards = newOfferedCards;
        setUp = true;
    }

    private void LateUpdate()
    {
        associatedText.transform.position = transform.position + new Vector3(0, offset, 0);
    }

    private void Update()
    {
        closestPair ??= new Pair<GameObject, float>(gameObject, 666);

        float distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
        if (distanceToPlayer < interactionDistance)
        {
            associatedText.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (closestPair.left == gameObject)
                {
                    OnInteracted();
                    return;
                }
            }

            if (closestPair.right > distanceToPlayer)
            {
                closestPair.left = gameObject;
                closestPair.right = distanceToPlayer;
            }
        }
        else
        {
            associatedText.SetActive(false);
        }
    }

    void OnInteracted()
    {
        closestPair = null;
        GameObject.Find("UI").GetComponent<UIManager>().cardSelectionUI.GetComponent<CardSelectionManager>().CreateCardSelectionMenu(offeredCards);
        Destroy(gameObject);
        Destroy(associatedText);
    }
}
