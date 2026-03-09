using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class craftingUI : MonoBehaviour
{
    public static craftingUI Instance;

    [Header("Icons")]
    public List<ItemIcon> itemIcons = new List<ItemIcon>();

    [Header("Panels")]
    public List<GameObject> itemPanels = new List<GameObject>();

    [Header("Craft State Panels")]
    public List<GameObject> craftingRequiredPanels = new List<GameObject>();
    public List<GameObject> craftingEquippedPanels = new List<GameObject>();

    [Header("Open Animation")]
    [SerializeField] private RectTransform itemIconContainer;
    [SerializeField] private float slideInOffsetX = -300f;
    [SerializeField] private float slideInSpeed = 10f;

    private int currentIndex = -1;
    private readonly List<bool> craftedStates = new List<bool>();

    private Vector2 itemIconTargetPosition;
    private bool isPlayingOpenAnimation;
    private bool isPlayingCloseAnimation;

    void Awake()
    {
        Instance = this;

        if (itemIconContainer == null)
        {
            itemIconContainer = transform as RectTransform;
        }

        if (itemIconContainer != null)
        {
            itemIconTargetPosition = itemIconContainer.anchoredPosition;
        }
    }

    void Start()
    {
        InitCraftStates();
        HideAllUI();
    }

    void OnEnable()
    {
        currentIndex = -1;
        HideAllUI();
        StartOpenAnimation();
    }

    void Update()
    {
        PlayOpenAnimation();
        PlayCloseAnimation();
    }

    void InitCraftStates()
    {
        craftedStates.Clear();

        for (int i = 0; i < itemIcons.Count; i++)
        {
            craftedStates.Add(false);
        }
    }

    public void SelectItem(int index)
    {
        if (index < 0 || index >= itemIcons.Count)
            return;

        currentIndex = index;
        RefreshUI();
    }

    void HideAllUI()
    {
        foreach (ItemIcon icon in itemIcons)
        {
            if (icon != null)
            {
                icon.hovering = false;
                icon.SetPressed(false);
            }
        }

        foreach (GameObject panel in itemPanels)
        {
            if (panel != null)
                panel.SetActive(false);
        }

        foreach (GameObject panel in craftingRequiredPanels)
        {
            if (panel != null)
                panel.SetActive(false);
        }

        foreach (GameObject panel in craftingEquippedPanels)
        {
            if (panel != null)
                panel.SetActive(false);
        }
    }

    void RefreshUI()
    {
        for (int i = 0; i < itemIcons.Count; i++)
        {
            if (itemIcons[i] != null)
                itemIcons[i].SetPressed(i == currentIndex);
        }

        for (int i = 0; i < itemPanels.Count; i++)
        {
            if (itemPanels[i] != null)
                itemPanels[i].SetActive(i == currentIndex);
        }

        UpdateRequiredDisplay();
        UpdateCraftPanels();
    }

    public void RefreshCurrentUI()
    {
        if (currentIndex < 0)
        {
            HideAllUI();
            return;
        }

        RefreshUI();
    }

    void UpdateCraftPanels()
    {
        if (currentIndex < 0 || currentIndex >= craftedStates.Count)
        {
            return;
        }

        bool crafted = craftedStates[currentIndex];

        for (int i = 0; i < craftingRequiredPanels.Count; i++)
        {
            if (craftingRequiredPanels[i] != null)
                craftingRequiredPanels[i].SetActive(i == currentIndex && !crafted);
        }

        for (int i = 0; i < craftingEquippedPanels.Count; i++)
        {
            if (craftingEquippedPanels[i] != null)
                craftingEquippedPanels[i].SetActive(i == currentIndex && crafted);
        }
    }

    public void SetCrafted(int index)
    {
        if (index < 0 || index >= craftedStates.Count)
            return;

        craftedStates[index] = true;

        RefreshCurrentUI();
    }

    public int GetCurrentIndex()
    {
        return currentIndex;
    }

    // ===============================
    // REQUIRED ITEM DISPLAY
    // ===============================

    void UpdateRequiredDisplay()
    {
        if (currentIndex < 0)
            return;

        if (CraftingSystem.Instance == null)
            return;

        if (currentIndex >= CraftingSystem.Instance.craftingRecipes.Count)
            return;

        if (currentIndex >= itemPanels.Count)
            return;

        CraftingRec recipe = CraftingSystem.Instance.craftingRecipes[currentIndex];

        if (recipe == null)
            return;

        Transform requiredRoot = itemPanels[currentIndex].transform.Find("Crafting Required/Required");

        if (requiredRoot == null)
        {
            Debug.LogWarning("Required slot root not found.");
            return;
        }

        for (int i = 0; i < requiredRoot.childCount; i++)
        {
            Transform slot = requiredRoot.GetChild(i);

            Image icon = slot.Find("Icon")?.GetComponent<Image>();
            TextMeshProUGUI txt = slot.Find("AmountTXT")?.GetComponent<TextMeshProUGUI>();

            if (i < recipe.requiredItems.Count)
            {
                Ingredient ingredient = recipe.requiredItems[i];

                int owned = GetInventoryItemAmount(ingredient.item);

                if (icon != null)
                {
                    icon.enabled = true;
                    icon.sprite = ingredient.item.icon;
                }

                if (txt != null)
                {
                    txt.text = owned + "/" + ingredient.amount;
                    txt.color = owned >= ingredient.amount ? Color.green : Color.red;
                }
            }
            else
            {
                if (icon != null)
                    icon.enabled = false;

                if (txt != null)
                    txt.text = "";
            }
        }
    }

    int GetInventoryItemAmount(ItemObject item)
    {
        if (item == null || InventorySystem.Instance == null)
            return 0;

        int total = 0;

        foreach (Slots slot in InventorySystem.Instance.allSlots)
        {
            if (slot.HasItem() && slot.GetItem() == item)
            {
                total += slot.GetAmount();
            }
        }

        return total;
    }

    // ===============================
    // ANIMATION
    // ===============================

    public void CloseWithAnimation()
    {
        if (itemIconContainer == null)
        {
            gameObject.SetActive(false);
            return;
        }

        isPlayingOpenAnimation = false;
        isPlayingCloseAnimation = true;
    }

    void StartOpenAnimation()
    {
        if (itemIconContainer == null)
            return;

        itemIconContainer.anchoredPosition = itemIconTargetPosition + new Vector2(slideInOffsetX, 0);

        isPlayingOpenAnimation = true;
        isPlayingCloseAnimation = false;
    }

    void PlayOpenAnimation()
    {
        if (!isPlayingOpenAnimation || itemIconContainer == null)
            return;

        itemIconContainer.anchoredPosition = Vector2.Lerp(
            itemIconContainer.anchoredPosition,
            itemIconTargetPosition,
            Time.deltaTime * slideInSpeed
        );

        if (Vector2.Distance(itemIconContainer.anchoredPosition, itemIconTargetPosition) < 0.5f)
        {
            itemIconContainer.anchoredPosition = itemIconTargetPosition;
            isPlayingOpenAnimation = false;
        }
    }

    void PlayCloseAnimation()
    {
        if (!isPlayingCloseAnimation || itemIconContainer == null)
            return;

        Vector2 closeTarget = itemIconTargetPosition + new Vector2(slideInOffsetX, 0);

        itemIconContainer.anchoredPosition = Vector2.Lerp(
            itemIconContainer.anchoredPosition,
            closeTarget,
            Time.deltaTime * slideInSpeed
        );

        if (Vector2.Distance(itemIconContainer.anchoredPosition, closeTarget) < 0.5f)
        {
            itemIconContainer.anchoredPosition = closeTarget;
            isPlayingCloseAnimation = false;

            gameObject.SetActive(false);
        }
    }
}