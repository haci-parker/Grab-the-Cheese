using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    
    [HideInInspector] public Transform parentAfterDrag;
    private Transform originalParent;
    private Vector3 originalLocalPos;

    [Header("Drag-Drop Ayarları")]
    [Tooltip("Bu objeyi tanımlayan etiket (örn: 'destek', 'tahta')")]
    public string itemTag = "";

    [Tooltip("Bu obje başarılı drop edildiğinde aktif olacak GameObject")]
    public GameObject activateOnDrop;

    [Tooltip("Bu objenin sürüklenebilmesi için önce tamamlanması gereken DraggableItem (sıralama için)")]
    public DraggableItem prerequisite;

    /// <summary>Bu obje başarıyla drop edildi mi?</summary>
    [HideInInspector] public bool isDropped = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        
        // CanvasGroup yoksa otomatik eklenir (Raycast kontrolü yapmamızı sağlıyor)
        canvasGroup = GetComponent<CanvasGroup>(); 
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Eğer önkoşul varsa ve henüz tamamlanmadıysa sürüklemeye izin verme
        if (prerequisite != null && !prerequisite.isDropped)
        {
            Debug.Log($"[DraggableItem] '{gameObject.name}' henüz sürüklenemez! Önce '{prerequisite.gameObject.name}' yerleştirilmeli.");
            eventData.pointerDrag = null; // Sürüklemeyi iptal et
            return;
        }

        // Zaten drop edildiyse tekrar sürüklemeye izin vermeyelim
        if (isDropped)
        {
            eventData.pointerDrag = null;
            return;
        }

        // Sürükleme başlarken eski konumunu ve babasını (kutusunu) hafızaya al
        originalParent = transform.parent;
        originalLocalPos = rectTransform.localPosition;
        parentAfterDrag = transform.parent;
        
        // Obje sürüklenirken diğer her şeyin en üstünde gözükmesi için en Dış Kanvas'a alınır
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();

        // **ÖNEMLİ:** Farenin arkasındaki hedef Yuvayı (DropZone) algılayabilmesi için...
        // ...bu objenin görünmez duvarını(mermi gibi çarpan tıklamaları) 1 saniyeliğine kapatıyoruz.
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Ekranın çözünürlük ölçeğine göre farenin hareketine paralel hareket et
        rectTransform.anchoredPosition += eventData.delta / GetCanvasScale();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Fareyi bıraktık. Obje gideceği yuvaya (Bura eski yeri veya yeni DropZone olabilir) geri yerleştiriliyor...
        transform.SetParent(parentAfterDrag);
        
        // Eğer hiçbir hedefe bırakılmadıysa (örneğin ekranın kenarına bırakıldıysa)
        if (parentAfterDrag == originalParent)
        {
            rectTransform.localPosition = originalLocalPos; // Eski oturduğu kordonita geri fırlat (Zıplama Efekti)
        }
        else
        {
            // Eğer yeni bir DropZone hedefine bırakıldıysa (parentAfterDrag değiştiyse)
            // Tam o yuvanın merkezine şak diye yapıştırılsın! (Magnet Etkisi)
            rectTransform.localPosition = Vector3.zero;

            // Başarılı drop!
            isDropped = true;

            // activateOnDrop atanmışsa aktif et
            if (activateOnDrop != null)
            {
                activateOnDrop.SetActive(true);
                Debug.Log($"[DraggableItem] '{gameObject.name}' drop edildi → '{activateOnDrop.name}' aktif edildi!");
            }

            // Drop edildikten sonra tekrar sürüklemeyi devre dışı bırak
            canvasGroup.blocksRaycasts = false;
            return;
        }

        // Tıklamaları tekrar aç ki obje tekrar sürüklemeye müsait olsun
        canvasGroup.blocksRaycasts = true;
    }
    
    // Olası farklı ekran çözünürlüklerinde sürükleme hızının bozulmaması için
    private float GetCanvasScale()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        return canvas != null ? canvas.scaleFactor : 1f;
    }
}
