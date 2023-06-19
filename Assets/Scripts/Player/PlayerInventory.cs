using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEditor;

public class PlayerInventory : MonoBehaviour
{
    private ControlKeys ck; // usado pra verificação de input
    public static PlayerInventory instance;

    private GameObject invGroup; // usado pra fade in/out do inventario

    public Sprite blankImage;
    public AudioSource audioSource;
    public AudioClip itemGet;
    public AudioClip buttonMove;
    public AudioClip buttonConfirm;
    public AudioClip buttonCancel;
    public AudioClip inventoryOpen;
    public List<GameObject> invSlots; // os slots do inventário em lista
    public List<GameObject> invSlotsChild = new List<GameObject>(12); // os overlays de item do inventário em lista

    [System.Serializable]
    public class InvItemInfo // classe que salva informação de cada item
    {
        public string name;
        public Sprite image;

        [SerializeField, TextArea]
        public string desc;

        public InvItemInfo(string name, Sprite image, string desc)
        {
            this.name = name;
            this.image = image;
            this.desc = desc;
        }
    }

    public InvItemInfo[] invItemInfo; // defina os itens pelo inspetor. Use o mesmo nome do inspetor ao adicionar um item ao player

    public List<InvItemInfo> invSlotsItems = new List<InvItemInfo>(12); // os itens do inventário em lista

    [SerializeField]
    public List<Vector2> invSlotsPosition; // posições dos slots pelo inspetor

    private Vector2[] invSlotsPositionRefs = new Vector2[12];
    private Vector2[] invSlotsScaleRefs = new Vector2[12];

    // referências para os objetos de UI do inventário
    private GameObject invLabelHeader;
    private GameObject invLabelName;
    private GameObject invLabelDesc;

    private GameObject invBackTop;
    private GameObject invBackBottom;

    private RectTransform invLabelHeaderRect;
    private RectTransform invLabelNameRect;
    private RectTransform invLabelDescRect;

    private RectTransform invBackTopRect;
    private RectTransform invBackBottomRect;

    private RectTransform invPopup;
    private CanvasGroup invPopupCanvasGroup;
    private Image invPopupIcon;
    private TextMeshProUGUI invPopupText;

    // referências para as posições e escalas dos objetos de UI do inventário
    private Vector2 invLabelHeaderRef = Vector2.zero;
    private Vector2 invLabelNameRef   = Vector2.zero;
    private Vector2 invLabelDescRef   = Vector2.zero;
    private Vector2 invBackTopRef     = Vector2.zero;
    private Vector2 invBackBottomRef  = Vector2.zero;

    private Vector2 invLabelHeaderScaleRef = Vector2.zero;
    private Vector2 invLabelNameScaleRef = Vector2.zero;
    private Vector2 invLabelDescScaleRef = Vector2.zero;
    private Vector2 invBackTopScaleRef = Vector2.zero;
    private Vector2 invBackBottomScaleRef = Vector2.zero;
    
    private Vector2 invPopupRef = Vector2.zero;

    public int selectedItem = 0; // índice do slot atualmente selecionado

    private Coroutine positioningCoroutine; // referência para a corrotina de posicionamento dos objetos de UI

    private Coroutine popupCoroutine;

    public bool isOpen; // flag para indicar se o inventário está aberto
    public bool hasClicked = false; // flag para indicar se o mouse foi pressionado

    private void Awake() { DontDestroyOnLoad(gameObject); ck = new ControlKeys(); }
    private void OnEnable() { ck.Enable(); }
    private void OnDisable() { ck.Disable(); }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        // encontra os objetos de UI do inventário
        invGroup = GetChildGameObject(gameObject, "InvGroup");

        invLabelHeader = GetChildGameObject(gameObject, "InvLabel_Header");
        invLabelName   = GetChildGameObject(gameObject, "InvLabel_Name");
        invLabelDesc   = GetChildGameObject(gameObject, "InvLabel_Desc");

        invBackTop    = GetChildGameObject(gameObject, "InvBack_Top");
        invBackBottom = GetChildGameObject(gameObject, "InvBack_Bottom");

        invLabelHeaderRect = invLabelHeader.GetComponent<RectTransform>();
        invLabelNameRect = invLabelName.GetComponent<RectTransform>();
        invLabelDescRect = invLabelDesc.GetComponent<RectTransform>();

        invBackTopRect = invBackTop.GetComponent<RectTransform>();
        invBackBottomRect = invBackBottom.GetComponent<RectTransform>();

        invPopup = GetChildGameObject(gameObject, "InvPopup").GetComponent<RectTransform>();
        invPopupCanvasGroup = invPopup.gameObject.GetComponent<CanvasGroup>();
        invPopupIcon = GetChildGameObject(gameObject, "InvNewItem").GetComponent<Image>();
        invPopupText = GetChildGameObject(gameObject, "InvNewItemText").GetComponent<TextMeshProUGUI>();

        for (int i = 0; i < 12; i++)
        {
            if (invSlots.Count > i)
            {
                invSlotsChild.Add(invSlots[i].gameObject.transform.GetChild(0).gameObject);
            }
            
            invSlotsPositionRefs[i] = Vector2.zero;
            invSlotsScaleRefs[i]    = Vector2.zero;
        }

        positioningCoroutine = StartCoroutine(EmptyCoroutine());
        popupCoroutine = StartCoroutine(EmptyCoroutine());
        ChangeSlotHighlight(selectedItem);
        ResetInventory();
    }

    // Update is called once per frame
    void Update()
    {
        if (ck.Player.Inventory.WasPressedThisFrame())
        {
            audioSource.PlayOneShot(inventoryOpen);
            for (int i = 11; i > -1; i--)
            {
                ChangeSlotHighlight(i);
            }
            selectedItem = 0;
            isOpen = !isOpen;
            if (positioningCoroutine != null)
            {
                StopCoroutine(positioningCoroutine);
            }
            if (isOpen)
            {
                positioningCoroutine = StartCoroutine(MoveIn());
            } else
            {
                positioningCoroutine = StartCoroutine(MoveOut());
            }
        }

        if (ck.Player.ForwardBack.WasPressedThisFrame() && isOpen)
        {
            selectedItem = (int)Wrap(selectedItem - (int)ck.Player.ForwardBack.ReadValue<float>() * 3, 0, 12);
            ChangeSlotHighlight(selectedItem);
            audioSource.PlayOneShot(buttonMove);
        }

        if (ck.Player.LeftRight.WasPressedThisFrame() && isOpen)
        {
            selectedItem = (int)Wrap(selectedItem + (int)ck.Player.LeftRight.ReadValue<float>(), 0, 12);
            ChangeSlotHighlight(selectedItem);
            audioSource.PlayOneShot(buttonMove);
        }

        if ((ck.Player.Confirm.WasPressedThisFrame() || hasClicked) && isOpen)
        {
            hasClicked = false;
            
            if (invSlotsChild[selectedItem].activeSelf == true)
            {
                audioSource.PlayOneShot(buttonConfirm);
                if (invSlotsItems[selectedItem].name != "Fire Rune" || invSlotsItems[selectedItem].name != "Ice Rune")
                {
                    if (invSlotsChild[selectedItem].TryGetComponent<Iitem>(out Iitem item))
                    {
                        item.UseItem(PlayerCharControl.instance.gameObject);
                        switch (invSlotsItems[selectedItem].name)
                        {
                            case "Speed Potion":

                                Destroy(invSlotsChild[selectedItem].GetComponent<SpeedPotion>());
                                break;

                            case "Damage Potion":

                                Destroy(invSlotsChild[selectedItem].GetComponent<DamagePotion>());
                                break;

                            case "Resistance Potion":

                                Destroy(invSlotsChild[selectedItem].GetComponent<ResistancePotion>());
                                break;

                            case "Health Potion":

                                Destroy(invSlotsChild[selectedItem].GetComponent<ResistancePotion>());
                                break;

                            case "Heavy Snowball":

                                Destroy(invSlotsChild[selectedItem].GetComponent<HeavySnowball>());
                                break;
                        }

                    }
                }
                
                isOpen = !isOpen;
                if (invSlotsItems[selectedItem].name != "Fire Rune" || invSlotsItems[selectedItem].name != "Ice Rune")
                {
                    invSlotsItems.Remove(invSlotsItems[selectedItem]);
                }
                StopCoroutine(positioningCoroutine);
                positioningCoroutine = StartCoroutine(MoveOut());
            } else
            {
                audioSource.PlayOneShot(buttonCancel);
            }
        }
    }

    private void ResetInventory()
    {
        invGroup.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(invGroup.GetComponent<CanvasGroup>().alpha, 0, 1);

        for (int j = 0; j < 12; j++)
        {
            invSlots[j].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            invSlots[j].GetComponent<RectTransform>().localScale = Vector2.zero;
        }

        invBackTopRect.anchoredPosition = Vector2.zero;
        invBackTopRect.localScale = Vector2.zero;

        invBackBottomRect.anchoredPosition = Vector2.zero;
        invBackBottomRect.localScale = Vector2.zero;

        invLabelHeaderRect.anchoredPosition = Vector2.zero;
        invLabelHeaderRect.localScale = Vector2.zero;

        invLabelNameRect.anchoredPosition = Vector2.zero;
        invLabelNameRect.localScale = Vector2.zero;

        invLabelDescRect.anchoredPosition = Vector2.zero;
        invLabelDescRect.localScale = Vector2.zero;
    }
    
    public void AddItem(string _itemName, GameObject _caller = null)
    {
        if (invSlotsItems.Count + 1 >= 13)
        {
            if (invSlotsItems[invSlotsItems.Count - 1].name != "Ice Rune" || invSlotsItems[invSlotsItems.Count - 1].name != "Fire Rune")
            {
                invSlotsItems.RemoveAt(invSlotsItems.Count - 1);
            }
            else if (invSlotsItems[invSlotsItems.Count - 2].name != "Ice Rune" || invSlotsItems[invSlotsItems.Count - 2].name != "Fire Rune")
            {
                invSlotsItems.RemoveAt(invSlotsItems.Count - 2);
            }
            else if (invSlotsItems[invSlotsItems.Count - 3].name != "Ice Rune" || invSlotsItems[invSlotsItems.Count - 3].name != "Fire Rune")
            {
                invSlotsItems.RemoveAt(invSlotsItems.Count - 3);
            }

        }

        audioSource.PlayOneShot(itemGet);

        // Percorre todos os itens de inventário disponíveis
        foreach (var invItem in invItemInfo)
        {
            // Verifica se o nome do item fornecido coincide com o nome do item atual
            if (_itemName == invItem.name)
            {
                // Cria uma nova instância de InvItemInfo com base nas informações do item atual
                InvItemInfo inv = new InvItemInfo(invItem.name, invItem.image, invItem.desc);

                // Adiciona o novo item à lista de itens de inventário
                invSlotsItems.Add(inv);
            }
        }

        StopCoroutine(popupCoroutine);
        popupCoroutine = StartCoroutine(PopupItem(_itemName));

        // Define a imagem do slot de inventário correspondente ao último item adicionado
        invSlotsChild[invSlotsItems.Count - 1].GetComponent<Image>().sprite = invSlotsItems[invSlotsItems.Count - 1].image;

        // Ativa o objeto do slot de inventário correspondente ao último item adicionado
        invSlotsChild[invSlotsItems.Count - 1].SetActive(true);

        switch (_itemName)
        {
            case "Speed Potion":
                invSlotsChild[invSlotsItems.Count - 1].AddComponent<SpeedPotion>();
                break;

            case "Damage Potion":
                invSlotsChild[invSlotsItems.Count - 1].AddComponent<DamagePotion>();
                break;

            case "Resistance Potion":
                invSlotsChild[invSlotsItems.Count - 1].AddComponent<ResistancePotion>();
                break;

            case "Health Potion":
                invSlotsChild[invSlotsItems.Count - 1].AddComponent<HealthPotion>();
                break;

            case "Heavy Snowball":
                invSlotsChild[invSlotsItems.Count - 1].AddComponent<HeavySnowball>();
                break;
        }

        if (_caller != null)
        {
            Destroy(_caller);
        }

        ChangeSlotHighlight(selectedItem);
    }

    private IEnumerator PopupItem(string _itemName)
    {
        invPopup.anchoredPosition = new Vector2(870, -450);
        invPopupCanvasGroup.alpha = 0;
        foreach (var invItem in invItemInfo)
        {
            if (_itemName == invItem.name)
            {
                invPopupIcon.sprite = invItem.image;
                invPopupText.text = "+1 " + invItem.name;
            }
        }

        while (invPopup.anchoredPosition.x >= 735.5f)
        {
            invPopupCanvasGroup.alpha = Mathf.Lerp(invPopupCanvasGroup.alpha, 1, 5f * Time.deltaTime);
            //invPopup.anchoredPosition = Vector2.SmoothDamp(invPopup.anchoredPosition, new Vector2(735, -450), ref invPopupRef, 0.005f * Time.deltaTime);
            var invPopupPos = invPopup.anchoredPosition;
            invPopupPos.x = Mathf.Lerp(invPopupPos.x, 735, 5f * Time.deltaTime);
            invPopup.anchoredPosition = invPopupPos;
            yield return null;
        }

        yield return new WaitForSeconds(1);

        while (invPopupCanvasGroup.alpha > 0)
        {
            invPopupCanvasGroup.alpha = Mathf.Lerp(invPopupCanvasGroup.alpha, 0, 4f * Time.deltaTime);
            yield return null;
        }
    }

    private void RemoveItem(string _itemName)
    {
        for (int i = 0; i <= invSlotsItems.Count; i++)
        {
            if (_itemName == invSlotsItems[i].name)
            {
                invSlotsItems.Remove(invSlotsItems[i]);
                break;
            }
        }
    }

    private IEnumerator EmptyCoroutine()
    {
        yield return null;
    }

    private IEnumerator MoveIn()
    {
        float lerpTime = 0.1f;
        for (int i = 0; invBackTopRect.anchoredPosition.y < 900; i++)
        {
            invGroup.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(invGroup.GetComponent<CanvasGroup>().alpha, 1, lerpTime);

            for (int j = 0; j < 12; j++)
            {
                invSlots[j].GetComponent<RectTransform>().anchoredPosition = Vector2.SmoothDamp(invSlots[j].GetComponent<RectTransform>().anchoredPosition, invSlotsPosition[j], ref invSlotsPositionRefs[j], 0.15f);
                invSlots[j].GetComponent<RectTransform>().localScale = Vector2.SmoothDamp(invSlots[j].GetComponent<RectTransform>().localScale, new Vector2(1, 1), ref invSlotsScaleRefs[j], 0.15f + (j * 0.05f));
            }

            invBackTopRect.anchoredPosition = Vector2.SmoothDamp(invBackTopRect.anchoredPosition, new Vector2(-225, 900), ref invBackTopRef, 0.15f);
            invBackTopRect.localScale = Vector2.SmoothDamp(invBackTopRect.localScale, new Vector2(1, 1), ref invBackTopScaleRef, 0.2f);

            invBackBottomRect.anchoredPosition = Vector2.SmoothDamp(invBackBottomRect.anchoredPosition, new Vector2(-225, 400), ref invBackBottomRef, 0.15f);
            invBackBottomRect.localScale = Vector2.SmoothDamp(invBackBottomRect.localScale, new Vector2(1, 1), ref invBackBottomScaleRef, 0.2f);

            invLabelHeaderRect.anchoredPosition = Vector2.SmoothDamp(invLabelHeaderRect.anchoredPosition, new Vector2(-225, 900), ref invLabelHeaderRef, 0.2f);
            invLabelHeaderRect.localScale = Vector2.SmoothDamp(invLabelHeaderRect.localScale, new Vector2(1, 1), ref invLabelHeaderScaleRef, 0.15f);

            invLabelNameRect.anchoredPosition = Vector2.SmoothDamp(invLabelNameRect.anchoredPosition, new Vector2(-225, 790), ref invLabelNameRef, 0.15f);
            invLabelNameRect.localScale = Vector2.SmoothDamp(invLabelNameRect.localScale, new Vector2(1, 1), ref invLabelNameScaleRef, 0.15f);

            invLabelDescRect.anchoredPosition = Vector2.SmoothDamp(invLabelDescRect.anchoredPosition, new Vector2(-225, 660), ref invLabelDescRef, 0.15f);
            invLabelDescRect.localScale = Vector2.SmoothDamp(invLabelDescRect.localScale, new Vector2(1, 1), ref invLabelDescScaleRef, 0.15f);

            yield return null;
        }
    }

    private IEnumerator MoveOut()
    {
        float lerpTime = 0.1f;
        for (int i = 0; invBackTopRect.anchoredPosition.x < 0; i++)
        {
            invGroup.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(invGroup.GetComponent<CanvasGroup>().alpha, 0, lerpTime);

            for (int j = 0; j < 12; j++)
            {
                invSlots[j].GetComponent<RectTransform>().anchoredPosition = Vector2.SmoothDamp(invSlots[j].GetComponent<RectTransform>().anchoredPosition, Vector2.zero, ref invSlotsPositionRefs[j], 0.15f);
                invSlots[j].GetComponent<RectTransform>().localScale = Vector2.SmoothDamp(invSlots[j].GetComponent<RectTransform>().localScale, Vector2.zero, ref invSlotsScaleRefs[j], 0.15f);
            }

            invBackTopRect.anchoredPosition = Vector2.SmoothDamp(invBackTopRect.anchoredPosition, Vector2.zero, ref invBackTopRef, 0.15f);
            invBackTopRect.localScale = Vector2.SmoothDamp(invBackTopRect.localScale, Vector2.zero, ref invBackTopScaleRef, 0.15f);

            invBackBottomRect.anchoredPosition = Vector2.SmoothDamp(invBackBottomRect.anchoredPosition, Vector2.zero, ref invBackBottomRef, 0.15f);
            invBackBottomRect.localScale = Vector2.SmoothDamp(invBackBottomRect.localScale, Vector2.zero, ref invBackBottomScaleRef, 0.15f);

            invLabelHeaderRect.anchoredPosition = Vector2.SmoothDamp(invLabelHeaderRect.anchoredPosition, Vector2.zero, ref invLabelHeaderRef, 0.15f);
            invLabelHeaderRect.localScale = Vector2.SmoothDamp(invLabelHeaderRect.localScale, Vector2.zero, ref invLabelHeaderScaleRef, 0.15f);

            invLabelNameRect.anchoredPosition = Vector2.SmoothDamp(invLabelNameRect.anchoredPosition, Vector2.zero, ref invLabelNameRef, 0.15f);
            invLabelNameRect.localScale = Vector2.SmoothDamp(invLabelNameRect.localScale, Vector2.zero, ref invLabelNameScaleRef, 0.15f);

            invLabelDescRect.anchoredPosition = Vector2.SmoothDamp(invLabelDescRect.anchoredPosition, Vector2.zero, ref invLabelDescRef, 0.15f);
            invLabelDescRect.localScale = Vector2.SmoothDamp(invLabelDescRect.localScale, Vector2.zero, ref invLabelDescScaleRef, 0.15f);

            yield return null;
        }
    }

    public void ChangeSlotHighlight(int _slotNumber)
    {
        // Itera através dos slots de inventário
        for (int i = 0; i < 12; i++)
        {
            // Verifica se o slot atual é o slot selecionado
            if (i == _slotNumber)
            {
                // Define a cor de destaque para o slot selecionado
                invSlots[i].GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1);
            }
            else
            {
                // Define a cor normal para os slots não selecionados
                invSlots[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
        }

        // Itera novamente através dos slots de inventário
        for (int i = 0; i < 12; i++)
        {
            // Verifica se há itens no slot selecionado e se o slot não está vazio
            if (invSlotsItems.Count > _slotNumber && (invSlotsItems[_slotNumber] != null))
            {
                // Define a imagem do slot de inventário correspondente ao item no slot selecionado
                invSlotsChild[_slotNumber].GetComponent<Image>().sprite = invSlotsItems[_slotNumber].image;

                // Ativa o objeto do slot de inventário correspondente ao item no slot selecionado
                invSlotsChild[_slotNumber].SetActive(true);

                // Define o tamanho desejado para o slot de inventário (100x100)
                invSlotsChild[_slotNumber].GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);

                // Verifica se o slot atual é o slot selecionado
                if (_slotNumber == i)
                {
                    // Atualiza o rótulo do nome e descrição do item no slot selecionado
                    invLabelName.GetComponent<TextMeshProUGUI>().text = invSlotsItems[_slotNumber].name;
                    invLabelDesc.GetComponent<TextMeshProUGUI>().text = invSlotsItems[_slotNumber].desc;
                }
            }
            else
            {
                // Verifica se o slot atual é o slot selecionado
                if (_slotNumber == i)
                {
                    // Define a imagem do slot como vazio
                    invSlotsChild[i].GetComponent<Image>().sprite = blankImage;

                    // Desativa o objeto do slot de inventário
                    invSlotsChild[i].SetActive(false);

                    // Define os rótulos de nome e descrição como "Nothing" e "Nothing here."
                    invLabelName.GetComponent<TextMeshProUGUI>().text = "Nothing";
                    invLabelDesc.GetComponent<TextMeshProUGUI>().text = "Nothing here.";

                } else if (i > invSlotsItems.Count) // Verifica se o slot está além do número de itens no inventário
                {
                    // Define a imagem do slot como vazio
                    invSlotsChild[i].GetComponent<Image>().sprite = blankImage;

                    // Desativa o objeto do slot de inventário
                    invSlotsChild[i].SetActive(false);
                }
            }
        }
    }

    private float Wrap(float _val, float _min, float _max)
    {
        _val = _val - (float)Mathf.Round((_val - _min) / (_max - _min)) * (_max - _min);
        if (_val < 0)
            _val = _val + _max - _min;
        return _val;
    }

    private GameObject GetChildGameObject(GameObject _fromGameObject, string _withName)
    {
        //Author: Isaac Dart, June-13.
        Transform[] ts = _fromGameObject.transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in ts) if (t.gameObject.name == _withName) return t.gameObject;
        return null;
    }
}
