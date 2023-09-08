using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
    [field: SerializeField] public int XIndex { get; private set; }
    [field: SerializeField] public int YIndex { get; private set; }

    private Coroutine _moveCoroutine;

    [SerializeField] private InterpolationType _interpolationType;
    public enum InterpolationType { Linear, EaseIn, EaseOut, SmoothStep,SmootherStep };

    public void SetIndex(int x, int y)
    {
        XIndex = x;
        YIndex = y;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move((int)transform.position.x + 1, (int)transform.position.y, 0.5f);
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move((int)transform.position.x - 1, (int)transform.position.y, 0.5f);
        }
    }

    public void Move(int destx, int desty,float timetomove)
    {
        if(_moveCoroutine ==null)
            _moveCoroutine = StartCoroutine(MoveRoutine(new Vector3(destx, desty, 0), timetomove));
    }

    private IEnumerator MoveRoutine(Vector3 destination, float timetomove)
    {
        bool reachdestination = false;
        Vector3 startpos = transform.position;
        float elapsedtime = 0;
        //ease in squared exponantial formula
        //t = t * t;

        //ease out squared exponantial formula
        //t = 1 - Mathf.Pow(1 - t, 2);

        //ease in and out smooth step https://en.wikipedia.org/wiki/Smoothstep
        //t = (3 * Mathf.Pow(t,2)) - (2 * Mathf.Pow(t,3));

        //ease in and out smoother step variation https://en.wikipedia.org/wiki/Smoothstep
        //t = (6 * Mathf.Pow(t, 5)) - (15 * Mathf.Pow(t, 4))+(10*Mathf.Pow(t,3));
        while (!reachdestination)
            {
                elapsedtime += Time.deltaTime;
                float t = elapsedtime / timetomove;
                switch(_interpolationType)
                {
                    case InterpolationType.Linear:
                        break;
                    case InterpolationType.EaseIn:
                        t = t * t;
                        break;
                    case InterpolationType.EaseOut:
                        t = 1 - Mathf.Pow(1 - t, 2);
                        break;
                    case InterpolationType.SmoothStep:
                        t = (3 * Mathf.Pow(t, 2)) - (2 * Mathf.Pow(t, 3));
                        break;
                    case InterpolationType.SmootherStep:
                        t = (6 * Mathf.Pow(t, 5)) - (15 * Mathf.Pow(t, 4)) + (10 * Mathf.Pow(t, 3));
                        break;
                }
           



    transform.position = Vector3.Lerp(startpos, destination, t);

            if (Vector2.Distance(transform.position,destination)<0.01f)
            {
                transform.position = destination;
                reachdestination = true;
                SetIndex((int)destination.x, (int)destination.y);
                _moveCoroutine = null;
            }

            yield return null;
        }

    }
}
