using Naninovel;
using Naninovel.Commands;
//using UnityCommon;
using UnityEngine;
using Vaz.ManagerSingletons;

[CommandAlias("adventure")]
public class SwitchToAdventureMode : Command
{
    [ParameterAlias("reset")]
    public BooleanParameter ResetState = true;

    public override async UniTask ExecuteAsync (AsyncToken asyncToken = default)
    {
        // 1. Disable Naninovel input.
        var inputManager = Engine.GetService<IInputManager>();
        inputManager.ProcessInput = false;

        // 2. Stop script player.
        var scriptPlayer = Engine.GetService<IScriptPlayer>();
        scriptPlayer.Stop();

        // 3. Hide text printer.
        var hidePrinter = new HidePrinter();
        hidePrinter.ExecuteAsync(asyncToken).Forget();

        // 4. Reset state (if required).
        if (ResetState)
        {
            var stateManager = Engine.GetService<IStateManager>();
            await stateManager.ResetStateAsync();
        }

        // 5. Switch cameras.
        var advCamera = GameObject.Find("Player/Main Camera").GetComponent<Camera>();
        if (advCamera == null) Debug.LogError("AdventureModeCamera not found");
        advCamera.enabled = true;
        var naniCamera = Engine.GetService<ICameraManager>().Camera;
        naniCamera.enabled = false;

        // 6. Enable character control.
        GameManager.singleton.isInputBlocked = false;
        GameManager.singleton.canDungeonPause = true;
        Debug.Log("inputs turned on");
    }
}
