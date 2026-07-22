/*using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MatchManager : MonoBehaviour
{
    [SerializeField] private GameObject[] elements;  // Reference to the element prefab
    [SerializeField] private int gridSize = 6;  // Grid dimensions
    [SerializeField] private float cellSize = 0.01f;  // Size of each grid cell
    [SerializeField] private Vector3 gridPosition = new Vector3(0, 0, 0);  // Position of the grid
    [SerializeField] private float spawnYOffset = 2f;  // Offset of the grid from the center of the screen
    [SerializeField] private float gravity = 9.8f;  // Gravity force for dropping
    [SerializeField] private float randomDelayRange = 0.2f;  // Random delay range for initialization
    [SerializeField] private float swipeThreshold = 0.5f;  // Minimum distance for a swipe to be recognized

    [SerializeField] private BattleManager battleManager;
    
    private GameObject[,] grid;  // 2D array to store the grid
    private bool isAnimating = false;  // Flag to prevent multiple animations at once
    private bool isReady = false;  // Flag to indicate if all elements are in place
    private int elementsInPlace = 0;  // Counter for elements that have finished dropping
    
    // Swipe detection variables
    private Vector2 touchStart;
    private bool isSwiping = false;
    private GameObject selectedElement = null;

    public bool canInteract = true;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeGrid();
    }
    
    void InitializeGrid()
    {
        grid = new GameObject[gridSize, gridSize];
        elementsInPlace = 0;
        isReady = false;
        StartCoroutine(InitializeGridWithAnimation());
    }
    
    IEnumerator InitializeGridWithAnimation()
    {
        // Create elements with random delays
        for (int x = 0; x < gridSize; x++)
        {
            StartCoroutine(InitializeColumn(x));
        }
        yield return null;
    }

    IEnumerator InitializeColumn(int x)
    {
        Vector3 startPos = transform.position + gridPosition;
        startPos.x -= gridSize * cellSize / 2f - cellSize / 2f;
        startPos.y -= gridSize * cellSize / 2f - cellSize / 2f;
        for (int y = 0; y < gridSize; y++)
        {
            // Random delay for each element
                float randomDelay = Random.Range(0f, randomDelayRange);
                yield return new WaitForSeconds(randomDelay);
                
                // Start position above the grid with random offset
                float randomX = 0; //Random.Range(-0.5f, 0.5f) * cellSize;
                // sprite bounds
                // var bounds = elements[0].GetComponent<SpriteRenderer>().sprite.rect;
                float randomY = Random.Range(.5f, 1.5f) * y * cellSize + spawnYOffset;
                Vector3 spawnPos = startPos + new Vector3(x * cellSize + randomX, gridSize * cellSize + randomY, -5f);
                Vector3 targetPos = startPos + new Vector3(x * cellSize, y * cellSize, -5f);
                
                
                var type = Random.Range(0, elements.Length);
                GameObject element = Instantiate(elements[type], spawnPos, Quaternion.identity);
                element.transform.SetParent(transform);
                
                // Add BoxCollider2D to the element
                element.AddComponent<BoxCollider2D>();
                
                elementData elementData = element.AddComponent<elementData>();
                elementData.x = x;
                elementData.y = y;
                elementData.elementType = elements[type].name;
                
                grid[x, y] = element;
                
                // Add click handler to the element
                // AddClickHandler(element);
                
                // Animate the element dropping to its position
                StartCoroutine(AnimateElementDrop(element, targetPos));
        }
    }
    
    IEnumerator AnimateElementDrop(GameObject element, Vector3 targetPosition)
    {
        // Mark element as animating
        elementData data = element.GetComponent<elementData>();
        if (data != null)
        {
            data.isAnimating = true;
        }

        float currentSpeed = 5f;
        
        while (Vector3.Distance(element.transform.position, targetPosition) > 0.001f)
        {
            // Apply gravity to speed
            currentSpeed += gravity * Time.deltaTime;
            
            // Calculate new position with gravity
            Vector3 newPosition = element.transform.position;
            newPosition.y -= currentSpeed * Time.deltaTime;
            
            // If we're about to overshoot the target, snap to it
            if (newPosition.y < targetPosition.y)
            {
                element.transform.position = targetPosition;
                break;
            }
            
            element.transform.position = newPosition;
            yield return null;
        }
        
        element.transform.position = targetPosition;
        
        // Mark element as no longer animating
        if (data != null)
        {
            data.isAnimating = false;
        }
        
        elementsInPlace++;
        
        // Check if all elements are in place
        if (elementsInPlace >= gridSize * gridSize)
        {
            isReady = true;
        }
    }
    
    // Add click handler to an element
    void AddClickHandler(GameObject element)
    {
        // Add a collider if it doesn't have one
        if (element.GetComponent<Collider2D>() == null)
        {
            element.AddComponent<BoxCollider2D>();
        }
        
        // Add the click handler component
        ElementClickHandler clickHandler = element.AddComponent<ElementClickHandler>();
        clickHandler.gridManager = this;
    }
    
    // Handle element click
    public void OnElementClicked(GameObject element)
    {
        if (!isReady || isAnimating)
            return;
            
        elementData data = element.GetComponent<elementData>();
        if (data == null)
            return;
            
        StartCoroutine(HandleElementDestruction(data.x, data.y));
    }
    
    // Handle element destruction and dropping
    IEnumerator HandleElementDestruction(int x, int y)
    {
        isAnimating = true;
        
        // Destroy the clicked element
        if (grid[x, y] != null)
        {
            Destroy(grid[x, y]);
            grid[x, y] = null;
        }
        
        // Drop all elements above
        for (int currentY = y + 1; currentY < gridSize; currentY++)
        {
            if (grid[x, currentY] != null)
            {
                // Move element down one position
                GameObject element = grid[x, currentY];
                elementData data = element.GetComponent<elementData>();
                
                // Update grid position
                grid[x, currentY - 1] = element;
                grid[x, currentY] = null;
                
                // Update element data
                data.y = currentY - 1;
                
                // Calculate new target position
                Vector3 startPos = transform.position + gridPosition;
                startPos.x -= gridSize * cellSize / 2f - cellSize / 2f;
                startPos.y -= gridSize * cellSize / 2f - cellSize / 2f;
                Vector3 targetPos = startPos + new Vector3(x * cellSize, (currentY - 1) * cellSize, -5f);
                
                // Animate the drop
                StartCoroutine(AnimateElementDrop(element, targetPos));
            }
        }
        
        // Spawn new elements at the top
        yield return StartCoroutine(SpawnNewElementsAtTop(x));
        
        isAnimating = false;
    }
    
    // Spawn new elements at the top of a column
    IEnumerator SpawnNewElementsAtTop(int x)
    {
        Vector3 startPos = transform.position + gridPosition;
        startPos.x -= gridSize * cellSize / 2f - cellSize / 2f;
        startPos.y -= gridSize * cellSize / 2f - cellSize / 2f;
        
        // Count empty spaces in the column
        int emptySpaces = 0;
        for (int y = 0; y < gridSize; y++)
        {
            if (grid[x, y] == null)
            {
                emptySpaces++;
            }
        }
        
        // Spawn new elements for each empty space
        for (int i = 0; i < emptySpaces; i++)
        {
            // Find the topmost empty position
            int targetY = -1;
            for (int y = gridSize - 1; y >= 0; y--)
            {
                if (grid[x, y] == null)
                {
                    targetY = y;
                    break;
                }
            }
            
            if (targetY == -1)
                break;
                
            // Spawn position above the grid
            float randomY = Random.Range(.5f, 1.5f) * targetY * cellSize + spawnYOffset;
            Vector3 spawnPos = startPos + new Vector3(x * cellSize, targetY * cellSize * 1.5f + randomY, -5f);
            Vector3 targetPos = startPos + new Vector3(x * cellSize, targetY * cellSize, -5f);
            
            // Create new element
            var type = Random.Range(0, elements.Length);
            GameObject element = Instantiate(elements[type], spawnPos, Quaternion.identity);
            element.transform.SetParent(transform);
            
            // Add BoxCollider2D to the element
            element.AddComponent<BoxCollider2D>();
            
            
            // Add component and set data
            elementData elementData = element.AddComponent<elementData>();
            elementData.x = x;
            elementData.y = targetY;
            elementData.elementType = elements[type].name;
            
            // Add click handler
            // AddClickHandler(element);
            
            // Update grid
            grid[x, targetY] = element;
            
            // Animate the drop
            StartCoroutine(AnimateElementDrop(element, targetPos));
            
            // Small delay between spawning elements
            yield return new WaitForSeconds(0.05f);
        }
    }
    
    // Public method to check if the grid is ready for gameplay
    public bool IsReady()
    {
        return isReady;
    }
    
    // Draw grid in editor for visualization
    void OnDrawGizmos()
    {
        // Calculate grid start position
        Vector3 startPos = transform.position + gridPosition;
        startPos.x -= gridSize * cellSize / 2f - cellSize / 2f;
        startPos.y -= gridSize * cellSize / 2f - cellSize / 2f;
        
        // Set gizmo color
        Gizmos.color = Color.yellow;
        
        // Draw vertical lines
        for (int x = 0; x <= gridSize; x++)
        {
            Vector3 lineStart = startPos + new Vector3(x * cellSize - cellSize / 2f, -cellSize / 2f, 0);
            Vector3 lineEnd = startPos + new Vector3(x * cellSize - cellSize / 2f, (gridSize - 1) * cellSize + cellSize / 2f, 0);
            Gizmos.DrawLine(lineStart, lineEnd);
        }
        
        // Draw horizontal lines
        for (int y = 0; y <= gridSize; y++)
        {
            Vector3 lineStart = startPos + new Vector3(-cellSize / 2f, y * cellSize - cellSize / 2f, 0);
            Vector3 lineEnd = startPos + new Vector3((gridSize - 1) * cellSize + cellSize / 2f, y * cellSize - cellSize / 2f, 0);
            Gizmos.DrawLine(lineStart, lineEnd);
        }
        
        // Draw grid center point
        Gizmos.color = Color.red;
        Vector3 centerPos = transform.position + gridPosition;
        Gizmos.DrawWireSphere(centerPos, cellSize * 0.1f);
        
        // Draw individual cell centers
        Gizmos.color = Color.green;
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector3 cellCenter = startPos + new Vector3(x * cellSize, y * cellSize, 0);
                Gizmos.DrawWireCube(cellCenter, Vector3.one * cellSize * 0.1f);
            }
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        // Handle swipe detection
        HandleSwipeInput();
    }
    
    // Handle swipe input detection
    void HandleSwipeInput()
    {
        if (!canInteract)
            return;

        // Mouse input for testing in editor
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            
            Debug.Log(hit.collider);
            if (hit.collider != null)
            {
                selectedElement = hit.collider.gameObject;
                touchStart = mousePosition;
                isSwiping = true;
            }
        }
        else if (Input.GetMouseButtonUp(0) && isSwiping && selectedElement != null)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 swipeDelta = mousePosition - touchStart;
            
            // Check if swipe distance is above threshold
            if (swipeDelta.magnitude > swipeThreshold)
            {
                // Determine swipe direction
                if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                {
                    // Horizontal swipe
                    if (swipeDelta.x > 0)
                    {
                        // Swipe right
                        TrySwapElements(selectedElement, 1, 0);
                    }
                    else
                    {
                        // Swipe left
                        TrySwapElements(selectedElement, -1, 0);
                    }
                }
                else
                {
                    // Vertical swipe
                    if (swipeDelta.y > 0)
                    {
                        // Swipe up
                        TrySwapElements(selectedElement, 0, 1);
                    }
                    else
                    {
                        // Swipe down
                        TrySwapElements(selectedElement, 0, -1);
                    }
                }
            }
            
            isSwiping = false;
            selectedElement = null;
        }
        
        // Touch input for mobile devices
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began)
            {
                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);
                
                if (hit.collider != null)
                {
                    selectedElement = hit.collider.gameObject;
                    touchStart = touchPosition;
                    isSwiping = true;
                }
            }
            else if (touch.phase == TouchPhase.Ended && isSwiping && selectedElement != null)
            {
                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                Vector2 swipeDelta = touchPosition - touchStart;
                
                // Check if swipe distance is above threshold
                if (swipeDelta.magnitude > swipeThreshold)
                {
                    // Determine swipe direction
                    if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                    {
                        // Horizontal swipe
                        if (swipeDelta.x > 0)
                        {
                            // Swipe right
                            TrySwapElements(selectedElement, 1, 0);
                        }
                        else
                        {
                            // Swipe left
                            TrySwapElements(selectedElement, -1, 0);
                        }
                    }
                    else
                    {
                        // Vertical swipe
                        if (swipeDelta.y > 0)
                        {
                            // Swipe up
                            TrySwapElements(selectedElement, 0, 1);
                        }
                        else
                        {
                            // Swipe down
                            TrySwapElements(selectedElement, 0, -1);
                        }
                    }
                }
                
                isSwiping = false;
                selectedElement = null;
            }
        }
    }
    
    // Try to swap two adjacent elements
    void TrySwapElements(GameObject element, int deltaX, int deltaY)
    {
        if (!isReady || isAnimating)
            return;

        elementData data = element.GetComponent<elementData>();
        if (data == null)
            return;
            
        int newX = data.x + deltaX;
        int newY = data.y + deltaY;
        
        // Check if the new position is within grid bounds
        if (newX < 0 || newX >= gridSize || newY < 0 || newY >= gridSize)
            return;
            
        // Check if there's an element at the target position
        if (grid[newX, newY] == null)
            return;
            
        // Swap the elements
        StartCoroutine(SwapElements(data.x, data.y, newX, newY));
    }
    
    // Swap two elements and check if the swap creates a match
    IEnumerator SwapElements(int x1, int y1, int x2, int y2)
    {
        isAnimating = true;
        
        // Notify battle manager that player has swaped
        battleManager.playerSwaped();
        
        GameObject element1 = grid[x1, y1];
        GameObject element2 = grid[x2, y2];
        
        // Get element data
        elementData data1 = element1.GetComponent<elementData>();
        elementData data2 = element2.GetComponent<elementData>();
        
        // Calculate positions
        Vector3 startPos = transform.position + gridPosition;
        startPos.x -= gridSize * cellSize / 2f - cellSize / 2f;
        startPos.y -= gridSize * cellSize / 2f - cellSize / 2f;
        
        Vector3 pos1 = startPos + new Vector3(x1 * cellSize, y1 * cellSize, -5f);
        Vector3 pos2 = startPos + new Vector3(x2 * cellSize, y2 * cellSize, -5f);
        
        // Animate the swap
        float swapDuration = 0.2f;
        float elapsedTime = 0f;
        
        Vector3 startPos1 = element1.transform.position;
        Vector3 startPos2 = element2.transform.position;
        
        while (elapsedTime < swapDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / swapDuration;
            
            // Use smoothstep for more natural movement
            t = t * t * (3f - 2f * t);
            
            element1.transform.position = Vector3.Lerp(startPos1, pos2, t);
            element2.transform.position = Vector3.Lerp(startPos2, pos1, t);
            
            yield return null;
        }
        
        // Update grid positions
        grid[x1, y1] = element2;
        grid[x2, y2] = element1;
        
        // Update element data
        data1.x = x2;
        data1.y = y2;
        data2.x = x1;
        data2.y = y1;
        
        // Check for matches after swap
        bool hasMatch = CheckForMatches();
        
        // If no match, swap back
        if (!hasMatch)
        {
            yield return new WaitForSeconds(0.2f);
            
            // Animate swap back
            elapsedTime = 0f;
            startPos1 = element1.transform.position;
            startPos2 = element2.transform.position;
            
            while (elapsedTime < swapDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / swapDuration;
                
                // Use smoothstep for more natural movement
                t = t * t * (3f - 2f * t);
                
                element1.transform.position = Vector3.Lerp(startPos1, pos1, t);
                element2.transform.position = Vector3.Lerp(startPos2, pos2, t);
                
                yield return null;
            }
            
            // Update grid positions back
            grid[x1, y1] = element1;
            grid[x2, y2] = element2;
            
            // Update element data back
            data1.x = x1;
            data1.y = y1;
            data2.x = x2;
            data2.y = y2;
            
            // No matches, so turn is finished
            yield return StartCoroutine(battleManager.playerSwapTurnFinished());
        }
        else
        {
            // Handle matches (destroy matched elements)
            yield return StartCoroutine(HandleMatches());
            
            // After all cascading matches are handled, turn is finished
            yield return StartCoroutine(battleManager.playerSwapTurnFinished());
        }
        
        isAnimating = false;
    }
    
    // Check for matches in the grid
    bool CheckForMatches()
    {
        bool hasMatch = false;
        
        // Check horizontal matches
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize - 2; x++)
            {
                if (grid[x, y] != null && grid[x + 1, y] != null && grid[x + 2, y] != null)
                {
                    elementData data1 = grid[x, y].GetComponent<elementData>();
                    elementData data2 = grid[x + 1, y].GetComponent<elementData>();
                    elementData data3 = grid[x + 2, y].GetComponent<elementData>();
                    
                    // Skip if any element is animating
                    if (data1.isAnimating || data2.isAnimating || data3.isAnimating)
                        continue;
                    
                    if (data1.elementType == data2.elementType && data2.elementType == data3.elementType)
                    {
                        hasMatch = true;
                        // Mark elements for destruction
                        data1.isMatched = true;
                        data2.isMatched = true;
                        data3.isMatched = true;
                    }
                }
            }
        }
        
        // Check vertical matches
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize - 2; y++)
            {
                if (grid[x, y] != null && grid[x, y + 1] != null && grid[x, y + 2] != null)
                {
                    elementData data1 = grid[x, y].GetComponent<elementData>();
                    elementData data2 = grid[x, y + 1].GetComponent<elementData>();
                    elementData data3 = grid[x, y + 2].GetComponent<elementData>();
                    
                    // Skip if any element is animating
                    if (data1.isAnimating || data2.isAnimating || data3.isAnimating)
                        continue;
                    
                    if (data1.elementType == data2.elementType && data2.elementType == data3.elementType)
                    {
                        hasMatch = true;
                        // Mark elements for destruction
                        data1.isMatched = true;
                        data2.isMatched = true;
                        data3.isMatched = true;
                    }
                }
            }
        }
        
        return hasMatch;
    }
    
    // Handle matched elements
    IEnumerator HandleMatches()
    {
        // Find all matched elements
        List<GameObject> matchedElements = new List<GameObject>();
        
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (grid[x, y] != null)
                {
                    elementData data = grid[x, y].GetComponent<elementData>();
                    if (data.isMatched)
                    {
                        matchedElements.Add(grid[x, y]);
                    }
                }
            }
        }
        
        // Group connected matched elements
        List<List<GameObject>> connectedGroups = new List<List<GameObject>>();
        HashSet<GameObject> processedElements = new HashSet<GameObject>();
        
        foreach (GameObject element in matchedElements)
        {
            if (!processedElements.Contains(element))
            {
                List<GameObject> currentGroup = new List<GameObject>();
                FindConnectedElements(element, currentGroup, processedElements);
                connectedGroups.Add(currentGroup);
            }
            grid[element.GetComponent<elementData>().x, element.GetComponent<elementData>().y] = null;
        }
        
        // Animate and destroy each group separately
        foreach (List<GameObject> group in connectedGroups)
        {
            // Animate the current group
            float duration = 0.3f;
            float elapsedTime = 0f;
            
            // Store original scales for all elements in the group
            Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>();
            foreach (GameObject element in group)
            {
                originalScales[element] = element.transform.localScale;
            }
            
            // Animate all elements in the group simultaneously
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                
                // Apply scale animation to all elements in the group
                foreach (GameObject element in group)
                {
                    //element.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);
                    if (element != null) // Check if element still exists
                    {
                        element.transform.localScale = Vector3.Lerp(originalScales[element], Vector3.zero, t);
                    }
                }
                
                yield return null;
            }

            var elements = new string[group.Count];
            for (int i = 0; i < group.Count; i++)
            {
                elements[i] = group[i].GetComponent<elementData>().elementType;
            }
            battleManager.elementsMatched(elements);
            
            // Destroy all elements in the group after animation completes
            foreach (GameObject element in group)
            {
                if (element != null) // Check if element still exists
                {
                    Destroy(element);
                }
            }
            
            // Small delay between groups
            yield return new WaitForSeconds(0.1f);
        }
        
        // Drop elements and spawn new ones
        yield return StartCoroutine(DropElementsAndSpawnNew());
    }
    
    // Helper method to find connected elements (horizontally or vertically)
    private void FindConnectedElements(GameObject element, List<GameObject> currentGroup, HashSet<GameObject> processedElements)
    {
        if (element == null || processedElements.Contains(element))
            return;
            
        processedElements.Add(element);
        currentGroup.Add(element);
        
        elementData data = element.GetComponent<elementData>();
        if (data == null)
            return;
            
        int x = data.x;
        int y = data.y;
        
        // Check horizontally (left and right)
        if (x > 0 && grid[x - 1, y] != null)
        {
            elementData leftData = grid[x - 1, y].GetComponent<elementData>();
            if (leftData != null && leftData.isMatched)
            {
                FindConnectedElements(grid[x - 1, y], currentGroup, processedElements);
            }
        }
        
        if (x < gridSize - 1 && grid[x + 1, y] != null)
        {
            elementData rightData = grid[x + 1, y].GetComponent<elementData>();
            if (rightData != null && rightData.isMatched)
            {
                FindConnectedElements(grid[x + 1, y], currentGroup, processedElements);
            }
        }
        
        // Check vertically (up and down)
        if (y > 0 && grid[x, y - 1] != null)
        {
            elementData downData = grid[x, y - 1].GetComponent<elementData>();
            if (downData != null && downData.isMatched)
            {
                FindConnectedElements(grid[x, y - 1], currentGroup, processedElements);
            }
        }
        
        if (y < gridSize - 1 && grid[x, y + 1] != null)
        {
            elementData upData = grid[x, y + 1].GetComponent<elementData>();
            if (upData != null && upData.isMatched)
            {
                FindConnectedElements(grid[x, y + 1], currentGroup, processedElements);
            }
        }
    }
    
    // Drop elements and spawn new ones
    IEnumerator DropElementsAndSpawnNew()
    {
        // Drop elements down
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (grid[x, y] == null)
                {
                    // Find the first non-null element above
                    for (int aboveY = y + 1; aboveY < gridSize; aboveY++)
                    {
                        if (grid[x, aboveY] != null)
                        {
                            // Move element down
                            grid[x, y] = grid[x, aboveY];
                            grid[x, aboveY] = null;
                            
                            // Update element data
                            elementData data = grid[x, y].GetComponent<elementData>();
                            data.y = y;
                            
                            // Calculate new position
                            Vector3 startPos = transform.position + gridPosition;
                            startPos.x -= gridSize * cellSize / 2f - cellSize / 2f;
                            startPos.y -= gridSize * cellSize / 2f - cellSize / 2f;
                            Vector3 targetPos = startPos + new Vector3(x * cellSize, y * cellSize, -5f);
                            
                            // Animate the drop
                            StartCoroutine(AnimateElementDrop(grid[x, y], targetPos));
                            
                            break;
                        }
                    }
                }
            }
        }
        
        // Wait for drops to complete
        yield return new WaitForSeconds(0.5f);
        
        // Spawn new elements at the top
        for (int x = 0; x < gridSize; x++)
        {
            yield return StartCoroutine(SpawnNewElementsAtTop(x));
        }

        // Wait for new elements to spawn
        yield return new WaitForSeconds(0.5f);
        
        // Check for new matches and handle them recursively until no more matches
        while (CheckForMatches())
        {
            yield return StartCoroutine(HandleMatches());
        }
    }
}

// Component to store element data
public class elementData : MonoBehaviour
{
    public int x;  // X position in the grid
    public int y;  // Y position in the grid
    public string elementType;  // Type of element (0 to elementTypes-1)
    public bool isMatched = false;  // Flag to mark matched elements
    public bool isAnimating = false;  // Flag to mark elements that are animating
}

// Component to handle element clicks
public class ElementClickHandler : MonoBehaviour
{
    public MatchManager gridManager;
    
    void OnMouseDown()
    {
        if (gridManager != null)
        {
            gridManager.OnElementClicked(gameObject);
        }
    }
}
*/