using System;

using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private Configuration _configuration = null;

    [SerializeField]
    private Canvas _titleCanvas = null;

    [SerializeField]
    private TextMeshProUGUI _copyright = null;

    [SerializeField]
    private Canvas _menuCanvas = null;

    [SerializeField]
    private Canvas _gameCanvas = null;

    [SerializeField]
    private TextMeshProUGUI _score = null;

    [SerializeField]
    private TextMeshProUGUI _game = null;

    [SerializeField]
    private TextMeshProUGUI _lives = null;

    private Vector3 _copyrightStartPosition;


    public delegate void SystemResetButtonClickEventHandler(object sender, EventArgs e);
    public static event SystemResetButtonClickEventHandler OnSystemResetButtonClicked;

    public delegate void OptionButtonClickEventHandler(object sender, EventArgs e);
    public static event OptionButtonClickEventHandler OnOptionButtonClicked;

    public delegate void SelectButtonClickEventHandler(object sender, EventArgs e);
    public static event SelectButtonClickEventHandler OnSelectButtonClicked;

    public delegate void StartButtonClickEventHandler(object sender, EventArgs e);
    public static event StartButtonClickEventHandler OnStartButtonClicked;


    public void OptionButtonClicked()
    {
        ShowGameCanvas(this, EventArgs.Empty);
        HideTitleCanvas(this, EventArgs.Empty);

        if (OnOptionButtonClicked != null)
        {
            OnOptionButtonClicked(this, EventArgs.Empty);
        }
    }

    public void SelectButtonClicked()
    {
        ShowGameCanvas(this, EventArgs.Empty);
        HideTitleCanvas(this, EventArgs.Empty);

        if (OnSelectButtonClicked != null)
        {
            OnSelectButtonClicked(this, EventArgs.Empty);
        }
    }

    public void SetGame(int game)
    {
        _game.text = game.ToString("D2");
    }

    public void SetLives(int lives)
    {
        _lives.text = lives.ToString();
    }

    public void SetScore(int score)
    {
        _score.text = score.ToString("D5");
    }

    public void StartButtonClicked()
    {
        ShowGameCanvas(this, EventArgs.Empty);
        HideMenuCanvas(this, EventArgs.Empty);
        HideTitleCanvas(this, EventArgs.Empty);

        if (OnStartButtonClicked != null)
        {
            OnStartButtonClicked(this, EventArgs.Empty);
        }
    }

    public void SystemResetButtonClicked()
    {
        if (OnSystemResetButtonClicked != null)
        {
            OnSystemResetButtonClicked(this, EventArgs.Empty);
        }
    }

    private void HideMenuCanvas(object sender, EventArgs e)
    {
        if (_menuCanvas.gameObject.activeSelf == true)
        {
            ToggleCanvas(_menuCanvas);
        }
    }

    private void HideTitleCanvas(object sender, EventArgs e)
    {
        if (_titleCanvas.gameObject.activeSelf == true)
        {
            ToggleCanvas(_titleCanvas);
        }
    }

    private void OnDisable()
    {
        GameController.OnGameOver -= ShowMenuCanvas;
        GameController.OnInvaderFormationLanded -= ShowMenuCanvas;
    }

    private void OnEnable()
    {
        GameController.OnGameOver += ShowMenuCanvas;
        GameController.OnInvaderFormationLanded += ShowMenuCanvas;
    }

    private void ScrollCopyrightText()
    {
        float newPositionX = _copyright.transform.position.x - _configuration.CopyrightTextScrollSpeed * Time.deltaTime;

        _copyright.transform.position = new Vector3(newPositionX, _copyrightStartPosition.y, 0f);

        if (_copyright.transform.position.x <= 0f)
        {
            _copyright.transform.position = new Vector3(_copyrightStartPosition.x, _copyrightStartPosition.y, 0f);
        }
    }

    private void ShowGameCanvas(object sender, EventArgs e)
    {
        if (_gameCanvas.gameObject.activeSelf != true)
        {
            ToggleCanvas(_gameCanvas);
        }
    }

    private void ShowMenuCanvas(object sender, EventArgs e)
    {
        if (_menuCanvas.gameObject.activeSelf != true)
        {
            ToggleCanvas(_menuCanvas);
        }
    }

    private void Start()
    {
        _copyrightStartPosition = new Vector3(_copyright.transform.position.x, _copyright.transform.position.y, 0f);
    }

    private void ToggleCanvas(Canvas canvas)
    {
        canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);
    }

    private void Update()
    {
        if (_titleCanvas.gameObject.activeSelf == true)
        {
            ScrollCopyrightText();
        }
    }
}