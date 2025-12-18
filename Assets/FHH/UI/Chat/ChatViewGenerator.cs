using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.Chat
{
    public sealed class ChatViewGenerator : ViewGeneratorBase<ChatPresenter, ChatView>
    {
        public override async UniTask<VisualElement> GenerateViewAsync()
        {
            var root = new VisualElement
            {
                name = "ChatRoot"
            };
            root.AddToClassList("i18n-skip");
            root.AddToClassList("chat-root");
            root.AddToClassList("col");

            // Chat area (scroll view)
            var scrollView = new ScrollView(ScrollViewMode.Vertical)
            {
                name = "ChatScrollView"
            };
            scrollView.AddToClassList("i18n-skip");
            scrollView.AddToClassList("chat-messages-scroll");

            // Container inside scroll view for messages (finite bounds for text wrapping):contentReference[oaicite:2]{index=2}
            var messagesContainer = new VisualElement
            {
                name = "ChatMessagesContainer"
            };
            messagesContainer.AddToClassList("i18n-skip");
            messagesContainer.AddToClassList("chat-messages-container");
            messagesContainer.AddToClassList("col");

            scrollView.Add(messagesContainer);

            // Input row
            var inputRow = new VisualElement
            {
                name = "ChatInputRow"
            };
            inputRow.AddToClassList("i18n-skip");
            inputRow.AddToClassList("chat-input-row");
            inputRow.AddToClassList("row");

            var inputField = new TextField
            {
                name = "ChatInputField",
                multiline = false
            };
            inputField.AddToClassList("i18n-skip");
            inputField.AddToClassList("chat-input-field");

            var sendButton = new Button
            {
                name = "ChatSendButton"
            };
            sendButton.AddToClassList("i18n-skip");
            sendButton.AddToClassList("chat-send-button");

            // Icon-only button content
            var iconElement = new VisualElement
            {
                name = "ChatSendIcon"
            };
            iconElement.AddToClassList("i18n-skip");
            iconElement.AddToClassList("chat-send-icon");

            var sendTexture = Resources.Load<Texture2D>("Icons/MeetingBar/send");
            if (sendTexture != null)
            {
                iconElement.style.backgroundImage = new StyleBackground(sendTexture);
            }

            sendButton.Add(iconElement);

            inputRow.Add(inputField);
            inputRow.Add(sendButton);

            root.Add(scrollView);
            root.Add(inputRow);

            // Wire into view for easier access from presenter
            View.ChatRoot = root;
            View.ChatScrollView = scrollView;
            View.MessagesContainer = messagesContainer;
            View.InputField = inputField;
            View.SendButton = sendButton;

            await UniTask.CompletedTask;
            return root;
        }
    }
}