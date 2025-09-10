

namespace MedXDataCollection
{
    public class DataCollection
    {
        public string? URL { get; set; }
        public string? TITLE { get; set; }
        public byte[] IMAGE { get; set; }
        public string? IMAGE_URL { get; set; }
        public string? NAME { get; set; }
        public string? STREANGTH { get; set; }
        public string? FULLNAME { get; private set; }
        public void FULLNAMESET()
        {
            FULLNAME = $"{NAME} {STREANGTH}";
        }
    }
}
