﻿using QuestDumper;
using UnityEngine;


public class AdbHandler : MonoBehaviour
{
    private BetterToggle _betterToggle;

    private void Start()
    {
        _betterToggle = GetComponent<BetterToggle>();
        // Set toggle
        SetBetterToggleValue(Adb.IsAdbInstalled(out _));
    }


    private void SetBetterToggleValue(bool val)
    {
        // Update the toggle manually
        if (_betterToggle.IsOn == val) return;

        _betterToggle.OnPointerClick(null);
    }

    /// <summary>
    /// Toggles ADB installation
    ///
    /// This in reality is just for downloading ADB,
    /// would rather it be a button and hidden when it is installed
    ///
    /// </summary>
    public void ToggleADB()
    {
        if (!Adb.IsAdbInstalled(out _))
        {
            StartCoroutine(AdbUI.DoDownload());
        }
        else
        {
            StartCoroutine(Adb.RemoveADB());
        }
        
        _betterToggle.IsOn = Adb.IsAdbInstalled(out _);
    }
}

