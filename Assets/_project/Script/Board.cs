using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private int _width;
    [SerializeField] private int _height;

    [SerializeField] private GameObject _tilePrefab;

    [SerializeField] private float _borderSize;

    private Tile[,] _tileArray;

    private void Awake()
    {
        _tileArray = new Tile[_width, _height];
        SetUpTiles();
        SetUpCamera();
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
                _tileArray[x, y].Init(x, y, this);
            }
        }
    }

    //Adjusting camera orthogonal size and position according to the sizeof grid
    private void SetUpCamera()
    {
        Camera cam = Camera.main;
        cam.transform.position = new Vector3((_width - 1f) / 2f, (_height - 1f) / 2f, cam.transform.position.z);
        float aspectratio = (float)Screen.width / (float)Screen.height;
        float vericalsize = (float)_height / 2 + _borderSize;
        float horizontalsize = ((float)_width / 2 + _borderSize) / aspectratio;

        cam.orthographicSize = vericalsize > horizontalsize ? vericalsize : horizontalsize;
        
    }
}
