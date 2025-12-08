using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Elevators : MonoBehaviour {
	public int startingFloor;
	public int currentElevatorFloor;
	public int currentPlayerFloor;

	public string floorString;
	public Text floorText;

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

		startingFloor = Random.Range(1, 100);
		floorString = string.Format("{0:00.#}", startingFloor);
		floorText.text = floorString;

		currentPlayerFloor = currentFindingTable[int.Parse(floorString[0].ToString())][int.Parse(floorString[1].ToString())];

		Debug.Log(currentPlayerFloor);
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
