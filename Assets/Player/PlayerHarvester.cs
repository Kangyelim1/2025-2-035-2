using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHarvester : MonoBehaviour
{
    public float rayDistance = 5f;        // ä�� ���� �Ÿ�
    public LayerMask hitMask = ~0;      // ������ �� ���̾� ���� �� (�ϴ�)
    public int toolDamage = 1;          // Ÿ�� ������
    public float hitCooldown = 0.15f;   // ��Ÿ ����

    private float _nextHitTime;
    private Camera _cam;
    public Inventory inventory;         // �÷��̾� �κ�(������ �ڵ� ����)
    InventoryUI invenUI;
    public GameObject selectedBlock;

    void Awake()
    {
        _cam = Camera.main;
        if (inventory == null)
            inventory = gameObject.AddComponent<Inventory>();

        invenUI = FindObjectOfType<InventoryUI>();
    }

    void Update()
    {
        if (invenUI.selectedIndex < 0)
        {
            // ���õ� idx�� -1�̸� ���� ���
            if (Input.GetMouseButton(0) && Time.time >= _nextHitTime)
            {
                _nextHitTime = Time.time + hitCooldown;

                Ray ray = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // ȭ�� �߾�
                if (Physics.Raycast(ray, out var hit, rayDistance, hitMask, QueryTriggerInteraction.Ignore))
                {
                    var block = hit.collider.GetComponent<Block>();
                    if (block != null)
                    {
                        block.Hit(toolDamage, inventory);
                    }
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                // ���õ� idx�� 0 �̻��̸� ��ġ ���
                Ray ray = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // ȭ�� �߾�
                if (Physics.Raycast(ray, out var hit, rayDistance, hitMask, QueryTriggerInteraction.Ignore))
                {
                    Vector3Int placePos = AdjacentCellOnHitFace(hit);

                    ItemType selected = invenUI.GetInventorySlot();
                    if (inventory.Consume(selected, 1))
                    {
                        FindObjectOfType<NoiseVoxelMap>().PlaceTile(placePos, selected);
                    }
                }
            }
        }
    }

    static Vector3Int AdjacentCellOnHitFace(in RaycastHit hit)
    {
        Vector3 baseCenter = hit.collider.transform.position; // ���� ������ �߽�(���� ��ǥ x,y,z)
        Vector3 adjCenter = baseCenter + hit.normal;          // �� ���� �ٱ������� ��Ȯ�� �� ĭ �̵�
        return Vector3Int.RoundToInt(adjCenter);
    }
}

