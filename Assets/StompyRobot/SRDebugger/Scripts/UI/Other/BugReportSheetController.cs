﻿using System;
using System.Collections;
using System.Linq;
using SRDebugger.Internal;
using SRDebugger.Services;
using SRF;
using SRF.Service;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if NETFX_CORE
using UnityEngine.Windows;
#endif

namespace SRDebugger.UI.Other
{
    public class BugReportSheetController : SRMonoBehaviourEx
    {
        [RequiredField] public GameObject ButtonContainer;

        [RequiredField] public Text ButtonText;

        [RequiredField] public Button CancelButton;

		public System.Action CancelPressed;

        [RequiredField] public InputField DescriptionField;

        [RequiredField] public InputField EmailField;

        [RequiredField] public Slider ProgressBar;

        [RequiredField] public Text ResultMessageText;

		public System.Action ScreenshotComplete;

        [RequiredField] public Button SubmitButton;

        public Action<bool, string> SubmitComplete;
		public System.Action TakingScreenshot;

        public bool IsCancelButtonEnabled
        {
            get { return CancelButton.gameObject.activeSelf; }
            set { CancelButton.gameObject.SetActive(value); }
        }

        protected override void Start()
        {
            base.Start();

            SetLoadingSpinnerVisible(false);
            ClearErrorMessage();
        }

        public void Submit()
        {
            EventSystem.current.SetSelectedGameObject(null);

            ProgressBar.value = 0;
            ClearErrorMessage();
            SetLoadingSpinnerVisible(true);
            SetFormEnabled(false);

            StartCoroutine(SubmitCo());
        }

        public void Cancel()
        {
            if (CancelPressed != null)
            {
                CancelPressed();
            }
        }

        private IEnumerator SubmitCo()
        {
            if (BugReportScreenshotUtil.ScreenshotData == null)
            {
                if (TakingScreenshot != null)
                {
                    TakingScreenshot();
                }

                yield return new WaitForEndOfFrame();

                yield return StartCoroutine(BugReportScreenshotUtil.ScreenshotCaptureCo());

                if (ScreenshotComplete != null)
                {
                    ScreenshotComplete();
                }
            }

            var s = SRServiceManager.GetService<IBugReportService>();

            var r = new BugReport();

            r.Email = EmailField.text;
            r.UserDescription = DescriptionField.text;

            r.ConsoleLog = Service.Console.AllEntries.ToList();
            r.SystemInformation = SRServiceManager.GetService<ISystemInformationService>().CreateReport();
            r.ScreenshotData = BugReportScreenshotUtil.ScreenshotData;

            BugReportScreenshotUtil.ScreenshotData = null;

            s.SendBugReport(r, OnBugReportComplete, OnBugReportProgress);
        }

        private void OnBugReportProgress(float progress)
        {
            ProgressBar.value = progress;
        }

        private void OnBugReportComplete(bool didSucceed, string errorMessage)
        {
            if (!didSucceed)
            {
                ShowErrorMessage("Error sending bug report", errorMessage);
            }
            else
            {
                ClearForm();
                ShowErrorMessage("Bug report submitted successfully");
            }

            SetLoadingSpinnerVisible(false);
            SetFormEnabled(true);

            if (SubmitComplete != null)
            {
                SubmitComplete(didSucceed, errorMessage);
            }
        }

        protected void SetLoadingSpinnerVisible(bool visible)
        {
            ProgressBar.gameObject.SetActive(visible);
            ButtonContainer.SetActive(!visible);
        }

        protected void ClearForm()
        {
            EmailField.text = "";
            DescriptionField.text = "";
        }

        protected void ShowErrorMessage(string userMessage, string serverMessage = null)
        {
            var txt = userMessage;

            if (!string.IsNullOrEmpty(serverMessage))
            {
                txt += " (<b>{0}</b>)".Fmt(serverMessage);
            }

            ResultMessageText.text = txt;
            ResultMessageText.gameObject.SetActive(true);
        }

        protected void ClearErrorMessage()
        {
            ResultMessageText.text = "";
            ResultMessageText.gameObject.SetActive(false);
        }

        protected void SetFormEnabled(bool e)
        {
            SubmitButton.interactable = e;
            CancelButton.interactable = e;
            EmailField.interactable = e;
            DescriptionField.interactable = e;
        }
    }
}
