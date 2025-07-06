using System.Collections;
using SmallHedge.SoundManager;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private bool isMoving = false;
    private Vector3 targetPosition;
    private Vector2Int currentGridPos;

    private PuzzleData puzzle;
    private GridManager grid;

    public void Init(PuzzleData puzzleData, Vector2Int startPos, GridManager gridRef)
    {
        puzzle = puzzleData;
        currentGridPos = startPos;
        grid = gridRef;
        transform.position = GridToWorld(currentGridPos);
    }

    void Update()
    {
        if (isMoving || puzzle == null || grid == null) return;

        if (Input.GetKeyDown(KeyCode.UpArrow)) TryMove(Vector2Int.up);
        else if (Input.GetKeyDown(KeyCode.DownArrow)) TryMove(Vector2Int.down);
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) TryMove(Vector2Int.left);
        else if (Input.GetKeyDown(KeyCode.RightArrow)) TryMove(Vector2Int.right);
    }

    void TryMove(Vector2Int dir)
    {
        SoundManager.PlaySound(SoundType.MOVE);
        isMoving = true;
        RotateToDirection(dir);
        StartCoroutine(SlideInDirection(dir));
    }

    IEnumerator SlideInDirection(Vector2Int direction)
    {
        Vector2Int nextPos = currentGridPos + direction;

        while (puzzle.InBounds(nextPos))
        {
            TileType tile = puzzle.GetTileAt(nextPos);

            if (tile == TileType.Wall || tile == TileType.Obstacle || tile == TileType.IceBlock)
            {
                SoundManager.PlaySound(SoundType.LOG);
                break;
            }

            // Goal tile – move and stop
                if (tile == TileType.Goal)
                {
                    Debug.Log("Goal reached!");
                    SoundManager.PlaySound(SoundType.WIN);
                    GameManager gm = FindObjectOfType<GameManager>();
                    if (gm != null)
                        gm.OnPuzzleSolved();

                    yield return StartCoroutine(MoveTo(nextPos));
                    yield break;
                }

            yield return StartCoroutine(MoveTo(nextPos));
            nextPos += direction;
        }

        isMoving = false;
    }

    IEnumerator MoveTo(Vector2Int newPos)
    {
        currentGridPos = newPos;
        targetPosition = GridToWorld(currentGridPos);

        while ((transform.position - targetPosition).sqrMagnitude > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    Vector3 GridToWorld(Vector2Int pos)
    {
        return new Vector3(pos.x * grid.tileSpacing, 0f, pos.y * grid.tileSpacing);
    }

    private void RotateToDirection(Vector2Int direction)
    {
        if (direction == Vector2Int.zero) return;

        Vector3 lookDirection = new Vector3(direction.x, 0, direction.y);
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }

}
