using UnityEngine;

[CreateAssetMenu(fileName = "NewPokemon", menuName = "Pokemon/PokemonData")]
public class PokemonData : ScriptableObject
{
    [Header("Identity")]
    public string pokemonName;
    public Sprite cardArt;
    public PokemonType type;

    [Header("Stats")]
    public int maxHP;
    public int currentHP;
    public bool isFainted => currentHP <= 0;

    [Header("Attacks")]
    public Attack[] attacks;

    [Header("Weakness / Resistance")]
    public PokemonType weakness;
    public PokemonType resistance;
    public int retreatCost;

    public void ResetHP() => currentHP = maxHP;

    public void TakeDamage(int damage)
    {
        currentHP = Mathf.Max(0, currentHP - damage);
    }
}

[System.Serializable]
public class Attack
{
    public string attackName;
    public int damage;
    public int energyCost;
    public string description;
}

public enum PokemonType
{
    Normal, Fire, Water, Grass, Electric,
    Ice, Fighting, Poison, Ground, Flying,
    Psychic, Bug, Rock, Ghost, Dragon,
    Dark, Steel, Fairy
}
