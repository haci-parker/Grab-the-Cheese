using UnityEngine;
using TMPro;

public class DialogueBubble : MonoBehaviour
{
    [SerializeField] private TMP_Text text1;

    void Start()
    {
        // Replace "{isim}" placeholder with the actual player name via GameManager
        text1.text = GameManager.Instance.ResolveName(text1.text);
    }
}
