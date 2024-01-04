using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaytimeCount : MonoBehaviour, IDataPersistence
{
    public int playTime = 0;
    private float countTime = 0f;

    // Update is called once per frame
    void FixedUpdate()
    {
        // count up the seconds

        countTime += Time.deltaTime;
        
        if (countTime > 1.0f)
        {
            countTime = 0f;
            playTime += 1;
            //Debug.Log("Playtime second increment: " + playTime);
        }
    }

    public void LoadData(GameData data)
    {
        Debug.Log(this + " loaded for interface");
        this.playTime = data.playTime;
    }

    public void SaveData(ref GameData data)
    {
        Debug.Log(this + " saved for interface");
        data.playTime = this.playTime;
    }
}
