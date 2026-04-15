using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ItemDropZone : MonoBehaviour, IDropHandler
{
    [Header("Drop Zone Ayarları")]
    [Tooltip("Bu drop zone'un kabul ettiği item etiketi (örn: 'destek', 'tahta')")]
    public string acceptedItemTag = "";

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
                // Etiket kontrolü — eğer acceptedItemTag boş değilse, sadece eşleşen etiketleri kabul et
                if (!string.IsNullOrEmpty(acceptedItemTag) && draggedItem.itemTag != acceptedItemTag)
                {
                    Debug.Log($"[ItemDropZone] '{draggedItem.gameObject.name}' (tag: {draggedItem.itemTag}) bu zone'a (tag: {acceptedItemTag}) uymuyor — reddedildi!");
                    return; // Eşleşmeyen item'ları kabul etme
                }

                // Sürüklenen eşyaya, "Senin yeni hedefin (yuvan) benim diyoruz."
                draggedItem.parentAfterDrag = transform;

                // Destek drop edildiğinde: destek arrowu kapat, tahta arrow ve collider aç
                if (draggedItem.itemTag == "destek")
                {
                    GameObject destekArrow = FindByName("destek_in_panel_arrow");
                    GameObject tahtaArrow = FindByName("tahta_in_panel_arrow");
                    GameObject tahtaCollider = FindByName("tahta_collider");

                    if (destekArrow != null) destekArrow.SetActive(false);
                    if (tahtaArrow != null) tahtaArrow.SetActive(true);
                    if (tahtaCollider != null) tahtaCollider.SetActive(true);

                    Debug.Log("[ItemDropZone] Destek drop edildi → destek_arrow kapatıldı, tahta_arrow ve tahta_collider açıldı!");
                }

                // Başarılı drop — collider objesini (kendimizi) gizle
                Debug.Log($"[ItemDropZone] '{draggedItem.gameObject.name}' başarıyla bırakıldı → '{gameObject.name}' gizleniyor!");
                gameObject.SetActive(false);
            }
        }
    }

    // Deaktif objeleri de bulabilen yardımcı metod
    private GameObject FindByName(string name)
    {
        GameObject obj = GameObject.Find(name);
        if (obj != null) return obj;

        foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            Transform result = FindChildRecursive(root.transform, name);
            if (result != null) return result.gameObject;
        }
        return null;
    }

    private Transform FindChildRecursive(Transform parent, string name)
    {
        if (parent.name == name) return parent;
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform result = FindChildRecursive(parent.GetChild(i), name);
            if (result != null) return result;
        }
        return null;
    }
}
