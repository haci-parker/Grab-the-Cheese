using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // Image kontrolü için ekliyoruz

public class ItemDropZone : MonoBehaviour, IDropHandler
{
    // Biri tam üzerimde fare tıkını bıraktığında (Drop) bu fonksiyon alevlenir
    public void OnDrop(PointerEventData eventData)
    {
        // Sürüklenerek üstüme getirilen bir şey var mı diye kontrol et
        if (eventData.pointerDrag != null)
        {
            // Üstüme bırakılan sürüklenen nesnenin DraggableItem kodunu kontrol et
            DraggableItem draggedItem = eventData.pointerDrag.GetComponent<DraggableItem>();
            
            // Eğer varsa (Demek ki geçerli bir eşya bırakılmış)
            if (draggedItem != null)
            {
                // Sürüklenen eşyaya, "Senin yeni hedefin (yuvan) benim diyoruz."
                // Eşyanın kendi (DraggableItem.cs) içindeki kod bunu algılayıp 
                // otomatik merkezime yapışmasını sağlayacaktır.
                draggedItem.parentAfterDrag = transform;

                // --- YENİ EKLENEN KISIM ---
                // Hedef yuvanın (DropZone'un kendi) Image bileşenini alıp transparanlığını iptal ediyoruz.
                Image myImage = GetComponent<Image>();
                if (myImage != null)
                {
                    Color newColor = myImage.color;
                    newColor.a = 1f; // Alfa (saydamlık) değerini 1 yapıyoruz (tamamen opak)
                    myImage.color = newColor;
                }
                // --------------------------
            }
        }
    }
}
