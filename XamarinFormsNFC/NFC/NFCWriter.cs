using Plugin.NFC;
using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinFormsNFC.NFC
{
    /// <summary>
    /// 將文字寫入 nfc tag
    /// </summary>
    public class NFCWriter
    {
        public static void Start()
        {
            NFCGlobal.Current.NfcFunctionRunning = true;

            CrossNFC.Current.OnMessagePublished += Current_OnMessagePublished;
            CrossNFC.Current.OnTagDiscovered += Current_OnTagDiscovered;
            CrossNFC.Current.StartPublishing();
        }

        public static void Stop()
        {
            CrossNFC.Current.OnMessagePublished -= Current_OnMessagePublished;
            CrossNFC.Current.OnTagDiscovered -= Current_OnTagDiscovered;

            NFCGlobal.Current.NfcFunctionRunning = false;
        }

        static void Current_OnMessagePublished(ITagInfo tagInfo)
        {
            try
            {
                CrossNFC.Current.StopPublishing();
                if (tagInfo.IsEmpty)
                {
                    NFCGlobal.Current.NfcLog = "Formatting tag successfully";
                }
                else
                {
                    NFCGlobal.Current.NfcLog = "Writing tag successfully";
                }
            }
            catch (System.Exception ex)
            {
                NFCGlobal.Current.NfcLog = ex.Message;
            }
        }

        static async void Current_OnTagDiscovered(ITagInfo tagInfo, bool format)
        {
            if (!CrossNFC.Current.IsWritingTagSupported)
            {
                NFCGlobal.Current.NfcLog = "Writing tag is not supported on this device";
                return;
            }

            try
            {
                NFCNdefRecord record = new NFCNdefRecord
                {
                    TypeFormat = NFCNdefTypeFormat.Mime,
                    MimeType = "application/com.tpi.digicy",
                    Payload = NFCUtils.EncodeToByteArray($"{NFCGlobal.Current.NfcContent}") // 寫入的文字
                };


                if (!format && record == null)
                    throw new Exception("Record can't be null.");

                tagInfo.Records = new[] { record };

                if (format)
                {
                    NFCGlobal.Current.NfcLog = "Clear Tag";
                    // 清除標籤
                    CrossNFC.Current.ClearMessage(tagInfo);
                }

                else
                {
                    NFCGlobal.Current.NfcLog = "Wrtie Tag";
                    //寫標籤
                    CrossNFC.Current.PublishMessage(tagInfo);
                }

                NFCWriter.Stop();
            }
            catch (System.Exception ex)
            {
                NFCGlobal.Current.NfcLog = ex.Message;
            }
        }
    }
}
