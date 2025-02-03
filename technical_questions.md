1. I've spent roughly 8 hours on the assignment.
-I would use a layered architecture for the solution. Now it is a small single purpose microservice, but if database related tasks will occur, this is mandatory.
-Add database context(Probably EF Core)- we can manage users, create profiles for them, etc.
-I would add more UT's to the solution: I didn't add service related UT's
-Better error handling and more appropriate unsuccessfull responses returned to user.
-I would enable distributed tracing.
-Much more stuff to do, if this will ever go live.

2. C#12:Primary Constructors- no need for defining explicit private fields. Reduces boilerplate code
-Before:
    private readonly CryptoPriceService _cryptoPriceService;
    private readonly ExchangeRateService _exchangeRateService;

    public CryptoQuoteService(CryptoPriceService cryptoPriceService, ExchangeRateService exchangeRateService)
    {
        _cryptoPriceService = cryptoPriceService;
        _exchangeRateService = exchangeRateService;
    }

-After:
    public class CryptoQuoteService(HttpClient _httpClient, IConfiguration _configuration)
    {
        
    }

3. Yes. I have tracked down performance issues in production. My regular pattern is the following:
-Identify the issues: determine what's slow or what is faulty. Check response times, CPU, Memory, Database queries execution times.
-Analyze logs and traces.
-Use profiling tools.
-Create a recovery/solution plan.

4. "I constantly read technical articles to stay up to speed with what's happening in this ever-evolving tech space. The last tech book I read was Enterprise Integration Patterns by Gregor Hohpe and Bobby Woolf. Even though this book is considered 'old,' it presents integration patterns that are still ubiquitously used in modern microservices and event-driven ecosystems."

5. A stright-forward assignment for testing development capabilities. At first glance, it seems a pretty easy task, but as you start coding, you start asking yourself questions about non-functional or functional requirements, and patterns/answers pop into your mind, that eventually get added into the code, making the app more resilient.

6. 
{
  "name": "Madalin Roventa",
  "age": 34,
  "intro": "Results-oriented Software Team Lead",
  "capabilities": [
    "Leadership & Team Management",
    "Software Development Expertise",
    "Excellent communicator",
    "Analytical mindset",
    "customer-focused problem-solving",
  ],
  "personalCapabilities": "father of two"
  "contact": {
    "github": "https://github.com/madar1",
    "linkedin": "https://www.linkedin.com/in/madalin-roventa/",
    "email": madalin.roventa@gmail.com
  }
}

