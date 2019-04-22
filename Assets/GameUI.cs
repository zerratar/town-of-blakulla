using System;
using System.Collections.Concurrent;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    private readonly ConcurrentQueue<GameMessage> messageQueue
        = new ConcurrentQueue<GameMessage>();

    private float lastMessageChanged = 0f;

    private GameMessage currentMessage;

    public TextMeshProUGUI lblMessage;

    public TextMeshProUGUI lastWillText;

    public GameObject lastWill;


    // Start is called before the first frame update
    void Start()
    {
        if (!lblMessage) lblMessage = GameObject.Find("lblMessage").GetComponent<TextMeshProUGUI>();
        if (!lastWillText) lastWillText = GameObject.Find("lblLastWill").GetComponent<TextMeshProUGUI>();
        if (!lastWill) lastWill = GameObject.Find("lastWill");
    }

    // Update is called once per frame
    void Update()
    {
        if (currentMessage != null)
        {
            var expired = Time.time - lastMessageChanged > currentMessage.Duration;
            if ((currentMessage.ShouldHide != null || !expired) &&
                (currentMessage.ShouldHide == null || !currentMessage.ShouldHide()))
            {
                return;
            }

            currentMessage.Hide();
        }

        if (messageQueue.TryDequeue(out currentMessage))
        {
            lastMessageChanged = Time.time;
            currentMessage.Show();
        }
    }

    public void SetMessagePositionDefault()
    {
        this.lblMessage.rectTransform.localPosition = new Vector3(0, 188.7f);
    }

    public void SetMessagePositionGallows()
    {
        this.lblMessage.rectTransform.localPosition = new Vector3(0, 58.4f);
    }

    public int QueuedMessageCount => messageQueue.Count;

    public void ShowMessage(string msg, float duration, Func<bool> shouldHide = null)
    {
        this.messageQueue.Enqueue(new Notification(msg, duration, shouldHide, this));
    }

    public void ShowLastWill(string lastWill, float duration, Func<bool> shouldHide = null)
    {
        this.messageQueue.Enqueue(new LastWill(lastWill, duration, shouldHide, this));
    }

    public void ShowDeathNote(string deathNote, float duration, Func<bool> shouldHide = null)
    {
        this.messageQueue.Enqueue(new DeathNote(deathNote, duration, shouldHide, this));
    }

    private class Notification : GameMessage
    {
        public Notification(string message, float duration, Func<bool> shouldHide, GameUI ui)
            : base(message, duration, shouldHide, ui) { }

        public override void Hide()
        {
            Ui.lblMessage.text = "";
        }

        public override void Show()
        {
            Ui.lblMessage.text = Message;
        }
    }

    private class LastWill : GameMessage
    {
        public LastWill(string message, float duration, Func<bool> shouldHide, GameUI ui)
            : base(message, duration, shouldHide, ui) { }

        public override void Hide()
        {
            this.Ui.lastWill.SetActive(false);
        }

        public override void Show()
        {
            this.Ui.lastWill.SetActive(true);
            this.Ui.lastWillText.text = Message;
        }
    }

    private class DeathNote : GameMessage
    {
        public DeathNote(string message, float duration, Func<bool> shouldHide, GameUI ui)
            : base(message, duration, shouldHide, ui) { }

        public override void Hide()
        {
        }

        public override void Show()
        {
        }
    }

    private abstract class GameMessage
    {
        public string Message { get; }
        public float Duration { get; }
        public Func<bool> ShouldHide { get; }
        public GameUI Ui { get; }

        public GameMessage(
            string message,
            float duration,
            Func<bool> shouldHide,
            GameUI ui)
        {
            Message = message;
            Duration = duration;
            ShouldHide = shouldHide;
            Ui = ui;
        }

        public abstract void Hide();

        public abstract void Show();
    }
}
