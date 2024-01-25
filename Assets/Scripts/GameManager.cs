using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //Zmienna przechowujaca ile wrogow zyje
    private int enemiesAlive = 0;
    //Zmienna przechowujaca ktora jest to runda
    private int round = 1;
    //Zabojstwa gracza
    private int kills = 0;
    //Max bariery bazy
    private float maxBarrier = 1000;
    //Bariera bazy
    private float barrier = 800;
    [Header("GUI Settings")]
    //Zmienna przechowujaca tekst rundy
    [SerializeField] TextMeshProUGUI roundText;
    //Zmienna przechowujaca tekst zywych wrogow
    [SerializeField] TextMeshProUGUI enemiesAliveText;
    //Zmienna przechowujaca tekst zabojstw
    [SerializeField] TextMeshProUGUI killsText;
    //Zmienna przechowujaca tekst wytrzymalosci bariery bazy
    [SerializeField] TextMeshProUGUI barrierText;
    [Header("Spawn Settings")]
    // ref do spawnera 
    [SerializeField] Spawner[] spawners;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Resetowanie stat
        ResetStats();
        // Pocz¹tkowa runda
        NextRound(round);
    }


    void Update()
    {
        //Sprawdz czy liczba zywych wrogow wynosi 0
        if (enemiesAlive <= 0)
        {
            //zwieksz zmienna rundy
            round++;
            //wywolaj metode z parametrem rundy
            NextRound(round);
            //wpisz do zmiennej tekstu rundy slowo + liczba przechowywana w zmiennej zmieniona na string
            roundText.text = "Round " + round.ToString();
        }else if(barrier <= 0)
        {
            EndGame();
        } 
    }

    //metoda do wywolania nastepnej nastepnej rundy
    public void NextRound(int round)
    {
        for (int i = 0; i < spawners.Length; i++)
        {
            spawners[i].Spawn(round * 2); 
        }
    }

    public void addEnemyAlive(int value)
    {
        enemiesAlive += value;
        enemiesAliveText.text = enemiesAlive.ToString();
    }

    public void addKills(int value)
    { 
        kills += value;
        killsText.text = kills.ToString();

    }

    public void addBarrierBase(float value)
    {
        barrier += value;
        if (barrier >= maxBarrier)
        {
            barrier = maxBarrier;
        }
        barrierText.text = barrier.ToString();

    }

    public float getBarrierBase()
    {
        return barrier;
    }

    public float getMaxBarrierBase()
    {
        return maxBarrier;
    }

    public void StartEndGameDelay()
    {
        Invoke("EndGame", 5f); // Uruchomi EndGame po 5 sekundach
    }

    //metoda do aktywowania ekranu pauzy gry
    public void PauseGame()
    {
        //zatrzymaj czas gry
        Time.timeScale = 0;
        //Odblokuj kursor myszy
        Cursor.lockState = CursorLockMode.None;
        //pauseScreen.SetActive(true);
    }
    public void ReasumeGame()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        //pauseScreen.SetActive(false);
    }
    //metoda do aktywowania ekranu konca gry
    public void EndGame()
    {
        //zatrzymaj czas gry
        Time.timeScale = 0;
        //Odblokuj kursor myszy
        Cursor.lockState = CursorLockMode.None;
        //Aktywuj ekran konca gry
        //endScreen.SetActive(true);
    }

    public void ResetStats()
    {
        barrier = 800;
        enemiesAlive = 0;
        kills = 0;
        round = 1;
        roundText.text = "Round " + round.ToString();
        barrierText.text = barrier.ToString();
        killsText.text = kills.ToString();
    }
}
