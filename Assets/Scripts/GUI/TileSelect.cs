using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSelect : MonoBehaviour
{
    public Grid groundGrid; //  You can also use the Tilemap object
    public Tilemap trapGrid; //  You can also use the Tilemap object
    public Sprite trap;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnGUI()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int coordinate = groundGrid.WorldToCell(mouseWorldPos);
            

        //    Debug.Log(trapGrid.GetTile(coordinate).name);
        }
    }
}



