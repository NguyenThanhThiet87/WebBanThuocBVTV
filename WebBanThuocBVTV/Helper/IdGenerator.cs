using Yitter.IdGenerator;

namespace WebBanThuocBVTV.Helper
{
    public class IdGeneratorHelper
    {
        private static IIdGenerator _idGenerator; // Fix: Use the correct interface type IIdGenerator

        public IdGeneratorHelper()
        {
            Setup();
        }

        public static void Setup()
        {
            var options = new IdGeneratorOptions
            {
                WorkerId = 1, // WorkerId (0-1023), tùy chỉnh theo máy chủ
                WorkerIdBitLength = 6, // Độ dài bit cho WorkerId (mặc định 6, tối đa 1023)
                SeqBitLength = 6, // Độ dài bit cho sequence (mặc định 6, tối đa 4095 ID/mili giây)
                BaseTime = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) // Fix: Use DateTime directly instead of its Ticks
            };
            YitIdHelper.SetIdGenerator(options); // Fix: Initialize the generator using YitIdHelper
            _idGenerator = YitIdHelper.IdGenInstance; // Fix: Assign the instance from YitIdHelper
        }

        public static long NextId()
        {
            return _idGenerator.NewLong();
        }
        public string GenerateOrderCode()
        {
            return NextId().ToString("X16").ToUpper(); // 16 ký tự hex
        }
    }
}
