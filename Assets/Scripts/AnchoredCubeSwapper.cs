using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class AnchoredCubeSwapper : MonoBehaviour
{
    [SerializeField] private ARAnchorManager anchorManager;
    [SerializeField] private PuzzleCube cubePrefab;
    [SerializeField] private float dragLiftHeight = 0.04f;
    [SerializeField] private float dragFollowLerp = 18f;
    [SerializeField] private float maxDropDistance = 0.25f;

    private readonly List<ARAnchor> slotAnchors = new();
    private readonly List<PuzzleCube> slotCubes = new();

    private PuzzleCube draggedCube;
    private int draggedSlotIndex = -1;
    private Vector3 dragBaseWorldPos;
    private bool isDragging;
    private bool isSwapping;
    private int movesUsed;

    public bool IsDragging => isDragging;

    public void BeginDrag(PuzzleCube cube)
    {
        PuzzleDefinition def = GameManager.Instance != null ? GameManager.Instance._puzzleDefinition : null;
        if (isSwapping || isDragging || cube == null || def == null)
            return;

        if (def.maxSwapMoves > 0 && movesUsed >= def.maxSwapMoves)
            return;

        int index = slotCubes.IndexOf(cube);
        if (index < 0)
            return;

        draggedCube = cube;
        draggedSlotIndex = index;
        dragBaseWorldPos = slotAnchors.Count > index && slotAnchors[index] != null
            ? slotAnchors[index].transform.position
            : cube.transform.position;

        isDragging = true;
        draggedCube.Highlight();
    }

    public void UpdateDrag(Vector2 screenPosition, Camera cam)
    {
        if (!isDragging || draggedCube == null || cam == null)
            return;

        // Project pointer onto a horizontal plane at the lifted cube height
        float planeY = dragBaseWorldPos.y + dragLiftHeight;
        Plane dragPlane = new Plane(Vector3.up, new Vector3(0f, planeY, 0f));
        Ray ray = cam.ScreenPointToRay(screenPosition);

        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            draggedCube.transform.position = Vector3.Lerp(
                draggedCube.transform.position,
                hitPoint,
                Time.deltaTime * dragFollowLerp
            );
        }
    }

    public void EndDrag(Vector2 screenPosition, Camera cam)
    {
        if (!isDragging || draggedCube == null)
            return;

        PuzzleDefinition def = GameManager.Instance != null ? GameManager.Instance._puzzleDefinition : null;
        PuzzleCube sourceCube = draggedCube;
        int sourceIndex = draggedSlotIndex;

        sourceCube.Normal();
        draggedCube = null;
        draggedSlotIndex = -1;
        isDragging = false;

        PuzzleCube targetCube = FindDropTargetCube(screenPosition, cam, sourceCube);
        int targetIndex = targetCube != null ? slotCubes.IndexOf(targetCube) : -1;

        if (targetIndex >= 0 && targetIndex != sourceIndex && def != null)
        {
            StartCoroutine(SwapRoutine(sourceIndex, targetIndex, def.swapDuration));
        }
        else
        {
            // Snap back to original anchored slot if not dropped onto another cube
            Transform home = (slotAnchors.Count > sourceIndex && slotAnchors[sourceIndex] != null)
                ? slotAnchors[sourceIndex].transform
                : sourceCube.transform.parent;

            sourceCube.transform.position = home.position;
        }
    }

    private PuzzleCube FindDropTargetCube(Vector2 screenPosition, Camera cam, PuzzleCube ignoredCube)
    {
        if (cam != null)
        {
            Ray ray = cam.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                PuzzleCube hitCube = hit.collider.GetComponentInParent<PuzzleCube>();
                if (hitCube != null && hitCube != ignoredCube && slotCubes.Contains(hitCube))
                {
                    return hitCube;
                }
            }
        }

        // Fallback: closest other cube within maxDropDistance on XZ
        PuzzleCube closest = null;
        float bestDist = maxDropDistance;

        for (int i = 0; i < slotCubes.Count; i++)
        {
            PuzzleCube candidate = slotCubes[i];
            if (candidate == null || candidate == ignoredCube)
                continue;

            Vector3 a = new Vector3(ignoredCube.transform.position.x, 0f, ignoredCube.transform.position.z);
            Vector3 b = new Vector3(candidate.transform.position.x, 0f, candidate.transform.position.z);
            float dist = Vector3.Distance(a, b);

            if (dist < bestDist)
            {
                bestDist = dist;
                closest = candidate;
            }
        }

        return closest;
    }

    private IEnumerator SwapRoutine(int indexA, int indexB, float duration)
    {
        isSwapping = true;

        PuzzleCube cubeA = slotCubes[indexA];
        PuzzleCube cubeB = slotCubes[indexB];

        Transform anchorA = slotAnchors.Count > indexA && slotAnchors[indexA] != null
            ? slotAnchors[indexA].transform
            : cubeA.transform.parent;
        Transform anchorB = slotAnchors.Count > indexB && slotAnchors[indexB] != null
            ? slotAnchors[indexB].transform
            : cubeB.transform.parent;

        Vector3 startA = cubeA.transform.position;
        Vector3 startB = cubeB.transform.position;
        Vector3 endA = anchorB.position;
        Vector3 endB = anchorA.position;

        slotCubes[indexA] = cubeB;
        slotCubes[indexB] = cubeA;
        cubeA.CubeIndex = indexB;
        cubeB.CubeIndex = indexA;

        float elapsed = 0f;
        float safeDuration = Mathf.Max(0.05f, duration);

        while (elapsed < safeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / safeDuration);
            cubeA.transform.position = Vector3.Lerp(startA, endA, t);
            cubeB.transform.position = Vector3.Lerp(startB, endB, t);
            yield return null;
        }

        cubeA.transform.SetParent(anchorB, false);
        cubeB.transform.SetParent(anchorA, false);
        cubeA.transform.localPosition = Vector3.zero;
        cubeB.transform.localPosition = Vector3.zero;

        cubeA.PlaySound();
        movesUsed++;
        isSwapping = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.CheckWin();
        }
    }

    public void ClearBoard()
    {
        draggedCube = null;
        draggedSlotIndex = -1;
        isDragging = false;
        isSwapping = false;
        movesUsed = 0;

        foreach (var anchor in slotAnchors)
        {
            if (anchor != null)
                Destroy(anchor.gameObject);
        }
        slotAnchors.Clear();

        foreach (var cube in slotCubes)
        {
            if (cube != null)
                Destroy(cube.gameObject);
        }
        slotCubes.Clear();
    }

    public async Task SetupBoard(Transform origin, PuzzleDefinition def, BoardDefinition targetBoard)
    {
        ClearBoard();

        if (def == null || cubePrefab == null || origin == null)
            return;

        int rows = def.rows;
        int cols = def.columns;
        float spacingX = def.slotSpacing.x;
        float spacingZ = def.slotSpacing.y;
        float width = (cols - 1) * spacingX;
        float depth = (rows - 1) * spacingZ;

        List<Pose> poses = new();
        for (int i = 0; i < rows * cols; i++)
        {
            int c = i % cols;
            int r = i / cols;
            Vector3 localPos = new Vector3(c * spacingX - width * 0.5f, 0.55f, r * spacingZ - depth * 0.5f);
            Vector3 worldPos = origin.TransformPoint(localPos);
            poses.Add(new Pose(worldPos, origin.rotation));
        }

        for (int i = 0; i < poses.Count; i++)
        {
            Transform parentTransform = origin;
            if (def.useAnchoredSlots && anchorManager != null)
            {
                var anchorResult = await anchorManager.TryAddAnchorAsync(poses[i]);
                if (anchorResult.status.IsSuccess() && anchorResult.value != null)
                {
                    slotAnchors.Add(anchorResult.value);
                    parentTransform = anchorResult.value.transform;
                }
            }

            PuzzleCube cube = Instantiate(cubePrefab, parentTransform);
            cube.transform.localPosition = Vector3.zero;
            cube.transform.localRotation = Quaternion.identity;
            cube.CubeIndex = i;

            CubeState initialState = (targetBoard != null && targetBoard.cells != null && i < targetBoard.cells.Length)
                ? targetBoard.cells[i].startingState
                : CubeState.Red;

            cube.Initialise(new CubeData { active = true, state = initialState });
            slotCubes.Add(cube);
        }

        ScrambleBySwaps(Mathf.Min(def.swapScrambleMoves, slotCubes.Count > 1 ? def.swapScrambleMoves : 0));
    }

    private void ScrambleBySwaps(int moves)
    {
        if (slotCubes.Count < 2 || moves <= 0)
            return;

        for (int m = 0; m < moves; m++)
        {
            int a = Random.Range(0, slotCubes.Count);
            int b = Random.Range(0, slotCubes.Count - 1);
            if (b >= a) b++;

            SwapImmediate(a, b);
        }
    }

    private void SwapImmediate(int indexA, int indexB)
    {
        PuzzleCube cubeA = slotCubes[indexA];
        PuzzleCube cubeB = slotCubes[indexB];

        slotCubes[indexA] = cubeB;
        slotCubes[indexB] = cubeA;

        cubeA.CubeIndex = indexB;
        cubeB.CubeIndex = indexA;

        Transform anchorA = slotAnchors.Count > indexA && slotAnchors[indexA] != null
            ? slotAnchors[indexA].transform
            : cubeA.transform.parent;
        Transform anchorB = slotAnchors.Count > indexB && slotAnchors[indexB] != null
            ? slotAnchors[indexB].transform
            : cubeB.transform.parent;

        cubeA.transform.SetParent(anchorB, false);
        cubeB.transform.SetParent(anchorA, false);
        cubeA.transform.localPosition = Vector3.zero;
        cubeB.transform.localPosition = Vector3.zero;
    }

    public bool MatchesPattern(CubeState[] pattern)
    {
        if (pattern == null || pattern.Length != slotCubes.Count)
            return false;

        for (int i = 0; i < slotCubes.Count; i++)
        {
            if (slotCubes[i] == null || slotCubes[i].Data.state != pattern[i])
                return false;
        }

        return true;
    }

}