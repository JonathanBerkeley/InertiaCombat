﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Multiplayer variant for the ammo pack
public class MultiplayerAmmoPack : MonoBehaviour
{
    public int respawnDelay = 10;
    public float spinSpeed = 200.0f;
    public int resupplyAmount = 10;
    public AudioClip[] packAudio;

    void Update()
    {
        //Spinning animation
        transform.Rotate(0, 0, spinSpeed * Time.deltaTime, Space.Self);
    }

    //Detecting if a player touches ammopack
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == 9)
        {
            //Increases the player's ammo that touches the ammo box
            try 
            {
                collision.gameObject.GetComponent<MultiplayerStats>().IncreaseAmmo(resupplyAmount);
            } 
            catch 
            { 
            }

            StartCoroutine(RespawnAmmoPack());
        }
    }

    IEnumerator RespawnAmmoPack()
    {
        //Play restore ammo sound
        AudioSource.PlayClipAtPoint(packAudio[1], transform.position);

        //Hide under world
        transform.position = new Vector3(
            transform.position.x
            , transform.position.y - 1000
            , transform.position.z);

        //Wait for two seconds efficiently
        yield return new WaitForSeconds(respawnDelay);

        //Reset to position
        transform.position = new Vector3(
            transform.position.x
            , transform.position.y + 1000
            , transform.position.z);

        //Play respawn sound
        AudioSource.PlayClipAtPoint(packAudio[0], transform.position);
    }
}
