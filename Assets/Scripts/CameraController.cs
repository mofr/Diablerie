using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float sizeChangeSpeed = 3f;

    new Camera camera;

    void Start()
    {
        camera = GetComponent<Camera>();
        camera.orthographicSize = CalcDesiredSize();
    }

    void LateUpdate()
    {
        UpdateSize();

        if (PlayerController.instance.character == null)
            return;

        transform.position = CalcTargetPos();
    }

    void UpdateSize()
    {
        float desiredSize = CalcDesiredSize();
        float diff = desiredSize - camera.orthographicSize;
        float speed = sizeChangeSpeed * Time.deltaTime;
        speed = Mathf.Min(speed, Mathf.Abs(diff)) * Mathf.Sign(diff);
        camera.orthographicSize += speed;
    }

    float CalcDesiredSize()
    {
        return camera.pixelHeight / Iso.pixelsPerUnit / 2;
    }

    Vector3 CalcTargetPos()
    {
        Vector3 targetPos = PlayerController.instance.character.transform.position;
        targetPos.z = transform.position.z;

        return targetPos;
    }
}