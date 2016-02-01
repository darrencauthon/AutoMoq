using Moq;

namespace AutoMoq
{
    public class Config
    {
        public MockBehavior MockBehavior { get; set; }

        public Config()
        {
            MockBehavior = MockBehavior.Loose;
        }
    }
}