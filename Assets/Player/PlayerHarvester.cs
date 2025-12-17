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

    void Start()
    {
        // 1. 게임이 시작되자마자 현재 나의 Y 좌표를 딱 한 번 저장합니다.
        startY = transform.position.y;
        Debug.Log("시작 높이가 저장되었습니다: " + startY);
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

        
        // 2. 현재 내 높이와 시작 높이의 차이를 계산합니다.
        // 시작 높이(예: 10) - 현재 높이(예: -10) = 판 깊이(20)
        float currentDepth = startY - transform.position.y;

        // 3. 만약 설정한 깊이(targetDepth)보다 더 깊이 내려갔다면 씬 이동!
        if (currentDepth >= targetDepth)
        {
            Debug.Log("목표 깊이에 도달했습니다! 씬 이동을 시작합니다.");
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

