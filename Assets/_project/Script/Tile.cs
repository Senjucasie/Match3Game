using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int XIndex { get; private set; }
    public int YIndex { get; private set; }
    private Board _board;

    private void Start()
    {
    
    }
    public void Init(int xindex,int yindex,Board board)
    {
        XIndex = xindex;
        YIndex = yindex;
        _board = board;
    }

    private void OnMouseDown()
    {
        if(_board!=null)
            _board.ClickTile(this);
    }
    private void OnMouseEnter()
    {
        if (_board != null)
            _board.DragToTile(this);
    }

    private void OnMouseUp()
    {
        if (_board != null)
            _board.ReleaseMouseButton();
    }

}
