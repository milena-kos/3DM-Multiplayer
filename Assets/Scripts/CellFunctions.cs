﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CellFunctions : MonoBehaviour
{	
	public GameObject gridManager;
	public GameObject player;
	public static Vector3[] rotations;
	public static int rotIndex;
	
	private static GameObject[] inventory;
	
	void Start() {
		inventory = player.GetComponent<Placing>().inventory;
		rotations = Placing.rotations;
		gridHeight = 300;
		gridWidth = 300;
		gridLength = 300;
		gridManager.GetComponent<GridManager>().InitGridSize();
	}
	void Update() {
		if (Input.GetKeyDown("r")) {
			GridManager.playSimulation = !GridManager.playSimulation;
		}
	}
	public void Reset() {
		foreach (GameObject g in GameObject.FindGameObjectsWithTag("cell")) {
			g.GetComponent<Cell>().transform.position = g.GetComponent<Cell>().spawnPosition;
		}
		gridManager.GetComponent<GridManager>().Reset();
	}
	public void Save() {
		GUIUtility.systemCopyBuffer = Worlds.Save();
	}
	public void Load() {
		Worlds.Load();
	}
	
	public static Direction_e[] directionUpdateOrder = { Direction_e.RIGHT, Direction_e.LEFT, Direction_e.UP, Direction_e.DOWN , Direction_e.FRONT , Direction_e.BACK };
    public static CellType_e[] cellUpdateOrder = {CellType_e.GENERATOR, CellType_e.FIXEDROTATOR, CellType_e.MOVER};
    public static Dictionary<CellType_e, CellUpdateType_e> cellUpdateTypeDictionary = new Dictionary<CellType_e, CellUpdateType_e>
    {
		[CellType_e.GENERATOR] = CellUpdateType_e.TRACKED,
        [CellType_e.MOVER] = CellUpdateType_e.TRACKED,
        [CellType_e.WALL] = CellUpdateType_e.BASE,
        [CellType_e.TRASH] = CellUpdateType_e.BASE,
		[CellType_e.FIXEDROTATOR] = CellUpdateType_e.TICKED,
    };

    //An disctionary defining the specialized ID's used in sorting, for tracked and 
    public static Dictionary<CellType_e, int> steppedCellIdDictionary = new Dictionary<CellType_e, int>();

    public static int gridWidth = 1;
    public static int gridHeight = 1;
	public static int gridLength = 1;
    //Used to check which cell might be at location x, y, z.
    public static Cell[,,] cellGrid;
    //Used to check if x, y, z is considered a placeable tile.
    public static bool[,,] placeableTiles;

    public static LinkedList<Cell> cellList;
    //Cells made during the simulation
    public static LinkedList<Cell> generatedCellList;

    //Cells that need to be updated but not in a specific order.
    //tickedCellList[CellType];
    public static LinkedList<Cell>[] tickedCellList;

    //Cells that need to be updated but in a specific order (Depending on direction).
    //trackedCells[TrackedCell ID][Direction, Distince];
    //public static LinkedList<Cell>[][,] trackedCells;
    // changed to [,][]
    // the jagged array is 2 dimensional
    public static LinkedList<Cell>[,][] trackedCells;


    //trackedCellRotationUpdateQueue[CellType] Cell type must be sorted into a new direction queue if it has been rotated since it was last sorted
    public static LinkedList<Cell>[] trackedCellRotationUpdateQueue;
    //trackedCellPositionUpdateQueue[CellType, Cell Direction] Cell type "X" facing direction "Y" must be sorted before Cells of type X facing direction Y are updated
    public static LinkedList<Cell>[,] trackedCellPositionUpdateQueue;

    public static int GetSteppedCellId(CellType_e type) {
		return steppedCellIdDictionary[type];
    }

    public static CellUpdateType_e GetUpdateType(CellType_e type) {
        return cellUpdateTypeDictionary[type];
    }
	
	public static CellType_e PrefabToCellType_e(GameObject prefab) {
		CellType_e r = CellType_e.WALL;
		if (prefab == inventory[0]) {
			r = CellType_e.MOVER;
		} else if (prefab == inventory[1]) {
			r = CellType_e.GENERATOR;
		} else if (prefab == inventory[2]) {
			r = CellType_e.FIXEDROTATOR;
		} else if (prefab == inventory[3]) {
			r = CellType_e.WALL;
		} else if (prefab == inventory[4]) {
			r = CellType_e.GENERATOR;
		} else {
			throw new NotImplementedException("Me when u forgor to update this if :skull:");
		}
		return r;
	}
	public static CellType_e StringToCellType_e(string name) {
		CellType_e r = CellType_e.WALL;
		if (name == "MOVER") {
			r = CellType_e.MOVER;
		} else if (name == "GENERATOR") {
			r = CellType_e.GENERATOR;
		} else if (name == "FIXEDROTATOR") {
			r = CellType_e.FIXEDROTATOR;
		} else if (name == "WALL") {
			r = CellType_e.WALL;
		} else if (name == "GENERATOR") {
			r = CellType_e.GENERATOR;
		} else {
			throw new NotImplementedException("Me when u forgor to update this if :skull:");
		}
		return r;
	}
	public static Direction_e Vector3ToDirection_e(Vector3 dir) {
		Direction_e r = Direction_e.FRONT;
		if (dir == rotations[0]) {
			r = Direction_e.FRONT;
		} else if (dir == rotations[1]) {
			r = Direction_e.RIGHT;
		} else if (dir == rotations[2]) {
			r = Direction_e.DOWN;
		} else if (dir == rotations[3]) {
			r = Direction_e.BACK;
		} else if (dir == rotations[4]) {
			r = Direction_e.LEFT;
		} else if (dir == rotations[5]) {
			r = Direction_e.UP;
		} else {
			Debug.Log(dir);
			throw new NotImplementedException("Me when u forgor to update this if :skull:");
		}
		return r;
	}
}
