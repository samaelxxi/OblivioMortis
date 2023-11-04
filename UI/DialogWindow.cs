using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogWindow : MonoBehaviour
{
    [SerializeField] Image _avatar;
    [SerializeField] TMP_Text _name;
    [SerializeField] TMP_Text _text;


    public event Action OnDialogFinished;
    public bool IsDialogActive => _dialogActive;


    DialogSettings _settings;
    DialogData _currentDialogData;
    int _currentLine;
    bool _dialogActive = false;
    bool _lineGoing = false;
    bool _continue = false;
    string _currentText = "";
    Coroutine _textCor;

    void Awake()
    {
        _settings = Globals.DialogSettings;
    }

    public void ActivateDialog(DialogData dialogData, bool isFinished)
    {
        _currentDialogData = dialogData;
        _currentLine = 0;
        _avatar.sprite = dialogData.NPC.Avatar;
        _avatar.SetNativeSize();
        _name.text = dialogData.NPC.Name;
        _text.text = "";
        _dialogActive = true;
        StartCoroutine(StartDialog(isFinished));
    }

    public void OnContinueClicked()
    {
        if (!_dialogActive)
            return;

        if (_lineGoing)
        {
            StopCoroutine(_textCor);
            _lineGoing = false;
            _text.text = _currentText;
            return;
        }
        else
        {
            _continue = true;
        }
    }

    IEnumerator StartDialog(bool isFinished)
    {
        yield return new WaitForSecondsRealtime(_settings.DelayBeforeStart);

        if (isFinished)
        {
            _currentText = _currentDialogData.EndLine;
            yield return ShowLine(_currentDialogData.EndLine);
        }
        else
        {
            while (_currentLine < _currentDialogData.Lines.Count)
            {
                _currentText = _currentDialogData.Lines[_currentLine];
                yield return ShowLine(_currentDialogData.Lines[_currentLine]);
                _currentLine++;
            }
        }

        _dialogActive = false;
        OnDialogFinished?.Invoke();
    }

    IEnumerator ShowLine(string text)
    {
        _text.text = "";
        _lineGoing = true;
        _textCor = StartCoroutine(ShowTextInternal(text));

        if (_settings.AutoGoToNextLine)
        {
            yield return _textCor;
            yield return new WaitForSecondsRealtime(_settings.EndTime);
        }
        else
            yield return new WaitUntil(() => _continue);
        _continue = false;
    }

    IEnumerator ShowTextInternal(string text)
    {
        foreach (var letter in text)
            yield return StartCoroutine(ShowSymbol(letter));

        _lineGoing = false;
        if (_settings.AutoGoToNextLine)
        {
            float waitTime = Mathf.Clamp(text.Length * 0.07f, 0.5f, 1.2f);
            yield return new WaitForSecondsRealtime(waitTime);
        }
    }

    IEnumerator ShowSymbol(char symbol)
    {
        _text.text += symbol;
        if (symbol == '.')  // TODO make static objects instead of new when time established
        {
            yield return new WaitForSecondsRealtime(_settings.DotTime);
        }
        else if (symbol == ',')
        {
            yield return new WaitForSecondsRealtime(_settings.CommaTime);
        }
        else if (symbol == ' ')
        {
            yield return new WaitForSecondsRealtime(_settings.SpaceTime);
        }
        else
            yield return new WaitForSecondsRealtime(_settings.LetterTime);
    }
}
