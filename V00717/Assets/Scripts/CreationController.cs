﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

// This class contains the baby model and controls its attributes
// TODO refactor monobehaviour specific routines into a seperate class, in order to add a normal constructor
public class CreationController
{
    /// <summary>
    /// Ref to the game controller
    /// </summary>
    public GameController GameController;
    private GameObject characterModelPrefab;
    // The max n of colonists (temporary n)
    public static int MAX_COLONISTS = 3;
    /// <summary>
    /// The possible tracklane positions to start each new character
    /// </summary>
    private Vector3[] trackLanePositions;
    /// <summary>
    /// These cameras follow/track a character in its lane (by index, going up to 3)
    /// </summary>
    private Camera[] laneFeedCams;
    public Camera[] LaneFeedCams { get { return laneFeedCams; } set { laneFeedCams = value; } }

    // SERVER REQUESTS
    public delegate void RequestColonistDataResponse(List<GameObject> colonists, Enums.DataRequests request);
    public static event RequestColonistDataResponse _OnRequestColonistDataResponse;

    public CreationController(GameObject characterModelPrefab, Vector3[] trackLanePositions, Camera[] laneFeedCams)
    {
        TriggerCreationMenu._OnTriggerCreationMenuAction += MallocNewCharacter;
        DashboardOSController._OnRequestColonistData += OnServerReply;
        CharacterCreationView._OnSexChanged += OnSexChanged;
        SaveSystem._SuccessfulSaveAction += ResetCharacterCache;
        GameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>(); // TODO temporary, will be passed by GameController itself
        this.characterModelPrefab = characterModelPrefab;
        this.trackLanePositions = trackLanePositions;
        this.laneFeedCams = laneFeedCams;
    }

    ~CreationController()
    {
        TriggerCreationMenu._OnTriggerCreationMenuAction -= MallocNewCharacter;
        DashboardOSController._OnRequestColonistData -= OnServerReply;
        CharacterCreationView._OnSexChanged -= OnSexChanged;
        SaveSystem._SuccessfulSaveAction -= ResetCharacterCache;
    }

    public void ResetCharacterCache()
    {
        MallocNewCharacter();
    }

    // Re-allocate memory for a new character
    public void MallocNewCharacter()
    {
        GameController.CharacterModel = new CharacterModelObject();
    }

    // Setter for new colonist nickname
    public void OnNickNameChanged(string nickName)
    {
        GameController.CharacterModel.NickName = nickName;
    }

    public void OnSexChanged(string sex)
    {
        GameController.CharacterModel.Sex = sex;
    }

    //Setter for skin color changed (red slider)
    public void OnSkinColorChanged_R(float r)
    {
        GameController.CharacterModel.SkinColorR = r;
    }

    //Setter for skin color changed (green slider)
    public void OnSkinColorChanged_G(float g)
    {
        GameController.CharacterModel.SkinColorG = g;
    }

    //Setter for skin color changed (blue slider)
    public void OnSkinColorChanged_B(float b)
    {
        GameController.CharacterModel.SkinColorB = b;
    }

    // Called on finalize creation menu
    public void CreateNewColonist()
    {
        if (!CreationMenuController.validEntry || GameController.Colonists.Count > MAX_COLONISTS)
        {
            return;
        }
        // Create a characterModel component to attach to its mesh game object
        // TODO update UUID in a more reliable new way
        CharacterModelObject.uniqueColonistPersonnelID++;
        CreateNewCharacterMesh(GameController.CharacterModel);
    }

    public string GetStartingItemKey()
    {
        int tier = 0;
        float donationMoney = GameController.DonationMoney;
        if (donationMoney <= 100.0f)
        {
            tier = 0;
        } else if (donationMoney >= 100.0f && donationMoney <= 500.0f)
        {
            tier = 1;
        } else if (donationMoney >= 500.0f && donationMoney <= 1000.0f)
        {
            tier = 2;
        } else if (donationMoney >= 1000.0f && donationMoney <= 5000.0f)
        {
            tier = 3;
        } else if (donationMoney >= 5000.0f && donationMoney <= 10000.0f)
        {
            tier = 4;
        } else if (donationMoney >= 10000.0f && donationMoney <= float.MaxValue)
        {
            tier = 5;
        }

        string[][] startingItems = {
            new string[] { "PAPER_PLANE", "BAND_AID", "COUGH_SYRUP", "BALLOON", "PLASTIC_SPOON" },
            new string[] { "WOODEN_SPOON", "SYRINGE", "EXPERIMENT_RAT", "METAL_CHAIR", "FIRST-AID" },
            new string[] { "METAL_SPOON", "DAGGER", "G-VACCINE", "PROTECTION_GOOGLES", "GAUZE" },
            new string[] { "DIAMOND_SPOON", "HEALING_POTION", "G-VACCINE", "RESPIRATOR", "NANO-MEDICATION" },
            new string[] { "LASER_GUN", "SPOON_HAMMER", "IMMUNE-AI-BOOSTER", "OXYGEN-TANK", "PARACHUTE" },
            new string[] { "ARBITER_FLUTE", "MASTER_GLOVE", "FREEDOM_CONTRACT", "NUKE" }
        };
        int randInt = UnityEngine.Random.Range(0, startingItems[tier].Length);

        return startingItems[tier][randInt];
    }

    public void GetStarterItem()
    {
       string itemKey = GetStartingItemKey();
       
    }

    internal void CreateNewCharacterMesh(CharacterModelObject newCharacterModel)
    {
        if (!CreationMenuController.validEntry)
        {
            return;
        }

        // Set the new Material runner games character to the last track position (set from live game character count)
        int trackLanePosition = FindAvailableCameraLane();

        GameObject newCharacterMesh = null;
        try
        {
            newCharacterMesh = GameObject.Instantiate(characterModelPrefab, trackLanePositions[trackLanePosition], Quaternion.Euler(new Vector3(90.0f, 0.0f, 0.0f)));
            newCharacterMesh.gameObject.name = newCharacterModel.Name();
            newCharacterMesh.GetComponent<CharacterModel>().InitCharacterModel(newCharacterModel);
            newCharacterMesh.GetComponent<CharacterModel>().InitEventsMarkersFeed(); // Inits events feed and last event but they're null at this stage
            newCharacterMesh.GetComponent<CharacterModel>().UniqueColonistPersonnelID_ = CharacterModelObject.uniqueColonistPersonnelID; // Sets the uuid field, not the static one as it wont be serialized
            // Update UUID for application length - then needs to be saved to file
            GameController.Colonists.Add(newCharacterMesh);

            // TODO Set its mesh to the players' choices using the character model component        
            SetTrackLanePosition(trackLanePosition, newCharacterMesh.transform);
        }
        catch (ArgumentNullException ane)
        {
            Debug.Log(ane.Message);
            Debug.LogError("Error: No prefab model for characters loaded.");
        }
        catch (ArgumentException ae)
        {
            Debug.LogError(ae.Message);
        }
    }

    public int FindAvailableCameraLane()
    {
        // Set the new Material runner games character to the last track position (set from live game character count)
        int trackLanePosition = 0;

        for (int i = 0; i < laneFeedCams.Length; i++)
        {
            // If the target is null (not occupied yet) or dead, then the new character can evict them/take their position
            CharacterTracker characterTracker = laneFeedCams[i].GetComponent<CharacterTracker>();

            if (characterTracker.Target == null || characterTracker.Target.GetComponent<CharacterModel>().isDead())
            {
                trackLanePosition = i;
                break;
            }
        }
        return trackLanePosition;
    }

    public void SetTrackLanePosition(int trackLanePosition, Transform cameraTarget)
    {
        laneFeedCams[trackLanePosition].GetComponent<CharacterTracker>().SetTarget(cameraTarget);
        cameraTarget.GetComponent<CharacterModel>().TrackLanePosition = trackLanePosition;
    }

    // Handle client requests
    public void OnServerReply(Enums.DataRequests requestPort)
    {
        switch (requestPort)
        {
            case Enums.DataRequests.LIVE_COLONISTS:
                _OnRequestColonistDataResponse(GameController.Colonists, Enums.DataRequests.LIVE_COLONISTS);
                break;
            case Enums.DataRequests.DEAD_COLONISTS:
                _OnRequestColonistDataResponse(GameController.DeadColonists, Enums.DataRequests.DEAD_COLONISTS);
                break;
            default:
                break;
        }
    }
}
