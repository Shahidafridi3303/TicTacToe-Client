using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TicTacToeManager : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    [SerializeField] private Sprite xSprite, oSprite; // Assign in the Inspector
    public TextMeshProUGUI turnText, resultText;

    private string roomName;
    private bool isPlayerTurn = false;
    private int playerID; // 1 for X, 2 for O

    void Start()
    {
        NetworkClientProcessing.SetTicTacToeManager(this);

        // Bind button clicks dynamically
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnCellClicked(index));
        }
    }

    public void InitializePlayer(string role, int turn)
    {
        playerID = (role == "X") ? 1 : 2;
        isPlayerTurn = (turn == 1); // First player's turn
        UpdateTurnText();
    }

    public void UpdateCell(int x, int y, int player)
    {
        int index = x * 3 + y;
        buttons[index].image.sprite = (player == 1) ? xSprite : oSprite; // Update sprite
        buttons[index].interactable = false;

        UpdateTurnText();
    }

    private void UpdateTurnText()
    {
        if (isPlayerTurn)
        {
            turnText.text = "Your Turn";
        }
        else
        {
            turnText.text = "Opponent's Turn";
        }
    }

    public void OnCellClicked(int index)
    {
        if (isPlayerTurn)
        {
            int x = index / 3;
            int y = index % 3;

            NetworkClientProcessing.SendMessageToServer($"11,{roomName},{x},{y}", TransportPipeline.ReliableAndInOrder);

            buttons[index].image.sprite = (playerID == 1) ? xSprite : oSprite;
            buttons[index].interactable = false;

            isPlayerTurn = false; // Wait for the server to confirm the move
            UpdateTurnText();
        }
        else
        {
            Debug.Log("Not your turn!");
        }
    }


    public void ShowGameResult(int result)
    {
        if (result == 1)
            resultText.text = "Player 1 (X) Wins!";
        else if (result == 2)
            resultText.text = "Player 2 (O) Wins!";
        else
            resultText.text = "It's a Draw!";
    }
}
