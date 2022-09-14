using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageCreator : GameManager
{
    string baseCode = "000000;board;Red,1,1,1,5;Blue,6,1,2,3;Yellow,1,6,3,5;White,6,6,4,1";
    int[,] goalCoordinates;
    Button[] randomizearrows;
    Die[] randomizeDice;
    public int numberIterations, numberOfCodes, codeLastGenerated = 0;
    private List<string> codesGenerated = new List<string>();
    public string codeTypeToProcude;

    protected override void Awake()
    {
        base.Awake();
        fastMode = true;
    }

    protected override void Start()
    {
        base.Start();
        

        

    }

    public override IEnumerator AfterStart()
    {
        yield return new WaitForFixedUpdate();
        StartRandomArrays();
        //Debug.Log("myTest:"+randomizeDice[0].name);
        //match.ImportCode(baseCode);
        match.UpdateTypeCode();
        
        //string[][] testReference = new string[][] { new string[] { match.typeCode, baseCode } };
        //match.LoadCodeReference(testReference);
        //saveSystem.Save("MatchReference");
        

        saveSystem.Load("MatchReference");
        //Debug.Log("myTest:"+"Match typecode: "+match.typeCode);
        //Debug.Log("myTest:"+"dictionary keys:" + match.codeReference.);  
        //Debug.Log("myTest:"+match.codeReference["000000-000-000-000-"][1]);
        //Debug.Log("myTest:"+"dictionary[Red]: "+dice["Red"].name);
        match.ImportCode(match.codeReference[match.typeCode][2]);

        RandomStartPosition();
        controls.UpdateArrows();
        StartCoroutine(GenerateCodes());
        //StartCoroutine(MoveRandomStuff(numberIterations));
    }

    private void RandomStartPosition()
    {
        int rnd = Random.Range(0, 4);
        
        //randomizes dice starting rotation
        rnd = Random.Range(0, 4);
        for (int i = 0; i < randomizeDice.Length; i++)
        {
            bool never13 = false;
            int timeout = 0;

            //this has to be outside the rotation loop so to preserve original top face value
            //and leave a correct sumTop at the end
            int previousTop = randomizeDice[rnd].faces["Top"];
            while (!never13&&timeout<100)
            {
                
                randomizeDice[rnd].RotationRandom();
                //Debug.Log("myTest:"+previousTop + " , " + randomizeDice[rnd].faces["Top"]);
                if (randomizeDice[rnd].faces["Top"] - previousTop + sumTop < 13)
                {
                    sumTop += randomizeDice[rnd].faces["Top"] - previousTop;
                    never13 = true;
                    //Debug.Log("myTest:"+sumTop+"+"+ randomizeDice[rnd].faces["Top"]+" - "+previousTop);
                }
                timeout++;
            }
            if (timeout >= 100)
            {
                Debug.LogError("can't find rotation within 13 at random start position");
                return;
            }
            rnd = (rnd + 1) % 4;
            //Debug.Log("myTest:"+rnd);
        }

        //randomizes dice starting coordinates within goal
        rnd = Random.Range(0, 4);
        for (int i = 0; i < randomizeDice.Length; i++)
        {
            Die die = randomizeDice[i];
            die.coordinates.coordinates = new int[] { goalCoordinates[rnd, 0], goalCoordinates[rnd, 1] };
            die.MatchChildren();
            //updates board
            board.Put(board.boardOccupants, die.coordinates, die.nameId);
            rnd = (rnd + 1) % 4;
            //die.DebugFaces();
        
        }

    }

    IEnumerator GenerateCodes()
    {
        codesGenerated.Clear();
        match.UpdateTypeCode();
        codesGenerated.Add(match.typeCode);
        for(; codeLastGenerated < numberOfCodes; codeLastGenerated++)
        {
            int timeout = 0;
            bool newCodeCreated = false;
            while (timeout < 100&&!newCodeCreated)
            {
                //same code as moverandom sutff, but cant call that because of pile order
                for (int j = 0; j < numberIterations; j++)
                {

                    //select a random die
                    int rnd = Random.Range(0, 4);
                    randomizeDice[rnd].Select(true);


                    rnd = Random.Range(0, 4);
                    if (randomizearrows[rnd].enabled)
                    {
                        randomizearrows[rnd].onClick.Invoke();


                    }

                    yield return new WaitForEndOfFrame();
                }

                //adds new state to code list
                string newCode = match.ExportCode(codeLastGenerated);
                if (!codesGenerated.Contains(newCode))
                {
                    codesGenerated.Add(newCode);
                    newCodeCreated = true;
                }
            }
            if (timeout >= 100)
            {
                Debug.LogError("could not create new code in time. Codes created: "+ codesGenerated.Count );
                yield return null;
            }
            
        }
        Debug.Log("myTest:"+"amount of codes created: "+codesGenerated.Count);
        ExportCodesGenerated();
    }

    IEnumerator MoveRandomStuff(int iterations)
    {
        yield return new WaitForEndOfFrame();
        

        for(int i = 0; i<iterations;i++)
        {
            
            //select a random die
            int rnd = Random.Range(0, 4);
            randomizeDice[rnd].Select(true);


            rnd = Random.Range(0, 4);
            if (randomizearrows[rnd].enabled)
            {
                randomizearrows[rnd].onClick.Invoke();
                

            }

            yield return new WaitForEndOfFrame();
        }
        
    }

    public void StartRandomArrays()
    {
        //create an array of dice and arrows for the random pick
        randomizeDice = new Die[] { dice["Red"], dice["Blue"], dice["Yellow"], dice["White"] };
        randomizearrows = new Button[] { controls.arrows["Right"], controls.arrows["Left"], controls.arrows["Up"], controls.arrows["Down"] };
        goalCoordinates = new int[,] { { 3, 3 }, { 3, 4 }, { 4, 3 }, { 4, 4 } };
    }

   


    public void ExportCodesGenerated()
    {
        
        string[] codeArray = codesGenerated.ToArray();
        string codeType = codeArray[0];
        if (match.codeReference.ContainsKey(codeType))
        {
            match.codeReference[codeType] = codeArray;
        }
        else
        {
            match.codeReference.Add(codeType, codeArray);
        }
        saveSystem.Save("MatchReference");
    }
}
