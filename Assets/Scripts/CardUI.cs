using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [SerializeField]
    private Text Mana;
    [SerializeField]
    private Text Damage;
    [SerializeField]
    private Text Armor;
    [SerializeField]
    private Text Name;
    [SerializeField]
    private Text Description;
    [SerializeField]
    private Image Sprite;
    
    
    private GameCard _gameCard;

    public void SetCard(GameCard gameCard)
    {
        _gameCard = gameCard;
        Refresh();
    }

    private void Refresh()
    {
        Mana.text = _gameCard.Mana.ToString();
        Damage.text = _gameCard.Damage.ToString();
        Armor.text = _gameCard.Armor.ToString();

        Name.text = _gameCard.Name;
        
        ObjectsPool.GetSprite(_gameCard.Sprite, sprite => { Sprite.sprite = sprite; });
    }
}
