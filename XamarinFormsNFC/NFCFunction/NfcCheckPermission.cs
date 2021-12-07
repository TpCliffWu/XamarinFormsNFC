using Plugin.NFC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace  XamarinFormsNFC.NFCFunction
{
    public class NfcCheckPermission
    {
        public static async Task<NfcCheckPermissionResponse> CheckStatus()
        {
            var resp = new NfcCheckPermissionResponse();

            if (CrossNFC.IsSupported)
            {
                //檢查 CrossNFC.Current.IsAvailable 以確認 NFC 是否可用。
                if (!CrossNFC.Current.IsAvailable)
                {
                    resp.IsGranted = false;
                    resp.ErrorMessage = "NFC is not available";
                    return resp;
                }
                //檢查 CrossNFC.Current.IsEnabled 以確認是否啟用了 NFC。
                if (!CrossNFC.Current.IsEnabled)
                {
                    resp.IsGranted = false;
                    resp.ErrorMessage = "NFC is disabled";
                    return resp;
                }

                //訂閱NFC事件
                NfcEvents.SubscribeEvents();

                if (Device.RuntimePlatform != Device.iOS)
                {
                    // Start NFC tag listening manually
                    CrossNFC.Current.StartListening();
                }
            }

            resp.IsGranted = false;
            resp.ErrorMessage = "NFC not Supported";
            return resp;
        }
    }

    public class NfcCheckPermissionResponse
    {
        public bool IsGranted { get; set; }

        public string ErrorMessage { get; set; }
    }
}
