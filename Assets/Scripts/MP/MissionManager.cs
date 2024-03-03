using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Photon.Pun;

public class MissionManager : MonoBehaviourPun
{
    private int numMissionsRemain;
    private int numMissions;
    private bool requestedSyncWithHost;
    [Header("Missions")]
    public bool killNumEnemies;
    public int enemyAmount;
    public bool getToEnd;
    public GameObject endTrigger;
    public bool killBoss;
    public GameObject boss;

    [Header("Other Info")]
    public int numEnemiesKilled;
    public int numPlayersFinished;
    public bool isBossDead;
    public bool allMissionsComplete;

    public static MissionManager instance;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Set allMissionsComplete to false for good measure.
        allMissionsComplete = false;

        //Set the amount of missions based off what bools are selected to be true in the scene.
        if (killNumEnemies) { numMissions++; }
        if (getToEnd) { numMissions++; }
        if (killBoss) { numMissions++; }

        //Missions can be combined. As such, we will keep track of the win condition based on the number of completed missions.
        numMissionsRemain = numMissions;

        //Set Pvp to False if numMissiosn > 1. These missions are co-op missions.
    }

    // Update is called once per frame
    void Update()
    {
        //Only run mission check code when mission is active.
        if (killNumEnemies) {CheckEnemyMission();}
        if (getToEnd) {CheckEndTriggerMission();}
        if (killBoss) { CheckBossMission(); }

        // All missions are complete when there is no more remaining missions.
        if (numMissionsRemain <= 0 && !allMissionsComplete && !GameManager.instance.pvp)
        {
            allMissionsComplete = true;
            GameManager.instance.CheckWinCondition();
        }
        
        // Checks if client sent a request to sync with host. Called by host to others.
    }
    

    //Checks if the "Kill certain number of enemies" mission is satisfied.
    void CheckEnemyMission() { 
        if(numEnemiesKilled >= enemyAmount - 1)
        {
            killNumEnemies = false;
            numMissionsRemain--;
        }
    
    }
    //Checks if the End Trigger Mission is satisfied by seeing if the number of finished players is greater than or equal to the alive player count.
    void CheckEndTriggerMission() { 
        if(numPlayersFinished >= GameManager.instance.alivePlayers && endTrigger != null)
        {
            getToEnd = false;
            numMissionsRemain--;
        }
    }
    //Checks if the Boss Mission is complete.
    void CheckBossMission() {
        if (isBossDead && boss != null)
        {
            killBoss = false;
            numMissionsRemain--;
        }
    
    }

    void CheckAllMission()
    {
        CheckEnemyMission();
        CheckEndTriggerMission();
    }

    //MODIFIER SECTION. Updates Stats and States. Implementation goes here. Other scripts should call these functions.

    public void KillEnemy()
    {
        photonView.RPC("KillEnemyRPC", RpcTarget.All);
    }

    public void enterEnd()
    {
        photonView.RPC("enterEndRPC", RpcTarget.All);
    }
    public void killedBoss()
    {
        photonView.RPC("killedBossRPC", RpcTarget.All);
    }

    [PunRPC]
    public void KillEnemyRPC() {
        numEnemiesKilled++;
        GameUI.instance.UpdatePlayerInfoText();
    }
    [PunRPC]
    public void enterEndRPC() {
        numPlayersFinished++;
        CheckEndTriggerMission();
        GameUI.instance.UpdatePlayerInfoText();
    }
    [PunRPC]
    public void killedBossRPC() {
        isBossDead = true;
        GameUI.instance.UpdatePlayerInfoText();
    }
    
    //The host is the one that keeps track of the mission, to keep it simple. This switch case should allow us to easily update the host, and then sync the host information with the others. This is to save on RPCs.
}
