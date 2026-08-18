using UnityEngine;
using Unity.Cinemachine;

public class MapTransition : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D mapBoundry;
    [SerializeField] private Direction direction;
    [SerializeField] private Transform teleportTargetPosition;
    private CinemachineConfiner2D confiner;
    private CinemachineCamera vcam;

    enum Direction { Up, Down, Left, Right, Teleport }

    private void Awake()
    {
        confiner = FindFirstObjectByType<CinemachineConfiner2D>();
        vcam = FindFirstObjectByType<CinemachineCamera>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            FadeTransition(collision.gameObject);
        }
    }

    async void FadeTransition(GameObject player)
    {
        await ScreenFader.Instance.FadeOut();

        confiner.BoundingShape2D = mapBoundry;
        UpdatePlayerPosition(player);

        await ScreenFader.Instance.FadeIn();
    }

    private void UpdatePlayerPosition(GameObject player)
    {
        Vector3 targetPos = player.transform.position;

        if (direction == Direction.Teleport)
        {
            if (teleportTargetPosition != null)
            {
                targetPos = teleportTargetPosition.position;
            }
        }

        switch (direction)
        {
            case Direction.Up:
                targetPos.y += 2;
                break;
            case Direction.Down:
                targetPos.y -= 2;
                break;
            case Direction.Left:
                targetPos.x -= 2;
                break;
            case Direction.Right:
                targetPos.x += 2;
                break;
        }

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.position = targetPos;
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            player.transform.position = targetPos;
        }

        if (vcam != null)
            vcam.OnTargetObjectWarped(player.transform, targetPos - player.transform.position);

        if (mapBoundry != null && confiner != null)
        {
            confiner.BoundingShape2D = mapBoundry;
            confiner.InvalidateBoundingShapeCache();
        }
    }
}