using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitCInput : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        cInput.Init();
    }

    void Start()
    {
        // Generic/UI Controls

        cInput.SetKey("Submit", Keys.E);
        cInput.SetKey("Cancel", Keys.F);
        
        cInput.SetKey("Up", Keys.W);
        cInput.SetKey("Down", Keys.S);
        cInput.SetKey("Left", Keys.A);
        cInput.SetKey("Right", Keys.D);

        cInput.SetAxis("Vertical", "Down", "Up");
        cInput.SetAxis("Horizontal", "Left", "Right");

        // Dungeon Crawl Controls

        cInput.SetKey("Turn Left", Keys.LeftShift);
        cInput.SetKey("Turn Right", Keys.Space);
        cInput.SetKey("Menu", Keys.Tab);
        cInput.SetKey("Interact", Keys.E);

        // Visual Novel Controls - InputConfiguration.cs - Naninovel

        // Submit
        // Cancel
        cInput.SetKey("Delete", Keys.Delete);
        cInput.SetKey("Continue", Keys.Delete);
        cInput.SetKey("Pause", Keys.Backspace);
        cInput.SetKey("Skip", Keys.LeftControl);
        cInput.SetKey("ToggleSkip", Keys.Tab);
        cInput.SetKey("AutoPlay", Keys.LeftShift); // originally A
        cInput.SetKey("ToggleUI", Keys.Space);
        cInput.SetKey("ShowBacklog", Keys.L);
        //cInput.SetKey("Rollback", ""); rollback isn't allowed
        //cInput.SetKey("CameraLookX", "");
        //cInput.SetKey("CameraLookY", "");
        
    /*
        // Battle Controls
        // This stuff is leftover from AURORAE. I used it as a reference for how to use cInput in this project.

        cInput.SetKey("Interact", "E");
        cInput.SetKey("Dodge", "LeftAlt");
        cInput.SetKey("Jump", "Space");
        cInput.SetKey("LeftLightAttack", "I");
        cInput.SetKey("LeftHeavyAttack", "U");
        cInput.SetKey("RightLightAttack", "O");
        cInput.SetKey("RightHeavyAttack", "P");
        cInput.SetKey("Macro", "LeftShift");

        cInput.SetKey("Menu", "Escape");
        cInput.SetKey("Inventory", "Return");
    */

        Debug.Log("CInput Initialised");

        for (int n = 0; n < cInput.length; n++) 
        {
            Debug.Log(n + ": " + cInput.GetText(n, 0) + " - " + cInput.GetText(n, 1) + " - " + cInput.GetText(n, 2));
        }

    }
}
