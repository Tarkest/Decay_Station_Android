using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CrewMember
{
    public int id;
    public string name;
    public float strength;
    public float agility;
    public float intelligence;
    public InventorySlot[] inventory;
    public InventorySlot[] equipment;
}
