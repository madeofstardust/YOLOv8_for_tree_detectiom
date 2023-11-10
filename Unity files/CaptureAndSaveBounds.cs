using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class CaptureAndSaveBounds : MonoBehaviour
{
    //public string scenePath = "YourScenePath"; // Replace with the actual path to your scene.
    public string[] prefabPaths; // An array of fake prefab paths to simulate different prefabs.
    public string[] prefabMaterials; // An array of fake prefab paths to simulate different prefabs.
    public string[] planeTextures;

    public int numberOfScreenshots = 5; // Number of screenshots to capture.
    public int numberOfTrees = 50; // Number of screenshots to capture.

    public int gridWidth = 500; // Number of trees in the X direction.
    public int gridLength = 500; // Number of trees in the Z direction.
    public float minSpacing = 20.0f; // Minimum spacing between trees in X and Z directions.
    public float cameraHeight = 100.0f; // Height of the camera above the trees.

    private Camera mainCamera;
    private Dictionary<GameObject, Bounds> objectBounds = new Dictionary<GameObject, Bounds>();
    private int screenshotIndex = 0; // Initialize the screenshot index.
    private bool cameraMoving = false;
    private bool generateTrees = false;


    void Start()
    {
        prefabPaths = new string[]
        {
            "dataset_generation\\prefabs\\tree_1",
            "dataset_generation\\prefabs\\tree_2",
            "dataset_generation\\prefabs\\tree_3",
            //"dataset_generation\\prefabs\\tree_4",
            "dataset_generation\\prefabs\\tree_5",

            //"dataset_generation\\prefabs\\tree_6",
            "dataset_generation\\prefabs\\tree_7",
            "dataset_generation\\prefabs\\tree_8",
            "dataset_generation\\prefabs\\tree_9",
            "dataset_generation\\prefabs\\tree_10",

            "dataset_generation\\prefabs\\tree_11",
            //"dataset_generation\\prefabs\\tree_12",
            //"dataset_generation\\prefabs\\tree_13",
            "dataset_generation\\prefabs\\tree_14",
            "dataset_generation\\prefabs\\tree_15",

            "dataset_generation\\prefabs\\tree_16",
        };
        prefabMaterials = new string[]
        {
            "dataset_generation\\materials\\mat_1.mat",
            "dataset_generation\\materials\\mat_2.mat",
            "dataset_generation\\materials\\mat_3.mat",
            "dataset_generation\\materials\\mat_4.mat",
        };

        planeTextures = new string[]
        {
            "dataset_generation\\texture\\0",
            "dataset_generation\\texture\\1",
            "dataset_generation\\texture\\2",
            "dataset_generation\\texture\\3",
            "dataset_generation\\texture\\4",
            "dataset_generation\\texture\\5",
            "dataset_generation\\texture\\6",
            "dataset_generation\\texture\\7"
        };

        // Load the specified scene.
        //UnityEngine.SceneManagement.SceneManager.LoadScene(scenePath, UnityEngine.SceneManagement.LoadSceneMode.Single);

        // Find the main camera in the scene.
        mainCamera = Camera.main;

        // Create a directory for saving screenshots and bounding boxes.
        Directory.CreateDirectory("Screenshots");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            cameraMoving = true; // Start camera movement
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            generateTrees = true; // Start camera movement
        }
    }


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            cameraMoving = true; // Start camera movement
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            generateTrees = true; // Start camera movement
        }
    

        if (cameraMoving)
        {

            // Capture and save a screenshot.
            CaptureAndSaveScreenshot(screenshotIndex);

            // Calculate and save the bounding boxes with screenshotIndex.
            CalculateObjectBoundingBoxes(screenshotIndex);

            screenshotIndex++; // Increment the screenshot index.
            cameraMoving=false;
        }

        if (generateTrees)
        {
            try
            {
                DestroyOldTrees();
            }
            catch
            {
                Debug.LogError("Error while deleting old trees: ");
            };
            ChangePlaneRandomTexture();
            PlaceTreesInGrid(gridWidth, gridLength, numberOfTrees);
            generateTrees=false;
        }
    }

    void ChangePlaneRandomTexture()
    {
        // Find the GameObject named "plane"
        GameObject plane = GameObject.Find("Plane");

        if (plane != null)
        {
            Renderer renderer = plane.GetComponent<Renderer>();

            if (renderer != null && planeTextures.Length > 0)
            {
                int randomTextureIndex = Random.Range(0, planeTextures.Length);
                string texturePath = planeTextures[randomTextureIndex];
                Texture newTexture = Resources.Load<Texture>(texturePath);

                if (newTexture != null)
                {
                    renderer.material.mainTexture = newTexture;
                }
                else
                {
                    Debug.LogError("Failed to load the new texture.");
                }
            }
            else
            {
                Debug.LogError("Renderer component not found on the plane or no textures available.");
            }
        }
        else
        {
            Debug.LogError("Plane GameObject not found.");
        }
    }

    void DestroyOldTrees()
    {
        GameObject[] oldTrees = GameObject.FindGameObjectsWithTag("TreeTag"); // Assuming trees have a tag "TreeTag"
        
        foreach (GameObject tree in oldTrees)
        {
            Destroy(tree);
        }
    }

    void PlaceTreesInGrid(int width, int length, int num_of_trees)
    {
        float xOffset = width * minSpacing / 2.0f;
        float zOffset = length * minSpacing / 2.0f;

        // Create a list to store the positions of the trees
        List<Vector3> treePositions = new List<Vector3>();


        // Place the trees at the generated positions
        for (int i = 0; i < num_of_trees; i++)
        {

            Vector3 randomPosition = GenerateRandomTreePosition(xOffset, zOffset, 15, treePositions);
            // Choose a fake prefab path from the array
            string fakePrefabPath = prefabPaths[Random.Range(0, prefabPaths.Length)];

            if (fakePrefabPath == "dataset_generation\\prefabs\\tree_4" || fakePrefabPath == "dataset_generation\\prefabs\\tree_12" || fakePrefabPath == "dataset_generation\\prefabs\\tree_13")
            {
                // Set the initial Y position to 20
                randomPosition.y = 20;
            }

            // Instantiate a placeholder object (simulating a prefab) with the fake path
            GameObject placeholderPrefab = Resources.Load<GameObject>(fakePrefabPath);
            if (placeholderPrefab != null)
            {
                // Successfully loaded the prefab, proceed with instantiation

                Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                GameObject instantiatedObject = Instantiate(placeholderPrefab, randomPosition, randomRotation);

                // Add a tag to the instantiated object
                instantiatedObject.tag = "TreeTag"; // Replace "TreeTag" with your desired tag name
                        
            }
            else
            {
                Debug.LogError("Prefab not found at the specified path: " + fakePrefabPath);
            }
        }
    }

    Vector3 GenerateRandomTreePosition(float xOffset, float zOffset, float minDistance, List<Vector3> existingPositions)
    {
        Vector3 randomPosition;
        bool validPosition = false;

        do
        {
            float x = Random.Range(-xOffset, xOffset);
            float z = Random.Range(-zOffset, zOffset);

            randomPosition = new Vector3(x, 0, z);

            validPosition = true;

            // Check the generated position against existing positions
            foreach (var existingPosition in existingPositions)
            {
                if (Vector3.Distance(randomPosition, existingPosition) < minDistance)
                {
                    validPosition = false;
                    break;
                }
            }
        } while (!validPosition);

        return randomPosition;
    }


    void CaptureAndSaveScreenshot(int screenshotIndex)
    {
        // Set camera to top-down view.
        mainCamera.transform.rotation = Quaternion.Euler(90, 0, 0);

        float gw = gridWidth-(float)10.000; // Assuming gridWidth is initially an int
        float gl = gridLength-(float)10.000; // Assuming gridLength is initially an int

        float xOffset = gw * minSpacing / 2.0f;
        float zOffset = gl * minSpacing / 2.0f;
        // Set a random camera position within the grid size.
        float randomX = Random.Range(-xOffset, xOffset); // Adjusted the range
        float randomZ = Random.Range(-zOffset, zOffset); // Adjusted the range
        float randomY = cameraHeight;

        mainCamera.transform.position = new Vector3(randomX, randomY, randomZ);

        Debug.Log("Position of the camera: " + mainCamera.transform.position);

        // Capture a screenshot.
        string screenshotFileName = $"Screenshots/{screenshotIndex}.png";
        ScreenCapture.CaptureScreenshot(screenshotFileName);
        Debug.Log("Saving a screenshot to: " + screenshotFileName);
    }

    bool IsScreenPointWithinFrustum(Vector3 screenPoint, Vector3[] frustumCorners)
    {
        float minX = frustumCorners[0].x;
        float maxX = frustumCorners[2].x;
        float minY = frustumCorners[0].y;
        float maxY = frustumCorners[1].y;

        return screenPoint.x >= minX && screenPoint.x <= maxX && screenPoint.y >= minY && screenPoint.y <= maxY;
    }

    void CalculateObjectBoundingBoxes(int screenshotIndex)
    {
        string filePath = $"Screenshots/{screenshotIndex}.txt";
        Debug.Log("Pixel rectamgle of the camera " + mainCamera.pixelRect);

        using (StreamWriter file = new StreamWriter(filePath))
        {
            foreach (var obj in FindObjectsOfType<GameObject>())
            {
                Renderer meshRenderer = obj.GetComponent<Renderer>();
                
                if (meshRenderer != null)
                {
                    Rect myRect = RendererBoundsInScreenSpace(meshRenderer);
                    Vector2 topLeftCorner = new Vector2(myRect.x, myRect.y);
                    Vector2 topRightCorner = new Vector2(myRect.x + myRect.width, myRect.y);
                    Vector2 bottomLeftCorner = new Vector2(myRect.x, myRect.y + myRect.height);
                    Vector2 bottomRightCorner = new Vector2(myRect.x + myRect.width, myRect.y + myRect.height);

                    Debug.Log("Camera FOV: " + mainCamera.fieldOfView);

                    Vector3[] frustumCorners = new Vector3[4];
                    mainCamera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), mainCamera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);
                    for (int i = 0; i < frustumCorners.Length; i++)
                    {
                        Debug.Log("Frustum Corner " + i + ": " + frustumCorners[i]);
                    }


                    file.WriteLine($"{obj.name},{topLeftCorner.x},{topLeftCorner.y}," +
                                                    $"{topRightCorner.x},{topRightCorner.y}," +
                                                    $"{bottomLeftCorner.x},{bottomLeftCorner.y}," +
                                                    $"{bottomRightCorner.x},{bottomRightCorner.y}");
                    
                }
            }
        }
    }

    bool IsScreenPointWithinFrustum(Vector3[] screenPoints)
    {
        foreach (var point in screenPoints)
        {
            if (point.x < 0 || point.x >= Screen.width || point.y < 0 || point.y >= Screen.height)
            {
                return false;
            }
        }
        return true;
    }
    static Vector3[] screenSpaceCorners;
    static Rect RendererBoundsInScreenSpace(Renderer r)
    {
        // This is the space occupied by the object's visuals
        // in WORLD space.
        Bounds bigBounds = r.bounds;

        if (screenSpaceCorners == null)
            screenSpaceCorners = new Vector3[8];

        Camera theCamera = Camera.main;

        // For each of the 8 corners of our renderer's world space bounding box,
        // convert those corners into screen space.
        screenSpaceCorners[0] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
        screenSpaceCorners[1] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));
        screenSpaceCorners[2] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
        screenSpaceCorners[3] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));
        screenSpaceCorners[4] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
        screenSpaceCorners[5] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));
        screenSpaceCorners[6] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z));
        screenSpaceCorners[7] = theCamera.WorldToScreenPoint(new Vector3(bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z));

        for (int i = 0; i < screenSpaceCorners.Length; i++)
        {
            Debug.Log("screenSpaceCorners[" + i + "] = " + screenSpaceCorners[i]);
        }

        // Now find the min/max X & Y of these screen space corners.
        float min_x = screenSpaceCorners[0].x;
        float min_y = screenSpaceCorners[0].y;
        float max_x = screenSpaceCorners[0].x;
        float max_y = screenSpaceCorners[0].y;

        for (int i = 1; i < 8; i++)
        {
            if (screenSpaceCorners[i].x < min_x)
            {
                min_x = screenSpaceCorners[i].x;
            }
            if (screenSpaceCorners[i].y < min_y)
            {
                min_y = screenSpaceCorners[i].y;
            }
            if (screenSpaceCorners[i].x > max_x)
            {
                max_x = screenSpaceCorners[i].x;
            }
            if (screenSpaceCorners[i].y > max_y)
            {
                max_y = screenSpaceCorners[i].y;
            }
        }

        return Rect.MinMaxRect(min_x, min_y, max_x, max_y);

    }



}
