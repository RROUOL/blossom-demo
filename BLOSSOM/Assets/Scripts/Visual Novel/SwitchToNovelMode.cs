using Naninovel;
//using UnityCommon;
using UnityEngine;
using Vaz.ManagerSingletons;

[CommandAlias("novel")]
public class SwitchToNovelMode : Command
{
    public StringParameter ScriptName;
    public StringParameter Label;

    public override async UniTask ExecuteAsync (AsyncToken asyncToken = default)
    {
        Debug.Log("Novel Mode Activated");

        // 1. Disable character control.
        GameManager.singleton.isInputBlocked = true;
        GameManager.singleton.canDungeonPause = false;
        Debug.Log("inputs turned off");

        // 2. Switch cameras.
        var advCamera = GameObject.Find("Player/Main Camera").GetComponent<Camera>();
        if (advCamera == null) Debug.LogError("AdventureModeCamera not found");
        advCamera.enabled = false;
        Debug.Log("advCamera: " + advCamera.name);
        var naniCamera = Engine.GetService<ICameraManager>().Camera;
        naniCamera.enabled = true;
        Debug.Log("naniCamera: " + naniCamera.name);

        // 3. Load and play specified script (is required).
        if (Assigned(ScriptName))
        {
            var scriptPlayer = Engine.GetService<IScriptPlayer>();
            await scriptPlayer.PreloadAndPlayAsync(ScriptName, label: Label);
        }

        // 4. Enable Naninovel input.
        var inputManager = Engine.GetService<IInputManager>();
        inputManager.ProcessInput = true;
    }
}
