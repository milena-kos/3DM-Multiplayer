﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public CellType_e cellType;
    //Old rotations and positions saved for animations.
    public Vector3 position;
    public Vector3 oldPosition;
    public Vector3 spawnPosition;
    //Rotation is multiplied by 90 then applied on the Z axis.
    public int rotation;
    public int oldRotation;
    public int spawnRotation;

    //For easy deletion later
    LinkedListNode<Cell> cellListNode;
    LinkedListNode<Cell> generatedCellListNode;

    //Suppresed cells should act like base cells for 1 generation.
    public bool suppresed = false;
    public bool deleted = false;
    public bool generated = false;

    public bool animate = true;

    //Converts the cells current rotation to a direction enum for easier use.
    public Direction_e getDirection() {
        return (Direction_e)(rotation % 6);
    }

    public void setPosition(Vector3 newPos) {
        if (deleted)
            return;
        CellFunctions.cellGrid[(int)position.x, (int)position.y, (int)position.z] = null;
        CellFunctions.cellGrid[(int)newPos.x, (int)newPos.y, (int)newPos.z] = this;
        this.position = newPos;
    }

    public void setPosition(int x, int y, int z)
    {
        if (deleted)
            return;
        CellFunctions.cellGrid[(int)position.x, (int)position.y, (int)position.z] = null;
        CellFunctions.cellGrid[x, y, z] = this;
        this.position = new Vector3(x, y, z);
    }

    virtual public void Setup(Vector3 position, Direction_e rotation, bool generated) {
        deleted = false;
        if (CellFunctions.cellGrid[(int)position.x, (int)position.y, (int)position.z] != null)
        {
            Cell other = CellFunctions.cellGrid[(int)position.x, (int)position.y, (int)position.z];
            other.Delete(true);
        }
        this.position = position;
        this.spawnPosition = position;
        this.rotation = (int)rotation;
        this.spawnRotation = (int)rotation;
        CellFunctions.cellGrid[(int)position.x, (int)position.y, (int)position.z] = this;
        if (cellListNode == null)
            cellListNode = CellFunctions.cellList.AddLast(this);
        if(generated)
            generatedCellListNode = CellFunctions.generatedCellList.AddLast(this);
    }

    virtual public void Delete(bool destroy) {
        CellFunctions.cellGrid[(int)position.x, (int)position.y, (int)position.z] = null;
        this.gameObject.SetActive(false);
        this.deleted = true;
        if (destroy) {
            CellFunctions.cellList.Remove(cellListNode);
            if(generatedCellListNode != null)
                CellFunctions.generatedCellList.Remove(generatedCellListNode);
            Destroy(this.gameObject);
        }
    }

	public void UpdatePosAndRot() {
		//gameObject.transform.position = this.position;
		//gameObject.transform.eulerAngles = Placing.rotations[this.rotation];
	}

    //Called when attempting to move.
    // the first bool is weather the cell can move, the second is weather it gets destroyed by moving
    public virtual (bool, bool) Push(Direction_e dir, int bias) {
        int targetX = (int)position.x;
        int targetY = (int)position.y;
		int targetZ = (int)position.z;

        switch (dir) {
            case (Direction_e.RIGHT):
                targetX += 1;
                break;
            case (Direction_e.DOWN):
                targetY += -1;
                break;
            case (Direction_e.LEFT):
                targetX += -1;
                break;
            case (Direction_e.UP):
                targetY += 1;
                break;
			case (Direction_e.FRONT):
                targetZ += 1;
                break;
			case (Direction_e.BACK):
                targetZ -= 1;
                break;
        }

        if (targetX >= CellFunctions.gridWidth || targetY >= CellFunctions.gridHeight || targetZ >= CellFunctions.gridLength)
            return (false, false);
        if (targetX < 0 || targetY < 0 || targetZ < 0)
            return (false, false);

        if (bias < 1) return (false, false);

        if (CellFunctions.cellGrid[targetX, targetY, targetZ] == null)
        {
            this.setPosition(targetX, targetY, targetZ);
            return (true, false);
        }
        (bool, bool) pushResult = CellFunctions.cellGrid[targetX, targetY, targetZ].Push(dir, bias);
        //if its a cell that deletes other cells
        if (pushResult.Item2)
        {
            this.Delete(this.generated);
            return (true, false);
        }
        if (pushResult.Item1)
        {
            this.setPosition(targetX, targetY, targetZ);
            return (true, false);
        }
        return (false, false);
    }

    public virtual void FaceDirection(Direction_e dir)
    {
        rotation = (int)dir;
    }

    public virtual void Rotate(int amount)
    {
        rotation = (rotation + amount) % 6;
        // in C#, if you modulo a negative number, the result can be negative. This is so annoying to debug.
        if (rotation < 0)
            rotation += 6;

    }
}
