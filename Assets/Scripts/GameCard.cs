using UnnyNet.Models;

public class GameCard
{
    private int _armor;
    private int _damage;
    private CardModel _cardModel;

    public int Armor => _armor;
    public int Damage => _damage;

    public string Name => _cardModel.Name;
    public string Sprite => _cardModel.Sprite;
    public int Mana => _cardModel.Mana;

    public GameCard(CardModel cardModel)
    {
        _cardModel = cardModel;
        _armor = cardModel.Armor;
        _damage = cardModel.Damage;
    }
}
