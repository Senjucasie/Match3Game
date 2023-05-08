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
}
