using UnityEngine;
using UnnyNet;

public class StoreWindow : MonoBehaviour
{
    [SerializeField]
    private Transform content;
    
    [SerializeField]
    private GameObject prefab;

    private void Start()
    {
        FillContent();
    }

    private void FillContent()
    {
        var allCards = DataEditor.CardModels;
        foreach (var card in allCards)
        {
            var newCardUI = GameObject.Instantiate(prefab);
            newCardUI.transform.SetParent(content);
            newCardUI.transform.localScale = Vector3.one;

            var cardUI = newCardUI.GetComponent<CardUI>();
            
            var gameCard = new GameCard(card);
            cardUI.SetCard(gameCard);
        }
    }
}
