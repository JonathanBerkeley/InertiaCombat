﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Multiplayer variant of LaunchProjectile
public class MultiplayerLaunchProjectile : MonoBehaviour
{
    public static float projectileSpeed = 50.0f;
    public float adjustSpawnPositionY = 0.3f;
    public float launchDelay = 1.0f;

    public GameObject launchEffect;
    public AudioClip[] soundEffects;
    public GameObject projectilePrefab;

    private Rigidbody launchBlock;
    private GameObject launchObject;
    private bool canLaunch;

    //For stats handling, ensuring player has ammo
    private GameObject _player;
    private MultiplayerStats _playerStats;

    void Start()
    {
        //Gets a reference to the block inside the launcher
        launchBlock = gameObject.GetComponent<Rigidbody>();
        launchBlock.transform.parent = transform.parent.transform;

        //Gets a reference to the player
        _player = gameObject.transform.root.gameObject;
        _playerStats = _player.GetComponent<MultiplayerStats>();

        canLaunch = true;
    }

    void Update()
    {
        if (StaticInput.GetShooting() && canLaunch)
        {
            //Embedded for efficiency
            int _pAmmo = _playerStats.GetAmmo();
            if (_pAmmo > 0)
            {
                FireProjectile();

                //Sets the ammo to one less
                _playerStats.SetAmmo(--_pAmmo);
                StartCoroutine(PauseFiring());
            }
        }

        //Keeps the firing particle effects on the launcher regardless of speed
        if (launchObject != null)
        {
            launchObject.transform.position = launchBlock.transform.position;
        }
    }

    void FireProjectile()
    {
        //Adjust Y position of projectile launch if necessary
        Vector3 adjustedPosition = transform.position;
        adjustedPosition.y -= adjustSpawnPositionY;

        //Launch particle effect
        launchObject = (GameObject)Instantiate(launchEffect, transform.position, transform.rotation);

        //Plays audio for launch
        AudioSource.PlayClipAtPoint(soundEffects[0], transform.position);

        //Creates rocket at top of launcher
        GameObject projectile = Instantiate(projectilePrefab, adjustedPosition, transform.rotation);

        //ClientSend.ProjectileLaunchData(adjustedPosition, transform.rotation);

        SendProjectileDataToServer(projectile);


        //Gives the rocket an ID to parent so that it doesn't collide with owner of rocket
        projectile.GetComponent<MultiplayerProjectile>()
            .SetParentByID(
                PlayerID.GetIDByGameObject(transform.parent.root.gameObject)
            );

        //Gives rocket momentum
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * projectileSpeed, ForceMode.VelocityChange);

        //Cleanup for efficiency
        Destroy(launchObject, 2);
    }

    IEnumerator PauseFiring()
    {
        canLaunch = false;
        yield return new WaitForSeconds(launchDelay);
        canLaunch = true;
    }

    private void SendProjectileDataToServer(GameObject projectile)
    {
        var _location = projectile.transform.position;
        var _rotation = projectile.transform.rotation;
        ClientSend.ProjectileLaunchData(_location, _rotation);
    }
}
