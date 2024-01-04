using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vaz.Constants.Consts;

public class ConstsCheck : MonoBehaviour
{
    public Text text;

    // Update is called once per frame
    void Update()
    {
        text.text = getConsts();
    }

    string getConsts()
    {
        string checker = "foobar";

        string RNG_MISS_CRIT = Consts.RNG_MISS_CRIT ? "1" : "0";
        string RNG_DMG_VARI = Consts.RNG_DMG_VARI ? "1" : "0";
        string RNG_ENEM_TABL = Consts.RNG_ENEM_TABL ? "1" : "0";
        string RNG_ENCNTR_RNDM = Consts.RNG_ENCNTR_RNDM ? "1" : "0";
        string RNG_ESCAPE_PROC = Consts.RNG_ESCAPE_PROC ? "1" : "0";

        return (RNG_MISS_CRIT + " " + RNG_DMG_VARI + " " + RNG_ENEM_TABL + " " + RNG_ENCNTR_RNDM + " " + RNG_ESCAPE_PROC);
    }
}
