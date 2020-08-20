# BookStore-API
A www.udemy.com-on található, Trevoir Williams által készített End to end ASP.NET Core 3.1 API and Blazor Development kurzus által elkészített demó alkalmazás.
Az oktatósorozat elérése:
https://www.udemy.com/course/end-to-end-aspnet-core-31-api-and-blazor-development

Megjegyzések:
1. Alap alkalmazás elkészítése
	- A varázslóban a "Web Application" sablont válasszuk, mert így lehet csak beállítani az "Individual User Accounts"-nál a "Store user accounts in-app" lehetőséget.
	  API esetén csak a már létező cloud-hoz csatlakozás lehetősége van megadva. Emiatt majd kell pár átalakítás a projekten.

2. Projekt átalakítása API-hoz
	- Töröljük a Web alkalmazáshoz kötődő mappákat (Areas, Pages, wwwroot), átalakítjuk a Startup.cs-t, valamint létrehozzuk a "HomeController"-t.

3. Swagger beüzemelése
	- Telepítsük a következő NuGet csomagokat: 
		Swashbuckle.AspNetCore.Swagger, 
		Swashbuckle.AspNetCore.SwaggerGen, 
		Swashbuckle.AspNetCore.SwaggerUi

	- A csomagokat a Startup.cs-ben konfiguráljuk be

	- Dokumentáció bekapcsolása: Projekt tulajdonságok, Build, Output, jelöljük be az XML documentation file-t.
	- Fordítási dokumentáció hiányát jelző warning-ok kikapcsolása: Suppress warnings-hoz adjuk hozzá: 1591

4. NLog bekötése a projektbe
	- Telepítsük fel a következő NuGet csomagot:
		NLog.Extensions.Logging
	- Hozzuk létre a konfigurációs fájlt (nlog.config)
	- Adjuk hozzá a LoggerService, ILoggerService fájlokat
	- Adjuk hozzá Singleton-ként a szolgáltatást a Startup.cs-ben
	- A Controller-ekben a HomeController-ben mutatott minta alapján használhatjuk

5. CORS beállítása
	- Startup.cs-ben állítsuk be a CORS szabályokat

6. Adatbázis létrehozása, beállítása a projekthez
	- MS SQL Szerverben futtassuk le az Adatbazis.sql szkriptet, ez létrehozza az adatbázist
	- appsettings.json-ban írjuk át a ConnectionString-et
	- ApplicationDbContext-ben hozzuk létre a táblákat, és az őket reprezentáló class-okat

6.5 Alternatíva a Scaffold-DbContext parancs a NuGet Package Manager Console-ban:
	Scaffold-DbContext "Server=(local);Database=BookStore;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Data
	





