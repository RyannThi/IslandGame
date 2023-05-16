using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerInventory : MonoBehaviour
{
    private ControlKeys ck; // usado pra verificação de input

    private GameObject invGroup; // usado pra fade in/out do inventario

    public Sprite blankImage;
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

    private int selectedItem = 0; // índice do slot atualmente selecionado

    private Coroutine positioningCoroutine; // referência para a corrotina de posicionamento dos objetos de UI

    private bool isOpen; // flag para indicar se o inventário está aberto

    private void Awake()
    {
        ck = new ControlKeys();
    }
    private void OnEnable()
    {
        ck.Enable();
    }
    private void OnDisable()
    {
        ck.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
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
        ChangeSlotHighlight(selectedItem);
        ResetInventory();
    }

    // Update is called once per frame
    void Update()
    {
        if (ck.Player.Jump.WasPressedThisFrame())
        {
            AddItem("Coin");
        }

        if (ck.Player.Inventory.WasPressedThisFrame())
        {
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
            selectedItem -= ((int)ck.Player.ForwardBack.ReadValue<float>() * 3);
            selectedItem = (int)Wrap(selectedItem, 0, 12);
            ChangeSlotHighlight(selectedItem);
        }

        if (ck.Player.LeftRight.WasPressedThisFrame() && isOpen)
        {
            selectedItem += (int)ck.Player.LeftRight.ReadValue<float>();
            selectedItem = (int)Wrap(selectedItem, 0, 12);
            ChangeSlotHighlight(selectedItem);
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

        invBackTop.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        invBackTop.GetComponent<RectTransform>().localScale = Vector2.zero;

        invBackBottom.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        invBackBottom.GetComponent<RectTransform>().localScale = Vector2.zero;

        invLabelHeaderRect.anchoredPosition = Vector2.zero;
        invLabelHeaderRect.localScale = Vector2.zero;

        invLabelName.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        invLabelName.GetComponent<RectTransform>().localScale = Vector2.zero;

        invLabelDesc.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        invLabelDesc.GetComponent<RectTransform>().localScale = Vector2.zero;
    }
    
    private void AddItem(string _itemName)
    {
        for (int i = 0; i <= invItemInfo.Length; i++)
        {
            if (_itemName == invItemInfo[i].name)
            {
                invSlotsItems.Add(new InvItemInfo(invItemInfo[i].name, invItemInfo[i].image, invItemInfo[i].desc));
                //invSlotsItems[i].name  = invItemInfo[i].name;
                //invSlotsItems[i].image = invItemInfo[i].image;
                //invSlotsItems[i].desc = invItemInfo[i].desc;
            }
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
        for (int i = 0; invBackTop.GetComponent<RectTransform>().anchoredPosition.y < 900; i++)
        {
            invGroup.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(invGroup.GetComponent<CanvasGroup>().alpha, 1, lerpTime);

            for (int j = 0; j < 12; j++)
            {
                invSlots[j].GetComponent<RectTransform>().anchoredPosition = Vector2.SmoothDamp(invSlots[j].GetComponent<RectTransform>().anchoredPosition, invSlotsPosition[j], ref invSlotsPositionRefs[j], 0.15f);
                invSlots[j].GetComponent<RectTransform>().localScale = Vector2.SmoothDamp(invSlots[j].GetComponent<RectTransform>().localScale, new Vector2(1, 1), ref invSlotsScaleRefs[j], 0.15f + (j * 0.05f));
            }

            invBackTop.GetComponent<RectTransform>().anchoredPosition = Vector2.SmoothDamp(invBackTop.GetComponent<RectTransform>().anchoredPosition, new Vector2(-225, 900), ref invBackTopRef, 0.15f);
            invBackTop.GetComponent<RectTransform>().localScale = Vector2.SmoothDamp(invBackTop.GetComponent<RectTransform>().localScale, new Vector2(1, 1), ref invBackTopScaleRef, 0.2f);

            invBackBottom.GetComponent<RectTransform>().anchoredPosition = Vector2.SmoothDamp(invBackBottom.GetComponent<RectTransform>().anchoredPosition, new Vector2(-225, 400), ref invBackBottomRef, 0.15f);
            invBackBottom.GetComponent<RectTransform>().localScale = Vector2.SmoothDamp(invBackBottom.GetComponent<RectTransform>().localScale, new Vector2(1, 1), ref invBackBottomScaleRef, 0.2f);

            invLabelHeaderRect.anchoredPosition = Vector2.SmoothDamp(invLabelHeaderRect.anchoredPosition, new Vector2(-225, 900), ref invLabelHeaderRef, 0.2f);
            invLabelHeaderRect.localScale = Vector2.SmoothDamp(invLabelHeaderRect.localScale, new Vector2(1, 1), ref invLabelHeaderScaleRef, 0.15f);

            invLabelName.GetComponent<RectTransform>().anchoredPosition = Vector2.SmoothDamp(invLabelName.GetComponent<RectTransform>().anchoredPosition, new Vector2(-225, 790), ref invLabelNameRef, 0.15f);
            invLabelName.GetComponent<RectTransform>().localScale = Vector2.SmoothDamp(invLabelName.GetComponent<RectTransform>().localScale, new Vector2(1, 1), ref invLabelNameScaleRef, 0.15f);

            invLabelDesc.GetComponent<RectTransform>().anchoredPosition = Vector2.SmoothDamp(invLabelDesc.GetComponent<RectTransform>().anchoredPosition, new Vector2(-225, 660), ref invLabelDescRef, 0.15f);
            invLabelDesc.GetComponent<RectTransform>().localScale = Vector2.SmoothDamp(invLabelDesc.GetComponent<RectTransform>().localScale, new Vector2(1, 1), ref invLabelDescScaleRef, 0.15f);

            yield return null;
        }
    }

    private IEnumerator MoveOut()
    {
        float lerpTime = 0.1f;
        for (int i = 0; invBackTop.GetComponent<RectTransform>().anchoredPosition.x < 0; i++)
        {
            invGroup.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(invGroup.GetComponent<CanvasGroup>().alpha, 0, lerpTime);

            for (int j = 0; j < 12; j++)
            {
                invSlots[j].GetComponent<RectTransform>().anchoredPosition = Vector2.SmoothDamp(invSlots[j].GetComponent<RectTransform>().anchoredPosition, Vector2.zero, ref invSlotsPositionRefs[j], 0.15f);
                invSlots[j].GetComponent<RectTransform>().localScale = Vector2.SmoothDamp(invSlots[j].GetComponent<RectTransform>().localScale, Vector2.zero, ref invSlotsScaleRefs[j], 0.15f);
            }

            invBackTop.GetComponent<RectTransform>().anchoredPosition = Vector2.SmoothDamp(invBackTop.GetComponent<RectTransform>().anchoredPosition, Vector2.zero, ref invBackTopRef, 0.15f);
            invBackTop.GetComponent<RectTransform>().localScale = Vector2.SmoothDamp(invBackTop.GetComponent<RectTransform>().localScale, Vector2.zero, ref invBackTopScaleRef, 0.15f);

            invBackBottom.GetComponent<RectTransform>().anchoredPosition = Vector2.SmoothDamp(invBackBottom.GetComponent<RectTransform>().anchoredPosition, Vector2.zero, ref invBackBottomRef, 0.15f);
            invBackBottom.GetComponent<RectTransform>().localScale = Vector2.SmoothDamp(invBackBottom.GetComponent<RectTransform>().localScale, Vector2.zero, ref invBackBottomScaleRef, 0.15f);

            invLabelHeaderRect.anchoredPosition = Vector2.SmoothDamp(invLabelHeaderRect.anchoredPosition, Vector2.zero, ref invLabelHeaderRef, 0.15f);
            invLabelHeaderRect.localScale = Vector2.SmoothDamp(invLabelHeaderRect.localScale, Vector2.zero, ref invLabelHeaderScaleRef, 0.15f);

            invLabelName.GetComponent<RectTransform>().anchoredPosition = Vector2.SmoothDamp(invLabelName.GetComponent<RectTransform>().anchoredPosition, Vector2.zero, ref invLabelNameRef, 0.15f);
            invLabelName.GetComponent<RectTransform>().localScale = Vector2.SmoothDamp(invLabelName.GetComponent<RectTransform>().localScale, Vector2.zero, ref invLabelNameScaleRef, 0.15f);

            invLabelDesc.GetComponent<RectTransform>().anchoredPosition = Vector2.SmoothDamp(invLabelDesc.GetComponent<RectTransform>().anchoredPosition, Vector2.zero, ref invLabelDescRef, 0.15f);
            invLabelDesc.GetComponent<RectTransform>().localScale = Vector2.SmoothDamp(invLabelDesc.GetComponent<RectTransform>().localScale, Vector2.zero, ref invLabelDescScaleRef, 0.15f);

            yield return null;
        }
    }

    private void ChangeSlotHighlight(int _slotNumber)
    {
        for (int i = 0; i < 12; i++)
        {
            if (i == _slotNumber)
            {
                invSlots[i].GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1);
            }
            else
            {
                invSlots[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
        }
        print(_slotNumber);
        if (invSlotsItems[_slotNumber] != null)
        {
            invSlotsChild[_slotNumber].GetComponent<Image>().sprite = invSlotsItems[_slotNumber].image;
            invLabelName.GetComponent<TextMeshProUGUI>().text = invSlotsItems[_slotNumber].name;
            invLabelDesc.GetComponent<TextMeshProUGUI>().text = invSlotsItems[_slotNumber].desc;
        } else
        {
            invSlotsChild[_slotNumber].GetComponent<Image>().sprite = blankImage;
            invLabelName.GetComponent<TextMeshProUGUI>().text = "Nothing";
            invLabelDesc.GetComponent<TextMeshProUGUI>().text = "Nothing here.";
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
