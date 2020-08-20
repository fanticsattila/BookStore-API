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