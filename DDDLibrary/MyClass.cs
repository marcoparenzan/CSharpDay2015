
namespace TestBed
{
    class MyClass
    {
        public string A { get; } = "CIAO"; // Autoproperty Initializers

        public string That(string B) => $"{A} {B}";

        public void Do(string b)
        {
            // WriteLine($"{That(A)} {b} {nameof(b)}");
        }
    }
}
