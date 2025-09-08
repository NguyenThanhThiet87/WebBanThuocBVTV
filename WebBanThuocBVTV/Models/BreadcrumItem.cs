namespace WebBanThuocBVTV.Models
{
    public class BreadcrumItem
    {
        public string Text { get; set; }
        public string Url { get; set; }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            BreadcrumItem other = (BreadcrumItem)obj;
            return Text == other.Text && Url == other.Url;
        }
    }
}
