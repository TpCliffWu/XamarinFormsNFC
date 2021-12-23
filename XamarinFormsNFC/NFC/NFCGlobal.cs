using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinFormsNFC.NFC
{
    /// <summary>
    /// nfc功能 全域變數
    /// </summary>
    public class NFCGlobal : BindableBase
    {
        public static NFCGlobal Current = new NFCGlobal()
        {
            NfcContent = "",
            NfcFunctionRunning = false
        };

        /// <summary>
        /// NFC 讀取/寫入的文字
        /// </summary>
        private string nfcContent;
        public string NfcContent
        {
            get { return nfcContent; }
            set { SetProperty(ref nfcContent, value); }
        }


        /// <summary>
        /// NFC執行log
        /// </summary>
        private string nfcLog;
        public string NfcLog
        {
            get { return nfcLog; }
            set { SetProperty(ref nfcLog, value); }
        }

        /// <summary>
        /// nfc 功能執行中
        /// </summary>
        private bool nfcFunctionRunning;
        public bool NfcFunctionRunning
        {
            get { return nfcFunctionRunning; }
            set { SetProperty(ref nfcFunctionRunning, value); }
        }
    }
}
