using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Singleton GameManager - persists across all scenes.
/// Stores the player's name and provides a helper to resolve "{isim}" placeholders.
/// </summary>
public class GameManager : MonoBehaviour
{
    // --- Singleton -----------------------------------------------------------
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // survive scene changes
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject); // destroy duplicate
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (var tmp in FindObjectsOfType<TMP_Text>())
        {
            if (tmp.text.Contains("{isim}"))
                tmp.text = tmp.text.Replace("{isim}", PlayerName);
        }
    }
    // -------------------------------------------------------------------------

    // The player's name, set from the start-menu input field
    public string PlayerName { get; private set; } = "Oyuncu";

    /// <summary>Stores the player name (trims whitespace).</summary>
    public void SetPlayerName(string name)
    {
        PlayerName = string.IsNullOrWhiteSpace(name) ? "Oyuncu" : name.Trim();
        Debug.Log($"[GameManager] Player name set to: {PlayerName}");
    }

    /// <summary>
    /// Replaces every "{isim}" token in <paramref name="template"/> with the
    /// current player name.
    /// Example: "Merhaba {isim}!" → "Merhaba Ali!"
    /// </summary>
    public string ResolveName(string template)
    {
        return template.Replace("{isim}", PlayerName);
    }
}
