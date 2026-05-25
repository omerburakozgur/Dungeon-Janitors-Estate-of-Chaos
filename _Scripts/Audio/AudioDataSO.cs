/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */

using UnityEngine;

/// <summary>
/// ScriptableObject container that holds all audio asset references for the game.
/// </summary>
[CreateAssetMenu(menuName = "Audio/AudioData")]
public class AudioDataSO : ScriptableObject
{
    [Header("Music")]
    [Tooltip("Background music played in the tavern/main menu.")]
    public AudioClip menuMusic;

    [Tooltip("Background music played during the main game scene.")]
    public AudioClip mainSceneMusic;

    [Header("Player")]
    [Tooltip("Collection of footstep audio clips.")]
    public AudioClip[] footSteps;

    [Tooltip("Audio clip played when the player jumps.")]
    public AudioClip jump;

    [Tooltip("Audio clip played when the player takes damage.")]
    public AudioClip hurt;

    [Tooltip("Audio clip played when the player dies.")]
    public AudioClip death;

    [Header("Footstep Sounds")]
    [Tooltip("Audio clips for walking on wood surfaces.")]
    public AudioClip[] woodFootsteps;

    [Tooltip("Audio clips for walking on stone surfaces.")]
    public AudioClip[] stoneFootsteps;

    [Tooltip("Audio clips for walking on carpet surfaces.")]
    public AudioClip[] carpetFootsteps;

    [Tooltip("Audio clips for walking on metal surfaces.")]
    public AudioClip[] metalFootsteps;

    [Header("Combat & Tools")]
    [Tooltip("Sound effect for swinging a sword.")]
    public AudioClip swordSwing;

    [Tooltip("Sound effect for drinking a potion.")]
    public AudioClip potionDrink;

    [Header("Cleaning Loops")]
    [Tooltip("Looping sound for the mop tool.")]
    public AudioClip mopLoop;

    [Tooltip("Looping sound for the broom tool.")]
    public AudioClip broomLoop;

    [Tooltip("Looping sound for the vacuum tool.")]
    public AudioClip vacuumLoop;

    [Tooltip("Default looping sound for generic cleaning actions.")]
    public AudioClip defaultCleaningLoop;

    [Header("Interactions & World")]
    [Tooltip("Sound played when opening a chest.")]
    public AudioClip chestOpen;

    [Tooltip("Sound played when picking up loot items.")]
    public AudioClip lootPickup;

    [Tooltip("Sound played when clicking on UI buttons.")]
    public AudioClip uiClick;

    [Tooltip("Sound effect for spike trap activation.")]
    public AudioClip spikeTrap;

    [Tooltip("Looping sound effect for torches.")]
    public AudioClip torchLoop;

    [Tooltip("Sound played when a mission is successfully completed.")]
    public AudioClip missionSuccess;

    [Tooltip("Sound played when a trap door is unlocked.")]
    public AudioClip trapDoorUnlock;

    [Tooltip("Sound played when a trap door creaks open.")]
    public AudioClip trapDoorCreak;

    [Header("Doors")]
    [Tooltip("Sound for opening a wooden door.")]
    public AudioClip woodDoorOpen;

    [Tooltip("Sound for closing a wooden door.")]
    public AudioClip woodDoorClose;

    [Tooltip("Sound for opening a metal door.")]
    public AudioClip metalDoorOpen;

    [Tooltip("Sound for closing a metal door.")]
    public AudioClip metalDoorClose;

    [Tooltip("Sound for opening a heavy gate.")]
    public AudioClip heavyGateOpen;

    [Tooltip("Sound for closing a heavy gate.")]
    public AudioClip heavyGateClose;

    [Header("Minecart")]
    [Tooltip("Sound for starting the minecart or releasing brakes.")]
    public AudioClip minecartStart;

    [Tooltip("Looping sound for the minecart moving on rails.")]
    public AudioClip minecartLoop;
}