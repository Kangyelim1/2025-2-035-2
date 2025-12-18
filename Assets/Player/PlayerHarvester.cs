using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    int dugCount = 0; // 팠던 블록 개수를 저장할 변수

    private float startY; // 태어났을 때의 높이를 저장할 변수
    public float targetDepth = 5f; // "시작점으로부터 20칸 아래"로 설정

    // 1. 무언가(잔디/바닥)를 밟았을 때 그 높이를 저장합니다.
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // 처음 무언가를 밟았을 때만 높이 저장
        if (startY == 0)
        {
            startY = transform.position.y;
            Debug.Log("기준 높이 저장 완료!");
        }
    }

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

        //씬 이동 조건


        // 2. 저장된 높이가 0이 아닐 때만(즉, 한 번이라도 밟았을 때만) 체크
        if (startY != 0 && startY - transform.position.y >= targetDepth)
        {
            SceneManager.LoadScene("2");
        }

    }

    static Vector3Int AdjacentCellOnHitFace(in RaycastHit hit)
    {
        Vector3 baseCenter = hit.collider.transform.position; // ���� ������ �߽�(���� ��ǥ x,y,z)
        Vector3 adjCenter = baseCenter + hit.normal;          // �� ���� �ٱ������� ��Ȯ�� �� ĭ �̵�
        return Vector3Int.RoundToInt(adjCenter);
    }
}

