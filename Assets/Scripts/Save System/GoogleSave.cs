using System;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine.SocialPlatforms;

public class GoogleSave : MonoBehaviour
{
    /*
     * Google play services is currently with and upload SH1 key
     * configured, which means it works on apk builds but only for the developer
     * to work on google play for everyone, check minute 23:
     * https://www.youtube.com/watch?v=0LGs1Xtt_1I
     */ 


    public static PlayGamesPlatform platform;
    public ISavedGameMetadata currentGame;
    public bool authenticated = false;

    SaveSystem saveSystem;
    
    


    // Start is called before the first frame update
    void Awake()
    {
        saveSystem = GetComponent<SaveSystem>();


        if (platform == null)
        {
            //creates a new configuration
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
                .Builder()
                .EnableSavedGames()
                .Build();

            //applies configurations to instance platform
            PlayGamesPlatform.InitializeInstance(config);


            //enable log for development
            PlayGamesPlatform.DebugLogEnabled = true;

            //saves the platform to an variable
            platform = PlayGamesPlatform.Activate();

        }

        //this is a lambda expression, that creates an methos for authetication on the fly
        Social.localUser.Authenticate(success =>
        {
            if (success)
            {
                Debug.Log("myTest:"+"Authentication worked");
                authenticated = true;
                
            }
            else
            {
                Debug.LogWarning("Authentication faield");
                
                
            }
            saveSystem.CheckMatchReference();
            saveSystem.Load("PlayerStatus");
        });
        
    }

    

    #region callBacks

    
    public void ReadPlayerReference(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log("myTest:"+"ReadPlayerReference Called");
            //register saved game metadata
            currentGame = game;

            Action<SavedGameRequestStatus, byte[]> readBinaryCallback =
            (SavedGameRequestStatus _status, byte[] data) =>
            {
                if (_status == SavedGameRequestStatus.Success)
                {
                    Debug.Log("myTest:"+"readBinaryCallback Called");
                    // Read MatchReference

                    //hand of the byte[]
                    saveSystem.CloudLoad(data);                    
                }
                else
                {
                    Debug.LogError("faield to read binary data from saved game");
                }

            };

            //this function requires a new function to be called when its finished,
            //to handle the byte[] it ouputs
            PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(game, readBinaryCallback);
            
        }
        else
        {
            Debug.LogError("faield to load savedGame");
        }
    }
    
    public void WritePlayerReference(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if(status == SavedGameRequestStatus.Success)
        {
            Debug.Log("myTest:"+"Saved to GPG");
        }
        else
        {
            Debug.Log("myTest:"+"faield to save to GPG");
        }
    }


    //future callback for conflict resolution
    public void ComparePlayerReferences()
    {

    }
    #endregion


    #region CodeFromGoogle


    //git source: https://github.com/playgameservices/play-games-plugin-for-unity

    //source: https://codelabs.developers.google.com/codelabs/playservices_unity/index.html?index=..%2F..index#9

    public void ReadSavedGame()
    {
        //gets the object for reference
        ISavedGameClient savedGameClient = platform.SavedGame;

        //callback for checking if there is saved game data
        Action<SavedGameRequestStatus, List<ISavedGameMetadata>> readSavedGame =
            (SavedGameRequestStatus status, List<ISavedGameMetadata> metaDataList) =>
        {
            if (status == SavedGameRequestStatus.Success)
            {
                if (metaDataList.Count > 0)
                {
                    Debug.Log("myTest:"+"found a save in cloud");
                    //when payments are implemented, create a manual callback for conflict resolution
                    savedGameClient.OpenWithAutomaticConflictResolution(
                        "PS",
                        DataSource.ReadCacheOrNetwork,
                        ConflictResolutionStrategy.UseLongestPlaytime,
                        ReadPlayerReference);
                        
                }
                else
                {
                    Debug.Log("myTest:"+"no saves found in cloud");
                    saveSystem.CloudLoad();
                    return;
                }
            }
            else
            {
                Debug.Log("myTest:"+"problem fetching cloud metadata");
            }
        };

        savedGameClient.FetchAllSavedGames(DataSource.ReadCacheOrNetwork, readSavedGame);


    }

    public void WriteSavedGame(byte[] playerBytes)
    {
        ISavedGameMetadata game;
        if (currentGame != null)
        {
            game = currentGame;

        }
        else
        {
            Debug.LogError("current game is null");
            return;
        }

        //creates a builder to update metadata
        SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder()
            .WithUpdatedPlayedTime(TimeSpan.FromMinutes(game.TotalTimePlayed.Minutes + 1))
            .WithUpdatedDescription("Saved at: " + System.DateTime.Now);

        //creates the updated metadata
        SavedGameMetadataUpdate updatedMetadata = builder.Build();


        //commit this game with updated metadada, saved data and do something       
        platform.SavedGame.CommitUpdate(game, updatedMetadata, playerBytes, WritePlayerReference);
    }
    #endregion
}
