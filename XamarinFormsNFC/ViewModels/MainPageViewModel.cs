using Plugin.NFC;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XamarinFormsNFC.NFCFunction;

namespace XamarinFormsNFC.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public DelegateCommand ReadCommand { get; set; }
        public DelegateCommand WritingCommand { get; set; }


        public const string ALERT_TITLE = "NFC";
        public const string MIME_TYPE = "application/com.tp.nfc";
        NFCNdefTypeFormat _type;

        private string nfcContent;
        public string NfcContent
        {
            get { return nfcContent; }
            set { SetProperty(ref nfcContent, value); }
        }

        public MainPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "NFC";

            ReadCommand = new DelegateCommand(ReadNfcTag);
            WritingCommand = new DelegateCommand(WritingNfc);
        }

        private async void WritingNfc()
        {
            try
            {
                CrossNFC.Current.StartPublishing(true);
            }
            catch (System.Exception ex)
            {
                await ShowAlert(ex.Message);
            }
        }


        private async void ReadNfcTag()
        {
            if (CrossNFC.IsSupported)
            {
                //檢查 CrossNFC.Current.IsAvailable 以確認 NFC 是否可用。
                if (!CrossNFC.Current.IsAvailable)
                    await ShowAlert("NFC is not available");
                //檢查 CrossNFC.Current.IsEnabled 以確認是否啟用了 NFC。
                if (!CrossNFC.Current.IsEnabled)
                    await ShowAlert("NFC is disabled");
                //訂閱NFC事件
                SubscribeEvents();

                if (Device.RuntimePlatform != Device.iOS)
                {
                    // Start NFC tag listening manually
                    CrossNFC.Current.StartListening();
                }
            }
        }

        void SubscribeEvents()
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

        async void Current_OniOSReadingSessionCancelled(object sender, EventArgs e) => await ShowAlert("User has cancelled NFC reading session");

        async void Current_OnMessageReceived(ITagInfo tagInfo)
        {
            if (tagInfo == null)
            {
                await ShowAlert("No tag found");
                return;
            }

            // Customized serial number
            var identifier = tagInfo.Identifier;
            var serialNumber = NFCUtils.ByteArrayToHexString(identifier, ":");
            var title = !string.IsNullOrWhiteSpace(serialNumber) ? $"Tag [{serialNumber}]" : "Tag Info";

            if (!tagInfo.IsSupported)
            {
                await ShowAlert("Unsupported tag", title);
            }
            else if (tagInfo.IsEmpty)
            {
                await ShowAlert("Empty tag", title);
            }
            else
            {
                var first = tagInfo.Records[0];
                await ShowAlert(GetMessage(first), title);
            }
        }

        async void Current_OnMessagePublished(ITagInfo tagInfo)
        {
            try
            {
                CrossNFC.Current.StopPublishing();
                if (tagInfo.IsEmpty)
                    await ShowAlert("Formatting tag successfully");
                else
                    await ShowAlert("Writing tag successfully");
            }
            catch (System.Exception ex)
            {
                await ShowAlert(ex.Message);
            }
        }

        async void Current_OnTagDiscovered(ITagInfo tagInfo, bool format)
        {
            if (!CrossNFC.Current.IsWritingTagSupported)
            {
                await ShowAlert("Writing tag is not supported on this device");
                return;
            }

            try
            {
                NFCNdefRecord record = null;
                switch (_type)
                {
                    case NFCNdefTypeFormat.WellKnown:
                        record = new NFCNdefRecord
                        {
                            TypeFormat = NFCNdefTypeFormat.WellKnown,
                            MimeType = MIME_TYPE,
                            Payload = NFCUtils.EncodeToByteArray("This is a text message!")
                        };
                        break;
                    case NFCNdefTypeFormat.Uri:
                        record = new NFCNdefRecord
                        {
                            TypeFormat = NFCNdefTypeFormat.Uri,
                            Payload = NFCUtils.EncodeToByteArray("https://google.fr")
                        };
                        break;
                    case NFCNdefTypeFormat.Mime:
                        record = new NFCNdefRecord
                        {
                            TypeFormat = NFCNdefTypeFormat.Mime,
                            MimeType = MIME_TYPE,
                            Payload = NFCUtils.EncodeToByteArray("This is a custom record!")
                        };
                        break;
                    default:
                        break;
                }

                if (!format && record == null)
                    throw new Exception("Record can't be null.");

                tagInfo.Records = new[] { record };

                if (format)
                    //清除標籤
                    CrossNFC.Current.ClearMessage(tagInfo);
                else
                {
                    //寫標籤
                    CrossNFC.Current.PublishMessage(tagInfo);
                }
            }
            catch (System.Exception ex)
            {
                await ShowAlert(ex.Message);
            }
        }

        string GetMessage(NFCNdefRecord record)
        {
            var message = $"Message: {record.Message}";
            message += Environment.NewLine;
            message += $"RawMessage: {Encoding.UTF8.GetString(record.Payload)}";
            message += Environment.NewLine;
            message += $"Type: {record.TypeFormat.ToString()}";

            if (!string.IsNullOrWhiteSpace(record.MimeType))
            {
                message += Environment.NewLine;
                message += $"MimeType: {record.MimeType}";
            }

            return message;
        }
    }
}
