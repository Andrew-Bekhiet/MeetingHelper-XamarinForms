using Xamarin.Forms;

namespace MeetingHelper.Models
{
    public class Contact
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PhoneN { get; set; }
        public string Address { get; set; }
        public ImageSource Image { get; set; }
        public override string ToString()
        {
            return Id + ";" + Name + ";" + PhoneN + ";" + Address;
        }
    }
}