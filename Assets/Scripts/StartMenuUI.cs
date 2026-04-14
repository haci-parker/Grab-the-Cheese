using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StartMenuUI : MonoBehaviour
{
    public TMP_InputField nameInput;
    public GameObject informationText;  // information_text objesi
    public GameObject hata;             // hata objesi

    public void StartGame()
    {
        string enteredName = nameInput.text.Trim();

        if (string.IsNullOrEmpty(enteredName))
        {
            // İsim boşsa: information_text kapat, hata göster
            if (informationText != null) informationText.SetActive(false);
            if (hata != null)            hata.SetActive(true);
            return;
        }

        // İsim doluysa: hatayı gizle (varsa), ismi kaydet, sahneye geç
        if (hata != null)            hata.SetActive(false);
        if (informationText != null) informationText.SetActive(true);

        GameManager.Instance.SetPlayerName(enteredName);
        SceneManager.LoadScene("StoryScene1");
    }
}