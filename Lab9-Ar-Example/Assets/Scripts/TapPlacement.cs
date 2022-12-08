using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class TapPlacement : MonoBehaviour
{
    public GameObject placementIndicator;
    public GameObject objectToPlace;
    public Camera MainCamera;

    private ARSessionOrigin _arSessionOrigin;
    private ARRaycastManager _arRaycastManager;
    private Pose posePlacement;
    private bool validPosePlacement;
    
    // Start is called before the first frame update
    void Start()
    {
        _arRaycastManager = FindObjectOfType<ARRaycastManager>();
        _arSessionOrigin = FindObjectOfType<ARSessionOrigin>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlacement();
        UpdatePlacementPose();

        if (validPosePlacement && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceObjectInWorld();
        }
    }

    private void PlaceObjectInWorld()
    {
        Instantiate(objectToPlace, posePlacement.position, posePlacement.rotation);
    }

    private void UpdatePlacementPose()
    {
        if (validPosePlacement)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(posePlacement.position, posePlacement.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacement()
    {
        var screenCentre = MainCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();

        _arRaycastManager.Raycast(screenCentre, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);
        validPosePlacement = hits.Count > 0;

        if (validPosePlacement)
        {
            posePlacement = hits[0].pose;

            var cameraForward = MainCamera.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z);
            posePlacement.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
}
