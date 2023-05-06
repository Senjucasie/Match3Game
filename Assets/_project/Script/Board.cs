using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private int _width;
    [SerializeField] private int _height;

    [SerializeField] private GameObject _tilePrefab;

    private Tile[,] _tileArray;

    private void Awake()
    {
        _tileArray = new Tile[_width, _height];
        SetUpTiles();
    }

    private void SetUpTiles()
    {
        for(int y=0;y<_height;y++)
        {
            for(int x=0;x<_width;x++)
            {
                Debug.Log(x + "," + y);
                GameObject tile = Instantiate(_tilePrefab, new Vector3(x, y, 0), Quaternion.identity,transform);
                tile.name = $"Tile {x},{y}";
                _tileArray[x, y] = tile.GetComponent<Tile>();
            }
        }
    }
}
