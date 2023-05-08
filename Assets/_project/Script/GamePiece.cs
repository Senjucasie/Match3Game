using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
    public int XIndex { get; private set; }
    public int YIndex { get; private set; }

    public void SetIndex(int x, int y)
    {
        XIndex = x;
        YIndex = y;
    }
}
