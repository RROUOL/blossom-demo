using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityCommon;
using Naninovel;

public class SetupNani : MonoBehaviour
{
    private async void Awake ()
    {
        // 1. Initialize Naninovel.
        await RuntimeInitializer.InitializeAsync();

        // 2. Enter novel mode.
        var switchCommand = new SwitchToNovelMode();
        await switchCommand.ExecuteAsync();
    }
}