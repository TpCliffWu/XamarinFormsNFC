using Plugin.NFC;
using Prism.AppModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace XamarinFormsNFC.ViewModels
{
    public class MainPageViewModel : ViewModelBase, IPageLifecycleAware
    {
        public DelegateCommand ReadCommand { get; set; }
        public DelegateCommand WritingCommand { get; set; }
        public DelegateCommand StopCommand { get; set; }
        public DelegateCommand SetCommand { get; set; }


        private bool _nfcIsEnabled;
        public bool NfcIsEnabled
        {
            get { return _nfcIsEnabled; }
            set
            {
                SetProperty(ref _nfcIsEnabled, value);
                NfcIsDisabled = !value;
            }
        }

        private bool _nfcIsDisabled;
        public bool NfcIsDisabled
        {
            get { return _nfcIsDisabled; }
            set { SetProperty(ref _nfcIsDisabled, value); }
        }



        bool _makeReadOnly = false;
        bool _eventsAlreadySubscribed = false;
        bool _isDeviceiOS = false;

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
            NfcContent = "20211208";
            NfcIsEnabled = true;
            NfcIsDisabled = true;
            ReadCommand = new DelegateCommand(ReadNfcTag);
            WritingCommand = new DelegateCommand(WritingNfc);
            StopCommand = new DelegateCommand(Stop);
            SetCommand = new DelegateCommand(Set);
        }

        private void Set()
        {
            Preferences.Set("nfc_text", NfcContent);
        }

        public void OnAppearing()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                NFCSubscribe();
            });
        }

        public void OnDisappearing()
        {
        }


        private async void Stop()
        {
            try
            {
                CrossNFC.Current.StopListening();

            }
            catch (Exception ex)
            {
                await ShowAlert(ex.Message);
            }
        }


        public override void OnResume()
        {
            base.OnResume();
        }


        private async void NFCSubscribe()
        {
            if (CrossNFC.IsSupported)
            {
                if (!CrossNFC.Current.IsAvailable)
                    await ShowAlert("NFC is not available");

                NfcIsEnabled = CrossNFC.Current.IsEnabled;
                if (!NfcIsEnabled)
                    await ShowAlert("NFC is disabled");

                if (Device.RuntimePlatform == Device.iOS)
                    _isDeviceiOS = true;

                SubscribeEvents();

                await StartListeningIfNotiOS();
            }
        }

        private async void ReadNfcTag()
        {
            try
            {
                CrossNFC.Current.StartListening();
            }
            catch (System.Exception ex)
            {
                //await ShowAlert(ex.Message);
                Console.WriteLine($"{ex.Message}");
            }
        }

        private async void WritingNfc()
        {
            await Publish(NFCNdefTypeFormat.Mime);
        }

        async Task Publish(NFCNdefTypeFormat? type = null)
        {
            await StartListeningIfNotiOS();
            try
            {
                //if (ChkReadOnly.IsChecked)
                //{
                //    if (!await DisplayAlert("Warning", "Make a Tag read-only operation is permanent and can't be undone. Are you sure you wish to continue?", "Yes", "No"))
                //    {
                //        ChkReadOnly.IsChecked = false;
                //        return;
                //    }
                //    _makeReadOnly = true;
                //}
                //else
                //    _makeReadOnly = false;

                if (type.HasValue) _type = type.Value;
                CrossNFC.Current.StartPublishing(!type.HasValue);
            }
            catch (Exception ex)
            {
                await ShowAlert(ex.Message);
            }
        }

        async Task StartListeningIfNotiOS()
        {
            if (_isDeviceiOS)
                return;
            await BeginListening();
        }

        async Task BeginListening()
        {
            try
            {
                CrossNFC.Current.StartListening();
            }
            catch (Exception ex)
            {
                await ShowAlert(ex.Message);
            }
        }

        // 檢查權限
        public async Task<bool> CheckPermission()
        {
            if (CrossNFC.IsSupported)
            {
                //檢查 CrossNFC.Current.IsAvailable 以確認 NFC 是否可用。
                if (!CrossNFC.Current.IsAvailable)
                {
                    await ShowAlert("NFC is not available");
                    return false;
                }

                //檢查 CrossNFC.Current.IsEnabled 以確認是否啟用了 NFC。
                if (!CrossNFC.Current.IsEnabled)
                {
                    await ShowAlert("NFC is disabled");
                    return false;
                }

                //訂閱NFC事件
                SubscribeEvents();

                if (Device.RuntimePlatform != Device.iOS)
                {
                    // Start NFC tag listening manua\ly
                    CrossNFC.Current.StartListening();
                }
                return true;
            }
            return false;
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

        void UnsubscribeEvents()
        {
            CrossNFC.Current.OnMessageReceived -= Current_OnMessageReceived;
            CrossNFC.Current.OnMessagePublished -= Current_OnMessagePublished;
            CrossNFC.Current.OnTagDiscovered -= Current_OnTagDiscovered;

            if (Device.RuntimePlatform == Device.iOS)
                CrossNFC.Current.OniOSReadingSessionCancelled -= Current_OniOSReadingSessionCancelled;
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
                //   await ShowAlert("Unsupported tag", title);
                Console.WriteLine($"Unsupported tag");
            }
            else if (tagInfo.IsEmpty)
            {
             //   await ShowAlert("Empty tag", title);
                Console.WriteLine($"Empty tag");
            }
            else
            {
                var first = tagInfo.Records[0];
             //   await ShowAlert(GetMessage(first), title);
    
                this.NfcContent = first.Message;
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
                            Payload = NFCUtils.EncodeToByteArray("Plugin.NFC is awesome!"),
                            LanguageCode = "en"
                        };
                        break;
                    case NFCNdefTypeFormat.Uri:
                        record = new NFCNdefRecord
                        {
                            TypeFormat = NFCNdefTypeFormat.Uri,
                            Payload = NFCUtils.EncodeToByteArray("https://github.com/franckbour/Plugin.NFC")
                        };
                        break;
                    case NFCNdefTypeFormat.Mime:
                        record = new NFCNdefRecord
                        {
                            TypeFormat = NFCNdefTypeFormat.Mime,
                            MimeType = MIME_TYPE,
                            Payload = NFCUtils.EncodeToByteArray($"{NfcContent}")
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