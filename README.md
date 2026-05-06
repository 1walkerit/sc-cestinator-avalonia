# SC CZ Toolkit – Star Citizen Linux Utility

![AppImage Build](https://github.com/1walkerit/sc-cz-toolkit/actions/workflows/build-appimage.yml/badge.svg)
![Flatpak Build](https://github.com/1walkerit/sc-cz-toolkit/actions/workflows/build-flatpak.yml/badge.svg)
![Release](https://img.shields.io/github/v/release/1walkerit/sc-cz-toolkit)
![Downloads](https://img.shields.io/github/downloads/1walkerit/sc-cz-toolkit/total)

Jednoduchý nástroj pro instalaci české lokalizace do hry Star Citizen na Linuxu.

> ⚠️ Tento projekt není oficiální součástí projektu Cestinator ani s ním není přímo spojen. Jedná se o komunitní nástroj pro Linux.

<img width="1252" height="735" alt="image" src="https://github.com/user-attachments/assets/91bbf008-83d4-4519-93f3-726edf77fb31" />

---

## ✨ Co aplikace dělá

- stáhne aktuální českou lokalizaci
- nainstaluje ji do hry Star Citizen
- podporuje LIVE / PTU / EPTU větve
- automaticky detekuje instalaci hry
- funguje jako AppImage i Flatpak aplikace
- integruje se do Linux desktop prostředí
- neprovádí žádné úpravy lokalizačních souborů

---

## 🐧 Pro koho je to určeno

- hráči Star Citizen na Linuxu (LUG / Wine / Proton)
- uživatelé, kteří chtějí jednoduchou instalaci češtiny bez ručních zásahů

---

## 📦 Použitá lokalizace (CZ)

Tento projekt využívá českou lokalizaci vytvořenou komunitním týmem kolem projektu Cestinator.

👉 Zdroj lokalizace:
https://github.com/JarredSC/Star-Citizen-CZ-lokalizace

Veškerá práva k lokalizačním souborům náleží jejich autorům.
Tento nástroj slouží pouze jako instalátor pro Linux a do samotných dat nijak nezasahuje ani je neupravuje.

Velké díky patří autorům lokalizace za jejich dlouhodobou práci ❤️

---

## 🔐 Soukromí

Aplikace:

- nesbírá žádná uživatelská data
- neodesílá žádná data na internet (kromě stažení lokalizace z GitHubu)

---

## 📦 Distribuce

### AppImage

Přenosná verze bez instalace.

### Flatpak

Integrovaná Linux desktop aplikace.

Flatpak bundle je dostupný v GitHub Releases.

---

## 🚀 Spuštění

1. stáhni AppImage
2. nastav práva:

    ```bash
    chmod +x SC-CZ-toolkit-*.AppImage
    ```

3. spusť:

    ```bash
    ./SC-CZ-Toolkit-*.AppImage
    ```

## 🔒 Flatpak a přístup ke hrám

Pokud máš Star Citizen mimo domovský adresář (`HOME`),
může být potřeba povolit přístup například:

```bash
flatpak override --user --filesystem=/home/data com.sccommunity.SCCZToolkit
```

---

## 🛠️ Technologie

- .NET 8
- Avalonia UI
- Linux
- AppImage
- Flatpak
- GitHub Actions CI/CD

---

## ⬇️ Stažení

Aktuální verze:
https://github.com/1walkerit/sc-cz-toolkit/releases

---

## 📄 Licence

Tento projekt je samostatný nástroj.
Lokalizační data nejsou součástí tohoto repozitáře a podléhají podmínkám jejich autorů.

---

## ❤️ Autor

Vytvořil: 1walkerit
Linux komunita 🤝 Star Citizen CZ komunita
