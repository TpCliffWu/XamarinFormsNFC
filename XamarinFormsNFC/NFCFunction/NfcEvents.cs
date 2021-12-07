using Plugin.NFC;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace XamarinFormsNFC.NFCFunction
{
    public class NfcEvents
    {
        public static void SubscribeEvents()
        {
            //收到 ndef 消息時引發的事件。
            CrossNFC.Current.OnMessageReceived += Current_OnMessageReceived;
            //發布 ndef 消息時引發的事件。
            CrossNFC.Current.OnMessagePublished += Current_OnMessagePublished;
            //發現標記時引發的事件。用於發布。
            CrossNFC.Current.OnTagDiscovered += Current_OnTagDiscovered;

            if (Device.RuntimePlatform == Device.iOS)
                CrossNFC.Current.OniOSReadingSessionCancelled += Current_OniOSReadingSessionCancelled;
        }

        private static void Current_OniOSReadingSessionCancelled(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void Current_OnTagDiscovered(ITagInfo tagInfo, bool format)
        {
            throw new NotImplementedException();
        }

        private static void Current_OnMessagePublished(ITagInfo tagInfo)
        {
            throw new NotImplementedException();
        }

        private static void Current_OnMessageReceived(ITagInfo tagInfo)
        {
            throw new NotImplementedException();
        }
    }
}
