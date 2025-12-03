// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple billboard component that orients the GameObject to face the main camera.
/// Useful for world-space UI canvases and labels so they remain legible to the player.
/// </summary>
public class Billboard : MonoBehaviour
{
    /// <summary>
    /// LateUpdate used to ensure camera transform has been updated for the frame
    /// before orienting the billboard, producing stable camera-facing behaviour.
    /// </summary>
    void LateUpdate()
    {
        // Look towards the camera by projecting camera forward from the object's position
        transform.LookAt(transform.position + Camera.main.transform.forward);
    }
}
