using KModkit;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Elevators : MonoBehaviour {
	public int startingFloor;
	public int currentElevatorFloor;
	public float currentElevatorFloorFloat;
	public int currentPlayerFloor;
    public int destinationFloor;

	public string floorString;
	public Text floorText;

	int shirt1Color;
	int shirt2Color;
	int pants1Color;
	int pants2Color;
	int glasses1;
	int glasses2;

    int sfxMultiplier;

    public GameObject leftElevatorDoor;
    public GameObject rightElevatorDoor;

    float leftDoorClosedPos = 0.1f / 12 * -6;
    float rightDoorClosedPos = 0.1f / 12 * 2;
    float leftDoorOpenPos = 0.1f / 12 * -10;
    float rightDoorOpenPos = 0.1f / 12 * 6;

    float doorStartingScale = 0.066665f;

    public KMAudio audio;
    public KMBombInfo bombInfo;
    public KMBombModule module;
    private int moduleId;
    private static int moduleIdCounter = 1;
    private bool moduleSolved;

    public KMSelectable upButton;
    public KMSelectable downButton;

    bool goingUp;
    bool goingDown;

    public bool goingToPlayer = true;

    bool elevatorIsOpening = false;
    bool elevatorIsOpen = false;
    float sfxReplayTimer = 10.0f;

    bool bobAppearance;

    int playedSoundEffect = 4;

    public AudioClip[][] sfxs;

    public AudioClip[] throatClears;
    public AudioClip[] sneezes;
    public AudioClip[] yawns;
    public AudioClip[] snorts;
    public AudioClip[] coughs;

    public KMAudio.KMAudioRef elevatorMusic;

    enum InitialDirection
    {
        Up,
        Down
    };

    InitialDirection initialDirection;

	//Objects in the arrays goes as follows: Parent Object, Glasses, Shirt, Pants, Nametag
	public GameObject[] guy1;
	public GameObject[] guy2;

	int[][] currentFindingTable;

	int[] tableRow0 = { 50, 25, 48, 75, 68, 56, 08, 87, 69, 27 };
	int[] tableRow1 = { 33, 95, 53, 15, 96, 66, 76, 74, 62, 67 };
	int[] tableRow2 = { 79, 30, 31, 11, 19, 98, 18, 41, 07, 47 };
	int[] tableRow3 = { 09, 65, 59, 54, 82, 13, 80, 52, 44, 24 };
	int[] tableRow4 = { 60, 88, 43, 58, 29, 63, 20, 12, 72, 10 };
	int[] tableRow5 = { 40, 78, 55, 04, 06, 86, 36, 77, 32, 94 };
	int[] tableRow6 = { 23, 22, 38, 73, 92, 00, 61, 02, 51, 90 };
	int[] tableRow7 = { 34, 01, 71, 93, 91, 46, 37, 81, 70, 89 };
	int[] tableRow8 = { 39, 35, 28, 14, 83, 26, 05, 64, 45, 49 };
	int[] tableRow9 = { 57, 17, 99, 84, 42, 16, 03, 97, 85, 21 };
	
	void Start ()
	{
		currentFindingTable = new int[][] {tableRow0, tableRow1, tableRow2, tableRow3, tableRow4, tableRow5, tableRow6, tableRow7, tableRow8, tableRow9 };
        sfxs = new AudioClip[][] { new AudioClip[1], yawns, snorts, sneezes, new AudioClip[1], coughs, throatClears};

		startingFloor = Random.Range(1, 100);
        currentElevatorFloor = startingFloor;
        currentElevatorFloorFloat = startingFloor;
		floorString = string.Format("{0:00.#}", startingFloor);
		floorText.text = floorString;

		currentPlayerFloor = currentFindingTable[int.Parse(floorString[0].ToString())][int.Parse(floorString[1].ToString())];

        if(startingFloor < currentPlayerFloor)
        {
            initialDirection = InitialDirection.Up;
        }
        else
        {
            initialDirection = InitialDirection.Down;
        }
	}

	void Update ()
	{
        if(goingUp && goingDown)
        {
            module.HandleStrike();
            goingUp = false;
            goingDown = false;
        }

        if (goingUp)
        {
            currentElevatorFloorFloat += Time.deltaTime * (bobAppearance?15:2);
            currentElevatorFloor = (int)Mathf.Floor(currentElevatorFloorFloat);
        }

        if (goingDown)
        {
            currentElevatorFloorFloat -= Time.deltaTime * 2;
            currentElevatorFloor = (int)Mathf.Floor(currentElevatorFloorFloat);
        }

        if (goingUp || goingDown) 
        {
            if (elevatorMusic == null)
            {
                elevatorMusic = audio.PlaySoundAtTransformWithRef("ElevatorMusic", this.transform);
            }
            CloseElevator();
        }
        else
        {
            if (elevatorMusic != null)
            {
                elevatorMusic.StopSound();
            }
            elevatorMusic = null;
        }

        if (elevatorIsOpen)
        {
            sfxReplayTimer -= Time.deltaTime;
            if (sfxReplayTimer <= 0)
            {
                sfxReplayTimer = 10;
                if(playedSoundEffect != 4)
                {
                    int clipToPlay = Random.Range(0, sfxs[playedSoundEffect].Length);
                    audio.PlaySoundAtTransform(sfxs[playedSoundEffect][clipToPlay].name, this.transform);
                }
            }
        }

        if (elevatorIsOpening && !elevatorIsOpen)
        {
            float newLeftPosition = leftElevatorDoor.transform.localPosition.x + Time.deltaTime * (leftDoorOpenPos - leftDoorClosedPos);
            float newRightPosition = rightElevatorDoor.transform.localPosition.x + Time.deltaTime * (rightDoorOpenPos - rightDoorClosedPos);

            float newScale = leftElevatorDoor.transform.localScale.x - Time.deltaTime * doorStartingScale;

            if (newScale < 0)
            {
                leftElevatorDoor.transform.localScale = new Vector3(0, leftElevatorDoor.transform.localScale.y, leftElevatorDoor.transform.localScale.z);
                rightElevatorDoor.transform.localScale = new Vector3(0, rightElevatorDoor.transform.localScale.y, rightElevatorDoor.transform.localScale.z);
            }
            else
            {
                leftElevatorDoor.transform.localScale = new Vector3(newScale, leftElevatorDoor.transform.localScale.y, leftElevatorDoor.transform.localScale.z);
                rightElevatorDoor.transform.localScale = new Vector3(-newScale, rightElevatorDoor.transform.localScale.y, rightElevatorDoor.transform.localScale.z);
            }

            if (newLeftPosition < leftDoorOpenPos)
            {
                leftElevatorDoor.transform.localPosition = new Vector3(leftDoorOpenPos, leftElevatorDoor.transform.localPosition.y, leftElevatorDoor.transform.localPosition.z);
                rightElevatorDoor.transform.localPosition = new Vector3(rightDoorOpenPos, rightElevatorDoor.transform.localPosition.y, rightElevatorDoor.transform.localPosition.z);
                elevatorIsOpen = true;
            }
            else
            {
                leftElevatorDoor.transform.localPosition = new Vector3(newLeftPosition, leftElevatorDoor.transform.localPosition.y, leftElevatorDoor.transform.localPosition.z);
                rightElevatorDoor.transform.localPosition = new Vector3(newRightPosition, rightElevatorDoor.transform.localPosition.y, rightElevatorDoor.transform.localPosition.z);
            }
        }
        else if(!elevatorIsOpening && elevatorIsOpen)
        {
            float newLeftPosition = leftElevatorDoor.transform.localPosition.x - Time.deltaTime * (leftDoorOpenPos - leftDoorClosedPos);
            float newRightPosition = rightElevatorDoor.transform.localPosition.x - Time.deltaTime * (rightDoorOpenPos - rightDoorClosedPos);

            float newScale = leftElevatorDoor.transform.localScale.x + Time.deltaTime * doorStartingScale;

            if (newScale > doorStartingScale)
            {
                leftElevatorDoor.transform.localScale = new Vector3(doorStartingScale, leftElevatorDoor.transform.localScale.y, leftElevatorDoor.transform.localScale.z);
                rightElevatorDoor.transform.localScale = new Vector3(-doorStartingScale, rightElevatorDoor.transform.localScale.y, rightElevatorDoor.transform.localScale.z);
            }
            else
            {
                leftElevatorDoor.transform.localScale = new Vector3(newScale, leftElevatorDoor.transform.localScale.y, leftElevatorDoor.transform.localScale.z);
                rightElevatorDoor.transform.localScale = new Vector3(-newScale, rightElevatorDoor.transform.localScale.y, rightElevatorDoor.transform.localScale.z);
            }

            if (newLeftPosition > leftDoorClosedPos)
            {
                leftElevatorDoor.transform.localPosition = new Vector3(leftDoorClosedPos, leftElevatorDoor.transform.localPosition.y, leftElevatorDoor.transform.localPosition.z);
                rightElevatorDoor.transform.localPosition = new Vector3(rightDoorClosedPos, rightElevatorDoor.transform.localPosition.y, rightElevatorDoor.transform.localPosition.z);
                elevatorIsOpen = false;
            }
            else
            {
                leftElevatorDoor.transform.localPosition = new Vector3(newLeftPosition, leftElevatorDoor.transform.localPosition.y, leftElevatorDoor.transform.localPosition.z);
                rightElevatorDoor.transform.localPosition = new Vector3(newRightPosition, rightElevatorDoor.transform.localPosition.y, rightElevatorDoor.transform.localPosition.z);
            }
        }

        floorString = string.Format("{0:00.#}", currentElevatorFloor);
        floorText.text = floorString;

        if (goingToPlayer)
        {
            if(initialDirection == InitialDirection.Up)
            {
                if(currentElevatorFloor > currentPlayerFloor)
                {
                    goingUp = false;
                    goingToPlayer = false;
                    currentElevatorFloor--;
                    currentElevatorFloorFloat--;
                    module.HandleStrike();
                    GeneratePeople();
                    OpenElevator();
                }
            }
            else
            {
                if(currentElevatorFloor < currentPlayerFloor)
                {
                    goingDown = false;
                    goingToPlayer = false;
                    currentElevatorFloor++;
                    currentElevatorFloorFloat++;
                    module.HandleStrike();
                    GeneratePeople();
                    OpenElevator();
                }
            }
        }
        else
        {
            if (bobAppearance)
            {
                if(currentElevatorFloor >= 606)
                {
                    module.HandlePass();
                    moduleSolved = true;
                    return;
                }
            }

            if (initialDirection == InitialDirection.Down)
            {
                if (currentElevatorFloor > destinationFloor)
                {
                    goingUp = false;
                    currentElevatorFloor--;
                    currentElevatorFloorFloat--;
                    module.HandleStrike();
                    goingToPlayer = true;
                }
            }
            else
            {
                if (currentElevatorFloor < destinationFloor)
                {
                    goingDown = false;
                    currentElevatorFloor++;
                    currentElevatorFloorFloat++;
                    module.HandleStrike();
                    goingToPlayer = true;
                }
            }
        }
	}

    private void Awake()
    {
        moduleId = moduleIdCounter++;

        upButton.OnInteract += delegate ()
        {
            if (moduleSolved)
            {
                return false;
            }

            if (goingToPlayer)
            {
                if (goingUp)
                {
                    if (currentElevatorFloor == currentPlayerFloor)
                    {
                        goingUp = false;
                        goingToPlayer = false;

                        GeneratePeople();
                        OpenElevator();
                    }
                    else
                    {
                        goingUp = false;
                        module.HandleStrike();
                    }
                }
                else
                {
                    if(initialDirection == InitialDirection.Up)
                    {
                        goingUp = true;
                    }
                    else
                    {
                        module.HandleStrike();
                        goingDown = false;
                    }
                }
            }
            else
            {
                if(initialDirection == InitialDirection.Down)
                {
                    if (!goingUp)
                    {
                        goingUp = true;
                    }
                    else
                    {
                        if(currentElevatorFloor == destinationFloor)
                        {
                            module.HandlePass();
                            moduleSolved = true;
                            goingUp = false;
                        }
                        else
                        {
                            module.HandleStrike();
                            goingUp = false;
                        }
                    }
                }
                else
                {
                    module.HandleStrike();
                    goingUp = false;
                    goingDown = false;
                }
            }

            return false;
        };

        downButton.OnInteract += delegate ()
        {
            if (moduleSolved)
            {
                return false;
            }

            if (goingToPlayer)
            {
                if (goingDown)
                {
                    if (currentElevatorFloor == currentPlayerFloor)
                    {
                        goingDown = false;
                        goingToPlayer = false;

                        GeneratePeople();
                        OpenElevator();
                    }
                    else
                    {
                        goingDown = false;
                        module.HandleStrike();
                    }
                }
                else
                {
                    if (initialDirection == InitialDirection.Down)
                    {
                        goingDown = true;
                    }
                    else
                    {
                        module.HandleStrike();
                        goingDown = false;
                    }
                }
            }
            else
            {
                if (initialDirection == InitialDirection.Up)
                {
                    if (!goingDown)
                    {
                        goingDown = true;
                    }
                    else
                    {
                        if (currentElevatorFloor == destinationFloor)
                        {
                            module.HandlePass();
                            moduleSolved = true;
                            goingDown = false;
                        }
                        else
                        {
                            module.HandleStrike();
                            goingDown = false;
                        }
                    }
                }
                else
                {
                    module.HandleStrike();
                    goingUp = false;
                    goingDown = false;
                }
            }

            return false;
        };
    }

    void OpenElevator()
    {
        sfxReplayTimer = 0;
        elevatorIsOpening = true;
    }

    void CloseElevator()
    {
        elevatorIsOpening = false;
    }

    void GeneratePeople()
	{
        shirt1Color = 0;
        shirt2Color = 0;
        pants1Color = 0;
        pants2Color = 0;
        glasses1 = 0;
        glasses2 = 0;
        sfxMultiplier = 0;

        guy1[4].SetActive(false);
        guy2[4].SetActive(false);
        guy1[0].SetActive(true);
        guy2[0].SetActive(false);



        switch (Random.Range(1,4))
		{
			case 1:
				shirt1Color = 2;
				guy1[2].GetComponent<MeshRenderer>().material.color = new Color(0, 0.7f, 0);
                Debug.Log("Green Shirt 1");
				break;
			case 2:
                shirt1Color = 5;
                guy1[2].GetComponent<MeshRenderer>().material.color = new Color(0.7f, 0, 0);
                Debug.Log("Red Shirt 1");
                break;
			case 3:
				shirt1Color = 6;
				guy1[2].GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0.7f);
                Debug.Log("Blue Shirt 1");
                break;
		}

		switch (Random.Range(1,3))
		{
            case 1:
                pants1Color = 3;
                guy1[3].GetComponent<MeshRenderer>().material.color = new Color(0.1f, 0.1f, 0.1f);
                Debug.Log("Black Pants 1");
                break;
            case 2:
                pants1Color = 1;
                guy1[3].GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0.7f);
                Debug.Log("Blue Pants 1");
                break;
        }

        switch (Random.Range(1, 3))
        {
            case 1:
                glasses1 = 4;
                guy1[1].SetActive(true);
                Debug.Log("Glasses On 1");
                break;
            case 2:
                glasses1 = 0;
                guy1[1].SetActive(false);
                Debug.Log("Glasses Off 1");
                break;
        }

		int peopleAmount = Random.Range(1, 3);

		if(peopleAmount == 1)
		{
			guy2[0].SetActive(false);
            Debug.Log("No guy 2");
		}
		else
		{
            switch (Random.Range(1, 4))
            {
                case 1:
                    shirt2Color = 2;
                    guy2[2].GetComponent<MeshRenderer>().material.color = new Color(0, 0.7f, 0);
                    Debug.Log("Green Shirt 2");
                    break;
                case 2:
                    shirt2Color = 5;
                    guy2[2].GetComponent<MeshRenderer>().material.color = new Color(0.7f, 0, 0);
                    Debug.Log("Red Shirt 2");
                    break;
                case 3:
                    shirt2Color = 6;
                    guy2[2].GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0.7f);
                    Debug.Log("Blue Shirt 2");
                    break;
            }

            switch (Random.Range(1, 3))
            {
                case 1:
                    pants2Color = 3;
                    guy2[3].GetComponent<MeshRenderer>().material.color = new Color(0.1f, 0.1f, 0.1f);
                    Debug.Log("Black Pants 2");
                    break;
                case 2:
                    pants2Color = 1;
                    guy2[3].GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0.7f);
                    Debug.Log("Blue Pants 2");
                    break;
            }

            switch (Random.Range(1, 3))
            {
                case 1:
                    glasses2 = 4;
                    guy2[1].SetActive(true);
                    Debug.Log("Glasses On 2");
                    break;
                case 2:
                    glasses2 = 0;
                    guy2[1].SetActive(false);
                    Debug.Log("Glasses Off 2");
                    break;
            }
        }

        int clipToPlay;
        playedSoundEffect = Random.Range(1,7);
        if (playedSoundEffect != 4)
        {
            clipToPlay = Random.Range(0, sfxs[playedSoundEffect].Length);
            audio.PlaySoundAtTransform(sfxs[playedSoundEffect][clipToPlay].name, this.transform);
            
        }
        sfxMultiplier = playedSoundEffect;

        int destinationModifier = (shirt1Color + shirt2Color + pants1Color + pants2Color + glasses1 + glasses2) * sfxMultiplier;

        if (initialDirection == InitialDirection.Up)
        {
            destinationFloor = currentPlayerFloor - destinationModifier;
        }
        else
        {
            destinationFloor = currentPlayerFloor + destinationModifier;
        }

        List<string> litIndicators = bombInfo.GetOnIndicators().ToList<string>();

        foreach (string indicator in litIndicators)
        {
            if (indicator == "BOB")
            {
                //if (Random.Range(1, 501) == 500)
                if (true)
                {
                    guy1[4].SetActive(true);
                    destinationFloor = 606;
                    bobAppearance = true;
                }
            }
        }
    }
}
