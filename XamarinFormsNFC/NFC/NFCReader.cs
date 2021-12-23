using Plugin.NFC;
using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinFormsNFC.NFC
{
    /// <summary>
    /// 讀取nfc tag
    /// </summary>
    public class NFCReader
    {
        public static void Start()
        {
            NFCGlobal.Current.NfcFunctionRunning = true;

            CrossNFC.Current.OnMessageReceived += Current_OnMessageReceived;

            CrossNFC.Current.StartListening();
        }

        public static void Stop()
        {
            CrossNFC.Current.OnMessageReceived -= Current_OnMessageReceived;
            NFCGlobal.Current.NfcFunctionRunning = false;
        }

        static async void Current_OnMessageReceived(ITagInfo tagInfo)
        {
            if (tagInfo == null)
            {
                NFCGlobal.Current.NfcLog = "No tag found";
                NFCReader.Stop();
                return;
            }

            if (!tagInfo.IsSupported)
            {
                NFCGlobal.Current.NfcLog = "Unsupported tag";
                NFCReader.Stop();
            }
            else if (tagInfo.IsEmpty)
            {
                NFCGlobal.Current.NfcLog = "Empty tag";
                NFCReader.Stop();
            }
            else
            {
                var first = tagInfo.Records[0];
                NFCGlobal.Current.NfcLog = "NFC Reader Success";

                NFCGlobal.Current.NfcContent = first.Message;
                NFCReader.Stop();
            }
        }
    }
}
