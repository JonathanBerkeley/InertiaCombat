﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to maintain game settings through restarts etc
//Made following Unity official tutorial https://www.youtube.com/watch?v=uD7y4T4PVk0
public class GamePreferencesManager : MonoBehaviour
{
    public static GamePreferencesManager instance;

    const string MasterVolumeKey = "MasterVolume";
    const string EffectsVolumeKey = "EffectsVolume";
    const string FullscreenKey = "Fullscreen";
    const string MusicKey = "Music";
    const string ResolutionKey = "Resolution";

    private void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        LoadPrefs();
    }

    //Hook to save preferences on quit
    private void OnApplicationQuit()
    {
        SavePrefs();
    }

    //Save preferences to the registry
    //Also called by menu back button
    public void SavePrefs()
    {
        if (GlobalAudioReference.instance)
        {
            PlayerPrefs.SetFloat(MasterVolumeKey, GlobalAudioReference.instance.GetMasterVolume());
            PlayerPrefs.SetFloat(EffectsVolumeKey, GlobalAudioReference.instance.GetEffectsVolume());
        }

        PlayerPrefs.SetInt(MusicKey, MusicHandler.MusicSetting ? 1 : 0); //Music on main menu setting

        if (VideoSettings.instance)
        {
            PlayerPrefs.SetInt(FullscreenKey, VideoSettings.instance.GetFullscreen());
            PlayerPrefs.SetInt(ResolutionKey, VideoSettings.instance.GetResolution());
        }

        PlayerPrefs.Save();
    }

    //Load saved preferences from registry
    public void LoadPrefs()
    {
        if (GlobalAudioReference.instance)
        {
            //To make code neater
            var GAR = GlobalAudioReference.instance;

            GAR.SetMasterVolume(PlayerPrefs.GetFloat(MasterVolumeKey, GAR.GetMasterVolume()));
            GAR.SetEffectsVolume(PlayerPrefs.GetFloat(EffectsVolumeKey, GAR.GetEffectsVolume()));
        }

        MusicHandler.MusicSetting = PlayerPrefs.GetInt(MusicKey, 1) == 1; //Music on main menu setting

        if (VideoSettings.instance)
        {
            bool fsEnable = PlayerPrefs.GetInt(FullscreenKey, 0) != 0;

            VideoSettings.instance.ChangeFullscreen(fsEnable);
            VideoSettings.instance.ChangeResolution(PlayerPrefs.GetInt(ResolutionKey, 0));
        }
    }

    //Delete all saved preferences
    public void DeletePrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
