
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

<img width="1238" height="712" alt="screenshot-sc" src="https://github.com/user-attachments/assets/9271c155-fe6b-4f9b-858d-0fdc99ffe049" />

## Požadavky

Pro běh ze zdrojového kódu:

- .NET 8 SDK
- Linux

## Spuštění ze zdrojového kódu

```bash
dotnet run --project src/ScCestinator/ScCestinator.csproj
