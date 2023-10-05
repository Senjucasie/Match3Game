using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private float _swapTime;
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
        _gamePieceArray = new GamePiece[_width, _height];
       
    }

    private void Start()
    {
        SetUpTiles();
    SetUpCamera();
    FillBoard();
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

    public void PlaceGamePiece(GamePiece gamepiece, int x, int y)
    {
        if (gamepiece == null)
        {
            Debug.LogWarning($"{gamepiece} is null");
            return;
        }
        else
        {
            if(IsWithInBounds(x,y))
            {
                gamepiece.transform.position = new Vector3Int(x, y, 0);
                gamepiece.Init(this);
                gamepiece.SetIndex(x, y);
                _gamePieceArray[x, y] = gamepiece;
            }
        }
    }

    private bool IsWithInBounds(int x, int y)
    {
        return (x >= 0 && x < _width && y >= 0 && y < _height);
    }
    private void FillBoard()
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                GamePiece gamepiece = FillRandom(x, y);
                int iterator = 0;
                while(HasMatchOnFill(gamepiece))
                {
                    ClearGamePiece(gamepiece);
                    gamepiece = FillRandom(x, y);
                    iterator++;
                    if(iterator>100)
                    {
                        break;
                    }
                   
                }
            }
        }
    }

    private GamePiece FillRandom(int x, int y)
    {
        GameObject randompiece = Instantiate(GetRandomGamePiece(), Vector3.zero, Quaternion.identity);
        if(randompiece!=null)
        {
            PlaceGamePiece(randompiece.GetComponent<GamePiece>(), x, y);
            return randompiece.GetComponent<GamePiece>();
        }
        else
        {
            return null;
        }
        
    }

    private bool HasMatchOnFill(GamePiece gamepiece,int minmatch=3)
    {
        List<GamePiece> leftmatches = FindMatches(gamepiece, Vector2.left, minmatch);
        List<GamePiece> downwardmatches = FindMatches(gamepiece, Vector2.down, minmatch);

        leftmatches = leftmatches == null ? new() : leftmatches;
        downwardmatches = downwardmatches == null ? new() : downwardmatches;

         return leftmatches.Count >= minmatch || downwardmatches.Count >= minmatch;
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
        if(_clickedTile!=null && IsNeighbourTile(_clickedTile,tile))
        {
            _targetTile = tile;
        }
    }

    public void ReleaseMouseButton()
    {
        if(_targetTile!=null && _clickedTile!=null)
        {
            SwitchTiles(_clickedTile, _targetTile);
        }

         _clickedTile = _targetTile = null;
    }

    private void SwitchTiles(Tile clickedtile,Tile targettile)
    {
        StartCoroutine(SwitchTilesRoutine(clickedtile, targettile));
    }
    private IEnumerator SwitchTilesRoutine(Tile clickedtile, Tile targettile)
    {
        GamePiece clickedgamepiece = _gamePieceArray[clickedtile.XIndex, clickedtile.YIndex];
        GamePiece targetgamepiece = _gamePieceArray[targettile.XIndex, targettile.YIndex];
        
        if(targetgamepiece!=null && clickedgamepiece!=null)
        {
            clickedgamepiece.Move(targettile.XIndex, targettile.YIndex, _swapTime);
            targetgamepiece.Move(clickedtile.XIndex, clickedtile.YIndex, _swapTime);

            yield return new WaitForSeconds(_swapTime);

            var clickgamepiecematch = FindMatchesAt(clickedgamepiece);
            var targetgamepiecematch = FindMatchesAt(targetgamepiece);
            if (clickgamepiecematch.Count == 0 && targetgamepiecematch.Count == 0)
            {
                clickedgamepiece.Move(clickedtile.XIndex, clickedtile.YIndex, _swapTime);
                targetgamepiece.Move(targettile.XIndex, targettile.YIndex, _swapTime);
            }
            else
            {
                var combinedgamepiece = clickgamepiecematch.Union(targetgamepiecematch).ToList();
                ClearGamePieces(combinedgamepiece);
            }
        }

        
    }
    private bool IsNeighbourTile(Tile clickedtile,Tile draggedtile)
    {
        if(Mathf.Abs(clickedtile.XIndex-draggedtile.XIndex)==1 && clickedtile.YIndex==draggedtile.YIndex)
        {
            return true;
        }
        else if(Mathf.Abs(clickedtile.YIndex - draggedtile.YIndex) == 1 && clickedtile.XIndex == draggedtile.XIndex)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private List<GamePiece> FindMatchesAt(GamePiece gamepiece,int minlength=3)
    {
        List<GamePiece> horizontalmatch = FindHorizontalMatch(gamepiece,minlength);
        List<GamePiece> verticalmatch = FindVerticalMatch(gamepiece,minlength);

        if (verticalmatch == null)
        {
            verticalmatch = new();
        }
        if (horizontalmatch == null)
        {
            horizontalmatch = new();
        }
        var combinedmatch = verticalmatch.Union(horizontalmatch).ToList();
        return combinedmatch;
    }

    public void HighLightTilesAT(Tile tile)
    {
        var macthedgamepiece = FindMatchesAt(_gamePieceArray[tile.XIndex, tile.YIndex]);
        foreach (GamePiece piece in macthedgamepiece)
        {
            _tileArray[piece.XIndex, piece.YIndex].GetComponent<SpriteRenderer>().color = piece.GetComponent<SpriteRenderer>().color;
        }
    }
    private void HightLightTilesOff(List<Tile> tiles)
    {
        foreach(Tile tile in tiles)
        {
            SpriteRenderer renderer=tile.GetComponent<SpriteRenderer>();
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0);
        }
    }
    private void HighlightMatches()
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                var combinedmatch = FindMatchesAt(_gamePieceArray[x,y]);
                
                foreach(GamePiece piece in combinedmatch)
                {
                    _tileArray[piece.XIndex, piece.YIndex].GetComponent<SpriteRenderer>().color = piece.GetComponent<SpriteRenderer>().color;
                }
            }
        }
    }
    public List<GamePiece> FindVerticalMatch(GamePiece startpiece,int minmatch=3)
    {
        List<GamePiece> upwardmatch = FindMatches(startpiece, Vector2.up, 2);
        List<GamePiece> downwardmatch = FindMatches(startpiece, Vector2.down, 2);
        if(upwardmatch==null)
        {
            upwardmatch = new();
        }
        if(downwardmatch==null)
        {
            downwardmatch = new();
        }
    
        var combinedmatches = upwardmatch.Union(downwardmatch).ToList();

        return combinedmatches.Count >= minmatch ? combinedmatches : null;
    }

    public List<GamePiece> FindHorizontalMatch(GamePiece startpiece, int minmatch = 3)
    {
        List<GamePiece> rightmatch = FindMatches(startpiece, Vector2.right, 2);
        List<GamePiece> leftmatch = FindMatches(startpiece, Vector2.left, 2);
        if (rightmatch == null)
        {
            rightmatch = new();
        }
        if (leftmatch == null)
        {
            leftmatch = new();
        }
        var combinedmatches = rightmatch.Union(leftmatch).ToList();

        return combinedmatches.Count >= minmatch ? combinedmatches : null;
    }
    private List<GamePiece> FindMatches(GamePiece startpiece,Vector2 direction,int minmatch=3)
    {
        List<GamePiece> matchlist = new();
        if(startpiece!=null)
        {
            matchlist.Add(startpiece);
        }
        else
        {
            return null;
        }

        int maxvalue = _width > _height ? _width : _height;

        for(int i=1; i<maxvalue;i++)
        {
            int x = startpiece.XIndex + i * (int)direction.x;
            int y= startpiece.YIndex + i * (int)direction.y;
            if(!IsWithInBounds(x,y))
            {
                break;
            }
            GamePiece nextpiece = _gamePieceArray[x, y];
            if (nextpiece !=null && nextpiece.Type==startpiece.Type && !matchlist.Contains(nextpiece))
            {
                matchlist.Add(nextpiece);
            }
            else
            {
                break;
            }
        }
        return matchlist.Count >= minmatch ? matchlist : null;
        
    }
    private void ClearGamePiece(GamePiece gamepiece)
    {
        Destroy(gamepiece.gameObject);
    }
    private void ClearGamePieces(List<GamePiece> gamepiece)
    {
        foreach(GamePiece piece in gamepiece)
        {
            ClearGamePiece(piece);
        }
    }
    
}
