using Android.App;
using Android.Content;
using Android.Nfc;
using Android.Nfc.CardEmulators;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;
using XamarinFormsNFC.Droid.NFC;

namespace XamarinFormsNFC.Droid
{
    [Service(
        Exported = true,
        Enabled = true,
        Permission = "android.permission.BIND_NFC_SERVICE"),
  IntentFilter(actions: new[] { "android.nfc.cardemulation.action.HOST_APDU_SERVICE" }),
  MetaData("android.nfc.cardemulation.host_apdu_service", Resource = "@xml/apduservice")]
    public class MyHostApduService : HostApduService
    {

        public override void OnCreate()
        {
            base.OnCreate();

            string text = Preferences.Get("nfc_text", "NO DATA");

            var NDEF_ID = new byte[] { 0xE1, 0x04 };
            var textBtye = Encoding.UTF8.GetBytes(text); // 文字byte

            var lang = "en";
            var byteLang = Encoding.ASCII.GetBytes(lang);

            var byteLangSizeString = byteLang.Length.ToString("X2");    // 語系byte長度

            var sizeLangBtye = byteLangSizeString.StringToByteArray(); // 語系byte

            var _payload1 = sizeLangBtye.Concat(byteLang); // 語系長度 + 語系
            var _payload = _payload1.Concat(textBtye).ToArray(); // 語系長度 + 語系 + 傳輸文字

            // 建立android nfc 物件
            var ndefRecord = new NdefRecord(tnf: NdefRecord.TnfWellKnown,
                                    type: NdefRecord.RtdText.ToArray(),
                                    id: NDEF_ID,
                                    payload: _payload);

            var ndefMsg = new NdefMessage(ndefRecord);

            this.NDEF_URI_BYTES = ndefMsg.ToByteArray();

            Log.Info(TAG, $"NDEF_URI_BYTES: {BitConverter.ToString(NDEF_URI_BYTES)}");


            var startByte = new byte[] { 0x00 };
            var hexSizeString = NDEF_URI_BYTES.Length.ToString("X2"); // 傳輸物件長度
            var sizeBtye = hexSizeString.StringToByteArray();
            this.NDEF_URI_LEN = startByte.Concat(sizeBtye).ToArray();

            Log.Info(TAG, $"NDEF_URI_LEN: {BitConverter.ToString(NDEF_URI_LEN)}");
        }

        private byte[] NDEF_URI_LEN { get; set; }

        private byte[] NDEF_URI_BYTES { get; set; }


        private const String TAG = "MyHostApduService";

        public override void OnDeactivated([GeneratedEnum] DeactivationReason reason)
        {

        }

        private bool READ_CAPABILITY_CONTAINER_CHECK = false;
        public override byte[] ProcessCommandApdu(byte[] commandApdu, Bundle extras)
        {
            var _hexCommandApdu = BitConverter.ToString(commandApdu);

            // 紀錄傳入的Command
            Log.Info(TAG, $"#0 commandApdu: {_hexCommandApdu}");

            if (!(commandApdu?.Length > 0))
            {
                Log.Info(TAG, "#999");
                return APDUResponse.UNKNOWN_CMD_SW;
            }




            /// <summary>
            /// CLA - (1) -  Class of instruction
            /// INS - (1) - Instruction code
            /// P1 -  (1) - Instruction parameter 1
            /// P2 -  (1) - Instruction parameter 2 
            /// Lc - (0,1,3) - field	-Number of bytes present in the data field of the command
            /// Data field- (Lc) -	 -String of bytes sent in the data field of the command
            /// Le field - (0,1,3) --Maximum number of bytes expected in the data field of the response to the command
            /// </summary>
            switch (_hexCommandApdu)
            {
                case "00-A4-04-00-07-D2-76-00-00-85-01-01-00":
                    // @xml/apduservice  D2760000850101
                    Log.Info(TAG, $"#1");
                    return APDUResponse.A_OKAY;

                case "00-A4-00-0C-02-E1-03":
                    Log.Info(TAG, $"#2");
                    return APDUResponse.A_OKAY;

                case "00-B0-00-00-0F":
                    if (!READ_CAPABILITY_CONTAINER_CHECK)
                    {
                        Log.Info(TAG, $"#3");
                        READ_CAPABILITY_CONTAINER_CHECK = true;
                        return APDUResponse.READ_CAPABILITY_CONTAINER_RESPONSE;
                    }
                    break;

                case "00-A4-00-0C-02-E1-04":
                    Log.Info(TAG, $"#4");
                    return APDUResponse.A_OKAY;

                case "00-B0-00-00-02":

                    var resp = NDEF_URI_LEN.Concat(APDUResponse.A_OKAY).ToArray();

                    Log.Info(TAG, $"#5 : {BitConverter.ToString(resp)}");

                    READ_CAPABILITY_CONTAINER_CHECK = false;
                    return resp;
            }

            var top2apddu = BitConverter.ToString(commandApdu.Take(2).ToArray());
            if (top2apddu == "00-B0")
            {
                // 完整訊息
                var fullResponse = NDEF_URI_LEN.Concat(NDEF_URI_BYTES).ToArray();

                // 切割訊息
                // 開始序列位置
                var offsetByte = commandApdu.Skip(2).Take(2).ToArray();
                Log.Info(TAG, $"#6 offsetByte :{ BitConverter.ToString(offsetByte)}");

                var offsetHex = BitConverter.ToString(offsetByte).Replace("-", "");
                var offset= Int32.Parse(offsetHex, System.Globalization.NumberStyles.HexNumber);

                // 序列長度
                var lengthByte = commandApdu.Skip(4).Take(1).ToArray();

                Log.Info(TAG, $"#6 lengthByte :{ BitConverter.ToString(lengthByte)}");

                var lengthHex = BitConverter.ToString(lengthByte).Replace("-", "");
                var length = Int32.Parse(lengthHex, System.Globalization.NumberStyles.HexNumber);

                // 切割訊息
                var slicedResponse = fullResponse.Skip(offset).Take(length).ToArray();

                Log.Info(TAG, $"#6 fullResponse { BitConverter.ToString(fullResponse)}");
                Log.Info(TAG, $"#6 offset :{offset}, length:{length}");

                READ_CAPABILITY_CONTAINER_CHECK = false;
                var resp = slicedResponse.Concat(APDUResponse.A_OKAY).ToArray();

                Log.Info(TAG, $"#6 { BitConverter.ToString(resp)}");
                return resp;
            }

            READ_CAPABILITY_CONTAINER_CHECK = false;

            Log.Info(TAG, "#999");
            return APDUResponse.UNKNOWN_CMD_SW;
        }
    }
}


// https://stackoverflow.com/questions/24587618/nfc-tag-4-protocol-questions