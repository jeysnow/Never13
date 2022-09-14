using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem instance;
    public int sceneToLoad, playerTotalEnergy;
    public bool isDownloading = false;
    public LoadingScreen loadingScreen;
    public PlayerStatus playerStatus;
    public Match match;
    public GoogleSave cloud;

    BinaryFormatter formatter;
    string basicPath;
    
    

    //SaveSystem will be initialized in loading scene, to make sure we load the correct scene
    private void Awake()
    {
        cloud = GetComponent<GoogleSave>();
        loadingScreen = GetComponentInChildren<LoadingScreen>();
        playerStatus = GetComponent<PlayerStatus>();
        match = GetComponent<Match>();

        #region singleton
        if (instance != this && instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        #endregion
        //initialize formatter
        formatter = new BinaryFormatter();
        DontDestroyOnLoad(this);

        //references
        

        //checks which plataform is used to play and adjust save path accordingly
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                basicPath = Application.persistentDataPath+"/";
                break;
            default:
                basicPath = "";
                break;

        }




    }

    //verifies if there is a localfile for matchreference, otherwise downloads it
    public void CheckMatchReference()
    {
        //check if has matchreference to play, if not, downloads it
        try
        {
            Load("MatchReference");
            Debug.Log("myTest:"+"MatchReference Loaded!");
        }
        catch
        {
            isDownloading = true;

            Action startOffine = () => {
                Load("MatchReference");
                
                
            };

            StartCoroutine(GetRequest(
                "MatchReference",
                "http://hostdatagamedev.000webhostapp.com/MatchReference.aj",
                startOffine
                ));
            return;
        }
        Load("MatchReference");
        
        
    }

    public bool CheckPlayerStatus(PlayerReference newPlayerReference)
    {
        if(newPlayerReference == null)
        {
            return false;            
        }        
        else
        {
            int newE = newPlayerReference.currentEnergy + newPlayerReference.spentEnergy,
                currentE = playerStatus.spentEnergy + playerStatus.currentEnergy;
            if (newE > currentE)
            {
                return true;
            }
            else
            { 
                return false;
                
            }
        }
        
    }

    public void Save(string fileName)
    {
        FileStream file;

        //set the name
        switch (fileName)
        {
            case "MatchReference":
                //creates or opens a file from name with custom ending
                fileName += ".aj";
                //create the file
                file = new FileStream(basicPath+fileName, FileMode.OpenOrCreate, FileAccess.Write);

                //serialize the data
                formatter.Serialize(file, match.matchReference);

                //dont forget to close!
                file.Close();

                break;
            case "PlayerStatus":
                

                fileName = "PS.aj";
                PlayerReference pR = playerStatus.SaveReference();
                LocalSave(fileName, pR);
                if(cloud.currentGame != null)
                {
                    cloud.WriteSavedGame(ObjectToByteArray(pR));
                }
                

                break;
            default:
                Debug.LogError("Wrong filename for Save");                
                return;
        }

        

        Debug.Log("myTest:"+"saved");
        
    }

    public void Load(string fileName)
    {
        
        
        switch (fileName)
        {
            case "MatchReference":
                FileStream file;
                fileName += ".aj";
                file = new FileStream(basicPath + fileName, FileMode.Open, FileAccess.Read);
                //check if file exists
                if (file != null)
                {
                    //Save the data on a Matchreference then pass the arrays to match.
                    match.matchReference = (MatchReference)formatter.Deserialize(file);
                    
                    //load from matchreference
                    match.LoadMatchference();

                    //Debug.Log("myTest:"+"loaded! first item of arrays:"+mR.CodeArrays[0][0]);
                    //dont forget to close!
                    file.Close();
                }
                else
                {
                    Debug.LogError("no file for MatchReference found");
                    
                }
                
                break;
            case "PlayerStatus":

                fileName = "PS.aj";
                
                var localPR = (PlayerReference)LocalLoad<PlayerReference>(fileName);

                if(localPR != null)
                {
                    playerStatus.LoadReference(localPR);

                    //choses correct scene to load after loading is done
                    
                }

                if (cloud.authenticated)
                {
                    cloud.ReadSavedGame();
                    Debug.Log("myTest:"+"readsavedgame called");
                }
                else
                {
                    Debug.Log("myTest:"+"not authenticated for loading");
                }
                loadingScreen.AfterLoad("Local");
                break;

            default:
                Debug.LogError("Wrong filename for Load");
                return;

        }

    }

    IEnumerator GetRequest(string filename, string request, Action callback = null)
    {
        filename += ".aj";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(request))
        {
            //request and wait for answer
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError)
            {
                Debug.LogError("Coudnl't find file at " + request);
            }

            // saves into a new file path the downloaded file
            
            //System.IO.File.WriteAllText(basicPath + filename, webRequest.downloadHandler.text);
            System.IO.File.WriteAllBytes(basicPath + filename, webRequest.downloadHandler.data);

        }
        isDownloading = false;
        Debug.Log("myTest:"+"file downloaded");
        if(callback != null)
        {
            callback();
            
        }
        
        
    }

    
    #region cloudCallbacks
    //this is called after localload is finished
    public void CloudLoad(byte[] playerData = null)
    {
        if(playerData != null)
        {            
            try
            {
                var pR = ByteArrayToObject<PlayerReference>(playerData);
                if (CheckPlayerStatus(pR))
                {
                    playerStatus.LoadReference(pR);
                }
            }
            catch
            {
                Debug.LogError("failed to convert and load data from cloud");
            }
        }
        //choses correct scene to load after loading is done
        loadingScreen.AfterLoad("Cloud");
    }

    #endregion


    #region localFiles

    public void LocalSave(string fileName, object obj)
    {   
        //separate step just to organize the code. 
        byte[] bytes = new byte[0];
        bytes = ObjectToByteArray(obj);

        //this creates or overwrites a file in Path, then closes it
        File.WriteAllBytes(basicPath + fileName, bytes);
    }

    public object LocalLoad<T>(string fileName)
    {
        try
        {
            //opens the file and gets the bytes
            var bytes = File.ReadAllBytes(basicPath + fileName);

            return ByteArrayToObject<T>(bytes);
            //Save the data on a Matchreference then pass the arrays to match.

        }
        catch (FileNotFoundException e)
        {
            Debug.LogWarning("File "+fileName+" not Found");
            return null;
        }
        catch
        {
            throw;
        }
    }

    #endregion

    #region byteArrayConversion
    //conversion method for array of bytes
    public byte[] ObjectToByteArray(object obj)
    {
        //checks for obj integrity
        if(obj == null)
        {
            return null;
        }

        //clean binary formatter
        BinaryFormatter bf = new BinaryFormatter();

        //the "using" makes sure the stream is closed
        using (MemoryStream ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

    }

    //takes the type of object wanted for output and byte[] from which to desserialize
    public T ByteArrayToObject<T>(byte[] bytes)
    {
        using (MemoryStream ms = new MemoryStream(bytes))
        {
            BinaryFormatter bf = new BinaryFormatter();
            return (T)bf.Deserialize(ms);
        }
    }
    #endregion

}
