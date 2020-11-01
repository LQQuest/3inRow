using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Board Variables")]
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int tardetX;
    public int tardetY;
    public bool isMatched = false;

    private FindMatches findMatches;
    private Board board;    
    private GameObject otherDot;
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosition;
    public float swipeAngle = 0;
    public float swipeResist = 1f;
    
    void Start()
    {
        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
        //tardetX = (int)transform.position.x;
        //tardetY = (int)transform.position.y;
        //row = tardetY;
        //column = tardetX;
        //previousColumn = column;
        //previousRow = row;
    }


    void Update()
    {
        //FindMatches();
        /*if (isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(1f,1f,1f,.2f);
        }
        */
        tardetX = column;
        tardetY = row;
        if (Mathf.Abs(tardetX - transform.position.x) > .1)
        {
            //Move Towards the target
            tempPosition = new Vector2(tardetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.allDots[column,row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            //Directly set the position
            tempPosition = new Vector2(tardetX, transform.position.y);
            transform.position = tempPosition;
        }
        if (Mathf.Abs(tardetY - transform.position.y) > .1)
        {
            //Move Towards the target
            tempPosition = new Vector2(transform.position.x, tardetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.allDots[column,row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            //Directly set the position
            tempPosition = new Vector2(transform.position.x, tardetY);
            transform.position = tempPosition;
            
        }

        StartCoroutine(CheckMoveCo());
    }

    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(.5f);
        if (otherDot != null)
        {
            if (!isMatched && !otherDot.GetComponent<Dot>().isMatched)
            {
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(.5f);
                board.currentState = GameState.move;
            }
            else
            {
                board.DestroyMatches();
                
            }

            otherDot = null;
        }
        
        
    }
    private void OnMouseDown()
    {
        if (board.currentState == GameState.move)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        
    }

    private void OnMouseUp()
    {
        if (board.currentState == GameState.move)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
            CalculateAngle(); 
        }
        
        
    }

    void CalculateAngle()
    {
        if (Math.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || 
            Math.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
           swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y,
                       finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
           MovePieces(); 
           board.currentState = GameState.wait;
        }
        else
        {
            board.currentState = GameState.move;
        }
        
    }

    void MovePieces()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.wight - 1)
        {
            //Right Swipe
            otherDot = board.allDots[column + 1, row];
            previousColumn = column;
            previousRow = row;
            otherDot.GetComponent<Dot>().column -= 1;
            column += 1;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            //Up Swipe
            otherDot = board.allDots[column, row + 1];;
            previousColumn = column;
            previousRow = row;
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;
        }
        else if (swipeAngle > 135 || swipeAngle <= -135 && column > 0)
        {
            //Left Swipe
            otherDot = board.allDots[column - 1, row];;
            previousColumn = column;
            previousRow = row;
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            //Down Swipe
            otherDot = board.allDots[column, row - 1];;
            previousColumn = column;
            previousRow = row;
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;
        }

    }

    void FindMatches()
    {
        if (column > 0 && column < board.wight -1)
        {
            var leftDot1 = board.allDots[column - 1, row];
            var rightDot1 = board.allDots[column + 1, row];
            if (leftDot1 != null && rightDot1 != null)
            {
                if (leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag)
                {
                    leftDot1.GetComponent<Dot>().isMatched = true;
                    rightDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
            
        }
        if (row > 0 && row < board.height -1)
        {
            GameObject upDot1 = board.allDots[column, row + 1];
            GameObject downDot1 = board.allDots[column, row - 1];
            if (upDot1 != null && downDot1 != null)
            {
               if (upDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag)
               {
                   upDot1.GetComponent<Dot>().isMatched = true;
                   downDot1.GetComponent<Dot>().isMatched = true;
                   isMatched = true;
               } 
            }
            
        }
    }
}
