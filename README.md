# Rate-Limiter
This is repository used to store solution of be-recruiting task

U ovom repozitorijumu nalaze se 2 foldera: 
  RateLimiter - imlpementacija trazene biblioteke
  Miscellaneous - primer koda koji se koristi prilikom integracije sa postojecim API-em
  
Da bi se RateLimiter biblioteka uspesno koristila u WebAPI projektu i da bi neophodna logika bila dodata u Middleware pipeline, potrebno je dodati jednu custom middleware klasu (poput one iz foldera Miscellaneous) i refaktorisati Invoke metodu na sledeci nacin:

        public async Task Invoke(HttpContext httpContext)
        {
            _logger.LogInformation("IP address: " + httpContext.Connection.RemoteIpAddress.ToString());
            RateLimiterManager rateLimiterManager = RateLimiterManager.Instance;
            
            int statusCode = 0;
            rateLimiterManager.CheckIpAddress(httpContext.Connection.RemoteIpAddress.ToString(), ref statusCode);
            
            if (statusCode == 429)
                await httpContext.Response.WriteAsync("429 - Too many requests.");
            else
                await _next.Invoke(httpContext);
        }
        
Takodje potrebno je procitati konfiguraciju RateLimiter-a (prethodno dodatu u fajl appsettings.json) u okviru Startup.cs klase projekta:
 
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            rateLimiterManager = RateLimiterManager.Instance;
            rateLimiterManager.DefaultRequestLimitCount = configuration.GetSection("RateLimiter").GetValue<int>("DefaultRequestLimitCount");
            rateLimiterManager.DefaultRequestLimitMs = configuration.GetSection("RateLimiter").GetValue<int>("DefaultRequestLimitMs");
        }
        
   i dodati sledeci poziv unutar Configure metode pre drugih poziva:
    
    app.UseRateLimiterMiddleware();
    
Sama logika funkcionisanja biblioteke zamisljena je na sledeci nacin:
 Postoji glavna singleton klasa RateLimiterManager koja rasporedjuje i odrzava listu objekata klase RateLimiterIP koja cuva ip adresu sa koje je poslat zahtev i
 pridruzeni "klizni prozor" (SlidingWindow), odnosno algoritam po kome se na osnovu prosledjenih parametara broja dozvoljenih zahteva u intervalu  (DefaultRequestLimitCount), duzine trajanja intervala (DefaultRequestLimitMs) i vremena prvog poslatog zahteva sa te ip adrese, proverava da li je maksimalni broj  zahteva prekoracen ili ne.  
   
    
