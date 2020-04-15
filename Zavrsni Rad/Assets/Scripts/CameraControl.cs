using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private Vector3 touchStartPosition;
    private float zoomOutMin = 1;
    private float zoomOutMax = 8;

    void Update()
    {
        if (TouchControl.isTouching == TouchControl.Touching.Area)
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchStartPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.touchCount == 2)
            {
                Pan();
                Zoom();
            }
            else if (Input.GetMouseButton(0))
            {
                Pan();
            }

            Zoom(Input.GetAxis("Mouse ScrollWheel"));
        }
    }

    void Pan()
    {
        Vector3 direction = touchStartPosition - Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 cameraPositionPlaceholder = Camera.main.transform.position + direction;

        if (!(cameraPositionPlaceholder.x < Generator.boundaryPositions["minX"] || cameraPositionPlaceholder.x > Generator.boundaryPositions["maxX"] || cameraPositionPlaceholder.y < Generator.boundaryPositions["minY"] || cameraPositionPlaceholder.y > Generator.boundaryPositions["maxY"]))
            Camera.main.transform.position += direction;
    }

    void Zoom()
    {
        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

        float difference = currentMagnitude - prevMagnitude;

        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - difference * 0.01f, zoomOutMin, zoomOutMax);
    }

    void Zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);
    }
}
