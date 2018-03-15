using UnityEngine;
using GoogleARCore;
using GoogleARCore.HelloAR;

public class InstantiateObjectOnTouch : MonoBehaviour {

    /// <summary>
    /// The first-person camera being used to render the passthrough camera.
    /// </summary>
    public Camera FirstPersonCamera;

    /// <summary>
    /// The gameobject to place when tapping the screen.
    /// </summary>
    public GameObject PlaceGameObject;

    // Update is called once per frame
    void Update ()
    {
        // Get the touch position from Unity to see if we have at least one touch event currently active
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        // Now that we know that we have an active touch point, do a raycast to see if it hits
        // a plane where we can instantiate the object on.
        TrackableHit hit;
        var raycastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;

        if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit) && PlaceGameObject != null)
        {
            // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
            // world evolves.
            var anchor = hit.Trackable.CreateAnchor(hit.Pose);

            // Intanstiate a game object as a child of the anchor; its transform will now benefit
            // from the anchor's tracking.
            var placedObject = Instantiate(PlaceGameObject, hit.Pose.position, hit.Pose.rotation);

            // Game object should look at the camera but still be flush with the plane.
            if ((hit.Flags & TrackableHitFlags.PlaneWithinPolygon) != TrackableHitFlags.None)
            {
                // Get the camera position and match the y-component with the hit position.
                Vector3 cameraPositionSameY = FirstPersonCamera.transform.position;
                cameraPositionSameY.y = hit.Pose.position.y;

                // Have game object look toward the camera respecting his "up" perspective, which may be from ceiling.
                placedObject.transform.LookAt(cameraPositionSameY, placedObject.transform.up);
            }

            // Make the newly placed object a child of the parent
            placedObject.transform.parent = anchor.transform;
        }
    }
}
