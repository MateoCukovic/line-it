using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private Vector3 touchStartPosition;
    private float zoomOutMin = 3;
    private float zoomOutMax;

    private bool cameraPositioned;

    private void Start()
    {
        cameraPositioned = false;
    }

    private void Update()
    {
        if(Generator.boundaryPositions != null && !cameraPositioned)
        {
            PositionCamera();
            cameraPositioned = true;
        }

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
        }
    }

    private void Pan()
    {
        Vector3 direction = touchStartPosition - Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 cameraPositionPlaceholder = Camera.main.transform.position + direction;

        if (!(cameraPositionPlaceholder.x < Generator.boundaryPositions["minX"] || cameraPositionPlaceholder.x > Generator.boundaryPositions["maxX"] || 
            cameraPositionPlaceholder.y < Generator.boundaryPositions["minY"] || cameraPositionPlaceholder.y > Generator.boundaryPositions["maxY"]))
            Camera.main.transform.position += direction;
    }

    // Relative to the nodes
    private void PositionCamera()
    {
        float x = (Generator.boundaryPositions["minX"] + Generator.boundaryPositions["maxX"]) / 2;
        float y = (Generator.boundaryPositions["minY"] + Generator.boundaryPositions["maxY"]) / 2;
        float z = Camera.main.transform.position.z;

        float distanceMinToMaxX = Mathf.Sqrt(Mathf.Pow(Generator.farthestNodes["farMinX"].y - Generator.farthestNodes["farMaxX"].y, 2) +
                                             Mathf.Pow(Generator.farthestNodes["farMinX"].x - Generator.farthestNodes["farMaxX"].x, 2));

        float distanceMinToMaxY = Mathf.Sqrt(Mathf.Pow(Generator.farthestNodes["farMinY"].y - Generator.farthestNodes["farMaxY"].y, 2) +
                                             Mathf.Pow(Generator.farthestNodes["farMinY"].x - Generator.farthestNodes["farMaxY"].x, 2));

        if(distanceMinToMaxX > distanceMinToMaxY)
        {
            Camera.main.orthographicSize = distanceMinToMaxX + 1f; 
        }
        else
        {
            Camera.main.orthographicSize = distanceMinToMaxY;
        }

        zoomOutMax = Camera.main.orthographicSize;

        Camera.main.transform.position = new Vector3(x, y, z);
    }

    private void Zoom()
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
}
