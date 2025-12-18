using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;
using TMPro;


public class CraftingPanel : MonoBehaviour
{
    public Inventory inventory;
    public List<CraftingRecipe> recipeList;
    public GameObject root;
    public TMP_Text plannedText;
    public Button clearButton;
    public Button craftButton;
    public TMP_Text hintText;

    readonly Dictionary<ItemType, int> planned = new();

    bool isOpen;

    // Start is called before the first frame update
    void Start()
    {
        SetOpen(false);
        craftButton.onClick.AddListener(DoCraft);
        clearButton.onClick.AddListener(ClearPlanned);
        RefreshplannedUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            SetOpen(!isOpen);
    }

    public void SetOpen(bool open)
    {
        isOpen = open;
        if (root)
            root.SetActive(open);
        if (!open)
            ClearPlanned();
    }

    public void Addplanned(ItemType type, int count = 1)
    {
        if (!planned.ContainsKey(type))
            planned[type] = 0;
        planned[type] += count;

        RefreshplannedUI();
        SetHint($"{type} x {count} 추가 완료");
    }

    public void ClearPlanned()
    {
        planned.Clear();
        RefreshplannedUI();
        SetHint("초기화 완료");
    }

    void RefreshplannedUI()
    {
        if (!plannedText)
            return;

        if (planned.Count == 0)
        {
            plannedText.text = "우클릭으로 재료를 추가하세요";
            return;
        }
        var sb = new StringBuilder();

        foreach (var item in planned)
            sb.AppendLine($"{item.Key} x {item.Value}");
        plannedText.text = sb.ToString();
    }

    void SetHint(string msg)
    {
        if (hintText)
            hintText.text = msg;
    }

    void DoCraft()
    {
        if (planned.Count == 0)
        {
            SetHint("재료가 부족합니다.");
            return;
        }

        foreach (var plannedItem in planned)
        {
            if (inventory.GetCount(plannedItem.Key) < plannedItem.Value)
            {
                SetHint($"{plannedItem.Key} 가 부족한니다");
                return;
            }

            var matchedProdnct = FindMatch(planned);
            if (matchedProdnct == null)
            {
                SetHint("알맞는 레시피가 없습니다.");
                return;
            }

            foreach (var itemForConsume in planned)
                inventory.Consume(itemForConsume.Key, itemForConsume.Value);

            foreach (var p in matchedProdnct.outputs)
                inventory.Add(p.type, p.count);

            ClearPlanned();

            SetHint($"조합 완료 : {matchedProdnct.displayName}");
        }
    }

    CraftingRecipe FindMatch(Dictionary<ItemType, int> planned)
    {
        foreach (var recipe in recipeList)
        {
            // 필요한 재료를 충분히 갖췄는지
            bool ok = true;
            foreach (var ing in recipe.inputs)
            {
                if (!planned.TryGetValue(ing.type, out int have) || have != ing.count)
                {
                    ok = false;
                    break;
                }
            }

            if (ok)
            {
                return recipe;
            }
        }
        return null;
    }
}
