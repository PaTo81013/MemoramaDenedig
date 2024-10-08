using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public ContenedorCartas cartasContenedor;
    public int filas = 8;
    public int columnas = 3;
    public Vector2 espacio = Vector2.zero;
    public GameObject cartaPrefab;
    public float Xpos = 4.91f;
    public float Ypos = 1.55f;
    public float nextwaveXpos;
    private int currentWave = 0;
    private int[] waveCardCounts = { 8, 12, 16, 20 };
    private int[] waveCardCountsModoSinFallos = { 8, 12, 16, 20 };
    private List<int> CardIDs;
    public Button waveButton;
    public bool modoSinFallos;

    private void Awake()
    {
        cartasContenedor = Settings.Instance.GetCarts();
    }
    void Start()
    {
        CardIDs = MixCards(waveCardCounts[currentWave]);
        SpawnCards(waveCardCounts[currentWave]);

        //si se esta jugando el modo sin fallos
        if (Settings.Instance.GetGameMode() == 3)
        {
            modoSinFallos = true;
            StartModoSinFallos();
        }

        waveButton.onClick.AddListener(OnWaveButtonClicked);
    }

    void StartModoSinFallos()
    {
        CardIDs = MixCards(waveCardCountsModoSinFallos[currentWave]);
        SpawnCards(waveCardCountsModoSinFallos[currentWave]);
        ActivateOpenAnimation();
        Invoke("ActivateCloseAnimation", 5f);
    }

    void ActivateOpenAnimation()
    {
        Cards[] cards = GetComponentsInChildren<Cards>();
        foreach (Cards card in cards)
        {
            Animator animator = card.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Open");
            }

            BoxCollider collider = card.GetComponent<BoxCollider>();
            if (collider != null)
            {
                collider.enabled = false;
            }
        }
    }

    void ActivateCloseAnimation()
    {
        Cards[] cards = GetComponentsInChildren<Cards>();
        foreach (Cards card in cards)
        {
            Animator animator = card.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Close");
            }

            BoxCollider collider = card.GetComponent<BoxCollider>();
            if (collider != null)
            {
                collider.enabled = true;
            }
        }
    }

    public void OnWaveButtonClicked()
    {
        if (currentWave < (modoSinFallos ? waveCardCountsModoSinFallos.Length : waveCardCounts.Length) - 1)
        {
            currentWave++;
            Xpos += nextwaveXpos;
            CardIDs = MixCards(modoSinFallos ? waveCardCountsModoSinFallos[currentWave] : waveCardCounts[currentWave]);

            if (modoSinFallos)
            {
                ActivateOpenAnimation();
                Invoke("ActivateCloseAnimation", 5f);
            }

            SpawnCards(modoSinFallos ? waveCardCountsModoSinFallos[currentWave] : waveCardCounts[currentWave]);
        }
    }

    void SpawnCards(int totalCards)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        int _cartasSpawneadas = 0;

        for (int x = 0; x < filas; x++)
        {
            for (int y = 0; y < columnas; y++)
            {
                if (_cartasSpawneadas < totalCards)
                {
                    GameObject cartas = Instantiate(cartaPrefab, transform);
                    Cards cartaenturno = cartas.GetComponent<Cards>();
                    cartaenturno.id = CardIDs[_cartasSpawneadas];
                    cartaenturno.AsignarImagendeCarta(cartasContenedor.cartas[cartaenturno.id].imagen);
                    cartaenturno.AsignarTextodeCarta(cartasContenedor.cartas[cartaenturno.id].nombre);

                    cartas.transform.position = new Vector2(x * espacio.x - Xpos, y * espacio.y - Ypos);
                    _cartasSpawneadas++;
                }
            }
        }
    }

    List<int> MixCards(int totalCards)
    {
        List<int> CardIDs = new List<int>();

        for (int i = 0; i < totalCards; i++)
        {
            CardIDs.Add(i / 2);
        }

        CardIDs.Shuffle();
        return CardIDs;
    }
}
