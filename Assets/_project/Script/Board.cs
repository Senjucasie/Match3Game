using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private int _width;
    [SerializeField] private int _height;

    [SerializeField] private GameObject _tilePrefab;

    [SerializeField] private float _borderSize;

    [SerializeField] private GameObject[] _gamePiecesPF;

    private GamePiece[,] _gamePieceArray;
    private Tile[,] _tileArray;

    private Tile _clickedTile;
    private Tile _targetTile;

    private void Awake()
    {
        _tileArray = new Tile[_width, _height];
      
    }

    private void Start()
    {
        SetUpTiles();
        SetUpCamera();
        FillRandom();
    }

    //Instantiating BackGround Tile
    private void SetUpTiles()
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                GameObject tile = Instantiate(_tilePrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
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

    private GameObject GetRandomGamePiece()
    {
        if (_gamePiecesPF == null)
        {
            Debug.LogWarning($"The array is null {_gamePieceArray}");
            return null;
        }
        else
        {
            int random = Random.Range(0, _gamePiecesPF.Length);
            return _gamePiecesPF[random];
        }

    }

    private void PlaceGamePiece(GamePiece gamepiece, int x, int y)
    {
        if (gamepiece == null)
        {
            Debug.LogWarning($"{gamepiece} is null");
            return;
        }
        else
        {
            gamepiece.transform.position = new Vector3Int(x, y, 0);
            gamepiece.SetIndex(x, y);
        }
    }

    private void FillRandom()
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                GameObject randompiece = Instantiate(GetRandomGamePiece(), Vector3.zero, Quaternion.identity);
                PlaceGamePiece(randompiece.GetComponent<GamePiece>(), x, y);
            }
        }
    }

    public void ClickTile(Tile tile)
    {
        if(_clickedTile== null)
        {
            _clickedTile = tile;
        }
        
    }

    public void DragToTile(Tile tile)
    {
        if(_clickedTile!=null)
        {
            _targetTile = tile;
        }
    }

    public void ReleaseMouseButton()
    {
        if(_targetTile!=null && _clickedTile!=null)
        {
        
        }

         _clickedTile = _targetTile = null;
    }

    

    
}
