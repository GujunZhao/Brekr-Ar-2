using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class GazeSpawner : MonoBehaviour
{
    public ARTrackedImageManager imageManager;
    public Camera arCamera;
    public GameObject cubePrefab;

    private Dictionary<TrackableId, GameObject> spawnedCubes = new();
    private float gazeTimer = 0f;
    private float gazeDuration = 2f;
    private ARTrackedImage currentlyGazedImage;

    void Update()
    {
        if (imageManager == null || arCamera == null)
            return;

        foreach (var trackedImage in imageManager.trackables)
        {
            if (trackedImage.trackingState != TrackingState.Tracking)
                continue;

            Ray ray = arCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == trackedImage.transform)
            {
                if (currentlyGazedImage != trackedImage)
                {
                    gazeTimer = 0f;
                    currentlyGazedImage = trackedImage;
                }

                gazeTimer += Time.deltaTime;
                if (gazeTimer >= gazeDuration)
                {
                    SpawnCube(trackedImage);
                    currentlyGazedImage = null;
                }
                return;
            }
        }

        // Reset if nothing is being gazed
        gazeTimer = 0f;
        currentlyGazedImage = null;
    }

    void SpawnCube(ARTrackedImage image)
    {
        if (spawnedCubes.ContainsKey(image.trackableId))
            return;

        var cube = Instantiate(cubePrefab, image.transform.position, image.transform.rotation);
        cube.transform.SetParent(image.transform);
        spawnedCubes.Add(image.trackableId, cube);
    }
}
