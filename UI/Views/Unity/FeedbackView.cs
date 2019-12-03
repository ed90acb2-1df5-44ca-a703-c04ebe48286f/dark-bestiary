using System;
using System.Collections.Generic;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    [Serializable]
    public class FeedbackResponse
    {
        public string message;
    }

    public class FeedbackView : View, IFeedbackView
    {
        public event Payload<FeedbackRequest> Submitted;

        [SerializeField] private TMP_Dropdown subject;
        [SerializeField] private TMP_InputField title;
        [SerializeField] private TMP_InputField text;
        [SerializeField] private Interactable close;
        [SerializeField] private Interactable submit;

        private static readonly List<string> Subjects = new List<string> {"Feedback", "Suggestion", "Bug"};

        private void Start()
        {
            this.close.PointerUp += Hide;
            this.submit.PointerUp += OnSubmit;
            this.submit.Active = false;

            this.subject.ClearOptions();
            this.subject.AddOptions(Subjects);

            this.title.onValueChanged.AddListener(OnTitleChanged);
            this.text.onValueChanged.AddListener(OnTextChanged);
        }

        private void OnTitleChanged(string value)
        {
            this.submit.Active = !string.IsNullOrEmpty(value) &&
                                 !string.IsNullOrEmpty(this.text.text);
        }

        private void OnTextChanged(string value)
        {
            this.submit.Active = !string.IsNullOrEmpty(value) &&
                                 !string.IsNullOrEmpty(this.title.text);
        }

        public void SetFormFieldsInteractable(bool interactable)
        {
            this.subject.interactable = interactable;
            this.title.interactable = interactable;
            this.text.interactable = interactable;
            this.submit.Active = interactable;
        }

        public void ClearFormFields()
        {
            this.title.text = "";
            this.text.text = "";
        }

        private void OnSubmit()
        {
            Submitted?.Invoke(new FeedbackRequest(
                Subjects[this.subject.value],
                this.title.text,
                this.text.text
            ));
        }
    }
}