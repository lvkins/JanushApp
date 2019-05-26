using System.IO;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace PromoSeeker
{
    public class NotificationManager
    {
        private ToastNotifier _notifier;

        /// <summary>
        /// <see cref="https://docs.microsoft.com/en-us/uwp/schemas/tiles/toastschema/element-audio"/>
        /// </summary>
        public static string Sound { get; } = "ms-winsoundevent:Notification.Looping.Alarm8";

        private static XmlDocument ProductNotificationTemplate(int type = 0)
        {
            // Get a toast XML template
            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText04);

            // Set toast attributes
            var toast = toastXml.FirstChild as XmlElement;
            toast.SetAttribute("duration", "long");

            // Fill in the text elements
            var stringElements = toastXml.GetElementsByTagName("text");
            //stringElements[0].AppendChild(toastXml.CreateTextNode($"Hello World {type}"));

            for (var i = 0; i < stringElements.Length; i++)
            {
                stringElements[i].AppendChild(toastXml.CreateTextNode($"Line {i} Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n\nKurła."));
            }

            // Specify the absolute path to an image
            var imagePath = "file:///" + Path.GetFullPath("../../Assets/Application.ico");
            var remoteImagePath = @"https://picsum.photos/id/883/48/48";

            var binding = toastXml.GetElementsByTagName("binding")[0];
            binding.Attributes.GetNamedItem("template").NodeValue = "ToastBlah";

            var imageElements = toastXml.GetElementsByTagName("image");
            //imageElements[0].Attributes.GetNamedItem("src").NodeValue = imagePath;
            //imageElements[0].Attributes.GetNamedItem("placement").NodeValue = "appLogoOverride";

            binding.RemoveChild(imageElements[0]);

            var logo = toastXml.CreateElement("image");
            logo.SetAttribute("src", remoteImagePath);
            logo.SetAttribute("placement", "appLogoOverride");
            binding.AppendChild(logo);
            //toastXml.DocumentElement.AppendChild(logo);

            // Create actions element
            var actions = toastXml.CreateElement("actions");
            toastXml.DocumentElement.AppendChild(actions);

            // Create action element
            var action = toastXml.CreateElement("action");
            action.SetAttribute("content", "Show details");
            action.SetAttribute("arguments", "viewdetails");
            action.SetAttribute("imageUri", @"E:\MT2\VSProjects\PromoSeeker\PromoSeeker\Assets\Application.ico");
            actions.AppendChild(action);

            // Create audio element
            var audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", Sound);
            audio.SetAttribute("loop", "true");
            toastXml.DocumentElement.AppendChild(audio);

            return toastXml;
        }

        public NotificationManager()
        {
            // Initialize toast notifier
            _notifier = ToastNotificationManager.CreateToastNotifier(System.Reflection.Assembly.GetEntryAssembly().Location);
        }

        public void SendNotification()
        {
            _notifier.Show(new ToastNotification(ProductNotificationTemplate(1)));
        }
    }
}
