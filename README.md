
# SC Češtinátor Avalonia

Linux GUI nástroj pro instalaci a správu české lokalizace do Star Citizen.

Toto je nová verze aplikace napsaná v C# / Avalonia UI.

Původní Python verze:
https://github.com/1walkerit/sc-cestinator-linux

## Funkce

- instalace a aktualizace české lokalizace
- odinstalace češtiny se zálohou původního souboru
- detekce lokální a online verze
- výběr složky Star Citizen
- uložení poslední použité cesty
- užitečné odkazy v horním menu
- moderní grafické rozhraní pro Linux

## Screenshot

<img width="1243" height="716" alt="image" src="https://github.com/user-attachments/assets/80d2928b-00ea-4486-bac0-ee9ee6959174" />


## Požadavky

Pro běh ze zdrojového kódu:

- .NET 8 SDK
- Linux

## Spuštění ze zdrojového kódu

```bash
dotnet run --project src/ScCestinator/ScCestinator.csproj
```

## ▶️ Spuštění (Linux)

```bash
chmod +x SC_Češtinátor-x86_64.AppImage
./SC_Češtinátor-x86_64.AppImage
