using UnityEngine;
using System.Collections.Generic;

public class CardDatabase : MonoBehaviour
{
    [Header("Drag all 12 PokemonData assets here")]
    public List<PokemonData> allPokemon = new List<PokemonData>();

    // Lookup dictionary built at runtime for fast access
    private Dictionary<string, PokemonData> _pokemonByName;

    private void Awake()
    {
        BuildLookup();
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Match a Vuforia image target name to a PokemonData asset.
    /// The target name in Vuforia MUST match PokemonData.pokemonName exactly.
    /// e.g. Vuforia target "Charizard" → Charizard.asset
    /// </summary>
    public PokemonData GetByName(string targetName)
    {
        if (_pokemonByName.TryGetValue(targetName, out PokemonData pokemon))
            return pokemon;

        Debug.LogWarning($"CardDatabase: No match found for Vuforia target '{targetName}'. " +
                         $"Make sure the image target name matches PokemonData.pokemonName exactly.");
        return null;
    }

    public bool Contains(string targetName) => _pokemonByName.ContainsKey(targetName);

    public int TotalCards => allPokemon.Count;

    // ── Private ───────────────────────────────────────────────────────────────

    private void BuildLookup()
    {
        _pokemonByName = new Dictionary<string, PokemonData>();

        foreach (var pokemon in allPokemon)
        {
            if (pokemon == null) continue;

            if (_pokemonByName.ContainsKey(pokemon.pokemonName))
            {
                Debug.LogWarning($"CardDatabase: Duplicate pokemonName '{pokemon.pokemonName}' found. Skipping.");
                continue;
            }

            _pokemonByName.Add(pokemon.pokemonName, pokemon);
        }

        Debug.Log($"CardDatabase: Loaded {_pokemonByName.Count} Pokémon.");
    }

    // ── Editor Helper ─────────────────────────────────────────────────────────
#if UNITY_EDITOR
    [ContextMenu("Log All Pokemon Names")]
    private void LogAllNames()
    {
        foreach (var p in allPokemon)
            Debug.Log($"  → {p?.pokemonName ?? "NULL"}");
    }
#endif
}