using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StoryDialogueManager : MonoBehaviour
{
    private System.Action[] steps;
    private int current = 0;
    public int CurrentLevel { get { return current; } }
    private int minLevel = 0; // current bir kez 11 olursa, bir daha 10 ve altına düşmesin
    private GameObject[] allObjects; // Geri için tüm objeleri takip et

    // Çağrı butonu pulse sistemi
    private GameObject buttonKulaklikObj;
    private Color originalButtonColor;
    private bool shouldPulse = false;
    private float pulseTimer = 0f;

    // Envanter butonu pulse sistemi
    private GameObject buttonCantaObj;
    private Color originalCantaColor;
    private bool shouldPulseCanta = false;
    private float pulseCantaTimer = 0f;

    // Seçenek Butonları
    private Button choice1Btn;
    private Button choice2Btn;
    private Button choice3Btn;

    // Yeni görsel eklenti butonları
    private GameObject choice1VisualBtn;
    private GameObject choice2VisualBtn;
    private GameObject choice3VisualBtn;

    private bool hasMadeChoice = false;
    private Color originalChoice1Color = Color.white;
    private Color originalChoice1DisabledColor = Color.white;
    private Color originalChoice1VisualColor = Color.white;

    // Mouse Movement objesi
    private GameObject mouseMovementObj;

    // Deaktif objeleri de bulabilen yardımcı metod
    static GameObject GO(string name)
    {
        GameObject obj = GameObject.Find(name);
        if (obj != null) return obj;

        foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            Transform result = FindChildRecursive(root.transform, name);
            if (result != null) return result.gameObject;
        }

        Debug.LogWarning($"[StoryDialogueManager] '{name}' adlı obje sahnede bulunamadı!");
        return null;
    }

    static Transform FindChildRecursive(Transform parent, string name)
    {
        if (parent.name == name) return parent;
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform result = FindChildRecursive(parent.GetChild(i), name);
            if (result != null) return result;
        }
        return null;
    }

    void Start()
    {
        GameObject text1         = GO("text1");
        GameObject text2         = GO("text2");
        GameObject text3         = GO("text3");
        GameObject text4         = GO("text4");
        GameObject text5         = GO("text5");
        GameObject text6         = GO("text6");
        GameObject text7         = GO("text7");
        GameObject text8         = GO("text8");
        GameObject text9         = GO("text9");
        GameObject text10        = GO("text10");
        GameObject text11        = GO("text11");
        GameObject buttonIleri   = GO("button_ileri");
        GameObject buttonTabi    = GO("button_tabi");
        GameObject kulaklikAlma  = GO("kulaklik_alma");
        GameObject cantaAlma     = GO("canta_alma");
        GameObject kulaklikPaneli = GO("kulaklik_paneli");
        GameObject cantaPaneli   = GO("canta_paneli");
        GameObject arrowKulaklik = GO("arrow_kulaklik");
        GameObject arrowCanta    = GO("arrow_canta");
        GameObject oldMice       = GO("old_mice");
        GameObject oldMiceHappy  = GO("old_mice_happy");
        GameObject introScreenBg1 = GO("intro_screen_bg1");
        GameObject introScreenBg2 = GO("intro_screen_bg2");
        GameObject transparentBrownPanel = GO("transparent_brown_panel");
        GameObject excitedMouse   = GO("excited_mouse");
        GameObject calmMouse      = GO("calm_mouse");
        // text1, transparent_brown_panel'in child'ı olduğu için path üzerinden buluyoruz
        GameObject bg2Text1       = introScreenBg2 != null ? introScreenBg2.transform.Find("transparent_brown_panel/text1")?.gameObject : null;
        // bg2 panel altındaki diğer textler
        GameObject bg2Text2 = transparentBrownPanel != null ? transparentBrownPanel.transform.Find("text2")?.gameObject : null;
        GameObject bg2Text3 = transparentBrownPanel != null ? transparentBrownPanel.transform.Find("text3")?.gameObject : null;
        GameObject bg2Text4 = transparentBrownPanel != null ? transparentBrownPanel.transform.Find("text4")?.gameObject : null;

        // Tüm objeleri kaydet (geri giderken sıfırlamak için) — bg2 objeleri de dahil
        allObjects = new GameObject[] { text1,text2,text3,text4,text5,text6,text7,text8,text9,text10,text11, buttonIleri,buttonTabi,kulaklikAlma,cantaAlma,kulaklikPaneli,cantaPaneli,oldMice,oldMiceHappy, excitedMouse,calmMouse,transparentBrownPanel,bg2Text1,bg2Text2,bg2Text3,bg2Text4, arrowKulaklik,arrowCanta };

        steps = new System.Action[]
        {
            /* 0  start    */ () => { Show(text1, buttonIleri, oldMice); Hide(text2,text3,text4,text5,text6,text7,text8,text9,text10,text11, buttonTabi,kulaklikAlma,cantaAlma,kulaklikPaneli,cantaPaneli,oldMiceHappy,arrowKulaklik,arrowCanta); },
            /* 1  ileri    */ () => { Show(text3, buttonTabi);        Hide(text1, buttonIleri); },
            /* 2  tabi     */ () => { Show(text4, buttonIleri, oldMiceHappy); Hide(text3, buttonTabi, oldMice); },
            /* 3  ileri    */ () => { Show(text5, oldMice);           Hide(text4, oldMiceHappy, arrowKulaklik, arrowCanta); },
            /* 4  ileri    */ () => { Show(kulaklikAlma);             Hide(text5, buttonIleri); },
            /* 5  kulaklık */ () => { Show(text6, buttonIleri, kulaklikPaneli, arrowKulaklik); Hide(kulaklikAlma); },
            /* 6  ileri    */ () => { Show(cantaAlma);                Hide(text6, buttonIleri, arrowKulaklik); },
            /* 7  canta    */ () => { Show(text7, buttonIleri, cantaPaneli, arrowCanta); Hide(cantaAlma); },
            /* 8  ileri    */ () => { Show(text8);                    Hide(text7, arrowCanta); },
            /* 9  ileri    */ () => { Show(text9);                    Hide(text8); },
            /*10  ileri    */ () => { Show(text10);                   Hide(text9); },
            /*11  ileri    */ () => { Show(text11);                   Hide(text10); },
            /*12  ileri    */ () => { Hide(introScreenBg1); Show(introScreenBg2); Hide(transparentBrownPanel, excitedMouse, bg2Text1); },
            /*13  kulaklık btn */ () => { Show(transparentBrownPanel, excitedMouse, bg2Text1); },
        };

        steps[0]();

        // Çağrı butonunu bul — kulaklik_paneli altındaki button_kulaklik'i hedefle
        // (kulaklik_alma altındaki button_kulaklik ile karıştırılmasın)
        GameObject kulaklikPaneliObj = GO("kulaklik_paneli");
        if (kulaklikPaneliObj != null)
        {
            Transform bk = FindChildRecursive(kulaklikPaneliObj.transform, "button_kulaklik");
            if (bk != null) buttonKulaklikObj = bk.gameObject;
        }
        if (buttonKulaklikObj != null)
        {
            Image img = buttonKulaklikObj.GetComponent<Image>();
            if (img != null) originalButtonColor = img.color;
            Debug.Log($"[StoryDialogueManager] Doğru button_kulaklik bulundu: {buttonKulaklikObj.transform.parent?.name}/{buttonKulaklikObj.name}");
        }

        // Envanter butonunu bul — canta_paneli altındaki button_canta'yı hedefle
        GameObject cantaPaneliObj = GO("canta_paneli");
        if (cantaPaneliObj != null)
        {
            Transform bc = FindChildRecursive(cantaPaneliObj.transform, "button_canta");
            if (bc != null) buttonCantaObj = bc.gameObject;
        }
        if (buttonCantaObj != null)
        {
            Image img = buttonCantaObj.GetComponent<Image>();
            if (img != null) originalCantaColor = img.color;
            Debug.Log($"[StoryDialogueManager] button_canta bulundu: {buttonCantaObj.transform.parent?.name}/{buttonCantaObj.name}");
        }
        // Pulse henüz başlamaz

        // Seçenek Butonlarını bul ve cache'le
        GameObject opt1 = GO("choice1");
        if (opt1 != null)
        {
            choice1Btn = opt1.GetComponent<Button>();
            if (choice1Btn != null) 
            {
                if (choice1Btn.image != null)
                    originalChoice1Color = choice1Btn.image.color;
                originalChoice1DisabledColor = choice1Btn.colors.disabledColor;
            }
        }
        GameObject opt2 = GO("choice2");
        if (opt2 != null) choice2Btn = opt2.GetComponent<Button>();
        GameObject opt3 = GO("choice3");
        if (opt3 != null) choice3Btn = opt3.GetComponent<Button>();

        // Yeni eklenen "choiceX_button" objelerini bul
        choice1VisualBtn = GO("choice1_button");
        if (choice1VisualBtn != null)
        {
            Image vImg = choice1VisualBtn.GetComponent<Image>();
            if (vImg != null) originalChoice1VisualColor = vImg.color;
        }
        choice2VisualBtn = GO("choice2_button");
        choice3VisualBtn = GO("choice3_button");

        // Mouse Movement objesini bul
        mouseMovementObj = GO("mouse_movement");
    }

    void Update()
    {
        Color blue  = new Color(0.2f, 0.4f, 1f, 1f);
        Color green = new Color(0.2f, 0.85f, 0.4f, 1f);
        string sceneName = SceneManager.GetActiveScene().name;

        // Kulaklık butonu pulse — current >= 12 iken
        if (shouldPulse && buttonKulaklikObj != null
            && sceneName == "StoryScene1" && current >= 12)
        {
            Image img = buttonKulaklikObj.GetComponent<Image>();
            if (img != null)
            {
                pulseTimer += Time.deltaTime;
                float t = Mathf.PingPong(pulseTimer, 1f);
                img.color = Color.Lerp(blue, green, t);
            }
        }

        // Çanta butonu pulse — current >= 16 iken
        if (shouldPulseCanta && buttonCantaObj != null
            && sceneName == "StoryScene1" && current >= 16)
        {
            Image img = buttonCantaObj.GetComponent<Image>();
            if (img != null)
            {
                pulseCantaTimer += Time.deltaTime;
                float t = Mathf.PingPong(pulseCantaTimer, 1f);
                img.color = Color.Lerp(blue, green, t);
            }
        }

        // Seçenek Butonları Interactable Kontrolü
        bool isChoiceInteractable = (current >= 19) && !hasMadeChoice;
        if (choice1Btn != null) choice1Btn.interactable = isChoiceInteractable;
        if (choice2Btn != null) choice2Btn.interactable = isChoiceInteractable;
        if (choice3Btn != null) choice3Btn.interactable = isChoiceInteractable;

        // "choiceX_button" objelerinin görünürlük kontrolü
        bool shouldShowVisuals = (current >= 19);
        if (choice1VisualBtn != null && choice1VisualBtn.activeSelf != shouldShowVisuals) 
            choice1VisualBtn.SetActive(shouldShowVisuals);
        if (choice2VisualBtn != null && choice2VisualBtn.activeSelf != shouldShowVisuals) 
            choice2VisualBtn.SetActive(shouldShowVisuals);
        if (choice3VisualBtn != null && choice3VisualBtn.activeSelf != shouldShowVisuals) 
            choice3VisualBtn.SetActive(shouldShowVisuals);

        // "mouse_movement" objesinin görünürlük kontrolü
        bool showMouseMovement = (current >= 21);
        if (mouseMovementObj != null && mouseMovementObj.activeSelf != showMouseMovement)
            mouseMovementObj.SetActive(showMouseMovement);
    }

    // İleri butonu
    public void OnIleri()
    {
        Debug.Log($"[StoryDialogueManager] OnIleri çağrıldı! current = {current}");

        // bg1 akışı
        int[] valid = { 0, 2, 3, 5, 7, 8, 9, 10, 11 };
        foreach (int s in valid)
        {
            if (current == s)
            {
                current++;
                Debug.Log($"[StoryDialogueManager] OnIleri → current = {current}");
                // current 11'e ulaştığında minLevel'i kilitle
                if (current >= 11 && minLevel < 11)
                {
                    minLevel = 11;
                }
                // current 12'ye ulaştığında pulse'u aktif et
                if (current >= 12 && !shouldPulse)
                {
                    shouldPulse = true;
                    pulseTimer = 0f;
                    Debug.Log($"[StoryDialogueManager] current = {current}, shouldPulse = true, renk değişimi başladı.");
                }
                steps[current]();
                return;
            }
        }

        // bg2 akışı — transparent_brown_panel altındaki objeleri doğrudan bul
        GameObject tp = GO("transparent_brown_panel");
        if (tp == null) return;

        if (current == 13)
        {
            // excited_mouse → calm_mouse, text1 → text2
            current = 14;
            GameObject em = GO("excited_mouse");
            GameObject cm = GO("calm_mouse");
            Transform t1 = tp.transform.Find("text1");
            Transform t2 = tp.transform.Find("text2");

            if (em != null) em.SetActive(false);
            if (cm != null) cm.SetActive(true);
            if (t1 != null) t1.gameObject.SetActive(false);
            if (t2 != null) t2.gameObject.SetActive(true);
        }
        else if (current == 14)
        {
            // text2 → text3
            current = 15;
            Transform t2 = tp.transform.Find("text2");
            Transform t3 = tp.transform.Find("text3");

            if (t2 != null) t2.gameObject.SetActive(false);
            if (t3 != null) t3.gameObject.SetActive(true);
        }
        else if (current == 15)
        {
            // text3 → text4
            current = 16;
            Transform t3 = tp.transform.Find("text3");
            Transform t4 = tp.transform.Find("text4");

            if (t3 != null) t3.gameObject.SetActive(false);
            if (t4 != null) t4.gameObject.SetActive(true);

            // Çanta butonu pulse'u başlat
            if (!shouldPulseCanta)
            {
                shouldPulseCanta = true;
                pulseCantaTimer = 0f;
                Debug.Log($"[StoryDialogueManager] current = {current}, shouldPulseCanta = true, envanter renk değişimi başladı.");
            }
        }
        else if (current == 17)
        {
            current = 18;
            GameObject bg3Text1 = GO("bg3_text1");
            GameObject bg3Text2 = GO("bg3_text2");

            if (bg3Text1 != null) bg3Text1.SetActive(false);
            if (bg3Text2 != null) bg3Text2.SetActive(true);

            Debug.Log($"[StoryDialogueManager] bg3_text1 kapatıldı, bg3_text2 açıldı. current = {current}");
        }
        else if (current == 18)
        {
            current = 19;
            GameObject bg3Text2 = GO("bg3_text2");
            GameObject bg3Text3 = GO("bg3_text3");
            GameObject choice1 = GO("choice1");
            GameObject choice2 = GO("choice2");
            GameObject choice3 = GO("choice3");

            if (bg3Text2 != null) bg3Text2.SetActive(false);
            if (bg3Text3 != null) bg3Text3.SetActive(true);
            if (choice1 != null) choice1.SetActive(true);
            if (choice2 != null) choice2.SetActive(true);
            if (choice3 != null) choice3.SetActive(true);

            Debug.Log($"[StoryDialogueManager] bg3_text2 kapatıldı, bg3_text3 ve choiceler açıldı. current = {current}");
        }
        else if (current == 20)
        {
            current = 21;
            GameObject lbg = GO("light_brown_bg");
            if (lbg != null) lbg.SetActive(false);
            Debug.Log($"[StoryDialogueManager] light_brown_bg deaktif edildi, current = {current}");
        }
    }

    private void HideChoiceTexts()
    {
        // Kullanıcının belirttiği ihtimale karşı transparent_brown_panel altındaki text3'ü de bulup kapatalım
        GameObject tp = GO("transparent_brown_panel");
        if (tp != null)
        {
            Transform t3 = tp.transform.Find("text3");
            if (t3 != null) t3.gameObject.SetActive(false);
        }

        // Bg3 kısmındaki aktif metni de kapatalım
        GameObject bg3Text3 = GO("bg3_text3");
        if (bg3Text3 != null) bg3Text3.SetActive(false);

        // Başka bir seçeneğe tıklanmasına karşı eski durumları kapatıyoruz
        GameObject pos = GO("bg3_text_positive");
        GameObject neg = GO("bg3_text_negative");
        if (pos != null) pos.SetActive(false);
        if (neg != null) neg.SetActive(false);
    }

    public void OnChoice1()
    {
        Debug.Log("[StoryDialogueManager] choice1 tıklandı!");
        hasMadeChoice = true;

        Color targetGreen = new Color(51f / 255f, 255f / 255f, 0f / 255f, 198f / 255f);

        // Ana butonun direkt arka planını ve inaktif olunca üstüne binmemesi için disabledColor'ını değiştir.
        if (choice1Btn != null)
        {
            if (choice1Btn.image != null) choice1Btn.image.color = targetGreen;
            ColorBlock cb = choice1Btn.colors;
            cb.disabledColor = targetGreen;
            choice1Btn.colors = cb;
        }

        // Görsel eklenti objesi (choice1_button) varsa onun da rengini aynı yeşile ayarla.
        if (choice1VisualBtn != null)
        {
            Image vImg = choice1VisualBtn.GetComponent<Image>();
            if (vImg != null) vImg.color = targetGreen;
        }

        HideChoiceTexts();
        GameObject pos = GO("bg3_text_positive");
        if (pos != null) pos.SetActive(true);
        current++;
        Debug.Log($"[StoryDialogueManager] current arttırıldı, yeni current = {current}");
    }

    public void OnChoice2()
    {
        Debug.Log("[StoryDialogueManager] choice2 tıklandı!");
        HideChoiceTexts();
        GameObject neg = GO("bg3_text_negative");
        if (neg != null) neg.SetActive(true);
    }

    public void OnChoice3()
    {
        Debug.Log("[StoryDialogueManager] choice3 tıklandı!");
        HideChoiceTexts();
        GameObject neg = GO("bg3_text_negative");
        if (neg != null) neg.SetActive(true);
    }

    // Tabi butonu — sadece step 1'de çalışır
    public void OnTabi()
    {
        Debug.Log($"[StoryDialogueManager] OnTabi çağrıldı! current = {current}");
        if (current == 1) { current = 2; steps[2](); }
    }

    // Kulaklık alma butonuna basınca — sadece step 4'te çalışır
    public void OnKulaklik()
    {
        Debug.Log($"[StoryDialogueManager] OnKulaklik çağrıldı! current = {current}");
        if (current == 4) { current = 5; steps[5](); }
    }

    // Çanta alma butonuna basınca — sadece step 6'da çalışır
    public void OnCanta()
    {
        Debug.Log($"[StoryDialogueManager] OnCanta çağrıldı! current = {current}");
        if (current == 6) { current = 7; steps[7](); }
    }

    // bg2'deki kulaklık butonuna basınca — sadece step 12'de çalışır
    public void OnButtonKulaklik()
    {
        Debug.Log($"[StoryDialogueManager] OnButtonKulaklik çağrıldı! current = {current}");

        // Pulse'u durdur ve orijinal renge döndür
        shouldPulse = false;
        if (buttonKulaklikObj != null)
        {
            Image img = buttonKulaklikObj.GetComponent<Image>();
            if (img != null) img.color = originalButtonColor;
        }
        Debug.Log("[StoryDialogueManager] shouldPulse = false, renk değişimi durduruldu!");

        if (current == 12)
        {
            current = 13;

            // Objeleri doğrudan bul ve aktif et
            GameObject tp = GO("transparent_brown_panel");
            GameObject em = GO("excited_mouse");

            // text1'i transparent_brown_panel'in child'ı olarak bul
            GameObject t1 = null;
            if (tp != null)
            {
                Transform t1Transform = tp.transform.Find("text1");
                if (t1Transform != null) t1 = t1Transform.gameObject;
            }

            Debug.Log($"[StoryDialogueManager] tp={tp != null}, em={em != null}, t1={t1 != null}");

            if (tp != null) tp.SetActive(true);
            if (em != null) em.SetActive(true);
            if (t1 != null) t1.SetActive(true);
        }
    }

    // Geri butonu — bir adım geri gider
    public void OnGeri()
    {
        if (current <= minLevel) return; // minLevel'in altına düşemeyiz

        current--;
        hasMadeChoice = false;
        if (choice1Btn != null)
        {
            if (choice1Btn.image != null) choice1Btn.image.color = originalChoice1Color;
            ColorBlock cb = choice1Btn.colors;
            cb.disabledColor = originalChoice1DisabledColor;
            choice1Btn.colors = cb;
        }
        if (choice1VisualBtn != null)
        {
            Image vImg = choice1VisualBtn.GetComponent<Image>();
            if (vImg != null) vImg.color = originalChoice1VisualColor;
        }
        // Ekstra güvenlik: current asla minLevel'in altına düşmesin
        if (current < minLevel) current = minLevel;

        Debug.Log($"[StoryDialogueManager] OnGeri çağrıldı! current = {current}, minLevel = {minLevel}");

        // Tüm objeleri gizle (bg2 objeleri dahil)
        Hide(allObjects);

        if (current >= 12)
        {
            // ── bg2 akışı — doğrudan state set et ──
            GameObject ib1 = GO("intro_screen_bg1");
            GameObject lbg = GO("light_brown_bg");
            if (lbg != null && current <= 20) lbg.SetActive(true);
            GameObject ib2 = GO("intro_screen_bg2");
            GameObject tp  = GO("transparent_brown_panel");
            GameObject em  = GO("excited_mouse");
            GameObject cm  = GO("calm_mouse");
            GameObject kp  = GO("kulaklik_paneli");
            GameObject cp  = GO("canta_paneli");
            GameObject bi  = GO("button_ileri");

            // bg1 kapalı, bg2 açık — her zaman
            if (ib1 != null) ib1.SetActive(false);
            if (ib2 != null) ib2.SetActive(true);

            // Kulaklık/çanta panelleri ve ileri butonu her zaman görünür
            if (kp != null) kp.SetActive(true);
            if (cp != null) cp.SetActive(true);
            if (bi != null) bi.SetActive(true);

            if (current == 12)
            {
                // bg2 gösterildi ama panel henüz açılmadı — kulaklık pulse aktif
                if (tp != null) tp.SetActive(false);
                if (em != null) em.SetActive(false);
                if (cm != null) cm.SetActive(false);
            }
            else if (current == 13)
            {
                // text1 + excited_mouse
                if (tp != null) tp.SetActive(true);
                if (em != null) em.SetActive(true);
                if (cm != null) cm.SetActive(false);
                SetBg2Text(tp, "text1");
            }
            else if (current == 14)
            {
                // text2 + calm_mouse
                if (tp != null) tp.SetActive(true);
                if (em != null) em.SetActive(false);
                if (cm != null) cm.SetActive(true);
                SetBg2Text(tp, "text2");
            }
            else if (current == 15)
            {
                // text3 + calm_mouse
                if (tp != null) tp.SetActive(true);
                if (em != null) em.SetActive(false);
                if (cm != null) cm.SetActive(true);
                SetBg2Text(tp, "text3");
            }
        }
        else
        {
            // ── bg1 akışı — steps replay ──
            int replayUpTo = Mathf.Min(current, steps.Length - 1);
            for (int i = 0; i <= replayUpTo; i++)
                steps[i]();
        }
    }

    // bg2 panel altında sadece belirtilen text'i göster, diğerlerini gizle
    private void SetBg2Text(GameObject panel, string activeTextName)
    {
        if (panel == null) return;
        string[] allTexts = { "text1", "text2", "text3", "text4" };
        foreach (string t in allTexts)
        {
            Transform child = panel.transform.Find(t);
            if (child != null) child.gameObject.SetActive(t == activeTextName);
        }
    }

    // ═══════════════ Item Panel Sistemi ═══════════════
    private bool item1Clicked = false;
    private bool item2Clicked = false;

    // button_canta'ya basınca — current 16 iken item_panel açılır
    public void OnButtonCanta()
    {
        Debug.Log($"[StoryDialogueManager] OnButtonCanta çağrıldı! current = {current}");

        // Pulse'u durdur ve orijinal renge döndür
        shouldPulseCanta = false;
        if (buttonCantaObj != null)
        {
            Image img = buttonCantaObj.GetComponent<Image>();
            if (img != null) img.color = originalCantaColor;
        }
        Debug.Log("[StoryDialogueManager] shouldPulseCanta = false, envanter renk değişimi durduruldu!");

        if (current == 16)
        {
            GameObject itemPanel = GO("item_panel");
            if (itemPanel != null) itemPanel.SetActive(true);

            // Envanter açıldığında transparent_brown_panel'i gizle
            GameObject tp = GO("transparent_brown_panel");
            if (tp != null) tp.SetActive(false);
        }
    }

    // item1 butonuna basınca
    public void OnItem1Click()
    {
        item1Clicked = true;
        Debug.Log("[StoryDialogueManager] item1 tıklandı!");
    }

    // item2 butonuna basınca
    public void OnItem2Click()
    {
        item2Clicked = true;
        Debug.Log("[StoryDialogueManager] item2 tıklandı!");
    }

    // Tamam butonu (items_button) — item1 ve item2 tıklandıysa panel kapanır
    public void OnItemsTamam()
    {
        Debug.Log($"[StoryDialogueManager] OnItemsTamam çağrıldı! item1={item1Clicked}, item2={item2Clicked}");

        if (item1Clicked && item2Clicked)
        {
            // item_panel kapat
            GameObject itemPanel = GO("item_panel");
            if (itemPanel != null) itemPanel.SetActive(false);

            // kulaklik_paneli ve canta_paneli kapat
            GameObject kp = GO("kulaklik_paneli");
            GameObject cp = GO("canta_paneli");
            if (kp != null) kp.SetActive(false);
            if (cp != null) cp.SetActive(false);

            // intro_screen_bg2 kapat, intro_screen_bg3 aç
            GameObject bg2 = GO("intro_screen_bg2");
            GameObject bg3 = GO("intro_screen_bg3");
            GameObject bg3Text1 = GO("bg3_text1");
            if (bg2 != null) bg2.SetActive(false);
            if (bg3 != null) bg3.SetActive(true);
            if (bg3Text1 != null) bg3Text1.SetActive(true);

            // current'ı artır
            current = 17;

            Debug.Log($"[StoryDialogueManager] İki item de tıklandı — panel kapatıldı! bg3 ve bg3_text1 açıldı, current = {current}");
        }
        else
        {
            Debug.Log("[StoryDialogueManager] Henüz iki item de tıklanmadı!");
        }
    }

    // ═══════════════ Info Panel Sistemi ═══════════════

    // button_info'ya basınca — sadece current 12 iken info1 paneli açılır
    public void OnButtonInfo()
    {
        Debug.Log($"[StoryDialogueManager] OnButtonInfo çağrıldı! current = {current}");

        if (current == 12)
        {
            GameObject info1 = GO("info1");
            if (info1 != null) info1.SetActive(true);
            Debug.Log("[StoryDialogueManager] info1 paneli açıldı!");
        }
    }

    // info_button1'e basınca — info1 paneli kapanır
    public void OnInfoButton1()
    {
        Debug.Log("[StoryDialogueManager] OnInfoButton1 çağrıldı!");
        GameObject info1 = GO("info1");
        if (info1 != null) info1.SetActive(false);
        Debug.Log("[StoryDialogueManager] info1 paneli kapatıldı!");
    }

    void Show(params GameObject[] objs) { foreach (var o in objs) if (o) o.SetActive(true); }
    void Hide(params GameObject[] objs) { foreach (var o in objs) if (o) o.SetActive(false); }
}
