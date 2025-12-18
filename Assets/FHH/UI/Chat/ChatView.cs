using Cysharp.Threading.Tasks;
using Foxbyte.Presentation;
using UnityEngine;
using UnityEngine.UIElements;

namespace FHH.UI.Chat
{
    public sealed class ChatView : ViewBase<ChatPresenter>
    {
        protected override string LocalizationTableName => "GeneralTable";
        protected override bool AutoHideOnClickOutside => false;
        protected override bool IsModal => false;

        public VisualElement ChatRoot { get; set; }
        public ScrollView ChatScrollView { get; set; }
        public VisualElement MessagesContainer { get; set; }
        public TextField InputField { get; set; }
        public Button SendButton { get; set; }

        protected override async UniTask<VisualElement> SetUpVisualElement(UIDocument uiDocument, VisualElement targetContainer)
        {
            var generator = ViewGeneratorBase<ChatPresenter, ChatView>.Create<ChatViewGenerator>(this);
            var root = await generator.GenerateViewAsync();

            CacheUiReferences(root);
            RegisterInputFieldCallback();
            return root;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnregisterInputFieldCallback();
        }

        private void CacheUiReferences(VisualElement root)
        {
            ChatRoot = root;
            ChatScrollView = root.Q<ScrollView>("ChatScrollView");
            MessagesContainer = root.Q<VisualElement>("ChatMessagesContainer");
            InputField = root.Q<TextField>("ChatInputField");
            SendButton = root.Q<Button>("ChatSendButton");
        }

        private void RegisterInputFieldCallback()
        {
            if (InputField == null) return;
            InputField.RegisterCallback<FocusInEvent>(OnFieldFocusIn);
            InputField.RegisterCallback<FocusOutEvent>(OnFieldFocusOut);
        }

        private void UnregisterInputFieldCallback()
        {
            if (InputField == null) return;
            InputField.UnregisterCallback<FocusInEvent>(OnFieldFocusIn);
            InputField.UnregisterCallback<FocusOutEvent>(OnFieldFocusOut);
        }

        private void OnFieldFocusIn(FocusInEvent evt)
        {
            Presenter?.OnInputFieldFocusIn();
        }

        private void OnFieldFocusOut(FocusOutEvent evt)
        {
            Presenter?.OnInputFieldFocusOut();
        }

        public void AppendMessage(string senderName, string timestampText, string messageText, bool fromSelf)
        {
            if (MessagesContainer == null)
            {
                return;
            }

            var messageRoot = new VisualElement
            {
                name = "ChatMessage"
            };
            messageRoot.AddToClassList("i18n-skip");
            messageRoot.AddToClassList("chat-message");
            if (fromSelf)
            {
                messageRoot.AddToClassList("chat-message-self");
            }
            else
            {
                messageRoot.AddToClassList("chat-message-remote");
            }

            var headerRow = new VisualElement
            {
                name = "ChatMessageHeaderRow"
            };
            headerRow.AddToClassList("i18n-skip");
            headerRow.AddToClassList("row");
            headerRow.AddToClassList("chat-message-header-row");

            var nameLabel = new Label(senderName ?? string.Empty)
            {
                name = "ChatMessageSender"
            };
            nameLabel.AddToClassList("i18n-skip");
            nameLabel.AddToClassList("chat-message-sender");

            var timeLabel = new Label(timestampText ?? string.Empty)
            {
                name = "ChatMessageTimestamp"
            };
            timeLabel.AddToClassList("i18n-skip");
            timeLabel.AddToClassList("chat-message-timestamp");

            headerRow.Add(nameLabel);
            headerRow.Add(timeLabel);

            var bodyLabel = new Label(messageText ?? string.Empty)
            {
                name = "ChatMessageBody"
            };
            bodyLabel.AddToClassList("i18n-skip");
            bodyLabel.AddToClassList("chat-message-body");

            messageRoot.Add(headerRow);
            messageRoot.Add(bodyLabel);

            MessagesContainer.Add(messageRoot);

            if (ChatScrollView == null) return;

            ChatScrollView.schedule.Execute(() =>
            {
                ChatScrollView.ScrollTo(messageRoot);
            }).ExecuteLater(100); // 100-frame delay for layout

            //if (ChatScrollView != null)
            //{
            //    ChatScrollView.ScrollTo(messageRoot);
            //}
        }
    }
}