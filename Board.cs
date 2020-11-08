using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public enum GameState
{
    wait,
    move
}

public enum TileKind
{
    Breakable,
    Blank,
    Normal
}

[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileKind tileKind;
}
public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move;
    public int wight;
    public int height;
    public int offSet;
    public GameObject tilePrefab;
    public GameObject[] dots;
    public GameObject[,] allDots;
    public GameObject destroyEffect;
    public GameObject breakableTilePrefab;
    public TileType[] boardLayout;
    private bool[,] blankSpaces;
    private BackgroundTile[,] breakableTiles;
    public Dot currentDot;
    private FindMatches findMatches;
    
    void Start()
    {
        breakableTiles = new BackgroundTile[wight, height];
        blankSpaces = new bool[wight, height];
        findMatches = FindObjectOfType<FindMatches>();
        allDots = new GameObject[wight, height];
        SetUp();
    }

    public void GenerateBlankSpaces()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Blank)
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }

    public void GenerateBreakableTiles()
    {
        //Look at all tiles in the layout
        for (int i = 0; i < boardLayout.Length; i++)
        {    
            //if a tile is a "Jelly" tile
            if (boardLayout[i].tileKind == TileKind.Breakable)
            {
                //Create a "Jelly" tile at the position
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(breakableTilePrefab, tempPosition, Quaternion.identity);
                breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }
    private void SetUp()
    {
        GenerateBlankSpaces();
        GenerateBreakableTiles();
        for (int i = 0; i < wight; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    GameObject backgroundTile =
                        Instantiate(tilePrefab, tempPosition, quaternion.identity) as GameObject;
                    backgroundTile.transform.parent = this.transform;
                    backgroundTile.name = "(" + i + "," + j + ")";
                    int dotToUse = Random.Range(0, dots.Length);
                    
                    int maxIteration = 0;
                    while (MatchesAt(i, j, dots[dotToUse]) && maxIteration < 100)
                    {
                        dotToUse = Random.Range(0, dots.Length);
                        maxIteration++;
                    }
                    maxIteration = 0;
                    
                    GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    dot.GetComponent<Dot>().row = j;
                    dot.GetComponent<Dot>().column = i;

                    dot.transform.parent = this.transform;
                    dot.name = "(" + i + "," + j + ")";
                    allDots[i, j] = dot;
                }
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
            {    
                if (allDots[column - 1, row].CompareTag(piece.tag) && allDots[column - 2, row].CompareTag(piece.tag))
                {
                    return true;
                }
            }
            if (allDots[column, row - 1] != null && allDots[column, row - 1] != null)
            {
                if (allDots[column, row - 1].CompareTag(piece.tag) && allDots[column, row - 2].CompareTag(piece.tag))
                {
                    return true;
                }
            }
        }else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
                {
                    if (allDots[column, row - 1].CompareTag(piece.tag) && allDots[column, row - 2].CompareTag(piece.tag))
                    {
                        return true;
                    }
                }
                
            }
            if (column > 1)
            {
                if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
                {
                  if (allDots[column - 1, row].CompareTag(piece.tag) && allDots[column - 2, row].CompareTag(piece.tag))
                  {
                      return true;
                  }  
                }
                
            }
        }
        return false;
    }

    private bool ColumnOrRow()
    {
        int numberHorizontal = 0;
        int numberVertical = 0;
        Dot firstPiece = findMatches.currentMatches[0].GetComponent<Dot>();
        if (firstPiece != null)
        {
            foreach (GameObject currentPiece in findMatches.currentMatches)
            {
                Dot dot = currentPiece.GetComponent<Dot>();
                if (dot.row == firstPiece.row)
                {
                    numberHorizontal++;
                }

                if (dot.column == firstPiece.column)
                {
                    numberVertical++;
                }
            }
        }

        return (numberHorizontal == 5 || numberVertical == 5);
    }

    private void CheckToMakeBomb()
    {
        if (findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7)
        {
            findMatches.CheckBombs();
        }
        if (findMatches.currentMatches.Count == 5 || findMatches.currentMatches.Count == 8)
        {
            if (ColumnOrRow())
            {
                if (currentDot != null)
                {
                    if (currentDot.isMatched)
                    {
                        if (!currentDot.isColorBomb)
                        {
                            currentDot.isMatched = false;
                            currentDot.MakeColorBomb();
                        }
                    }
                    else
                    {
                        if (currentDot.otherDot != null)
                        {
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if (otherDot.isMatched)
                            {
                                if (!otherDot.isColorBomb)
                                {
                                    otherDot.isMatched = false;
                                    otherDot.MakeColorBomb();
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (currentDot != null)
                {
                    if (currentDot.isMatched)
                    {
                        if (!currentDot.isAdjacentBomb)
                        {
                            currentDot.isMatched = false;
                            currentDot.MakeAdjecantBomb();
                        }
                    }
                    else
                    {
                        if (currentDot.otherDot != null)
                        {
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if (otherDot.isMatched)
                            {
                                if (!otherDot.isAdjacentBomb)
                                {
                                    otherDot.isMatched = false;
                                    otherDot.MakeAdjecantBomb();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    private void DestroyMatchesAt(int column, int row)
    {
        if (allDots[column,row].GetComponent<Dot>().isMatched)
        {
            if (findMatches.currentMatches.Count == 4)
            {
                CheckToMakeBomb();
            }
            //Does a tile need a break?
            if (breakableTiles[column, row] != null)
            {
                //if it does, give one damage
                breakableTiles[column, row].TakeDamage(1);
                if (breakableTiles[column, row].hitPoints <= 0)
                {
                    breakableTiles[column, row] = null;
                }
            }
            GameObject particle = Instantiate(destroyEffect, 
                                              allDots[column, row].transform.position, 
                                              Quaternion.identity);
            Destroy(particle, .5f);
            Destroy(allDots[column,row]);
            allDots[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < wight; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i,j] != null)
                {
                    DestroyMatchesAt(i,j);
                }
            }
        }
        findMatches.currentMatches.Clear();
        StartCoroutine(DecreaseRowCo2());    
    }

    public IEnumerator DecreaseRowCo2()
    {
        for (int i = 0; i < wight; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //if current spot isn't blank and is empty...
                if (blankSpaces[i,j] && allDots[i,j] == null)
                {
                    // loop from the space above to the top of the column
                    for (int k = 0; k < height; k++)
                    {
                        // its a dot is found
                        if (allDots[i,j] != null)
                        {
                            //move this dot to this empty space
                            allDots[i, k].GetComponent<Dot>().row = j;
                            //set the spot to be null
                            allDots[i, k] = null;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }
    
    public IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
        for (int i = 0; i < wight; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i,j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }

            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard()
    {
        for (int i = 0; i < wight; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i,j] == null && !blankSpaces[i,j])
                {
                    Vector2 tempPosition = new Vector2(i,j + offSet);
                    int dotToUse = Random.Range(0, dots.Length);
                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < wight; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i,j]!=null)
                {
                    if (allDots[i,j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        findMatches.currentMatches.Clear();
        currentDot = null;
        yield return new WaitForSeconds(.5f);

        if (IsDeadLocked())
        {
            ShuffleBoard();
            Debug.Log("Dead Locked!");
        }
        currentState = GameState.move;
    }

    private void SwitchPieces(int column, int row, Vector2 direction)
    {
        //Take the second piece and save it in holder 
        GameObject holder = allDots[column + (int) direction.x, row + (int) direction.y] as GameObject;
        //Switch the first dot to be the second position
        allDots[column + (int) direction.x, row + (int) direction.y] = allDots[column, row];
        //Set the first dot to be the second dot
        allDots[column, row] = holder;
    }

    private bool CheckForMatches()
    {
        for (int i = 0; i < wight; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    //Make sure that one and two to the right in the board 
                    if (i < wight - 2)
                    {
                        //Check if the dots to the right and two the right exist 
                        if (allDots[i + 1, j] != null && allDots[i + 2, j] != null)
                        {
                            if (allDots[i + 1, j].CompareTag(allDots[i, j].tag) && 
                                allDots[i + 2, j].CompareTag(allDots[i,j].tag) )
                            {
                               return true;
                            }
                        } 
                    }

                    if (j < height - 2)
                    {
                        //Check if the dots above exist
                        if (allDots[i, j + 1]!=null && allDots[i, j + 2]!=null)
                        {
                            if (allDots[i, j + 1].CompareTag(allDots[i, j].tag) && 
                                allDots[i, j + 2].CompareTag(allDots[i, j].tag) )
                            {
                                return true;
                            }
                        } 
                    } 
                }
            }
        }
        return false;
    }

    private bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchPieces(column, row, direction);
        if (CheckForMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }
        SwitchPieces(column, row, direction);
        return false;
    }

    private bool IsDeadLocked()
    {
        for (int i = 0; i < wight; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i,j] != null)
                {
                    if (i < wight - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }
                    if (j < height - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        
        return true;
    }

    private void ShuffleBoard()
    {
        //Create a list of gameObjects
        List<GameObject> newBoard = new List<GameObject>();
        //Add every pieces to this list
        for (int i = 0; i < wight; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i,j] != null)
                {
                    newBoard.Add(allDots[i,j]);
                }
            }
        }
        //for every spot on the board...
        for (int i = 0; i < wight; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //if this spot shouldn't be blank
                if (!blankSpaces[i,j])
                {    
                    //Pick a random number
                    int piecetoUse = Random.Range(0, newBoard.Count);
                    
                    int maxIteration = 0;
                    while (MatchesAt(i, j, newBoard[piecetoUse]) && maxIteration < 100)
                    {
                        piecetoUse = Random.Range(0, newBoard.Count);
                        maxIteration++;
                    }
                    maxIteration = 0;
                    
                    //Make a container for the piece
                    Dot piece = newBoard[piecetoUse].GetComponent<Dot>();
                    //Assign the column to the piece
                    piece.column = i;
                    //Assign the row to the piece
                    piece.row = j;
                    //Fill the dots array with new piece
                    allDots[i, j] = newBoard[piecetoUse];
                    //Remove it from the list
                    newBoard.Remove(newBoard[piecetoUse]);
                }
            }
        }
        //Check if it's still deadLocked
        if (IsDeadLocked())
        {
            ShuffleBoard();
        }
    }
    
}
