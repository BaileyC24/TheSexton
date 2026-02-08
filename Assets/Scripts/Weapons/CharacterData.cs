using System.Collections.Generic;
using UnityEngine;

public enum CombatStyle
{
    Melee,
    Ranged,
    Hybrid
}

public enum CharacterRole
{
    DPS,
    Support,
    Control
}

[CreateAssetMenu(fileName = "CharacterData", menuName = "Game/Characters/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Identity")]
    public string characterName;
    [TextArea(2, 8)]
    public string bio;

    [Header("Presentation")]
    public Sprite profilePicture;
    public Sprite cardPicture;

    [Header("Gameplay Tags")]
    public CombatStyle combatStyle;
    public CharacterRole role;

    [Header("Weapons")]
    public WeaponData primaryWeapon;
    public WeaponData secondaryWeapon;

    [Header("Starting Resources")]
    public List<GameObject> startingInventory = new List<GameObject>();
    public int startingCoins;
}