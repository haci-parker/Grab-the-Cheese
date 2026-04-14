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

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        
        // CanvasGroup yoksa otomatik eklenir (Raycast kontrolü yapmamızı sağlıyor)
        canvasGroup = GetComponent<CanvasGroup>(); 
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
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
